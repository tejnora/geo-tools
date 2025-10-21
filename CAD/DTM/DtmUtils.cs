using System.Diagnostics;
using CAD.Canvas;
using CAD.DTM.Configuration;

namespace CAD.DTM
{
    public static class DtmUtils
    {
        public static DtmGraphicElementScaleEnum GetScale(ICanvas canvas)
        {
            var zoom = canvas.getZoom();
            var scale = 1.0 / zoom;
            if (scale <= 500)
                return DtmGraphicElementScaleEnum._500;
            if (scale <= 5000)
                return DtmGraphicElementScaleEnum._5000;
            if (scale <= 10000)
                return DtmGraphicElementScaleEnum._10000;
            if (scale <= 25000)
                return DtmGraphicElementScaleEnum._25000;
            if (scale <= 50000)
                return DtmGraphicElementScaleEnum._50000;
            return DtmGraphicElementScaleEnum._100000;
        }
    }
}
