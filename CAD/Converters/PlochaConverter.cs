using System;
using System.Windows.Data;

namespace CAD.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    class PlochaConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            double souradnice = (double)value;
            if (double.IsNaN(souradnice))
                return string.Empty;
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00}", souradnice);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            string cv = (string)value;
            return System.Convert.ToDouble(cv);
        }
    }
}
