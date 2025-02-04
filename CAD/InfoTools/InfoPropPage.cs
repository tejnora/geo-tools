using System;
using System.Windows;
using CAD.Canvas;
using CAD.GUI;
using GeoBase.Utils;

namespace CAD.InfoTools
{
    public class InfoPropPage
        : PpWindow
    {
                public InfoPropPage(String aRegistryValue)
            : base(aRegistryValue)
        {

        }
                        public virtual void Draw(ICanvas canvas )
        {
            
        }
        public virtual void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
        }
        public virtual Rect GetBoundingRect(ICanvas canvas)
        {
            return new Rect();            
        }
            }
}
