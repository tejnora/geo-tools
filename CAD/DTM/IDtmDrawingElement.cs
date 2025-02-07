using CAD.DTM.Configuration;
namespace CAD.DTM
{
    public interface IDtmDrawingElement
    {
        IDtmDrawingGroup Group { get; set; }
        IDtmElement GetDtmElement { get; }
    }
}
