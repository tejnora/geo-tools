using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using CAD.BackgrounImages;
using CAD.Utils;
using CAD.Images;

namespace CITReader
{
    public class CitReader : IImage
    {
        public CitReader()
        {
        }

        public CitReader(string aLocation)
        {
            FileLocation=aLocation;
        }

        #region IBackgroundImage
        public ImageType Type
        {
            get { return ImageType.itCit; }
        }

        public string FileLocation
        {
            set;
            get;
        }

        public void saveToFile(string aLocation)
        {
            if (_init)
                parse();
            FileStream stream = new FileStream(aLocation, FileMode.Create);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(_bitmapData));
            encoder.Save(stream);
        }

        public BitmapSource ImageData
        {
            get
            {
                parse();
                return _bitmapData;
            }
        }

        public bool getTestImage()
        {
            try
            {
                openFile();
                _stream.BaseStream.Seek(0, SeekOrigin.Begin);
                if (_stream.ReadUInt16() == 0x0908)
                    return true;
                _stream.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        public System.Drawing.Rectangle getRect()
        {
            Point pp=getSize();
            return new System.Drawing.Rectangle(0, 0, System.Convert.ToInt32(pp.X), System.Convert.ToInt32(pp.Y));
        }
        private Transform _matrixTransformation=null;
        public Transform getTransformation()
        {
            if (_matrixTransformation == null)
            {
                parse();
                _matrixTransformation = new MatrixTransform(_Header.TransformationMatrix._m11, _Header.TransformationMatrix._m12,
                                           _Header.TransformationMatrix._m21, _Header.TransformationMatrix._m22,
                                           _Header.TransformationMatrix._m14, _Header.TransformationMatrix._m24);
            }
            return _matrixTransformation;
        }

        public Point getSize()
        {
            parse();
            //i dont have enought information, so a I try this
            return new Point(2 * _bitmapData.PixelWidth * 254 / _bitmapData.DpiX, 2 * _bitmapData.PixelHeight * 254 / _bitmapData.DpiY);
        }

        #endregion

        public delegate void progressBar(Int16 aValue);

        public progressBar ProgressBar
        {
            get;
            set;
        }

        private BinaryReader _stream;
        private BitmapSource _bitmapData=null;
        private bool _init = true;
        private void openFile()
        {
            if(_stream==null)
                _stream = new BinaryReader(File.Open(FileLocation, FileMode.Open));
        }
        private void parse()
        {
            if (_init)
            {
                openFile();
                _stream.BaseStream.Seek(0, SeekOrigin.Begin);
                readHeader();
                readHeader2();
                byte[] imageData = _stream.ReadBytes((int)_stream.BaseStream.Length - System.Convert.ToInt32(_stream.BaseStream.Position));
                PixelFormat pf = PixelFormats.BlackWhite;
                int rawStride = (int)(_Header.PixelsPerLine * pf.BitsPerPixel + 7) / 8;
                byte[] rawImage = new byte[rawStride * _Header.NumberOfLines];
                getRowImageData(ref imageData, ref rawImage);
//                invertAllBit(ref rawImage);
                _bitmapData = BitmapSource.Create((int)_Header.PixelsPerLine, (int)_Header.NumberOfLines,
                508, 508, pf, null,
                rawImage, rawStride);
                _init = false;
                _stream.Close();
            }
        }

        private void invertAllBit(ref byte[] aValue)
        {
            byte mask = 0xFF;
            for(int i=0;i<aValue.Length;i++)
            {
                aValue[i] ^= mask;
            }
        }

        private BitmapSource Resize(BitmapSource original,
                    double originalScale,
                    double newScale)
        {
            double s = newScale / originalScale;
            return new TransformedBitmap(original, new ScaleTransform(s, s));
        }


        struct TransformationMatrix
        {
            public Double _m11, _m12, _m13, _m14;
            public Double _m21, _m22, _m23, _m24;
            public Double _m31, _m32, _m33, _m34;
            public Double _m41, _m42, _m43, _m44;
        };

        struct Header
        {
            public UInt16 HeaderTypeCode;
            public UInt16 WordsToFollow;
            public UInt16 DataTypeCode;
            public UInt16 ApplicationType;
            public Double XViewOrigin;
            public Double YViewOrigin;
            public Double ZViewOrigin;
            public Double XViewExtent;
            public Double YViewExtent;
            public Double ZViewExtent;
            public TransformationMatrix TransformationMatrix;
            public UInt32 PixelsPerLine;
            public UInt32 NumberOfLines;
            public Int16 DeviceResolution;
            public Char ScanlineOrientation;
            public Char ScannableFlag;
            public Double RotationAngle;
            public Double SkewAngle;
            public UInt16 DataTypeModifier;
            public byte[] DesignFileName;
            public byte[] DataBaseFileName;
            public byte[] ParentGridFileName;
            public byte[] FileDescription;
            public char GridFileVersion;
        };

        private Header _Header;
        private void readHeader()
        {
            _Header.HeaderTypeCode = _stream.ReadUInt16();
            _Header.WordsToFollow = _stream.ReadUInt16();
            _Header.DataTypeCode = _stream.ReadUInt16();
            _Header.ApplicationType = _stream.ReadUInt16();
            _Header.XViewOrigin = _stream.ReadDouble();
            _Header.YViewOrigin = _stream.ReadDouble();
            _Header.ZViewOrigin = _stream.ReadDouble();
            _Header.XViewExtent = _stream.ReadDouble();
            _Header.YViewExtent = _stream.ReadDouble();
            _Header.ZViewExtent = _stream.ReadDouble();
            _Header.TransformationMatrix._m11 = _stream.ReadDouble();
            _Header.TransformationMatrix._m12 = _stream.ReadDouble();
            _Header.TransformationMatrix._m13 = _stream.ReadDouble();
            _Header.TransformationMatrix._m14 = _stream.ReadDouble();
            _Header.TransformationMatrix._m21 = _stream.ReadDouble();
            _Header.TransformationMatrix._m22 = _stream.ReadDouble();
            _Header.TransformationMatrix._m23 = _stream.ReadDouble();
            _Header.TransformationMatrix._m24 = _stream.ReadDouble();
            _Header.TransformationMatrix._m31 = _stream.ReadDouble();
            _Header.TransformationMatrix._m32 = _stream.ReadDouble();
            _Header.TransformationMatrix._m33 = _stream.ReadDouble();
            _Header.TransformationMatrix._m34 = _stream.ReadDouble();
            _Header.TransformationMatrix._m41 = _stream.ReadDouble();
            _Header.TransformationMatrix._m42 = _stream.ReadDouble();
            _Header.TransformationMatrix._m43 = _stream.ReadDouble();
            _Header.TransformationMatrix._m44 = _stream.ReadDouble();
            _Header.PixelsPerLine = _stream.ReadUInt32();
            _Header.NumberOfLines = _stream.ReadUInt32();
            _Header.DeviceResolution = _stream.ReadInt16();
            _Header.ScanlineOrientation = _stream.ReadChar();
            _Header.ScannableFlag = _stream.ReadChar();
            _Header.RotationAngle = _stream.ReadDouble();
            _Header.SkewAngle = _stream.ReadDouble();
            _Header.DataTypeModifier = _stream.ReadUInt16();
            _Header.DesignFileName = _stream.ReadBytes(66);
            _Header.DataBaseFileName = _stream.ReadBytes(66);
            _Header.ParentGridFileName = _stream.ReadBytes(66);
            _Header.FileDescription = _stream.ReadBytes(80);
            //Minimum Value BEGIN
            _stream.ReadUInt64();
            _stream.ReadUInt64();
            _stream.ReadBytes(3);
            //Minimum Value END
            _Header.GridFileVersion = _stream.ReadChar();
        }

        struct Header2
        {
            public char Gain;
            public char OffsetThreshold;
            public char View1;
            public char View2;
            public char ViewNumber;
            public double AspectRatio;
            public UInt32 CatenatedFilePointer;
            public UInt16 ColorTableType;
            public UInt32 NumberOfCTEntries;
            public UInt32 ApplicationPacketPointer;
            public UInt32 ApplicationPacketLength;
            public UInt16[] ApplicationData;
        };
        private Header2 _header2;
        private void readHeader2()
        {
            _header2.Gain = _stream.ReadChar();
            _header2.OffsetThreshold = _stream.ReadChar();
            _header2.View1 = _stream.ReadChar();
            _header2.View2 = _stream.ReadChar();
            _header2.ViewNumber = _stream.ReadChar();
            _stream.ReadChar();
            _stream.ReadUInt16();
            _header2.AspectRatio = _stream.ReadDouble();
            _header2.CatenatedFilePointer = _stream.ReadUInt32();
            _header2.ColorTableType = _stream.ReadUInt16();
            _stream.ReadUInt16();
            _header2.NumberOfCTEntries = _stream.ReadUInt32();
            _header2.ApplicationPacketPointer = _stream.ReadUInt32();
            _header2.ApplicationPacketLength = _stream.ReadUInt32();
            _stream.ReadBytes(220);
            _header2.ApplicationData = new UInt16[128];
            for (UInt16 i = 0; i < 128; i++)
                _header2.ApplicationData[i] = _stream.ReadUInt16();

        }

        private byte swapBits(byte aSrc)
        {
            int src = aSrc;
            //            src = (src & 11110000) >> 4 | (src & 00001111) << 4;
            aSrc = (byte)((aSrc & 0xf0) >> 4 | (aSrc & 0x0f) << 4);
            //            src = (src & 11001100) >> 2 | (src & 00110011 << 2;
            aSrc = (byte)((aSrc & 0xCC) >> 2 | (aSrc & 0x33) << 2);
            //            src = src & 10101010 >> 1 | src & 01010101 << 1;
            aSrc = (byte)((aSrc & 0xAA) >> 1 | (aSrc & 0x55) << 1);
            return aSrc;
        }

        private int _rawStride;
        private bool getPixel(int aY, int aX, ref byte[] aData)
        {
            int index = aY * _rawStride + (aX >> 3);
            byte mask = (byte)(0x80 >> (aX & 0x7));
            int n = aData[index] & mask;
            return n > 0 ? true : false;
        }

        private void setPixel(int aY, int aX, ref byte[] aData, bool aWhite)
        {
            int index = aY * _rawStride + (aX >> 3);
            byte mask = (byte)(0x80 >> (aX & 0x7));
            if (aWhite)
                aData[index] |= mask;
            else
                aData[index] &= (byte)(mask ^ 0xff);

        }
        private void setPixels(int aY, int aX, int aCount, ref byte[] aData, bool aWhite)
        {
            if (aCount <= 0) return;
            int index = aY * _rawStride + (aX >> 3);
            int bitPos = (aX & 0x7);
            while (aCount-- > 0 && bitPos < 8)
            {
                byte mask = (byte)(0x80 >> bitPos);
                if (aWhite)
                    aData[index] |= mask;
                else
                    aData[index] &= (byte)(mask ^ 0xff);
                bitPos++;
            }
            aCount++;
            if (aCount <= 0) return;
            while (true)
            {
                if (aCount - 8 < 0)
                    break;
                aCount-=8;
                if (aWhite)
                    aData[++index] =255;
                else
                    aData[++index] =0;
            }
            ++index;
            for(int i=0;i<aCount;i++)
            {
                byte mask = (byte)(0x80 >> i);
                if (aWhite)
                    aData[index] |= mask;
                else
                    aData[index] &= (byte)(mask ^ 0xff);
            }

        }
        private Int32 getRowImageData(ref byte[] aSrc, ref byte[] aDest)
        {
            _rawStride = (int)(_Header.PixelsPerLine * 1 + 7) / 8;
            int x, r, t = 0;
            int ch;
            int c, k = 0;

            int holding = 0;
            int bitptr = 0;
            int srcBytePos = 0;
            int deltaPBValue = (int)(_Header.NumberOfLines / 100);
            int deltaProgressBar = deltaPBValue;

            for (int y = 0; y < _Header.NumberOfLines; y++)
            {
                if (y == deltaProgressBar)
                {
                    if (ProgressBar!=null)
                    {
                        ProgressBar((Int16)(deltaProgressBar / deltaPBValue));
                    }
                    deltaProgressBar += deltaPBValue;
                }
                bool blackwhite = true;
                x = 0;
                int xP = 0;

                if (_Header.DataTypeCode == 23) //Group 4
                {
                    System.Diagnostics.Debug.Assert(false);//todo
                    c = bitptr;

                    while (c < 12) c = c + 8;
                    /*
                          if ((bitptr%8)>=4)
                          { c=12; }
                            else
                          { c=16; }
                    */

                    while (bitptr < c)
                    {
                        ch = swapBits(aSrc[srcBytePos++]);
                        holding = (holding << 8) + ch;
                        bitptr = bitptr + 8;
                    }

                    bitptr = bitptr - c;
                    holding = holding & ((1 << bitptr) - 1);

                    if (bitptr < 8)
                    {
                        ch = swapBits(aSrc[srcBytePos++]);
                        holding = (holding << 8) + ch;
                        bitptr = bitptr + 8;
                    }

                    k = holding >> (bitptr - 1);
                    bitptr = bitptr - 1;
                    holding = holding & ((1 << bitptr) - 1);
                }

                while (x < _Header.PixelsPerLine)
                {
                    /* if ((group==3 && k==0) || group==4) */
                    if (k == 0)
                    {
                        for (t = 0; t < 11; t++)
                        {
                            while (_ccitt_2d_len[t] > bitptr)
                            {
                                ch = swapBits(aSrc[srcBytePos++]);
                                holding = (holding << 8) + ch;
                                bitptr = bitptr + 8;
                            }

                            if (_ccitt_2d_code[t] == (holding >> (bitptr - _ccitt_2d_len[t])))
                            { break; }
                        }

                        if (t == 11)
                        {
                            Console.WriteLine("Error: Uknown CCITT huffman code");
                            return -1;
                        }
                        bitptr = bitptr - _ccitt_2d_len[t];
                        holding = holding & ((1 << bitptr) - 1);
                    }

                    if (k == 1)
                    { }
                    else
                        if (y == 0)
                        {
                            if (t == 0)
                            {
                                for (t = 0; t < _Header.PixelsPerLine - x; t++)
                                    setPixel(y, x + t, ref aDest, blackwhite);
                                break;
                            }
                            else
                            {
                                if (t == 2)
                                {
                                    for (r = 0; r < _Header.PixelsPerLine - x; r++)
                                    {
                                        setPixel(y, x + r, ref aDest, blackwhite);
                                    }
                                    x = x + r;
                                    break;
                                }
                                else
                                    if (t >= 6 && t <= 8)
                                    {
                                        for (r = 0; r < _Header.PixelsPerLine - x - (t - 5); r++)
                                        {
                                            setPixel(y, x + r, ref aDest, blackwhite);
                                            x++;
                                        }
                                        blackwhite=!blackwhite;
                                        continue;
                                    }
                                    else
                                        if (t != 1)
                                        { Console.WriteLine("wtf? t={0}", t); }

                            }
                        }
                        else
                        {
                            if (t == 0)
                            {
                                r=0;
                                for (t = 0; t < _Header.PixelsPerLine - xP; t++)
                                {
                                    if (r == 0 && getPixel(y - 1,xP + t,ref aDest) != blackwhite)
                                    {
                                        r = 1;
                                    }
                                    else if (r == 1 && getPixel(y - 1,xP + t, ref aDest) == blackwhite)
                                    {
                                        r=2;
                                        break;
                                    }
                                }
                                xP += t; ;
                                setPixels(y, x, xP - x, ref aDest, blackwhite);
                                x = xP;
/*                                while (x < xP)
                                {
                                    setPixel(y, x, ref aDest, blackwhite);
                                    x++;
                                }*/
                                continue;
                            }
                            else if (t >= 2 && t <= 8)
                            {
                                for (r = 0; r < _Header.PixelsPerLine - xP; r++)
                                {
                                    if (getPixel(y - 1, xP + r,ref aDest) != blackwhite)
                                    {
                                        break;
                                    }
                                }
                                xP += r;
                                setPixels(y, x, xP - x, ref aDest, blackwhite);
                                x = xP;
/*                                while (x < xP)
                                    {
                                       setPixel(y, x,ref aDest, blackwhite);
                                    x++;
                                    }*/
                                if (t >= 6 && t <= 8)
                                {
                                    for (r = 0; r < -_ccitt_2d_value[t]; r++)
                                    {
                                        setPixel(y, x - r - 1,ref  aDest, blackwhite);
                                    }
                                    x += _ccitt_2d_value[t];
                                }
                                else if (t >= 3 && t <= 5)
                                {
                                    for (r = 0; r < _ccitt_2d_value[t]; r++)
                                    {
                                        setPixel(y, x + r,ref aDest, blackwhite);
                                    }

                                    x += _ccitt_2d_value[t];
                                }
                                if (x != xP)
                                {
                                    r = 0;
                                    while (r < _Header.PixelsPerLine - x)
                                    {
                                        if (getPixel(y - 1, x + r,ref aDest) != blackwhite)
                                            break;
                                        r++;
                                    }
                                    xP = x + r;

                                }
                                blackwhite=!blackwhite;
                                continue;
                            }
                            else if (t == 9 || t == 10)
                            {
                                Console.WriteLine("Error: Extension");
                                return -2;
                            }
                        }
                    for (c = 0; c < 2; c++)
                    {
                        if (blackwhite/* == 1*/)
                        {//white pixels
                            for (t = 0; t < _TOTAL_CODES; t++)
                            {
                                while (_huffman_white_len[t] > bitptr)
                                {
                                    ch = swapBits(aSrc[srcBytePos++]);
                                    holding = (holding << 8) + ch;
                                    bitptr = bitptr + 8;
                                }
                                if (_huffman_white_code[t] == (holding >> (bitptr - _huffman_white_len[t])))
                                { break; }
                            }

                            if (t == _TOTAL_CODES)
                            {
                                Console.WriteLine("Illegal huffman code\n");
                                return -1;
                            }

                            if (t == -1)
                            {
                                return 0;
                            }

                            bitptr = bitptr - _huffman_white_len[t];
                            holding = holding & ((1 << bitptr) - 1);

                            t = _huffman_white_value[t];
                            setPixels(y, x, t, ref aDest, blackwhite);
//                            for (r = 0; r < t; r++)
//                            {
//                                setPixel(y, x + r,ref aDest, blackwhite);
 //                           }
                            x = x + t;
                        }
                        else
                        {//black pixels
                            for (t = 0; t < _TOTAL_CODES; t++)
                            {
                                while (_huffman_black_len[t] > bitptr)
                                {
                                    ch = swapBits(aSrc[srcBytePos++]);
                                    holding = (holding << 8) + ch;
                                    bitptr = bitptr + 8;
                                }

                                if (_huffman_black_code[t] == (holding >> (bitptr - _huffman_black_len[t])))
                                { break; }
                            }

                            if (t == _TOTAL_CODES)
                            {
                                Console.WriteLine("Illegal huffman code");
                                return -1;
                            }

                            if (t == -1)
                            { return 0; }

                            bitptr = bitptr - _huffman_black_len[t];
                            holding = holding & ((1 << bitptr) - 1);

                            t = _huffman_black_value[t];
                            setPixels(y, x, t, ref aDest, blackwhite);
/*                            for (r = 0; r < t; r++)
                            {
                                setPixel(y, x + r,ref aDest, blackwhite);
                            }
 */
                            x = x + t;
                        }

                        System.Diagnostics.Debug.Assert(x <= _Header.PixelsPerLine);
                        if (t < 64) blackwhite=!blackwhite;
                        else
                        { c--; }
                    }
                    xP = x;
                    for (r = 0; r < _Header.PixelsPerLine - xP; r++)
                    {
                        if (getPixel(y - 1, xP + r,ref aDest) == blackwhite)
                            break;
                    }
                    xP += r;
                }

                if (x != _Header.PixelsPerLine)
                {
                    Console.WriteLine("Error: Run length codes under/overran width {0}\n", x);
                    // return -1;
                }

                /*
                if (group==3)
                {
                    if (bitptr>=24)
                    {
                      holding=holding&0xffffff;
                      bitptr=24;
                    }
                      else
                    if (bitptr>=16)
                    {
                      holding=holding&0xffff;
                      bitptr=16;
                    }
                      else
                    if (bitptr>=8)
                    {
                      holding=holding&0xff;
                      bitptr=8;
                    }
                      else
                    {
                      holding=0;
                      bitptr=0;
                    }
                }
                */
            }
            return 0;
        }

        #region Conversion Tables
        const Int32 _TOTAL_CODES = 105;

        Int32[] _huffman_white_code = {
        53,    7,    7,    8,   11,   12,   14,   15,   19,   20, 
         7,    8,    8,    3,   52,   53,   42,   43,   39,   12, 
         8,   23,    3,    4,   40,   43,   19,   36,   24,    2, 
         3,   26,   27,   18,   19,   20,   21,   22,   23,   40, 
        41,   42,   43,   44,   45,    4,    5,   10,   11,   82, 
        83,   84,   85,   36,   37,   88,   89,   90,   91,   74, 
        75,   50,   51,   52,   27,   18,   23,   55,   54,   55, 
       100,  101,  104,  103,  204,  205,  210,  211,  212,  213, 
       214,  215,  216,  217,  218,  219,  152,  153,  154,   24, 
       155,    1,    8,   12,   13,   18,   19,   20,   21,   22, 
        23,   28,   29,   30,   31 };

        Int32[] _huffman_white_len = {
         8,    6,    4,    4,    4,    4,    4,    4,    5,    5, 
         5,    5,    6,    6,    6,    6,    6,    6,    7,    7, 
         7,    7,    7,    7,    7,    7,    7,    7,    7,    8, 
         8,    8,    8,    8,    8,    8,    8,    8,    8,    8, 
         8,    8,    8,    8,    8,    8,    8,    8,    8,    8, 
         8,    8,    8,    8,    8,    8,    8,    8,    8,    8, 
         8,    8,    8,    8,    5,    5,    6,    7,    8,    8, 
         8,    8,    8,    8,    9,    9,    9,    9,    9,    9, 
         9,    9,    9,    9,    9,    9,    9,    9,    9,    6, 
         9,   12,   11,   11,   11,   12,   12,   12,   12,   12, 
        12,   12,   12,   12,   12 };

        Int32[] _huffman_white_value = {
         0,    1,    2,    3,    4,    5,    6,    7,    8,    9, 
        10,   11,   12,   13,   14,   15,   16,   17,   18,   19, 
        20,   21,   22,   23,   24,   25,   26,   27,   28,   29, 
        30,   31,   32,   33,   34,   35,   36,   37,   38,   39, 
        40,   41,   42,   43,   44,   45,   46,   47,   48,   49, 
        50,   51,   52,   53,   54,   55,   56,   57,   58,   59, 
        60,   61,   62,   63,   64,  128,  192,  256,  320,  384, 
       448,  512,  576,  640,  704,  768,  832,  896,  960, 1024, 
      1088, 1152, 1216, 1280, 1344, 1408, 1472, 1536, 1600, 1664, 
      1728,   -1, 1792, 1856, 1920, 1984, 2048, 2112, 2176, 2240, 
      2304, 2368, 2432, 2496, 2560 };

        Int32[] _huffman_black_code = {
        55,    2,    3,    2,    3,    3,    2,    3,    5,    4, 
         4,    5,    7,    4,    7,   24,   23,   24,    8,  103, 
       104,  108,   55,   40,   23,   24,  202,  203,  204,  205, 
       104,  105,  106,  107,  210,  211,  212,  213,  214,  215, 
       108,  109,  218,  219,   84,   85,   86,   87,  100,  101, 
        82,   83,   36,   55,   56,   39,   40,   88,   89,   43, 
        44,   90,  102,  103,   15,  200,  201,   91,   51,   52, 
        53,  108,  109,   74,   75,   76,   77,  114,  115,  116, 
       117,  118,  119,   82,   83,   84,   85,   90,   91,  100, 
       101,    1,    8,   12,   13,   18,   19,   20,   21,   22, 
        23,   28,   29,   30,   31 };

        Int32[] _huffman_black_len = {
        10,    3,    2,    2,    3,    4,    4,    5,    6,    6, 
         7,    7,    7,    8,    8,    9,   10,   10,   10,   11, 
        11,   11,   11,   11,   11,   11,   12,   12,   12,   12, 
        12,   12,   12,   12,   12,   12,   12,   12,   12,   12, 
        12,   12,   12,   12,   12,   12,   12,   12,   12,   12, 
        12,   12,   12,   12,   12,   12,   12,   12,   12,   12, 
        12,   12,   12,   12,   10,   12,   12,   12,   12,   12, 
        12,   13,   13,   13,   13,   13,   13,   13,   13,   13, 
        13,   13,   13,   13,   13,   13,   13,   13,   13,   13, 
        13,   12,   11,   11,   11,   12,   12,   12,   12,   12, 
        12,   12,   12,   12,   12 };

        Int32[] _huffman_black_value = {
         0,    1,    2,    3,    4,    5,    6,    7,    8,    9, 
        10,   11,   12,   13,   14,   15,   16,   17,   18,   19, 
        20,   21,   22,   23,   24,   25,   26,   27,   28,   29, 
        30,   31,   32,   33,   34,   35,   36,   37,   38,   39, 
        40,   41,   42,   43,   44,   45,   46,   47,   48,   49, 
        50,   51,   52,   53,   54,   55,   56,   57,   58,   59, 
        60,   61,   62,   63,   64,  128,  192,  256,  320,  384, 
       448,  512,  576,  640,  704,  768,  832,  896,  960, 1024, 
      1088, 1152, 1216, 1280, 1344, 1408, 1472, 1536, 1600, 1664, 
      1728,   -1, 1792, 1856, 1920, 1984, 2048, 2112, 2176, 2240, 
      2304, 2368, 2432, 2496, 2560 };

        Int32[] _ccitt_2d_code = {
        1,  1,  1,  3,  3,  3,  2,  2,  2,  1,  1 };

        Int32[] _ccitt_2d_len = {
        4,  3,  1,  3,  6,  7,  3,  6,  7, 10, 12 };

        Int32[] _ccitt_2d_value = {
        0,  0,  0,  1,  2,  3, -1, -2, -3,  0,  0 };
        #endregion
    }
}
