using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VFK.Tables;
using CAD.Utils;
using CAD.Utils.Cloning;

namespace VFK
{
    public class VFKSimpleLine : BaseObject, IVFKRegisterIface
    {
        public enum SimpleLineTypeE
        {
            HP,
            DPM
        };
        public VFKSimpleLine(VFKSOBRTableItem aP1, VFKSOBRTableItem aP2, VFKSBPTableItem aSBP1, VFKSBPTableItem aSBP2,
            VFKHPTableItem aHP, VFKDPMTableItem aDPM)
        {
            P1 = aP1;
            P2 = aP2;
            SBP1 = aSBP1;
            SBP2 = aSBP2;
            if (aHP != null)
            {
                HP = aHP;
                SimpleLineType = SimpleLineTypeE.HP;
                _mTYPPPD_KOD = HP.TYPPPD_KOD;
            }
            else if (aDPM != null)
            {
                DPM = aDPM;
                SimpleLineType = SimpleLineTypeE.DPM;
                _mTYPPPD_KOD = DPM.TYPPPD_KOD;
            }
            else
            {
                System.Diagnostics.Debug.Assert(false);
            }
        }
        public VFKSimpleLine()
        {
        }
        public SimpleLineTypeE SimpleLineType
        {
            get;
            set;
        }
        public VFKDPMTableItem DPM
        {
            get;
            set;
        }
        public VFKSOBRTableItem P1
        {
            get;
            set;
        }
        public VFKSOBRTableItem P2
        {
            get;
            set;
        }
        public VFKSBPTableItem SBP1
        {
            get;
            private set;
        }
        public VFKSBPTableItem SBP2
        {
            get;
            private set;
        }
        public VFKHPTableItem HP
        {
            get;
            private set;
        }
        private UInt32 _mTYPPPD_KOD;
        public UInt32 TYPPPD_KOD
        {
            get
            {
                return _mTYPPPD_KOD;
            }
            set
            {
                _mTYPPPD_KOD = value;
                if (HP != null)
                {
                    HP.TYPPPD_KOD = _mTYPPPD_KOD;
                }
                else if (DPM != null)
                {
                    DPM.TYPPPD_KOD = _mTYPPPD_KOD;
                }
            }
        }

        #region IVFKRegisterIface
        public void RegisterSegment(IVFKMain aOwner)
        {
            if (SBP1 == null)
                SBP1 = aOwner.getNewSBP(P1);
            else
                SBP1 = aOwner.UpdateSBP(SBP1);
            if (SBP2 == null)
                SBP2 = aOwner.getNewSBP(P2);
            else
                SBP2 = aOwner.UpdateSBP(SBP2);
            if (SimpleLineType == SimpleLineTypeE.HP)
            {
                if (HP == null)
                    HP = aOwner.getNewHP();
                else
                    HP = aOwner.UpdateHP(HP);
                SBP1.HP_ID = HP.ID;
                SBP1.PORADOVE_CISLO_BODU = "1";
                SBP2.HP_ID = HP.ID;
                SBP2.PORADOVE_CISLO_BODU = "2";
                HP.TYPPPD_KOD = _mTYPPPD_KOD;
            }
            else if (SimpleLineType == SimpleLineTypeE.DPM)
            {
                if (DPM == null)
                    DPM = aOwner.getNewDPM();
                else
                    DPM = aOwner.UpdateDPM(DPM);
                SBP1.DPM_ID = DPM.ID;
                SBP1.PORADOVE_CISLO_BODU = "1";
                SBP2.DPM_ID = DPM.ID;
                SBP2.PORADOVE_CISLO_BODU = "2";
                DPM.TYPPPD_KOD = _mTYPPPD_KOD;
            }
        }
        public void DeleteSegment(IVFKMain aOwner)
        {
            System.Diagnostics.Debug.Assert(SBP1 != null && SBP2 != null && HP != null);
            if (HP != null)
            {
                aOwner.DeleteHP(HP);
                HP = null;
            }
            else if (DPM != null)
            {
                aOwner.DeleteDPM(DPM);
                DPM = null;
            }
            aOwner.DeleteSBP(SBP1);
            aOwner.DeleteSBP(SBP2);
            SBP1 = null;
            SBP2 = null;
        }
        #endregion
    }
}
