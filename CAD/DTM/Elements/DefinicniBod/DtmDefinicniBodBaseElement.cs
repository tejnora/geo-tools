using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmDefinicniBodBaseElement
        : DtmElement
    {
        public override DtmElementType ElementType => DtmElementType.DefinicniBod;

        protected DtmSpolecneAtributyObjektuDefinicnichBodu SpolecneAtributyObjektuDefinicnichBodu { get; set; }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            ImportSpolecneAtributyObjektuDefinicnichBodu(xmlElement);
        }

        protected void ImportSpolecneAtributyObjektuDefinicnichBodu(XmlElement xmlElement)
        {
            xmlElement = DtmImporter.FindElement(xmlElement, "SpolecneAtributyObjektuDefinicnichBodu");
            if (xmlElement == null)
                return;
            var atributy = new DtmSpolecneAtributyObjektuDefinicnichBodu();
            foreach (XmlElement e in xmlElement)
            {
                switch (e.LocalName)
                {
                    case "UrovenUmisteniObjektuZPS":
                        atributy.UrovenUmisteniObjektuZPS = int.Parse(e.InnerText);
                        break;
                }
            }
            SpolecneAtributyObjektuDefinicnichBodu = atributy;
        }

        protected void ExportSpolecneAtributyObjektuDefinicnichBodu(IDtmExporter exporter)
        {
            exporter.BeginElement("atr", "SpolecneAtributyObjektuDefinicnichBodu");
            exporter.AddElement(null, "UrovenUmisteniObjektuZPS", SpolecneAtributyObjektuDefinicnichBodu.UrovenUmisteniObjektuZPS);
            exporter.EndElement();
        }

    }
}
