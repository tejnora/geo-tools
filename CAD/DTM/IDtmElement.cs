using CAD.Canvas;

namespace CAD.DTM
{
    public interface IDtmElement
    {
        IDrawObject CreateDrawObject();
        bool IsDeleted { get; set; }
    }
}
