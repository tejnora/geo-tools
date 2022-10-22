using System.Drawing;
using CAD.Canvas;
using GeoBase.Utils;

namespace CAD.Export
{
    public interface IExport
    {
        void Init();
        void Finish();
        void SinkLayer(ICanvasLayer layer);
        void SetTopLayerName(string name);
        string GetTopLayerName();
        void RiseLayer();

        void AddLine(UnitPoint p1, UnitPoint p2, Color color, double width);
        void AddPolyline(ref UnitPoint[] points, Color color, double width);
        void AddArc(UnitPoint centerPoint, double radius, double startAngle, double endAngle, Color color, double width);
        void AddPath(PathImpl path, Color color, double width);
        void AddText(string text, double x, double y, double height, string styleName, Color color, double rotation);
        string AddStyle(bool shape, double height, double width, double obliqueAngle, bool backward, bool upsidedown,
                        double lastHeightUsed, string primaryFontFile);
        void AddPoint(double x, double y, Color color);
    }
}
