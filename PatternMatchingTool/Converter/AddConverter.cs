using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PatternMatchingTool.Converter
{
    public class AddConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double sum = 0;

            // 모든 values 더하기
            foreach (var value in values)
            {
                if (value != DependencyProperty.UnsetValue)
                {
                    sum += System.Convert.ToDouble(value);
                }
            }

            // parameter가 있으면 추가
            if (parameter != null)
            {
                sum += System.Convert.ToDouble(parameter);
            }

            return sum;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}