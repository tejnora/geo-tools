using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VFK.Tables;
using CAD.Utils;
using CAD.Utils.Cloning;

namespace VFK
{
    public interface IVFKSimpleMark
    {
        UInt32 TYPPPD_KOD { get; set; }
        double SOURADNICE_Y { get; set; }
        double SOURADNICE_X { get; set; }
        bool GetMustBeConnectedWithSnap();
    }

    public class VFKSimpleDPMMark : BaseObject, IVFKRegisterIface, IVFKSimpleMark
    {
        public VFKSimpleDPMMark(VFKDPMTableItem aDPM, VFKSOBRTableItem aSOBR)
        {
            DPM = aDPM;
            SOBR = aSOBR;
            TYPPPD_KOD = DPM.TYPPPD_KOD;
        }
        public VFKSimpleDPMMark()
        {
        }
        private VFKDPMTableItem DPM
        {
            get;
            set;
        }
        private VFKSOBRTableItem SOBR
        {
            get;
            set;
        }
        #region IVFKSimpleMark
        public UInt32 TYPPPD_KOD
        {
            get;
            set;
        }
        public double SOURADNICE_X
        {
            get
            {
                if (SOBR != null)
                    return SOBR.SOURADNICE_X;
                return DPM.SOURADNICE_X;
            }
            set 
            {
                if (SOBR != null)
                    SOBR.SOURADNICE_X = value;
                else
                    DPM.SOURADNICE_X = value;
            }
        }
        public double SOURADNICE_Y
        {
            get
            {
                if (SOBR != null)
                    return SOBR.SOURADNICE_Y;
                return DPM.SOURADNICE_Y;
            }
            set
            {
                if (SOBR != null)
                    SOBR.SOURADNICE_Y = value;
                else
                    DPM.SOURADNICE_Y = value;
            }
        }
        public bool GetMustBeConnectedWithSnap()
        {
            if (TYPPPD_KOD == 105)//Hraniční znak
                return true;
            return false;
        }
        #endregion
        #region IVFKRegisterIface
        public void RegisterSegment(IVFKMain aOwner)
        {
            VFKElement vkfType = Singletons.VFKElements.getElement(TYPPPD_KOD);
            if (vkfType.DPM_TYPE == DPM_TYPE.BPP)
            {
                if (DPM == null)
                    DPM = aOwner.getNewDPM();
                else
                    DPM = aOwner.UpdateDPM(DPM);
                DPM.TYPPPD_KOD = TYPPPD_KOD;
                DPM.DPM_TYPE = vkfType.DPM_TYPE.ToString();
            }
        }
        public void DeleteSegment(IVFKMain aOwner)
        {
            VFKElement vkfType = Singletons.VFKElements.getElement(TYPPPD_KOD);
            if (vkfType.DPM_TYPE == DPM_TYPE.BPP)
            {
                aOwner.DeleteDPM(DPM);
                DPM = null;
            }
        }
        #endregion IVFKRegisterIface
    }

    public class VFKSimpleOPMark : BaseObject, IVFKRegisterIface, IVFKSimpleMark
    {
        public VFKSimpleOPMark(VFKOPTableItem aOP)
        {
            OP = aOP;
            TYPPPD_KOD = OP.TYPPPD_KOD;
        }
        public VFKSimpleOPMark()
        {
        }
        private VFKOPTableItem OP
        {
            get;
            set;
        }
        #region IVFKSimpleMark
        public UInt32 TYPPPD_KOD
        {
            get;
            set;
        }
        public double SOURADNICE_X
        {
            get { return OP.SOURADNICE_X; }
            set { OP.SOURADNICE_X = value; }
        }
        public double SOURADNICE_Y
        {
            get { return OP.SOURADNICE_Y; }
            set { OP.SOURADNICE_Y = value; }
        }
        public bool GetMustBeConnectedWithSnap()
        {
            return false;
        }
        #endregion
        #region IVFKRegisterIface
        public void RegisterSegment(IVFKMain aOwner)
        {
            VFKElement vkfType = Singletons.VFKElements.getElement(TYPPPD_KOD);
            if (vkfType.OPAR_TYPE == OPAR_TYPE.ZDP)
            {
                if (OP == null)
                    OP = aOwner.getNewOP();
                else
                    OP = aOwner.UpdateOP(OP);
                OP.TYPPPD_KOD = TYPPPD_KOD;
                OP.OPAR_TYPE = vkfType.OPAR_TYPE.ToString();
            }
        }
        public void DeleteSegment(IVFKMain aOwner)
        {
            VFKElement vkfType = Singletons.VFKElements.getElement(TYPPPD_KOD);
            if (vkfType.OPAR_TYPE == OPAR_TYPE.ZDP)
            {
                aOwner.DeleteOP(OP);
                OP = null;
            }
        }
        #endregion IVFKRegisterIface
    }

    public class VFKSimpleOBBPMark : BaseObject, IVFKRegisterIface, IVFKSimpleMark
    {
        public VFKSimpleOBBPMark(VFKOBBPTableItem aOBBP, VFKSOBRTableItem aSOBR)
        {
            OBBP = aOBBP;
            SOBR = aSOBR;
            TYPPPD_KOD = aOBBP.TYPPPD_KOD;
        }
        public VFKSimpleOBBPMark()
        {
        }
        private VFKOBBPTableItem OBBP
        {
            get;
            set;
        }
        private VFKSOBRTableItem SOBR
        {
            get;
            set;
        }
        #region IVFKSimpleMark
        public UInt32 TYPPPD_KOD
        {
            get;
            set;
        }
        public double SOURADNICE_X
        {
            get
            {
                if (SOBR != null)
                    return SOBR.SOURADNICE_X;
                return OBBP.SOURADNICE_X;
            }
            set { }
        }
        public double SOURADNICE_Y
        {
            get
            {
                if (SOBR != null)
                    return SOBR.SOURADNICE_Y;
                return OBBP.SOURADNICE_Y;
            }
            set { }
        }
        public bool GetMustBeConnectedWithSnap()
        {
            //Bod PBP - pouze podzemní značka,Bod jednotné nivelační sítě,Stabilizovaný bod technické nivelace,Hraniční znak
            if (TYPPPD_KOD == 101 || TYPPPD_KOD == 102 || TYPPPD_KOD == 103 || TYPPPD_KOD == 104)
            {
                return true;
            }
            return false;
        }
        #endregion
        #region IVFKRegisterIface
        public void RegisterSegment(IVFKMain aOwner)
        {
        }
        public void DeleteSegment(IVFKMain aOwner)
        {
        }
        #endregion IVFKRegisterIface
    }
}
