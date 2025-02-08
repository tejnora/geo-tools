using CAD.DTM.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmPlotElement
        : DtmElement
    {
        public int DruhPlotu { get; set; }
        public bool HraniceJinehoObjektu { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "DruhPlotu", DruhPlotu);
            exporter.AddElement("atr", "HraniceJinehoObjektu", HraniceJinehoObjektu);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "DruhPlotu":
                        DruhPlotu = int.Parse(x.InnerText);
                        break;
                    case "HraniceJinehoObjektu":
                        HraniceJinehoObjektu = bool.Parse(x.InnerText);
                        break;
                }
            }
        }

    }
}
