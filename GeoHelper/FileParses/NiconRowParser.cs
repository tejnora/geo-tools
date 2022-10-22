using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GeoBase.Localization;

namespace GeoHelper.FileParses
{
    internal class NiconRowParser
    {
        readonly List<IRecord> _records = new List<IRecord>();

        public List<IRecord> Records
        {
            get { return _records; }
        }

        public void ParseFile(FileInfo location)
        {
            string[] lines = File.ReadAllLines(location.FullName, Encoding.ASCII);
            parseLines(lines);
        }

        public void parseLines(string[] lines)
        {
            foreach (string line in lines)
            {
                if (line.Length == 0)
                    continue;
                if (line.StartsWith("ST"))
                {
                    _records.Add(new SationRecord(line));
                }
                else if (line.StartsWith("CP"))
                {
                    _records.Add(new ControlPointRecord(line));
                }
                else if (line.StartsWith("SS"))
                {
                    _records.Add(new SideshotRecord(line));
                }
                else if (line.StartsWith("UP"))
                {
                    _records.Add(new UserPointRecord(line));
                }
                else if (line.StartsWith("CO"))
                {
                }
                else
                {
                    LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                    object res = dictionary.Translate("4", "Text", "Bad file format.", typeof (string));
                    throw new ParseException((string) res);
                }
            }
        }
    }

    internal class SationRecord : IRecord, IStationRecord
    {
        public SationRecord(string value)
        {
            string[] items = value.Split(',');
            if (items.Length != 8 || items[0] != "ST")
            {
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                object res = dictionary.Translate("4", "Text", "File can not be opened.", typeof (string));
                throw new ParseException((string) res);
            }
            try
            {
                if (items[1].Length > 0)
                    StationPointNumber = items[1];
                if (items[2].Length > 0)
                    BacksightPointNumber = UInt32.Parse(items[2]);
                if (items[3].Length > 0)
                    HeightOfInstrument = Double.Parse(items[3], CultureInfo.InvariantCulture);
                if (items[4].Length > 0)
                    BacksightAzimuth = Double.Parse(items[4], CultureInfo.InvariantCulture);
                if (items[5].Length > 0)
                    BacksightHorizontalAngle = Double.Parse(items[5], CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                object res = dictionary.Translate("4", "Text", "File can not be opened.", typeof (string));
                throw new ParseException((string) res);
            }
        }

        public UInt32 BacksightPointNumber { get; set; }
        public double BacksightAzimuth { get; set; }
        public double BacksightHorizontalAngle { get; set; }
        public string StationPointNumber { get; set; }
        public double HeightOfInstrument { get; set; }
    }

    internal class ControlPointRecord : IRecord
    {
        public ControlPointRecord(string value)
        {
            string[] items = value.Split(',');
            if (items.Length != 8 || items[0] != "CP")
            {
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                object res = dictionary.Translate("4", "Text", "File can not be opened.", typeof (string));
                throw new ParseException((string) res);
            }
            try
            {
                if (items[1].Length > 0)
                    PointNumber = items[1];
                if (items[2].Length > 0)
                    HeightOfTarget = Double.Parse(items[2], CultureInfo.InvariantCulture);
                if (items[3].Length > 0)
                    SlopeDistance = Double.Parse(items[3], CultureInfo.InvariantCulture);
                if (items[4].Length > 0)
                    HorizontalAngle = Double.Parse(items[4], CultureInfo.InvariantCulture);
                if (items[5].Length > 0)
                    VerticalAngle = Double.Parse(items[5], CultureInfo.InvariantCulture);
                if (items[6].Length > 0)
                    HourTimeStamp = items[6];
                if (items[7].Length > 0)
                    FeatureCode = items[7];
            }
            catch (Exception)
            {
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                object res = dictionary.Translate("4", "Text", "File can not be opened.", typeof (string));
                throw new ParseException((string) res);
            }
        }

        public string PointNumber { get; set; }
        public double HeightOfTarget { get; set; }
        public double SlopeDistance { get; set; }
        public double HorizontalAngle { get; set; }
        public double VerticalAngle { get; set; }
        public string HourTimeStamp { get; set; }
        public string FeatureCode { get; set; }
    }

    internal class SideshotRecord : IRecord, INodeSideshot
    {
        public SideshotRecord(string value)
        {
            string[] items = value.Split(',');
            if (items.Length != 8 || items[0] != "SS")
            {
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                object res = dictionary.Translate("4", "Text", "File can not be opened.", typeof (string));
                throw new ParseException((string) res);
            }
            try
            {
                if (items[1].Length > 0)
                    PointNumber = items[1];
                if (items[2].Length > 0)
                    HeightOfTarget = Double.Parse(items[2], CultureInfo.InvariantCulture);
                if (items[3].Length > 0)
                    SlopeDistance = Double.Parse(items[3], CultureInfo.InvariantCulture);
                if (items[4].Length > 0)
                    HorizontalAngle = Double.Parse(items[4], CultureInfo.InvariantCulture);
                if (items[5].Length > 0)
                    VerticalAngle = Double.Parse(items[5], CultureInfo.InvariantCulture);
                if (items[6].Length > 0)
                    HourTimeStamp = items[6];
                if (items[7].Length > 0)
                    FeatureCode = items[7];
            }
            catch (Exception)
            {
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                object res = dictionary.Translate("4", "Text", "File can not be opened.", typeof (string));
                throw new ParseException((string) res);
            }
        }

        public Double HorizontalAngle { get; set; }
        public string HourTimeStamp { get; set; }
        public string FeatureCode { get; set; }
        public String PointNumber { get; set; }
        public Double HeightOfTarget { get; set; }
        public Double SlopeDistance { get; set; }
        public Double VerticalAngle { get; set; }

        public Double Hz
        {
            get { return HorizontalAngle; }
            set { }
        }

        public double Z
        {
            get { return VerticalAngle; }
            set { }
        }
    }

    public class UserPointRecord : IRecord, ISouradnice
    {
        public UserPointRecord(string value)
        {
            string[] items = value.Split(',');
            if (items.Length != 7 || items[0] != "UP")
            {
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                object res = dictionary.Translate("4", "Text", "File can not be opened.", typeof (string));
                throw new ParseException((string) res);
            }
            try
            {
                if (items[1].Length > 0)
                    PointNumber = items[1];
                if (items[3].Length > 0)
                    Y = Double.Parse(items[3], CultureInfo.InvariantCulture);
                if (items[4].Length > 0)
                    X = Double.Parse(items[4], CultureInfo.InvariantCulture);
                if (items[5].Length > 0)
                    Z = Double.Parse(items[5], CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                object res = dictionary.Translate("4", "Text", "File can not be opened.", typeof (string));
                throw new ParseException((string) res);
            }
        }

        public string PointNumber { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}