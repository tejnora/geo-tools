using System;
using System.Globalization;
using System.IO;
using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Points;

namespace GeoHelper.Calculations
{
    public class PolygonTraversImporter
    {
        public delegate void SettingCallbackDelagete(string paramName, string value);
        public SettingCallbackDelagete SettingCallback { set; get; }

        public void ImportFromFile(string fileName, PolygonTraverseContext calculationContext)
        {
            var lines = File.ReadAllLines(fileName);
            var sectionType = SectionType.None;
            var wasFirtPoint = false;
            foreach (string line in lines)
            {
                if (line.StartsWith("["))
                {
                    wasFirtPoint = false;
                    var begin = line.IndexOf("[") + 1;
                    var end = line.IndexOf("]", begin);
                    if (end == -1) throw new ArgumentException("Ivalid selectin type.");
                    var selectin = line.Substring(begin, end - begin);
                    switch (selectin.Trim())
                    {
                        case "Header":
                            sectionType = SectionType.Header;
                            break;
                        case "Settings":
                            sectionType = SectionType.Settings;
                            break;
                        case "StartPnt":
                            sectionType = SectionType.StartPnt;
                            break;
                        case "EndPnt":
                            sectionType = SectionType.EndPnt;
                            break;
                        case "MeasuredData":
                            sectionType = SectionType.MeasuredData;
                            break;
                        default:
                            sectionType = SectionType.None;
                            break;
                    }
                    continue;
                }
                if (line.Trim().Length == 0)
                    continue;
                switch (sectionType)
                {
                    case SectionType.None:
                    case SectionType.Header:
                        break;
                    case SectionType.Settings:
                        ParseSttingLine(line);
                        break;
                    case SectionType.StartPnt:
                        {
                            if (!wasFirtPoint)
                            {
                                parseLine(calculationContext.BeginPoint, line);
                                wasFirtPoint = true;
                            }
                            else
                            {
                                var orientace = new OrientationPoint();
                                parseLine(orientace, line);
                                calculationContext.BeginOrientationContext.TableNodes.Add(orientace);
                            }
                        }
                        break;
                    case SectionType.EndPnt:
                        {
                            if (!wasFirtPoint)
                            {
                                parseLine(calculationContext.EndPoint, line);
                                wasFirtPoint = true;
                            }
                            else
                            {
                                var orientace = new OrientationPoint();
                                parseLine(orientace, line);
                                calculationContext.EndOrientationContext.TableNodes.Add(orientace);
                            }
                        }
                        break;
                    case SectionType.MeasuredData:
                        {
                            var measuredData = new MeasuredPoint();
                            parseLine(measuredData, line);
                            calculationContext.MeasuredContext.TableNodes.Add(measuredData);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        void parseLine(IPointImport import, string line)
        {
            var elements = line.Split(',');
            foreach (string element in elements)
            {
                var elementEndIndex = element.IndexOf('=');
                if (elementEndIndex == -1) throw new ArgumentOutOfRangeException();
                var elementName = element.Substring(0, elementEndIndex).Trim();
                ImportExportElementType exportElementType;
                switch (elementName)
                {
                    case "NUM":
                        exportElementType = ImportExportElementType.NUM;
                        break;
                    case "X":
                        exportElementType = ImportExportElementType.X;
                        break;
                    case "Y":
                        exportElementType = ImportExportElementType.Y;
                        break;
                    case "Z":
                        exportElementType = ImportExportElementType.Z;
                        break;
                    case "MEAS":
                        exportElementType = ImportExportElementType.MEAS;
                        break;
                    case "HZ":
                        exportElementType = ImportExportElementType.HZ;
                        break;
                    case "HZ1":
                        exportElementType = ImportExportElementType.HZ1;
                        break;
                    case "HZ2":
                        exportElementType = ImportExportElementType.HZ2;
                        break;
                    case "VERT1":
                        exportElementType = ImportExportElementType.VERT1;
                        break;
                    case "VERT2":
                        exportElementType = ImportExportElementType.VERT2;
                        break;
                    case "DIST1":
                        exportElementType = ImportExportElementType.DIST1;
                        break;
                    case "DIST2":
                        exportElementType = ImportExportElementType.DIST2;
                        break;
                    case "DISABLED":
                        exportElementType = ImportExportElementType.DISABLED;
                        break;
                    case "DH1":
                        exportElementType = ImportExportElementType.DH1;
                        break;
                    case "DH2":
                        exportElementType = ImportExportElementType.DH2;
                        break;
                    case "SIGNAL1":
                        exportElementType = ImportExportElementType.SIGNAL1;
                        break;
                    case "SIGNAL2":
                        exportElementType = ImportExportElementType.SIGNAL2;
                        break;
                    case "IH":
                        exportElementType = ImportExportElementType.IH;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                switch (exportElementType)
                {
                    case ImportExportElementType.MEAS:
                    case ImportExportElementType.NUM:
                        {
                            var beginComma = element.IndexOf('\"', elementEndIndex);
                            if (beginComma == -1) throw new ArgumentOutOfRangeException();
                            var endComma = element.IndexOf('\"', beginComma + 1);
                            if (endComma == -1) throw new ArgumentOutOfRangeException();
                            var elementValue = element.Substring(beginComma + 1, endComma - beginComma - 1);
                            import.SetElement(exportElementType, elementValue);
                        }
                        break;
                    case ImportExportElementType.DISABLED:
                        break;
                    default:
                        {
                            var elementValue = element.Substring(elementEndIndex + 1);
                            import.SetElement(exportElementType, double.Parse(elementValue, CultureInfo.InvariantCulture));
                        }
                        break;
                }
            }
        }

        void ParseSttingLine(string line)
        {
            var equalSign = line.IndexOf("=");
            if (equalSign == -1) throw new ArgumentOutOfRangeException();
            var name = line.Substring(0, equalSign);
            var param = line.Substring(equalSign + 1);
            if (SettingCallback != null)
                SettingCallback(name, param);
        }

        enum SectionType
        {
            None,
            Header,
            Settings,
            StartPnt,
            EndPnt,
            MeasuredData
        }
    }
}
