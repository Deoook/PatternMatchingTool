using Cognex.VisionPro;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using PatternMatchingTool.Data;
using PatternMatchingTool.Model;
using PatternMatchingTool.Process;
using System.Diagnostics;

namespace PatternMatchingTool.ViewModel
{
    public partial class SettingPatternPageVM : ObservableObject
    {
        [ObservableProperty]
        private Define.PatternType patternType = Define.PatternType.PATTERN_TYPE_ID;

        [ObservableProperty]
        private List<string> patternTypes = new List<string>
        {
            "ID", "Pattern Matching"
        };

        [ObservableProperty]
        private bool isPatternRectClick = false;

        [ObservableProperty]
        private Bitmap displayBitmap = null;

        [ObservableProperty]
        private Bitmap patternBitmap = null;

        [ObservableProperty]
        private Model.Rect resultRect;

        [ObservableProperty]
        private Model.Rect patternRect;

        [ObservableProperty]
        private double patternRectLeftTopX;
        [ObservableProperty]
        private double patternRectLeftTopY;

        [ObservableProperty]
        private double patternRectRightTopX;
        [ObservableProperty]
        private double patternRectRightTopY;

        [ObservableProperty]
        private double patternRectLeftBottomX;
        [ObservableProperty]
        private double patternRectLeftBottomY;

        [ObservableProperty]
        private double patternRectRightBottomX;
        [ObservableProperty]
        private double patternRectRightBottomY;

        [ObservableProperty]
        private bool isDraggingRect = false;

        [ObservableProperty]
        private System.Windows.Point clickPoint;

        [ObservableProperty]
        private double threshold;

        [ObservableProperty]
        private double angle;

        public SettingPatternPageVM()
        {
            if (false == Initialize())
                return;
        }

        public bool Initialize()
        {
            bool bReturn = false;
            do
            {
                //DisplayBitmap = new Bitmap(@"D:\Desktop\스크린샷 2025-10-23 095029.png");

                ResultRect = new Model.Rect(0, 0, 0, 0);

                patternRect = new Model.Rect(0, 0, 100, 100);


                bReturn = true;
            } while (false);

            return bReturn;
        }

        [RelayCommand]
        private void LoadImage()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                DisplayBitmap = new Bitmap(dlg.FileName);

            }
        }

        [RelayCommand]
        private void PatternRect_MouseLeftButtonDown()
        {
            IsPatternRectClick = true;
        }

        [RelayCommand]
        private void Page_MouseLeftButtonDown()
        {
            IsPatternRectClick = false;
        }

        /// <summary>
        /// 이미지 새로 촬영 후에 가져오기
        /// </summary>
        [RelayCommand]
        private void GrabCamera()
        {
            var pDocument = Document.GetDocument;
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.OnFrameReceived += OnFrameReceived;

            // 카메라 열고 Trigger 날리기
            pDocument.m_objProcessMain.m_objProcessCameraManager.Start();
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.Trigger();

            // 찍는 동안 100ms Delay
            Thread.Sleep(100);

            // 찍은 후 카메라 닫기
            pDocument.m_objProcessMain.m_objProcessCameraManager.Stop();
        }

        private void OnFrameReceived(Bitmap bmp)
        {
            var pDocument = Document.GetDocument;
            DisplayBitmap = bmp;
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.OnFrameReceived -= OnFrameReceived;
        }

        private void GrabImage()
        {
        }

        double scaleX;
        double scaleY;

        [RelayCommand]
        private void Cut()
        {
            if (DisplayBitmap == null) return;

            using Mat src = BitmapConverter.ToMat(DisplayBitmap);

            // 이거 Behavior 써서 실제 렌더링 크기 가져오기.
            double displayWidth = 778.99;   
            double displayHeight = 650;

            // 원본 이미지 크기
            double imageWidth = DisplayBitmap.Width;
            double imageHeight = DisplayBitmap.Height;

            // 스케일 비율 계산
            scaleX = imageWidth / displayWidth;
            scaleY = imageHeight / displayHeight;

            // Canvas 좌표를 이미지 좌표로 변환
            int x = (int)(PatternRect.X * scaleX);
            int y = (int)(PatternRect.Y * scaleY);
            int width = (int)(PatternRect.Width * scaleX);
            int height = (int)(PatternRect.Height * scaleY);

            // 범위 체크
            x = Math.Max(0, Math.Min(x, src.Width - 1));
            y = Math.Max(0, Math.Min(y, src.Height - 1));
            width = Math.Min(width, src.Width - x);
            height = Math.Min(height, src.Height - y);

            using Mat crop = new Mat(src, new OpenCvSharp.Rect(x, y, width, height));
            PatternBitmap = crop.ToBitmap();

            TestRun();
        }

        private void TestRun()
        {
            // patternRect만큼 이미지 잘라서 넣고
            // 패턴 등록 후에
            // Test Run 하기.

            Stopwatch sw = new Stopwatch();
            sw.Start();

            ProcessPattern objPattern = new ProcessPattern();
            objPattern.Initialize();
            objPattern.SetPattern(PatternBitmap);
            OutputPattern result = objPattern.FindPattern(DisplayBitmap);

            ResultRect.X = result.Rect.X * scaleX;
            ResultRect.Y = result.Rect.Y * scaleY;
            ResultRect.Width = result.Rect.Width * scaleX;
            ResultRect.Height = result.Rect.Height * scaleY;

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
