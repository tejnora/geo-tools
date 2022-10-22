using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace CAD.VFK.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    class VfkBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = (string)value;
            if (stringValue == "a")
                return true;
            if (stringValue == "n")
                return false;
            throw new InvalidDataException(string.Format("String value{0} is not suppored.", stringValue));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;
            return boolValue ? "a" : "n";
        }
    }
}
