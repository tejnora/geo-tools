using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Tabulky;

namespace GeoHelper.FileParses
{
    internal class GromaCrdFileParser
        :GromaParserBase
    {
        enum PointAge
        {
            Actual = 0,
            Old = 1
        };

        [Flags]
        enum Flags
        {
            Lock = 0x1,
            Flag1 = 0x2,
            Flag2 = 0x4,
            Flag3 = 0x8,
            Flag4 = 0x10,
            Flag5 = 0x20,
            Flag6 = 0x40,
            Flag7 = 0x80
        }

        public List<TableNodesBase> Nodes { get; private set; }

        public void ParseFile(FileInfo fileName)
        {
            var indexes = ReadIndexFile(fileName);
            Nodes = new List<TableNodesBase>();
            using (var stream = File.OpenRead(fileName.FullName))
            {
                if (stream.ReadByte() != 0x047 || stream.ReadByte() != 0x52 || stream.ReadByte() != 0x4D || stream.ReadByte() != 0x32)
                    throw new ArgumentException("File is not valid.");
                //stream.Seek(0x0419C, SeekOrigin.Begin);
                var pointBuffer = new byte[128];
                foreach (var index in indexes)
                {
                    stream.Seek(index, SeekOrigin.Begin);
                    if (stream.Read(pointBuffer, 0, 128) != 128)
                        throw new ArgumentException("File is not valid.");
                    if ((PointAge)pointBuffer[0] == PointAge.Old)
                        continue;
                    Nodes.Add(ParsePoint(pointBuffer));
                }
            }
        }

        IEnumerable<long> ReadIndexFile(FileInfo fileName)
        {
            var indexFileName = Path.ChangeExtension(fileName.FullName, ".crx");
            var indexes = new List<Int64>();
            using (var stream = File.OpenRead(indexFileName))
            {
                if (stream.ReadByte() != 0x047 || stream.ReadByte() != 0x52 || stream.ReadByte() != 0x4D || stream.ReadByte() != 0x32)
                    throw new ArgumentException("File is not valid.");
                stream.Seek(0x3856, SeekOrigin.Begin);
                var pointBuffer = new byte[36];
                while (stream.Position < stream.Length)
                {
                    if (stream.Read(pointBuffer, 0, 36) != 36)
                        throw new ArgumentException("File is not valid.");
                    //var predcisli = ReadInt32(pointBuffer, 2);
                    //var cislo = ReadCompressNumber(pointBuffer, 6);
                    var offset = ReadInt64(pointBuffer, 28);
                    indexes.Add(offset);
                }
            }
            return indexes;
        }

        static TableNodesBase ParsePoint(byte[] pointBuffer)
        {
            var point = new TableCoordinateListNode();
            var flags = (Flags)pointBuffer[1];
            point.Locked = (flags & Flags.Lock) == Flags.Lock;
            point.Prefix = ReadInt32(pointBuffer, 6).ToString(CultureInfo.InvariantCulture);
            point.Number = ReadCompressNumber(pointBuffer, 10);
            //28-31 create dateTime
            //32-35 modified dateTime
            //36 type dos/novy
            if (pointBuffer[37] != 0)
                point.Quality = (uint)(pointBuffer[37] - (byte)'0');
            point.X = ReadDouble(pointBuffer, 38);
            point.Y = ReadDouble(pointBuffer, 46);
            point.Z = ReadDouble(pointBuffer, 54);
#if DEBUG
            var anything = new byte[6];
            Array.Copy(pointBuffer, 62, anything, 0, 6);
#endif
            point.Description = ReadString(pointBuffer, 68, 18);
            return point;
        }
    }
}
