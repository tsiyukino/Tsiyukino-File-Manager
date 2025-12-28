using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TSFM.Converters
{
    public class DepthToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int depth)
            {
                return new Thickness(8 + (depth * 20), 0, 8, 0);
            }
            return new Thickness(8, 0, 8, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
