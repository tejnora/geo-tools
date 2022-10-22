using System.Globalization;
using GeoBase.Utils;

namespace GeoHelper.ExtensionMethods
{
    internal static class DoubleExtensions
    {
        const int _lengthWithoutPad = 2;
        const int _lengthWithPad = 10;

        public static string ToSouradnice(this double value, bool withPad)
        {
            ProgramOption op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                              "VstypVystupSouradnicePocetDesMist");
            return value.ToString(withPad, op.getInt(2), ToSouradniceFce);
        }

        static string ToSouradniceFce(double value, bool withPad, int padLength)
        {
            string format = "0.".PadRight(padLength + _lengthWithoutPad, '0');
            string returnValue = string.Empty;
            if (!double.IsNaN(value))
                returnValue = string.Format(CultureInfo.InvariantCulture, "{0:" + format + "}", value);
            if (withPad)
                returnValue = returnValue.PadLeft(padLength + _lengthWithPad, ' ');
            return returnValue;
        }

        public static string ToUhel(this double value, bool withPad)
        {
            ProgramOption op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                              "VstypVystupUhelPocetDesMist");
            return value.ToString(withPad, op.getInt(2), ToUhelFce);
        }

        static string ToUhelFce(double value, bool withPad, int padLength)
        {
            string format = "0.".PadRight(padLength + _lengthWithoutPad, '0');
            string returnValue = string.Empty;
            if (!double.IsNaN(value))
                returnValue = string.Format(CultureInfo.InvariantCulture, "{0:" + format + "}", value);
            if (withPad)
                returnValue = returnValue.PadLeft(padLength + _lengthWithPad, ' ');
            return returnValue;
        }

        public static string ToDelka(this double value, bool withPad)
        {
            ProgramOption op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                              "VstypVystupDelkyPocetDesMist");
            return value.ToString(withPad, op.getInt(2), ToDelkaFce);
        }

        static string ToDelkaFce(double value, bool withPad, int padLength)
        {
            string format = "0.".PadRight(padLength + _lengthWithoutPad, '0');
            string returnValue = string.Empty;
            if (!double.IsNaN(value))
                returnValue = string.Format(CultureInfo.InvariantCulture, "{0:" + format + "}", value);
            if (withPad)
                returnValue = returnValue.PadLeft(padLength + _lengthWithPad, ' ');
            return returnValue;
        }

        public static string ToPlocha(this double value, bool withPad)
        {
            ProgramOption op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                              "VstypVystupPlochaPocetDesMist");
            return value.ToString(withPad, op.getInt(2), ToPlochaFce);
        }

        static string ToPlochaFce(double value, bool withPad, int padLength)
        {
            string format = "0.".PadRight(padLength + _lengthWithoutPad, '0');
            string returnValue = string.Empty;
            if (!double.IsNaN(value))
                returnValue = string.Format(CultureInfo.InvariantCulture, "{0:" + format + "}", value);
            if (withPad)
                returnValue = returnValue.PadLeft(padLength + _lengthWithPad, ' ');
            return returnValue;
        }

        public static string ToVyska(this double value, bool withPad)
        {
            ProgramOption op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                              "VstypVystupVyskyPocetDesMist");
            return value.ToString(withPad, op.getInt(2), ToVyskaFce);
        }

        static string ToVyskaFce(double value, bool withPad, int padLength)
        {
            string format = "0.".PadRight(padLength + _lengthWithoutPad, '0');
            string returnValue = string.Empty;
            if (!double.IsNaN(value))
                returnValue = string.Format(CultureInfo.InvariantCulture, "{0:" + format + "}", value);
            if (withPad)
                returnValue = returnValue.PadLeft(padLength + _lengthWithPad, ' ');
            return returnValue;
        }

        static string ToString(this double value, bool withPad, int padLength, Converter converter)
        {
            if (double.IsNaN(value))
            {
                if (withPad)
                    return " ".PadLeft(padLength + _lengthWithPad);
                return " ".PadLeft(padLength + _lengthWithoutPad);
            }
            if (double.IsNegativeInfinity(value))
                return "-";
            if (double.IsPositiveInfinity(value))
                return "+";
            return converter(value, withPad, padLength);
        }

        public static string ToOdchylka(this double value)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.000}", value);
        }

        public static string ToGradyWithPad(this double value)
        {
            string format = "0.00000";
            string returnValue = string.Empty;
            if (!double.IsNaN(value))
                returnValue = string.Format(CultureInfo.InvariantCulture, "{0:" + format + "}", value).PadLeft(9);
            return returnValue;
        }

        delegate string Converter(double value, bool withPad, int padLength);
    }
}