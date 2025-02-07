using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    public class DtmPodrobnyBodZPSElement
    : DtmElement
    {
        public override void ExportToDtm(IDtmExporter exporter)
        {
            exporter.AddElement("cmn", "ZapisObjektu", ZapisObjektu.ToString());
            exporter.BeginElement(null, "AtributyObjektu");
            exporter.AddSpolecneAtributyVsechObjektu();
            exporter.AddElement("atr", "UrovenUmisteniObjektuZPS", 0);
            exporter.AddElement("atr", "TridaPresnostiPoloha", 3);
            exporter.AddElement("atr", "TridaPresnostiVyska", 3);
            exporter.AddElement("atr", "ZpusobPorizeniPB_ZPS", 3);
            exporter.AddElement("atr", "CisloBodu", CisloBodu);
            exporter.EndElement();

        }
    }
}
