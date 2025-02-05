using System;
using CAD.Canvas;

namespace CAD.DTM
{
    public class DtmElement
    : IDtmElement
    {
        public int ZpusobPorizeniZPS { get; set; }
        public int UrovenUmisteniObjektuZPS { get; set; }
        public char ZapisObjektu { get; set; }
        public DateTime DatumVkladu { get; set; }
        public DateTime DatumZmeny { get; set; }
        public int TridaPresnostiPoloha { get; set; }
        public int TridaPresnostiVyska { get; set; }
        public double VyskaNaTerenu { get; set; }

        public IDtmGeometry Geometry { get; set; }
        public string ID { get; set; }
        public string CisloBodu { get; set; }

        public IDrawObject CreateDrawObject()
        {
            if(Geometry is DtmPointGeometry)
                return new DtmDrawingPointElement(this);
            return new DtmDrawingCurveElement(this);
        }
    }
}
