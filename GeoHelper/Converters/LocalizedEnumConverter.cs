using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using GeoBase.Localization;

namespace GeoHelper.Converters
{
    [ValueConversion(typeof(Enum), typeof(string))]
    class LocalizedEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumValue = value.ToString();
            var traslate = LanguageDictionary.Current.Translate<string>((string)parameter, enumValue);
            return traslate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(false);
            return null;
        }
    }
}
