using System;

namespace CAD.DTM
{
    public class DtmPoint : ICloneable
    {
        public DtmPoint(string y, string x, string z)
        {
            X = double.Parse(y);
            Y = double.Parse(x);
            Z = double.Parse(z);
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }

        public string ExportToDtm()
        {
            return $"{Y:##.00} {X:##.00} {Z:##.00}";
        }
    }
}
