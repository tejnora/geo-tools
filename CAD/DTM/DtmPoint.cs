using CAD.Utils;
using System;
using System.Globalization;

namespace CAD.DTM
{
    public class DtmPoint
        : ICloneable
        , IPointConvexHull
    {
        public DtmPoint()
        {

        }
        public DtmPoint(string y, string x, string z)
        {
            X = double.Parse(y, CultureInfo.InvariantCulture);
            Y = double.Parse(x, CultureInfo.InvariantCulture);
            Z = double.Parse(z, CultureInfo.InvariantCulture);
        }
        public DtmPoint(string y, string x)
        {
            X = double.Parse(y, CultureInfo.InvariantCulture);
            Y = double.Parse(x, CultureInfo.InvariantCulture);
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

        static string NumberPattern = "0.00";
        public string ExportToDtm(int srsDimension)
        {
            switch (srsDimension)
            {
                case 2: return $"{X.ToString(NumberPattern, CultureInfo.InvariantCulture)} {Y.ToString(NumberPattern, CultureInfo.InvariantCulture)}";
                case 3: return $"{X.ToString(NumberPattern, CultureInfo.InvariantCulture)} {Y.ToString(NumberPattern, CultureInfo.InvariantCulture)} {Z.ToString(NumberPattern, CultureInfo.InvariantCulture)}";
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
