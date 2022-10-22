using System;
using System.Globalization;
using System.Windows.Data;
using GeoHelper.ExtensionMethods;

namespace GeoHelper.Converters
{
    [ValueConversion(typeof(Double), typeof(string))]
    internal class DoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            var cv = (Double)value;
            if (double.IsNaN(cv))
                return "";
            cv = Math.Round(cv, 2);
            return cv.ToString("0.0000", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            var cv = (string)value;
            if (string.IsNullOrEmpty(cv))
                return double.NaN;
            return cv.ToDouble();
        }
    }
}