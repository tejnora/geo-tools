using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CAD.DTM.Elements;

namespace CAD.DTM
{
    public class DtmIdentickeBodyProtocolBuilder
    {
        class Data
        {
            public string RefPointNumber { get; set; }
            public DtmPoint RefPoint { get; set; }
            public string MezPointNumber { get; set; }
            public DtmPoint MezPoint { get; set; }
        }
        List<Data> _data = new List<Data>();

        readonly IDtmMain _main;
        StringBuilder _sb = new StringBuilder();
        public DtmIdentickeBodyProtocolBuilder(IDtmMain main)
        {
            _main = main;
        }

        public string CreateProtocol()
        {
            //InitTestData();
            Init();
            AddPoints();
            AddRozidlySouradnic();
            AddVyskaBody();
            return _sb.ToString();
        }
        void AddPoints()
        {
            AddLine("č.b.(pův)     Y(pův)     X(pův) Z(pův) č.b.(měř)   Y(měř)     X(měř) Z(měř)");
            foreach (var d in _data)
            {
                AddLine($"{d.RefPointNumber.PadLeft(9)} {d.RefPoint.Y:#####0.00} {d.RefPoint.X:######0.00} {d.RefPoint.Z:###0.00} " +
                        $"{d.MezPointNumber.PadLeft(9)} {d.MezPoint.Y:#####0.00} {d.MezPoint.X:######0.00} {d.MezPoint.Z:###0.00} ");
            }
            AddLineSeparator();
        }
        void AddRozidlySouradnic()
        {
            AddLine("Rozdíly souřadnic (pův)-(měř)");
            AddLineSeparator();
            AddLine("dvojice delta(y) delta(x) poloh.odchylka delta(p)<0.239 pro 3.tř.př.");
            var i = 1;
            foreach (var data in _data)
            {
                var dy = data.RefPoint.Y - data.MezPoint.Y;
                var dx = data.RefPoint.X - data.MezPoint.X;
                var p = Math.Sqrt(Math.Pow(dy, 2) + Math.Pow(dx, 2));
                AddLine($"{i:0000000} {dy:0.000} {dx:0.000} {p:0.000}");
                i++;
            }
            AddLineSeparator();
            AddLine("Kritérium přesnosti souřadnic pro 3.tř.př.: sigma(xy)=0.14m");
            var sx = SmerodatnaOdchylka(_data.Select(n => (n.RefPoint.X - n.MezPoint.X)).ToArray());
            var sy = SmerodatnaOdchylka(_data.Select(n => (n.RefPoint.Y - n.MezPoint.Y)).ToArray());
            var sxy = (sx + sy) / 2.0;
            AddLine($"směrodatné odchylky souřadnic pro měření o stejné přesnosti (k=2): Sx={sy:0.###}, Sy={sx:0.###}");
            AddLine($"výběrová směrodatná souřadnicová odchylka pro měření o stejné přesnosti: Sxy={sxy:0.###}");
            AddLineSeparator();
        }

        void AddVyskaBody()
        {
            AddLine("Rozdíly výšek (pův)-(měř)");
            AddLineSeparator();
            AddLine("dvojice delta(H) < 0.241m pro zp.povrch a vyšší přesnost < 0.34m pro zp.povrch a shod. přesnost");
            var i = 1;
            var sum = 0.0;
            foreach (var data in _data)
            {
                var dz = data.RefPoint.Z - data.MezPoint.Z;
                AddLine($"{i:0000000} {dz:0.000}");
                sum += dz;
            }
            AddLine("-----------------------");
            AddLine($"průměr {(sum / _data.Count):0.###}");
            AddLineSeparator();
            AddLine($"Kritérium přesnosti výšek pro 3.tř.př.: sigma(H)=0.12m pro zpev.povrch (0.36m pro nezp.povrch)");
            AddLineSeparator();
            var sh = SmerodatnaOdchylka(_data.Select(n => n.RefPoint.Z - n.MezPoint.Z).ToArray());
            AddLine($"výběrová směrodatná výšková odchylka pro měření o stejné přesnosti (k=2): Sh={sh:0.###}");
        }

        double SmerodatnaOdchylka(double[] values)
        {
            //*
            var prumer = values.Sum() / values.Length;
            var res = 0.0;
            foreach (var value in values)
            {
                var div = Math.Pow(prumer - value, 2);
                res += div;
            }

            res /= (values.Length - 1);
            return Math.Sqrt(res);
            /*/
            var res = 0.0;
            foreach (var value in values)
            {
                var div = Math.Pow(value, 2);
                res += div;
            }

            res = res / values.Length;
            return Math.Sqrt(res);
            //*/
        }
        void InitTestData()
        {
            _data.Add(DataFromString("2 646096.00 1060561.04 219.94 2 646095.99 1060561.02 219.99"));
            _data.Add(DataFromString("26 646084.80 1060566.93 220.00 26 646084.81 1060566.91 219.93"));
            _data.Add(DataFromString("29 646091.41 1060577.48 220.08 29 646091.51 1060577.44 220.03"));
            _data.Add(DataFromString("3 646096.00 1060574.67 219.87 3 646096.01 1060574.66 219.83"));
            _data.Add(DataFromString("27 646093.42 1060569.51 219.95 27 646093.45 1060569.50 219.89"));
            _data.Add(DataFromString("1 646099.77 1060562.14 219.85 1 646099.78 1060562.13 219.88"));
        }

        Data DataFromString(string data)
        {
            var items = data.Split(' ');
            return new Data
            {
                RefPointNumber = items[0],
                RefPoint = new DtmPoint(items[2], items[1], items[3]),
                MezPointNumber = items[4],
                MezPoint = new DtmPoint(items[6], items[5], items[7])
            };
        }
        void Init()
        {
            foreach (var points in _main.IndeticalPointsMapping)
            {
                var refPoint = _main.GetIdentickyBod(points.Key, true);
                var measurePoint = _main.GetIdentickyBod(points.Value, false);
                var refP = GetPoint(refPoint);
                var mesP = GetPoint(measurePoint);
                _data.Add(new Data()
                {
                    MezPoint = mesP.FlipValues(),
                    MezPointNumber = measurePoint.CisloBodu,
                    RefPoint = refP.FlipValues(),
                    RefPointNumber = refPoint.CisloBodu
                });
            }
        }
        DtmPoint GetPoint(DtmIdentickyBodElement element)
        {
            return ((DtmPointGeometry)element.Geometry).Point;
        }
        void AddLine(string line)
        {
            _sb.Append(line + "\r\n");
        }

        void AddLineSeparator()
        {
            AddLine("---------------------------------------------------------------------------------------------------");
        }
    }
}
