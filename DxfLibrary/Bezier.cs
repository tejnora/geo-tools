using GeoCalculations.BasicTypes;

namespace DxfLibrary
{
    public class Bezier : PolyLine
    {
        public Bezier(string layer)
            : base(layer, CurvesSmoothSurfaceType.Bezier)
        {

        }

        public void AddBezier(Path2D points)
        {
            var linePoints = points.ToLineOnly(6e-3);
            for (var i = 0; i < linePoints.Count; i++)
            {
                AddVertex(linePoints[i].X, linePoints[i].Y);
            }
        }
    }
}
