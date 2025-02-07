using System;
using CAD.Canvas;
using CAD.DTM.Gui;
using CAD.VFK;

namespace CAD.DTM.Elements
{
    public class DtmElement
    : IDtmElement
    {
        public int ZpusobPorizeniZPS { get; set; }
        public int UrovenUmisteniObjektuZPS { get; set; }
        public char ZapisObjektu { get; set; }
        public int TridaPresnostiPoloha { get; set; }
        public int TridaPresnostiVyska { get; set; }
        public double VyskaNaTerenu { get; set; }

        public IDtmGeometry Geometry { get; set; }
        public string ID { get; set; }
        public string CisloBodu { get; set; }

        public IDrawObject CreateDrawObject()
        {
            if (Geometry is DtmPointGeometry)
                return new DtmDrawingPointElement(this);
            return new DtmDrawingCurveElement(this);
        }
        public bool IsDeleted { get; set; } = false;
        public bool ExportToOutput => !IsDeleted && ZapisObjektu != 'r';
        public virtual void ExportToDtm(IDtmExporter exporter)
        {
            //throw new NotImplemented();
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
