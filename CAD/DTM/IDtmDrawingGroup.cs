using CAD.DTM.Configuration;

namespace CAD.DTM
{
    public interface IDtmDrawingGroup
    {
        DtmElementOption Options { get; set; }
        string Name { get; set; }
    }
}
