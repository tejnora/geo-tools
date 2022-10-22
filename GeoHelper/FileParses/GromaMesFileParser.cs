using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using GeoHelper.Tables.TableNodes;

namespace GeoHelper.FileParses
{
    class GromaMesFileParser
        : GromaParserBase
    {
        public List<TableNodesBase> Nodes { get; private set; }

        enum Flags
        {
            Deleted = 0x1,
            Flag1 = 0x2,
            Orientation = 0x4,
            PointOfView = 0x8,
            Flag4 = 0x10,
            Flag5 = 0x20,
            Flag6 = 0x40,
            Flag7 = 0x80
        }


        public void ParseFile(FileInfo fileName)
        {
            Nodes = new List<TableNodesBase>();
            using (var stream = File.OpenRead(fileName.FullName))
            {
                if (stream.ReadByte() != 0x047 || stream.ReadByte() != 0x52 || stream.ReadByte() != 0x4D ||
                    stream.ReadByte() != 0x32)
                    throw new ArgumentException("File is not valid.");
                stream.Seek(0x425A, SeekOrigin.Begin);
                var pointBuffer = new byte[128];
                while (stream.Position < stream.Length)
                {
                    if (stream.Read(pointBuffer, 0, 128) != 128)
                        throw new ArgumentException("File is not valid.");
                    var newNode = new TableMeasureListNode();
                    var flags = (Flags)pointBuffer[0];
                    if ((flags & Flags.Deleted) == Flags.Deleted) continue;
                    if ((flags & Flags.Orientation) == Flags.Orientation)
                        newNode.PointType = TableMeasureListNode.PointTypes.Orientation;
                    else if ((flags & Flags.PointOfView) == Flags.PointOfView)
                        newNode.PointType = TableMeasureListNode.PointTypes.PointOfView;
                    newNode.Prefix = ReadInt32(pointBuffer, 6).ToString(CultureInfo.InvariantCulture);
                    newNode.Number = ReadCompressNumber(pointBuffer, 10);//10-25
                    //26-27
                    newNode.ZenitAngle = FromRadianToGrad(ReadDouble(pointBuffer, 70)); //70-77
                    newNode.Hz = FromRadianToGrad(ReadDouble(pointBuffer, 62));//62-69
                    newNode.HorizontalDistance = ReadDouble(pointBuffer, 78);//78-85
                    newNode.Signal = ReadDouble(pointBuffer, 86);//86-93
                    newNode.ElevationDefference = ReadDouble(pointBuffer, 94);//94-100
                    newNode.Description = ReadString(pointBuffer, 103, 20);//103-122
                    Nodes.Add(newNode);
                }

            }
        }

    }
}
