using System;
using System.Collections.Generic;
using System.IO;
using Utils.Bocu1Codec;

namespace GeoHelper.Utils
{
    public static class DateTimeExtensions
    {
        /**
       * DateTime.Ticks 
       *
       * A single tick represents one hundred nanoseconds or one ten-millionth of a second. There are 10,000 ticks in a millisecond.
       * The value of this property represents the number of 100-nanosecond intervals that have elapsed since 12:00:00 midnight, January 1, 0001, which represents
       */

        //save into PNetT double DateTime format
        public static double ToDouble(this DateTime time)
        {
            //Tick value of PNetT DateTime zero 599264352000000000L
            double dt = time.Ticks - 599264352000000000L;
            dt /= 10000000L*60*60*24;
            return dt;
        }

        //set time from PNetT double DateTime format
        public static DateTime FromDouble(double value)
        {
            var ticks = (long) (10000000L*60*60*24*value);
            ticks += 599264352000000000L;
            DateTime dt = DateTime.FromBinary(ticks);
            return dt;
        }
    }

    public static class CollectionsExtensions
    {
        public static T Back<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }

        public static void Pop<T>(this List<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }

        public static void IncrementTop<T>(this List<int> list)
        {
            list[list.Count - 1]++;
        }
    }

    public static class ExtensionsStream
    {
        public static void WritePackedInt(this Stream stream, int value)
        {
            uint val;
            if (value < 0)
                val = (uint) (-value*2 + 1);
            else
                val = (uint) (value*2);
            byte packedLen;
            while (true)
            {
                packedLen = (byte) (val & 127);
                if ((val >> 7) != 0)
                {
                    packedLen |= 128;
                    stream.WriteByte(packedLen);
                }
                else
                {
                    stream.WriteByte(packedLen);
                    break;
                }
                val = val >> 7;
            }
        }

        public static void WriteInt32(this Stream stream, int value)
        {
            stream.WriteByte((byte) value);
            stream.WriteByte((byte) (value >> 8));
            stream.WriteByte((byte) (value >> 16));
            stream.WriteByte((byte) (value >> 24));
        }


        public static void WriteInt64(this Stream stream, long value)
        {
            stream.WriteByte((byte) value);
            stream.WriteByte((byte) (value >> 8));
            stream.WriteByte((byte) (value >> 16));
            stream.WriteByte((byte) (value >> 24));
            stream.WriteByte((byte) (value >> 32));
            stream.WriteByte((byte) (value >> 40));
            stream.WriteByte((byte) (value >> 48));
            stream.WriteByte((byte) (value >> 56));
        }

        public static int ReadInt32(this Stream stream)
        {
            int result = stream.ReadByte();
            result |= stream.ReadByte() << 8;
            result |= stream.ReadByte() << 16;
            result |= stream.ReadByte() << 24;
            return result;
        }

        public static long ReadInt64(this Stream stream)
        {
            var result = (ulong) stream.ReadByte();
            result |= ((uint) stream.ReadByte() << 8);
            result |= ((uint) stream.ReadByte() << 16);
            result |= ((uint) stream.ReadByte() << 24);
            result |= ((uint) stream.ReadByte() << 32);
            result |= ((uint) stream.ReadByte() << 40);
            result |= ((uint) stream.ReadByte() << 48);
            result |= ((uint) stream.ReadByte() << 56);
            return (long) result;
        }

        public static int ReadPackedInteger(this Stream stream)
        {
            uint len = 0;
            byte packedLen = 0;
            int index = 0;
            while (true)
            {
                packedLen = (byte) stream.ReadByte();
                if ((packedLen & 128) != 0)
                {
                    len |= (len) | (uint) (((packedLen & 127) << (7*index)));
                }
                else
                {
                    len |= (len) | (uint) (((packedLen & 127) << (7*index)));
                    break;
                }
                index++;
            }
            int result;
            if ((len & 1) != 0)
                result = (int) (-len/2);
            else
                result = (int) (len/2);
            return result;
        }

        public static void WriteString(this Stream stream, string value)
        {
            Bocu1Encoding.WriteStringBocu1(stream, value);
        }

        public static string ReadString(this Stream stream)
        {
            return Bocu1Encoding.ReadStringBocu1(stream);
        }

        public static void WriteDouble(this Stream stream, double value)
        {
            stream.WriteInt64(BitConverter.DoubleToInt64Bits(value));
        }

        public static double ReadDouble(this Stream stream)
        {
            return BitConverter.Int64BitsToDouble(stream.ReadInt64());
        }
    }
}