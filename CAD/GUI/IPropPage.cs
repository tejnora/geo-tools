using CAD.Canvas;

namespace CAD.GUI
{
    public interface IPropPage
    {
        void Load(IDrawObject drawObject);
        void InvalidateCanvas();
    }
}
