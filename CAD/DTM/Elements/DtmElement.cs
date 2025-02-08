using System;
using System.Xml;
using CAD.Canvas;
using CAD.DTM.Gui;
using CAD.VFK;

namespace CAD.DTM.Elements
{
    public class DtmElement
    : IDtmElement
    {
        public char ZapisObjektu { get; set; }
        public string ID { get; set; }
        public IDtmGeometry Geometry { get; set; }
        public string CisloBodu { get; set; }

        public DtmElementSpolecneAtributy SpolecneAtributy { get; set; }
        public DtmSpolecneAtributyZPS SpolecneAtributyZPS { get; set; }
        public DtmSpolecneAtributyObjektuDefinicnichBodu SpolecneAtributyObjektuDefinicnichBodu { get; set; }
        public IDrawObject CreateDrawObject()
        {
            if (Geometry is DtmPointGeometry)
                return new DtmDrawingPointElement(this);
            return new DtmDrawingCurveElement(this);
        }
        public bool IsDeleted { get; set; } = false;
        public bool ExportToOutput => !IsDeleted && ZapisObjektu != 'r';
        public virtual void ExportAttributesToDtm(IDtmExporter exporter)
        {
            throw new NotImplemented();
        }

        public virtual void ImportDtmAttributes(XmlElement xmlElement)
        {

        }
        public char EvaluateZapisObjektuForExportToDtm()
        {
            if (IsDeleted)
            {
                if (ZapisObjektu != 'r')
                    throw new ArgumentOutOfRangeException();
                return 'd';
            }
            if (ZapisObjektu == 'r')
                throw new ArgumentOutOfRangeException();
            return ZapisObjektu;
        }

        public void ExportSpolecneAtributyVsechObjektu(IDtmExporter exporter)
        {
            exporter.BeginElement("atr", "SpolecneAtributyVsechObjektu");
            exporter.AddElement(null, "DatumVkladu", DateTime.Now);
            exporter.AddElement(null, "DatumZmeny", DateTime.Now);
            exporter.EndElement();
        }

        protected void ExportSpolecneAtributyObjektuZPS(IDtmExporter exporter)
        {
            exporter.BeginElement("atr", "SpolecneAtributyObjektuZPS");
            exporter.AddElement(null, "TridaPresnostiPoloha", SpolecneAtributyZPS.TridaPresnostiPoloha);
            exporter.AddElement(null, "TridaPresnostiVyska", SpolecneAtributyZPS.TridaPresnostiVyska);
            exporter.AddElement(null, "UrovenUmisteniObjektuZPS", SpolecneAtributyZPS.UrovenUmisteniObjektuZPS);
            exporter.AddElement(null, "ZpusobPorizeniZPS", SpolecneAtributyZPS.ZpusobPorizeniZPS);
            exporter.EndElement();
        }

        protected void ExportSpolecneAtributyObjektuDefinicnichBodu(IDtmExporter exporter)
        {
            exporter.BeginElement("atr", "SpolecneAtributyObjektuDefinicnichBodu");
            exporter.AddElement(null, "UrovenUmisteniObjektuZPS", SpolecneAtributyObjektuDefinicnichBodu.UrovenUmisteniObjektuZPS);
            exporter.EndElement();
        }

        protected void ImportSpolecneAtributyObjektuDefinicnichBodu(XmlElement xmlElement)
        {
            var spolElem = xmlElement["SpolecneAtributyObjektuDefinicnichBodu"];
            SpolecneAtributyObjektuDefinicnichBodu = new DtmSpolecneAtributyObjektuDefinicnichBodu();
            foreach (XmlElement el in spolElem)
            {
                switch (el.LocalName)
                {
                    case "UrovenUmisteniObjektuZPS":
                        SpolecneAtributyObjektuDefinicnichBodu.UrovenUmisteniObjektuZPS = int.Parse(el.InnerText);
                        break;
                }
            }
        }
        public string ZapisObjektuPopis
        {
            get
            {
                switch (ZapisObjektu)
                {
                    case 'i': return "Insert";
                    case 'u': return "Update";
                    case 'd': return "Delete";
                    case 'r': return "Reference";
                }
                throw new NotImplemented();
            }
        }
    }
}
