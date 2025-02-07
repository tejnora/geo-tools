using System;

namespace CAD.DTM
{
    class DtmPoint : ICloneable
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
    }
}
