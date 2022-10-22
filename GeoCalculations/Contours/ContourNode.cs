using GeoCalculations.BasicTypes;
using GeoCalculations.ConvexHull;

namespace GeoCalculations.Contours
{
    public class ContourNode : IVertex
    {
        public ContourNode(string pointNumber, double cordinateX, double cordinateY, double height)
        {
            PointNumber = pointNumber;
            Position = new[] { cordinateX, cordinateY };
            Height = height;
        }

        public double[] Position
        {
            get;
            set;
        }

        public double Height { get; private set; }
        public double CordinateX { get { return Position[0]; } }
        public double CordinateY { get { return Position[1]; } }
        public Point PositionWithHeight
        {
            get { return new Point(Position[0], Position[1], Height); }
        }
        public string PointNumber { get; private set; }
    }
}
