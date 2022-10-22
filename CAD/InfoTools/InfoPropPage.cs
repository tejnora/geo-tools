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
        #region Constructor
        public InfoPropPage(String aRegistryValue)
            : base(aRegistryValue)
        {

        }
        #endregion
        #region Methods
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
        #endregion
    }
}
