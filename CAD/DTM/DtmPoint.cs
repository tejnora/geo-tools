using System;

namespace CAD.DTM
{
    public class DtmPoint : ICloneable
    {
        public DtmPoint()
        {

        }
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

        public DtmPoint FlipValues()
        {
            return new DtmPoint { X = -Y, Y = -X, Z = Z };
        }

        public string ExportToDtm(int srsDimension)
        {
            switch (srsDimension)
            {
                case 2: return $"{X:##.00} {Y:##.00}";
                case 3: return $"{X:##.00} {Y:##.00} {Z:##.00}";
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
