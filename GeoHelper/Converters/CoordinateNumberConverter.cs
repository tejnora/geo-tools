using System;
using System.Globalization;
using System.Windows.Data;

namespace GeoHelper.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    internal class CoordinateNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cv = (string)value;
            return cv.PadLeft(12, '0');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}