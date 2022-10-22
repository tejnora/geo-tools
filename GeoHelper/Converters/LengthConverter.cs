using System;
using System.Globalization;
using System.Windows.Data;
using GeoHelper.ExtensionMethods;

namespace GeoHelper.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    internal class LengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var souradnice = (double)value;
            return double.IsNaN(souradnice) ? string.Empty : souradnice.ToDelka(false);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cv = (string)value;
            return cv.ToDouble();
        }
    }
}