using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Utils.Bocu1Codec
{
    public class Bocu1Encoding
    {
        /*
       * Byte value map for control codes,
       * from external byte values 0x00..0x20
       * to trail byte values 0..19 (0..0x13) as used in the difference calculation.
       * External byte values that are illegal as trail bytes are mapped to -1.
       */

        const int BOCU1_ASCII_PREV = 0x40;
        const int BOCU1_MIN = 0x21;
        const int BOCU1_MIDDLE = 0x90;
        const int BOCU1_MAX_LEAD = 0xfe;
        const int BOCU1_MAX_TRAIL = 0xff;
        const int BOCU1_RESET = 0xff;
        const int BOCU1_TRAIL_CONTROLS_COUNT = 20;
        const int BOCU1_SINGLE = 64;
        const int BOCU1_TRAIL_COUNT = ((BOCU1_MAX_TRAIL - BOCU1_MIN + 1) + BOCU1_TRAIL_CONTROLS_COUNT);
        const int BOCU1_LEAD_2 = 43;
        const int BOCU1_LEAD_3 = 3;
        const int BOCU1_LEAD_4 = 1;
        const int BOCU1_REACH_POS_1 = (BOCU1_SINGLE - 1);
        const int BOCU1_REACH_POS_2 = (BOCU1_REACH_POS_1 + BOCU1_LEAD_2*BOCU1_TRAIL_COUNT);
        const int BOCU1_REACH_NEG_1 = (-BOCU1_SINGLE);
        const int BOCU1_REACH_NEG_2 = (BOCU1_REACH_NEG_1 - BOCU1_LEAD_2*BOCU1_TRAIL_COUNT);
        const int BOCU1_START_POS_2 = (BOCU1_MIDDLE + BOCU1_REACH_POS_1 + 1);
        const int BOCU1_START_POS_3 = (BOCU1_START_POS_2 + BOCU1_LEAD_2);
        const int BOCU1_START_POS_4 = (BOCU1_START_POS_3 + BOCU1_LEAD_3);
        const int BOCU1_START_NEG_2 = (BOCU1_MIDDLE + BOCU1_REACH_NEG_1);
        const int BOCU1_START_NEG_3 = (BOCU1_START_NEG_2 - BOCU1_LEAD_2);
        const int BOCU1_START_NEG_4 = (BOCU1_START_NEG_3 - BOCU1_LEAD_3);

        const int BOCU1_REACH_POS_3 =
            (BOCU1_REACH_POS_2 + BOCU1_LEAD_3*BOCU1_TRAIL_COUNT*BOCU1_TRAIL_COUNT);

        const int BOCU1_REACH_NEG_3 =
            (BOCU1_REACH_NEG_2 - BOCU1_LEAD_3*BOCU1_TRAIL_COUNT*BOCU1_TRAIL_COUNT);

        const int BOCU1_TRAIL_BYTE_OFFSET = (BOCU1_MIN - BOCU1_TRAIL_CONTROLS_COUNT);

        static readonly sbyte[] bocu1ByteToTrail = new sbyte[]
                                                       {
                                                           /*  0     1     2     3     4     5     6     7    */
                                                           -1, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, -1,
                                                           /*  8     9     a     b     c     d     e     f    */
                                                           -1, -1, -1, -1, -1, -1, -1, -1,
                                                           /*  10    11    12    13    14    15    16    17   */
                                                           0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d,
                                                           /*  18    19    1a    1b    1c    1d    1e    1f   */
                                                           0x0e, 0x0f, -1, -1, 0x10, 0x11, 0x12, 0x13,
                                                           /*  20   */
                                                           -1
                                                       };

        /*
       * Byte value map for control codes,
       * from trail byte values 0..19 (0..0x13) as used in the difference calculation
       * to external byte values 0x00..0x20.
       */

        static readonly sbyte[] bocu1TrailToByte = new sbyte[]
                                                       {
                                                           /*  0     1     2     3     4     5     6     7    */
                                                           0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x10, 0x11,
                                                           /*  8     9     a     b     c     d     e     f    */
                                                           0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,
                                                           /*  10    11    12    13   */
                                                           0x1c, 0x1d, 0x1e, 0x1f
                                                       };

        /**
       * Compute the next "previous" value for differencing
       * from the current code point.
       *
       * @param c current code point, 0..0x10ffff
       * @return "previous code point" state value
       */

        static int bocu1Prev(int c)
        {
            /* compute new prev */
            if (c < 0x3040) // most used case first
            {
                /* mostly small scripts */
                return (c & ~0x7f) + BOCU1_ASCII_PREV;
            }
            if (0x3040 <= c && c <= 0x309f)
            {
                /* Hiragana is not 128-aligned */
                return 0x3070;
            }
            if (0x4e00 <= c && c <= 0x9fa5)
            {
                /* CJK Unihan */
                return 0x4e00 - BOCU1_REACH_NEG_2;
            }
            if (0xac00 <= c && c <= 0xd7a3)
            {
                /* Korean Hangul */
                return (0xd7a3 + 0xac00)/2;
            }
            /* mostly small scripts */
            return (c & ~0x7f) + BOCU1_ASCII_PREV;
        }

        static int encodeBocu1(ref int apPrev, int aC)
        {
            Debug.Assert(aC >= 0 && aC <= 0x10ffff);
            if (aC <= 0x20)
            {
                /*
            * ISO C0 control & space:
            * Encode directly for MIME compatibility,
            * and reset state except for space, to not disrupt compression.
            */
                if (aC != 0x20)
                {
                    apPrev = BOCU1_ASCII_PREV;
                }
                return 0x01000000 | aC;
            }
            /*
        * all other Unicode code points c==U+0021..U+10ffff
        * are encoded with the difference c-prev
        *
        * a new prev is computed from c,
        * placed in the middle of a 0x80-block (for most small scripts) or
        * in the middle of the Unihan and Hangul blocks
        * to statistically minimize the following difference
        */
            int prev = apPrev;
            apPrev = bocu1Prev(aC);
            return packDiff(aC - prev);
        }

        static int decodeBocu1(ref Bocu1Rx apRx, byte aB)
        {
            int prev, c, count;
            prev = apRx.Prev;
            count = apRx.Count;

            if (count == 0)
            {
                /* byte in lead position */
                if (aB <= 0x20)
                {
                    /*
               * Direct-encoded C0 control code or space.
               * Reset prev for C0 control codes but not for space.
               */
                    if (aB != 0x20)
                    {
                        apRx.Prev = BOCU1_ASCII_PREV;
                    }
                    return aB;
                }

                /*
            * b is a difference lead byte.
            *
            * Return a code point directly from a single-byte difference.
            *
            * For multi-byte difference lead bytes, set the decoder state
            * with the partial difference value from the lead byte and
            * with the number of trail bytes.
            *
            * For four-byte differences, the signedness also affects the
            * first trail byte, which has special handling farther below.
            */
                if (aB >= BOCU1_START_NEG_2 && aB < BOCU1_START_POS_2)
                {
                    /* single-byte difference */
                    c = prev + (aB - BOCU1_MIDDLE);
                    apRx.Prev = bocu1Prev(c);
                    return c;
                }
                else if (aB == BOCU1_RESET)
                {
                    /* only reset the state, no code point */
                    apRx.Prev = BOCU1_ASCII_PREV;
                    return -1;
                }
                else
                {
                    return decodeBocu1LeadByte(ref apRx, aB);
                }
            }
            else
            {
                /* trail byte in any position */
                return decodeBocu1TrailByte(ref apRx, aB);
            }
        }

        /**
       * Encode a difference -0x10ffff..0x10ffff in 1..4 bytes
       * and return a packed integer with them.
       *
       * The encoding favors small absolut differences with short encodings
       * to compress runs of same-script characters.
       *
       * @param diff difference value -0x10ffff..0x10ffff
       * @return
       *      0x010000zz for 1-byte sequence zz
       *      0x0200yyzz for 2-byte sequence yy zz
       *      0x03xxyyzz for 3-byte sequence xx yy zz
       *      0xwwxxyyzz for 4-byte sequence ww xx yy zz (ww>0x03)
       */

        static int packDiff(int aDiff)
        {
            int result, m, lead, count, shift;

            if (aDiff >= BOCU1_REACH_NEG_1)
            {
                /* mostly positive differences, and single-byte negative ones */
                if (aDiff <= BOCU1_REACH_POS_1)
                {
                    /* single byte */
                    return 0x01000000 | (BOCU1_MIDDLE + aDiff);
                }
                else if (aDiff <= BOCU1_REACH_POS_2)
                {
                    /* two bytes */
                    aDiff -= BOCU1_REACH_POS_1 + 1;
                    lead = BOCU1_START_POS_2;
                    count = 1;
                }
                else if (aDiff <= BOCU1_REACH_POS_3)
                {
                    /* three bytes */
                    aDiff -= BOCU1_REACH_POS_2 + 1;
                    lead = BOCU1_START_POS_3;
                    count = 2;
                }
                else
                {
                    /* four bytes */
                    aDiff -= BOCU1_REACH_POS_3 + 1;
                    lead = BOCU1_START_POS_4;
                    count = 3;
                }
            }
            else
            {
                /* two- and four-byte negative differences */
                if (aDiff >= BOCU1_REACH_NEG_2)
                {
                    /* two bytes */
                    aDiff -= BOCU1_REACH_NEG_1;
                    lead = BOCU1_START_NEG_2;
                    count = 1;
                }
                else if (aDiff >= BOCU1_REACH_NEG_3)
                {
                    /* three bytes */
                    aDiff -= BOCU1_REACH_NEG_2;
                    lead = BOCU1_START_NEG_3;
                    count = 2;
                }
                else
                {
                    /* four bytes */
                    aDiff -= BOCU1_REACH_NEG_3;
                    lead = BOCU1_START_NEG_4;
                    count = 3;
                }
            }

            /* encode the length of the packed result */
            if (count < 3)
            {
                result = (count + 1) << 24;
            }
            else /* count==3, MSB used for the lead byte */
            {
                result = 0;
            }

            /* calculate trail bytes like digits in itoa() */
            shift = 0;
            do
            {
                m = aDiff%(BOCU1_TRAIL_COUNT);
                aDiff /= BOCU1_TRAIL_COUNT;
                if (m < 0)
                {
                    --aDiff;
                    m += (BOCU1_TRAIL_COUNT);
                }
                result |= (m >= BOCU1_TRAIL_CONTROLS_COUNT ? m + BOCU1_TRAIL_BYTE_OFFSET : bocu1TrailToByte[m]) << shift;
                shift += 8;
            } while (--count > 0);

            /* add lead byte */
            result |= (lead + aDiff) << shift;

            return result;
        }

        static int decodeBocu1LeadByte(ref Bocu1Rx apRx, byte aB)
        {
            int c, count;

            if (aB >= BOCU1_START_NEG_2)
            {
                /* positive difference */
                if (aB < BOCU1_START_POS_3)
                {
                    /* two bytes */
                    c = (aB - BOCU1_START_POS_2)*BOCU1_TRAIL_COUNT + BOCU1_REACH_POS_1 + 1;
                    count = 1;
                }
                else if (aB < BOCU1_START_POS_4)
                {
                    /* three bytes */
                    c = (aB - BOCU1_START_POS_3)*BOCU1_TRAIL_COUNT*BOCU1_TRAIL_COUNT + BOCU1_REACH_POS_2 + 1;
                    count = 2;
                }
                else
                {
                    /* four bytes */
                    c = BOCU1_REACH_POS_3 + 1;
                    count = 3;
                }
            }
            else
            {
                /* negative difference */
                if (aB >= BOCU1_START_NEG_3)
                {
                    /* two bytes */
                    c = (aB - BOCU1_START_NEG_2)*BOCU1_TRAIL_COUNT + BOCU1_REACH_NEG_1;
                    count = 1;
                }
                else if (aB > BOCU1_MIN)
                {
                    /* three bytes */
                    c = (aB - BOCU1_START_NEG_3)*BOCU1_TRAIL_COUNT*BOCU1_TRAIL_COUNT + BOCU1_REACH_NEG_2;
                    count = 2;
                }
                else
                {
                    /* four bytes */
                    c = -BOCU1_TRAIL_COUNT*BOCU1_TRAIL_COUNT*BOCU1_TRAIL_COUNT + BOCU1_REACH_NEG_3;
                    count = 3;
                }
            }

            /* set the state for decoding the trail byte(s) */
            apRx.Diff = c;
            apRx.Count = count;
            return -1;
        }

        static int decodeBocu1TrailByte(ref Bocu1Rx apRx, byte aB)
        {
            int c;
            int count;
            int t;
            if (aB <= 0x20)
            {
                /* skip some C0 controls and make the trail byte range contiguous */
                t = bocu1ByteToTrail[aB];
                if (t < 0)
                {
                    /* illegal trail byte value */
                    apRx.Prev = BOCU1_ASCII_PREV;
                    apRx.Count = 0;
                    throw new ApplicationException("decodeBocu1 CRCError");
                }
            }
            else
            {
                t = aB - BOCU1_TRAIL_BYTE_OFFSET;
            }

            /* add trail byte into difference and decrement count */
            c = apRx.Diff;
            count = apRx.Count;

            if (count == 1)
            {
                /* final trail byte, deliver a code point */
                c = apRx.Prev + c + t;
                if (0 <= c && c <= 0x10ffff)
                {
                    /* valid code point result */
                    apRx.Prev = bocu1Prev(c);
                    apRx.Count = 0;
                    return c;
                }
                else
                {
                    /* illegal code point result */
                    apRx.Prev = BOCU1_ASCII_PREV;
                    apRx.Count = 0;
                    throw new ApplicationException("decodeBocu1 CRCError");
                }
            }

            /* intermediate trail byte */
            if (count == 2)
            {
                apRx.Diff = c + t*BOCU1_TRAIL_COUNT;
            }
            else /* count==3 */
            {
                apRx.Diff = c + t*BOCU1_TRAIL_COUNT*BOCU1_TRAIL_COUNT;
            }
            apRx.Count = count - 1;
            return -1;
        }

        // virtual Void writeStringBocu1(StreamIfacePtr aStream,StringCref aValue);
        // virtual String readStringBocu1(StreamIfacePtr aStream);

        /* State for BOCU-1 decoder function. */

        public static void WriteStringBocu1(Stream stream, string value)
        {
            int len = 0;
            byte packedLen = 0;
            len = value.Length*2;
            while (true)
            {
                packedLen = (byte) (len & 127);
                if ((len >> 7) != 0)
                {
                    packedLen |= 128;
                    stream.WriteByte(packedLen);
                }
                else
                {
                    stream.WriteByte(packedLen);
                    break;
                }
                len = len >> 7;
            }
            //convert to Bocu1
            int prev, res;
            prev = BOCU1_ASCII_PREV; // InitValue

            foreach (char c in value)
            {
                res = encodeBocu1(ref prev, c);
                var count = ((uint) ((UInt32) (res) < 0x04000000 ? (res) >> 24 : 4));
                switch (count)
                {
                    case 4:
                        stream.WriteByte((byte) (res >> 24));
                        stream.WriteByte((byte) (res >> 16));
                        stream.WriteByte((byte) (res >> 8));
                        stream.WriteByte((byte) res);
                        break;
                    case 3:
                        stream.WriteByte((byte) (res >> 16));
                        stream.WriteByte((byte) (res >> 8));
                        stream.WriteByte((byte) res);
                        break;
                    case 2:
                        stream.WriteByte((byte) (res >> 8));
                        stream.WriteByte((byte) res);
                        break;
                    case 1:
                        stream.WriteByte((byte) res);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void WriteStringBocu1Raw(Stream stream, string value, MemoryStream temp)
        {
            temp.SetLength(0);
            //convert to Bocu1
            int prev, res;
            prev = BOCU1_ASCII_PREV; // InitValue

            foreach (char c in value)
            {
                res = encodeBocu1(ref prev, c);
                var count = ((uint) ((UInt32) (res) < 0x04000000 ? (res) >> 24 : 4));
                switch (count)
                {
                    case 4:
                        temp.WriteByte((byte) (res >> 24));
                        temp.WriteByte((byte) (res >> 16));
                        temp.WriteByte((byte) (res >> 8));
                        temp.WriteByte((byte) res);
                        break;
                    case 3:
                        temp.WriteByte((byte) (res >> 16));
                        temp.WriteByte((byte) (res >> 8));
                        temp.WriteByte((byte) res);
                        break;
                    case 2:
                        temp.WriteByte((byte) (res >> 8));
                        temp.WriteByte((byte) res);
                        break;
                    case 1:
                        temp.WriteByte((byte) res);
                        break;
                    default:
                        break;
                }
            }
            byte packedLen = 0;
            int len = (int) temp.Length*2;
            while (true)
            {
                packedLen = (byte) (len & 127);
                if ((len >> 7) != 0)
                {
                    packedLen |= 128;
                    stream.WriteByte(packedLen);
                }
                else
                {
                    stream.WriteByte(packedLen);
                    break;
                }
                len = len >> 7;
            }
            temp.Position = 0;
            var contentBuffer = new byte[temp.Length];
            temp.Read(contentBuffer, 0, contentBuffer.Length);
            stream.Write(contentBuffer, 0, contentBuffer.Length);
//         temp.CopyTo(stream);
        }

        public static string ReadStringBocu1(Stream stream)
        {
            int len = 0;
            byte packedLen = 0;
            int index = 0;
            while (true)
            {
                packedLen = (byte) stream.ReadByte();
                if ((packedLen & 128) != 0)
                {
                    len |= (len) | ((packedLen & 127) << (7*index));
                }
                else
                {
                    len |= (len) | ((packedLen & 127) << (7*index));
                    break;
                }
                index++;
            }
            len = len/2;
            var sb = new StringBuilder();
            sb.EnsureCapacity(len);

            Bocu1Rx rx;
            rx.Prev = BOCU1_ASCII_PREV;
            rx.Count = 0;
            rx.Diff = 0;
            int c;
            byte p = 0;

            while (len > 0)
            {
                p = (byte) stream.ReadByte();
                c = decodeBocu1(ref rx, p);
                Debug.Assert(c >= -1);
                if (c >= 0)
                {
                    if ((uint) (c) <= 0xffff)
                    {
                        sb.Append((char) c);
                        len--;
                    }
                    else
                    {
                        sb.Append((char) ((c >> 10) + 0xd7c0));
                        len--;
                        sb.Append((char) (((c) & 0x3ff) | 0xdc00));
                        len--;
                    }
                }
            }
            return sb.ToString();
        }

        public static string ReadStringBocu1Raw(Stream stream, int byteLength)
        {
            var sb = new StringBuilder(byteLength);

            Bocu1Rx rx;
            rx.Prev = BOCU1_ASCII_PREV;
            rx.Count = 0;
            rx.Diff = 0;
            int c;
            byte p = 0;

            while (byteLength > 0)
            {
                p = (byte) stream.ReadByte();
                byteLength--;
                c = decodeBocu1(ref rx, p);
                Debug.Assert(c >= -1);
                if (c >= 0)
                {
                    if ((uint) (c) <= 0xffff)
                    {
                        sb.Append((char) c);
                    }
                    else
                    {
                        sb.Append((char) ((c >> 10) + 0xd7c0));
                        sb.Append((char) (((c) & 0x3ff) | 0xdc00));
                    }
                }
            }
            return sb.ToString();
        }

        public struct Bocu1Rx
        {
            public int Count;
            public int Diff;
            public int Prev;
        }
    }
}