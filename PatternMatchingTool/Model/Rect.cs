using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatchingTool.Model
{
    public partial class Rect : ObservableObject
    {
        [ObservableProperty] private double x;
        [ObservableProperty] private double y;
        [ObservableProperty] private double width;
        [ObservableProperty] private double height;

        public Rect(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }
}
