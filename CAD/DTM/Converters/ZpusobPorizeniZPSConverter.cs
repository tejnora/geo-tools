using System;
using System.Globalization;
using System.Windows.Data;

namespace CAD.DTM.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class ZpusobPorizeniZPSConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {
                case 1:
                    return "geodeticky - terestricky";
                case 2:
                    return "geodeticky - fotogrammetricky";
                case 3:
                    return "geodeticky - pozemním laserovým skenováním";
                case 4:
                    return "přibližným zákresem";
                case 5:
                    return "odvozením";
                default:
                    return "nezjištěno";
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
