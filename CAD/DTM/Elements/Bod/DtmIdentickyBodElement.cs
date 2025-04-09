using CAD.DTM.Configuration;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    public class DtmIdentickyBodElement
        : DtmBodBaseElement
    {
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "CisloBodu", CisloBodu);
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
        }
        public override string GetInfoAsString()
        {
            return $"Cislo bodu: {CisloBodu}";
        }

    }
}
