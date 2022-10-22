using System;
using System.Diagnostics;
using GeoBase.Localization;
using VFK.Tables;
using CAD.Utils.Cloning;
using System.Globalization;
using BaseObject = CAD.Utils.BaseObject;

namespace VFK
{
    public class VfkProxyActivePoint : BaseObject, IVfkRegister
    {
        #region Constructor
        public VfkProxyActivePoint(VFKSOBRTableItem item, VFKSPOLTableItem spol,IVFKMain vfkMain)
        {
            VfkSobrItem = item;
            VfkSpolItem = spol;
            _mVFKMain = vfkMain;
        }
        #endregion
        #region Fields & Properties
        IVFKMain _mVFKMain;
        public VFKSOBRTableItem VfkSobrItem
        {
            get;
            set;
        }
        public VFKSPOLTableItem VfkSpolItem
        {
            get;
            set;
        }
        public string PointFullName
        {
            get
            {
                if(VfkSobrItem==null)
                    return "";
                string name = VfkSobrItem.UPLNE_CISLO.ToString().PadLeft(9,'0');
                return name;
            }
            set
            {
                if (value.Length != 9)
                {
                    throw new Exception(LanguageDictionary.Current.Translate<string>("98","Text"));
                }
                try
                {
                    UInt32 zpmz = Convert.ToUInt32(value.Substring(0, 5));
                    UInt32 vlastniCislo = Convert.ToUInt32(value.Substring(5, 4));
                    VfkSobrItem.CISLO_ZPMZ = zpmz;
                    VfkSobrItem.CISLO_BODU = vlastniCislo;
                    VfkSobrItem.UPLNE_CISLO = Convert.ToUInt64(value);
                    VfkSpolItem.UPLNE_CISLO = VfkSobrItem.UPLNE_CISLO;
                    VfkSpolItem.CISLO_BODU = VfkSobrItem.CISLO_BODU;
                    VfkSpolItem.CISLO_ZPMZ = VfkSobrItem.CISLO_ZPMZ;
                    VfkSpolItem.CISLO_ZPMZ_MER = VfkSpolItem.CISLO_ZPMZ;
                }
                catch (Exception)
                {
                    throw new Exception(LanguageDictionary.Current.Translate<string>("98", "Text"));
                }
            }
        }
        public string SouradniceObrazuY
        {
            get
            {
                return VfkSobrItem.SOURADNICE_Y.ToString();
            }
            set
            {
                try
                {
                    VfkSobrItem.SOURADNICE_Y = Convert.ToDouble(value,CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    throw new Exception(LanguageDictionary.Current.Translate<string>("101", "Text"));
                }
            }
        }
        public string SouradniceObrazuX
        {
            get
            {
                return VfkSobrItem.SOURADNICE_X.ToString();
            }
            set
            {
                try
                {
                    VfkSobrItem.SOURADNICE_X = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    throw new Exception(LanguageDictionary.Current.Translate<string>("100", "Text"));
                }
            }
        }
        public string VfkSouradnicePolohyX
        {
            get
            {
                return VfkSpolItem.SOURADNICE_X.ToString();
            }
            set
            {
                try
                {
                    VfkSpolItem.SOURADNICE_X = Convert.ToDouble(value,CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    throw new Exception(LanguageDictionary.Current.Translate<string>("102", "Text"));
                }
            }
        }
        public string VfkSouradnicePolohyY
        {
            get
            {
                return VfkSpolItem.SOURADNICE_Y.ToString();
            }
            set
            {
                try
                {
                    VfkSpolItem.SOURADNICE_Y = Convert.ToDouble(value,CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    throw new Exception(LanguageDictionary.Current.Translate<string>("103", "Text"));
                }
            }
        }
        public bool IsSobrReadOnly
        {
            get
            {
                return !VfkSobrItem.CanDelete() ;
            }
        }
        public bool IsSpolReadOnly
        {
            get
            {
                return !VfkSpolItem.CanDelete();
            }
        }
        public string VfkKkb
        {
            get
            {
                if (VfkSpolItem.KODCHB_KOD != UInt32.MaxValue)
                {
                    return VfkSpolItem.KODCHB_KOD.ToString();
                }
                return VfkSobrItem.KODCHB_KOD.ToString();
            }
            set
            {
                try
                {
                    UInt32 convertedValue = Convert.ToUInt32(value);
                    if (convertedValue <= 3)
                    {
                        VfkSpolItem.KODCHB_KOD = convertedValue;
                        VfkSobrItem.KODCHB_KOD = UInt32.MaxValue;
                    }
                    else if (convertedValue <= 8)
                    {
                        VfkSobrItem.KODCHB_KOD = convertedValue;
                        VfkSpolItem.KODCHB_KOD = UInt32.MaxValue;
                    }
                    else
                    {
                        throw new Exception("Out of range");
                    }
                }
                catch(Exception)
                {
                    throw new Exception(LanguageDictionary.Current.Translate<string>("104", "Text"));
                }
            }
        }
        public bool CanRemove()
        {
            return _mVFKMain.CanDeleteSobrAndSpol(VfkSobrItem.ID);
        }
        #endregion
        #region IVFKRegister
        public bool RegisterSegment(IVFKMain aOwner)
        {
            VfkSobrItem = aOwner.updateSOBR(VfkSobrItem);
            VfkSpolItem = aOwner.updateSPOL(VfkSpolItem, VfkSobrItem);
            return true;
        }
        public void DeleteSegment(IVFKMain aOwner)
        {
            if(VfkSobrItem==null)
                return;
            Debug.Assert(aOwner.CanDeleteSobrAndSpol(VfkSobrItem.ID));
            aOwner.DeleteSobr(VfkSobrItem);
            aOwner.DeleteSpol(VfkSpolItem);
        }
        public void InitFromElement(VfkElement aElement)
        {

        }
        #endregion IVFKRegister
    }
}
