using System;
using System.Globalization;
using System.Windows.Data;
using GeoHelper.ExtensionMethods;

namespace GeoHelper.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    internal class AngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var angleValue = (double)value;
            return double.IsNaN(angleValue) ? string.Empty : angleValue.ToUhel(false);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cv = (string)value;
            return cv.ToDouble();
        }
    }
}