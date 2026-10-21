using System.Xml;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    public class DtmSchodisteDefinicniBodElement
        : DtmDefinicniBodBaseElement
    {
        public DtmDruhSchodisteEnum DruhSchodiste { get; set; }

        public override string GetInfoAsString()
        {
            return $"Druh schodiste: {DruhSchodiste}";
        }
    }
}
