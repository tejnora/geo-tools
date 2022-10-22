using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VFK.Tables;
using CAD.Utils;
using CAD.Utils.Cloning;

namespace VFK
{
    public class VFKSimpleActivePoint : BaseObject, IVFKRegisterIface
    {
        IVFKMain _mVFKMain = null;
        public VFKSimpleActivePoint(VFKSOBRTableItem aItem, VFKSPOLTableItem aSPOL,IVFKMain aVFKMain)
        {
            VFKSOBRItem = aItem;
            VFKSPOLItem = aSPOL;
            _mVFKMain = aVFKMain;
        }
        public VFKSOBRTableItem VFKSOBRItem
        {
            get;
            set;
        }
        public VFKSPOLTableItem VFKSPOLItem
        {
            get;
            set;
        }
        public string PointFullName
        {
            get
            {
                if(VFKSOBRItem==null)
                    return "";
                string name=string.Empty;
                name += "1234";
                name += VFKSOBRItem.CISLO_ZPMZ.ToString().PadLeft(4, '0');
                name += VFKSOBRItem.CISLO_BODU.ToString().PadLeft(4, '0');
                return name;
            }
            set
            {
                if (value.Length != 12)
                {
                    return;
                }
                else
                {
                    try
                    {
                        UInt32 ck = System.Convert.ToUInt32(value.Substring(0, 4));
                        UInt32 zpmz = System.Convert.ToUInt32(value.Substring(4, 4));
                        UInt32 vlastniCislo = System.Convert.ToUInt32(value.Substring(8, 4));
                        VFKSOBRItem.CISLO_ZPMZ = zpmz;
                        VFKSOBRItem.CISLO_BODU = vlastniCislo;
                        VFKSOBRItem.UPLNE_CISLO = System.Convert.ToUInt64(value);
                        VFKSPOLItem.UPLNE_CISLO = VFKSOBRItem.UPLNE_CISLO;
                        VFKSPOLItem.CISLO_BODU = VFKSOBRItem.CISLO_BODU;
                        VFKSOBRItem.CISLO_ZPMZ = VFKSOBRItem.CISLO_ZPMZ;
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
            }
        }
        public string SouradniceObrazuY
        {
            get
            {
                return VFKSOBRItem.SOURADNICE_Y.ToString();
            }
            set
            {
                try
                {
                    VFKSOBRItem.SOURADNICE_Y = System.Convert.ToDouble(value);
                }
                catch (Exception ex)
                {
                }
            }
        }
        public string SouradniceObrazuX
        {
            get
            {
                return VFKSOBRItem.SOURADNICE_X.ToString();
            }
            set
            {
                try
                {
                    VFKSOBRItem.SOURADNICE_X = System.Convert.ToDouble(value);
                }
                catch (Exception e)
                {
                }
            }
        }
        public string VFKSouradnicePolohyX
        {
            get
            {
                return VFKSPOLItem.SOURADNICE_X.ToString();
            }
            set
            {
                try
                {
                    VFKSPOLItem.SOURADNICE_X = System.Convert.ToDouble(value);
                }
                catch (Exception e)
                {
                }
            }
        }
        public string VFKSouradnicePolohyY
        {
            get
            {
                return VFKSPOLItem.SOURADNICE_Y.ToString();
            }
            set
            {
                try
                {
                    VFKSPOLItem.SOURADNICE_Y = System.Convert.ToDouble(value);
                }
                catch (Exception e)
                {
                }
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return !VFKSOBRItem.CanDelete() || !VFKSPOLItem.CanDelete();
            }
            set
            {
                System.Diagnostics.Debug.Assert(false);
            }
        }
        public string VFKKKB
        {
            get
            {
                if (VFKSPOLItem.KODCHB_KOD != 0)
                {
                    return VFKSPOLItem.KODCHB_KOD.ToString();
                }
                else
                {
                    return VFKSOBRItem.KODCHB_KOD.ToString();
                }
            }
            set
            {
                try
                {
                    UInt32 convertedValue = System.Convert.ToUInt32(value);
                    if (convertedValue <= 3)
                    {
                        VFKSPOLItem.KODCHB_KOD = convertedValue;
                    }
                    else if (convertedValue <= 8)
                    {
                        VFKSOBRItem.KODCHB_KOD = convertedValue;
                    }
                    else
                    {
                        throw new Exception("Out of range");
                    }
                }
                catch(Exception ex)
                {
                    //todo
                }
            }
        }
        public bool CanRemove()
        {
            if (IsReadOnly) return false;
            return true;
        }
        #region IVFKRegisterIface
        public void RegisterSegment(IVFKMain aOwner)
        {
            VFKSOBRItem = aOwner.getNewSOBR();
            VFKSPOLItem = aOwner.getNewSPOL(VFKSOBRItem);
        }
        public void DeleteSegment(IVFKMain aOwner)
        {
        }
        #endregion IVFKRegisterIface
    }
}
