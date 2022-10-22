
namespace DxfLibrary
{
    public class PolyLine : Entity
    {
        public enum CurvesSmoothSurfaceType
        {
            NoSmooth = 0,
            QuadraticBSpline = 5,
            CubicBSpline = 6,
            Bezier = 8
        }

        public PolyLine(string layer, CurvesSmoothSurfaceType smoothSurface)
            : base("POLYLINE", layer)
        {
            DataAcceptanceList.AddRange(new[] { 66, 10, 20, 30, 39, 70, 40, 41, 71, 72, 73, 74, 75, 210, 220, 230, 66 });
            AddElement(new SeqEnd(layer));
            AddReplace(66, (short)1);
            AddReplace(75, (short)smoothSurface);
        }
        public void AddVertex(Vertex v)
        {
            InsertElement(ElementCount() - 1, v);
        }
        public void AddVertex(double x, double y)
        {
            AddVertex(new Vertex(x, y, Layer));
        }
    }

}
