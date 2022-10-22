using System;
using System.Globalization;
using System.Windows.Data;

namespace GeoHelper.Converters
{
    [ValueConversion(typeof (bool), typeof (bool))]
    internal class InvertBoolConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            return !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            return !(bool) value;
        }
    }
}