using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Points;

namespace GeoHelper.Calculations
{
    internal class CalculationExporter : IPointExporter
    {
        readonly StringBuilder _line = new StringBuilder();
        List<string> _lines;
        public IPointExport Settings { get; set; }

        public void AddEmptyLine()
        {
            _lines.Add("");
        }

        public void AddValue(ImportExportElementType type, object value)
        {
            if (type == ImportExportElementType.NUM || type == ImportExportElementType.MEAS)
            {
                _line.Append(type + "=\"" + (string)value + "\",");
            }
            else if (type == ImportExportElementType.DISABLED)
            {
                //todo
            }
            else
            {
                var doubleValue = (double)value;
                if (double.IsNaN(doubleValue) || double.IsInfinity(doubleValue)) return;
                _line.Append(string.Format("{0}={1},", type, doubleValue.ToString(CultureInfo.InvariantCulture)));
            }
        }

        public void FlushValues()
        {
            if (_line.Length > 0)
                _line.Length = _line.Length - 1;
            _lines.Add(_line.ToString());
            _line.Length = 0;
        }

        public void AddSettingValue(string name, string value)
        {
            _lines.Add(name + "=" + value);
        }

        public void ExportToFile(string fileName, PolygonTraverseContext calculationContext)
        {
            _lines = new List<string>();
            _lines.Add("[Header]");
            _lines.Add(string.Format("Application=Geo Helper - {0}", Assembly.GetExecutingAssembly().GetName().Version));
            AddEmptyLine();
            _lines.Add("[Settings]");
            if (Settings != null)
                Settings.Export(this);
            AddEmptyLine();
            _lines.Add("[StartPnt]");
            calculationContext.BeginPoint.Export(this);
            FlushValues();
            foreach (OrientationPoint node in calculationContext.BeginOrientationContext.TableNodes)
            {
                node.Export(this);
                FlushValues();
            }
            AddEmptyLine();
            _lines.Add("[EndPnt]");
            calculationContext.EndPoint.Export(this);
            FlushValues();
            foreach (OrientationPoint node in calculationContext.EndOrientationContext.TableNodes)
            {
                node.Export(this);
                FlushValues();
            }
            AddEmptyLine();
            _lines.Add("[MeasuredData]");
            foreach (MeasuredPoint node in calculationContext.MeasuredContext.TableNodes)
            {
                node.Export(this);
                FlushValues();
            }
            File.WriteAllLines(fileName, _lines);
        }
    }
}