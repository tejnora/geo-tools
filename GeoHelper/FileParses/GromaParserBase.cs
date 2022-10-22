using System;
using System.Globalization;

namespace GeoHelper.FileParses
{
    internal class GromaParserBase
    {
        protected static int ReadInt32(byte[] buffer, int offset)
        {
            var intBuffer = new byte[4];
            Array.Copy(buffer, offset, intBuffer, 0, 4);
            var value = BitConverter.ToInt32(intBuffer, 0);
            return value;
        }

        protected static Int64 ReadInt64(byte[] buffer, int offset)
        {
            var intBuffer = new byte[8];
            Array.Copy(buffer, offset, intBuffer, 0, 8);
            var value = BitConverter.ToInt64(intBuffer, 0);
            return value;
        }

        protected static double ReadDouble(byte[] buffer, int offset)
        {
            var doubleBuffer = new byte[8];
            Array.Copy(buffer, offset, doubleBuffer, 0, 8);
            var value = BitConverter.ToDouble(doubleBuffer, 0);
            if (value == 0) return double.NaN;
            return value;
        }

        protected static string ReadString(byte[] buffer, int offset, int maxLength)
        {
            var text = new byte[maxLength];
            Array.Copy(buffer, offset, text, 0, maxLength);
            var descriptionEnd = Array.IndexOf(text, (byte)0);
            if (descriptionEnd == -1)
                return System.Text.Encoding.ASCII.GetString(text);
            return System.Text.Encoding.GetEncoding("iso-8859-2").GetString(text, 0, descriptionEnd);
        }

        protected static string ReadCompressNumber(byte[] buffer, int offset)
        {
            if (buffer[offset + 2] == 0)
            {
                var number = new byte[2];
                Array.Copy(buffer, offset, number, 0, 2);
                var intNumber = BitConverter.ToInt16(number, 0);
                return intNumber.ToString(CultureInfo.InvariantCulture);
            }
            return ReadString(buffer, offset + 2, 14);
        }

        protected static double FromRadianToGrad(double value)
        {
            if (value == 0) return double.NaN;
            return value * 200.0 / Math.PI;
        }

    }
}
