using System;
using System.Globalization;

namespace VFK
{
    class Utils
    {
        public static double parseDouble(string aValue, ref bool aResult)
        {
            aResult = true;
            try
            {
                return Convert.ToDouble(aValue, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
            }
            aResult = false;
            return 0.0;
        }

        public static DateTime parseDateTime(string aValue, ref bool aResult)
        {
            aResult = true;
            try
            {
                return DateTime.ParseExact(aValue, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
            }
            aResult = false;
            return DateTime.Now;
        }

        public static UInt16 parseUInt16(string aValue, ref bool aResult)
        {
            aResult = true;
            try
            {
                return Convert.ToUInt16(aValue);
            }
            catch (Exception)
            {
            }
            aResult = false;
            return 0;
        }

        public static string getCssStringValue(string aText, ref int aIndexPos)
        {
            var endIndex = aText.IndexOf(";", aIndexPos);
            string value;
            if (endIndex == -1)
            {
                value = aText.Substring(aIndexPos, aText.Length - aIndexPos);
                aIndexPos = -1;

            }
            else
            {
                value = aText.Substring(aIndexPos, endIndex - aIndexPos);
                aIndexPos = endIndex + 1;
            }
            value = value.Trim(new[] { '\"' });
            return value;
        }

    }
}
