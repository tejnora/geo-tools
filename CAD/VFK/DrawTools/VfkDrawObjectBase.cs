using System;
using System.Drawing;
using CAD.Canvas;
using CAD.Canvas.DrawTools;
using VFK;
using VFK.Tables;

namespace CAD.VFK.DrawTools
{
    public abstract class VfkDrawObjectBase : DrawObjectBase, IVFKTool
    {
                public VfkElement VfkElement
        { get; private set; }
        private VfkElement.ElementDrawInfo _drawInfo;
                        public abstract void RegisterObject(IVFKMain aOwner);
        public abstract void DeleteObject(IVFKMain aOwner);
        public virtual VFKSOBRTableItem getActivePoint(uint aIdx)
        {
            throw new UnExpectException();
        }
        public virtual UInt32 TYPPPD_KOD
        {
            get
            {
                return VfkElement.TYPPPD_KOD;
            }
            set { throw new UnExpectException(); }
        }
        public virtual void SetVfkElement(VfkElement vfkElement)
        {
            VfkElement = vfkElement;
            _drawInfo = vfkElement.PritomnyStav;
        }
        public virtual bool GetMustBeConnectedWithSnap()
        {throw new UnExpectException();}
                        public new double Width
        {
            set { throw new UnExpectException(); }
            get
            {
                return _drawInfo.Width;
            }
        }
        public new Color Color
        {
            set { throw new UnExpectException(); }
            get
            {
                return _drawInfo.Color;
            }
        }
        public virtual void Copy(VfkDrawObjectBase acopy)
        {
            base.Copy(acopy);
            VfkElement = acopy.VfkElement;
            _drawInfo = acopy._drawInfo;
        }
            }
}
