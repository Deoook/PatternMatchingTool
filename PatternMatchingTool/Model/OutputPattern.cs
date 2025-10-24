using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatchingTool.Model
{
    public class OutputPattern
    {
        public double Score;
        public double Angle;
        public Rect Rect;
        public Point2f Center;

        public OutputPattern()
        {
            Score = 0;
            Angle = 0;
            Rect = new Rect(0, 0, 0, 0);
            Center = new Point2f();
        }
    }
}
