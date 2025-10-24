using OpenCvSharp;
using OpenCvSharp.Features2D;
using PatternMatchingTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace PatternMatchingTool.Process
{
    public class ProcessPattern
    {
        private SIFT sift;
        private BFMatcher matcher;
        private Mat patternDescriptors;
        private KeyPoint[] patternKeypoints;
        private OpenCvSharp.Size patternSize;

        public double RatioThreshold { get; set; } = 0.75; // Lowe's ratio test
        public int MinMatchCount { get; set; } = 10; // 최소 매칭 포인트 수
        public double RansacThreshold { get; set; } = 5.0; // RANSAC 임계값

        public ProcessPattern()
        {
            
        }

        public bool Initialize()
        {
            bool bReturn = false;
            do
            {
                sift = SIFT.Create();

                matcher = new BFMatcher();

                bReturn = true;
            } while (false);

            return bReturn;
        }

        /// <summary>
        /// 패턴 등록 함수
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool SetPattern(Bitmap bmp)
        {
            bool bReturn = false;
            do
            {
                Mat patternImage = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
                patternSize = patternImage.Size();

                Mat grayPattern = new Mat();
                if (patternImage.Channels() == 3)
                    Cv2.CvtColor(patternImage, grayPattern, ColorConversionCodes.BGR2GRAY);
                else
                    grayPattern = patternImage.Clone();

                patternDescriptors = new Mat();
                sift.DetectAndCompute(grayPattern, null, out patternKeypoints, patternDescriptors);

                grayPattern.Dispose();

                if (patternKeypoints.Length < MinMatchCount)
                    break;

                bReturn = true;
            } while (false);

            return bReturn;
        }

        public OutputPattern FindPattern(Bitmap bmp, double minScore = 0.1)
        {
            Mat targetImage = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
            OutputPattern result = new OutputPattern();
            do
            {
                if (patternDescriptors == null || patternDescriptors.Empty())
                    break;

                if (targetImage == null || targetImage.Empty())
                    break;

                // 그레이스케일 변환
                Mat grayTarget = new Mat();
                if (targetImage.Channels() == 3)
                    Cv2.CvtColor(targetImage, grayTarget, ColorConversionCodes.BGR2GRAY);
                else
                    grayTarget = targetImage.Clone();

                // 특징점 검출 및 디스크립터 추출
                KeyPoint[] targetKeypoints;
                Mat targetDescriptors = new Mat();
                sift.DetectAndCompute(grayTarget, null, out targetKeypoints, targetDescriptors);

                grayTarget.Dispose();

                if (targetKeypoints.Length < MinMatchCount)
                {
                    targetDescriptors.Dispose();
                    return null;
                }

                // KNN 매칭 (k=2)
                DMatch[][] knnMatches = matcher.KnnMatch(patternDescriptors, targetDescriptors, k: 2);

                // Lowe's ratio test로 좋은 매칭만 선택
                List<DMatch> goodMatches = new List<DMatch>();
                foreach (var match in knnMatches)
                {
                    if (match.Length == 2 && match[0].Distance < RatioThreshold * match[1].Distance)
                    {
                        goodMatches.Add(match[0]);
                    }
                }

                targetDescriptors.Dispose();

                // 매칭 포인트가 충분한지 확인
                if (goodMatches.Count < MinMatchCount)
                    return null;

                // 매칭된 포인트 좌표 추출
                Point2f[] srcPoints = goodMatches.Select(m => patternKeypoints[m.QueryIdx].Pt).ToArray();
                Point2f[] dstPoints = goodMatches.Select(m => targetKeypoints[m.TrainIdx].Pt).ToArray();

                // Homography 행렬 계산 (RANSAC)
                Mat mask = new Mat();
                Mat homography = Cv2.FindHomography(
                    InputArray.Create(srcPoints),
                    InputArray.Create(dstPoints),
                    HomographyMethods.Ransac,
                    RansacThreshold,
                    mask
                );

                if (homography.Empty())
                {
                    mask.Dispose();
                    return null;
                }

                // Inlier 개수 계산
                int inlierCount = Cv2.CountNonZero(mask);
                double matchScore = (double)inlierCount / goodMatches.Count;

                mask.Dispose();

                if (matchScore < minScore)
                {
                    homography.Dispose();
                    return null;
                }

                // 패턴의 모서리를 변환하여 결과 영역 계산
                Point2f[] patternCorners = new Point2f[]
                {
                new Point2f(0, 0),
                new Point2f(patternSize.Width, 0),
                new Point2f(patternSize.Width, patternSize.Height),
                new Point2f(0, patternSize.Height)
                };

                Point2f[] transformedCorners = Cv2.PerspectiveTransform(patternCorners, homography);
                homography.Dispose();

                // 중심점 계산
                Point2f center = CalculateCenter(transformedCorners);

                // 회전 각도 계산 (상단 두 점의 기울기로 계산)
                double angle = CalculateAngle(transformedCorners[0], transformedCorners[1]);

                // Axis-aligned bounding box 계산
                float minX = transformedCorners.Min(p => p.X);
                float maxX = transformedCorners.Max(p => p.X);
                float minY = transformedCorners.Min(p => p.Y);
                float maxY = transformedCorners.Max(p => p.Y);

                Model.Rect rectangle = new Model.Rect(
                    (int)minX,
                    (int)minY,
                    (int)(maxX - minX),
                    (int)(maxY - minY)
                );

                result.Score = matchScore;
                result.Angle = angle;
                result.Rect = rectangle;
                result.Center = center;
                // 결과 생성
                //MatchResult result = new MatchResult
                //{
                //    Score = matchScore,
                //    Rectangle = rectangle,
                //    Angle = angle,
                //    Center = center,
                //    InlierCount = inlierCount,
                //    TotalMatches = goodMatches.Count,
                //    BoundingBox = transformedCorners
                //};

            } while (false);

            return result;
        }

        private Point2f CalculateCenter(Point2f[] corners)
        {
            float x = corners.Average(p => p.X);
            float y = corners.Average(p => p.Y);
            return new Point2f(x, y);
        }

        private double CalculateAngle(Point2f p1, Point2f p2)
        {
            // 두 점 사이의 각도를 계산 (degree)
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double angleRad = Math.Atan2(dy, dx);
            double angleDeg = angleRad * 180.0 / Math.PI;

            // -180 ~ 180 범위로 정규화
            if (angleDeg > 180) angleDeg -= 360;
            if (angleDeg < -180) angleDeg += 360;

            return angleDeg;
        }
    }
}
