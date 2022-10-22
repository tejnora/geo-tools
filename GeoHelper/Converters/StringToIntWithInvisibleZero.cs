using System;
using System.Globalization;
using System.Windows.Data;

namespace GeoHelper.Converters
{
    [ValueConversion(typeof (UInt32), typeof (string))]
    internal class StringToIntWithInvisibleZero : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            var cv = (UInt32) value;
            if (cv == 0)
                return string.Empty;
            return cv.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            var cv = (string) value;
            if (cv.Length == 0)
                return 0;
            return UInt32.Parse(cv);
        }
    }
}