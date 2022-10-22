using System;
using System.Globalization;

namespace GeoHelper.ExtensionMethods
{
    internal static class StringExtensions
    {
        public static string ToPredcisliBodu(this string value)
        {
            return value.PadLeft(8, '0');
        }

        public static string ToCisloBodu(this string value)
        {
            return value.PadLeft(4, '0');
        }

        public static string Icrement(this string value)
        {
            UInt32 intValue;
            if (UInt32.TryParse(value, out intValue))
            {
                intValue++;
                value = intValue.ToString();
            }
            return value;
        }

        public static double ToDouble(this string value)
        {
            if (value.Length == 0)
                return double.NaN;
            if (value.Length == 1 && value[0] == '-')
                return double.NegativeInfinity;
            if (value.Length == 1 && value[0] == '+')
                return double.PositiveInfinity;
            return double.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture);
        }
    }
}