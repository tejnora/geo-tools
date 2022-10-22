using System;
using System.Windows;
using CAD.Utils.Cloning;
using CAD.VFK;
using GeoBase.Localization;
using BaseObject = CAD.Utils.BaseObject;

namespace VFK.Tables
{
    public interface IVFKDataTable
    {
        void UpdateMaxValue(string aMaxValue);
    }
    [Serializable]
    public abstract class VFKDataTableBase : IVFKDataTable
    {
        UInt64 _mMaxId=1; //Max id when you load
        public void UpdateMaxValue(string aMaxValue)
        {
            try
            {
                UInt64 convertedInt = System.Convert.ToUInt64(aMaxValue);
                if (convertedInt >= _mMaxId)
                {
                    _mMaxId = convertedInt + 1;
                }
            }
            catch (Exception)
            {
                LanguageDictionary.Current.ShowMessageBox("105", null, MessageBoxButton.OK, MessageBoxImage.Warning);
                throw new UnExpectException();
            }
        }
        public UInt64 getMaxValueFromBlock()
        {
            return _mMaxId;
        }
    }

    public interface IVFKDataTableItem
    {
        void MarkItemFromImport();
    }

    [Serializable]
    public abstract class VFKDataTableBaseItem : BaseObject, IVFKDataTableItem
    {
        bool _mItemFromImport=false;
        public void MarkItemFromImport()
        {
            _mItemFromImport = true;
        }
        public bool CanDelete()
        {
            return !_mItemFromImport;
        }
        public bool ItemFromImport()
        {
            return _mItemFromImport;
        }
    }

    [Serializable]
    public abstract class VFKDataTableBaseItemWithProp : VFKDataTableBaseItem
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number,30,0, true, 0)]
        public string ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number,2,0, false, 1)]
        public Int32 STAV_DAT
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Date, false, 2)]
        public DateTime DATUM_VZNIKU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Date, false, 3)]
        public DateTime DATUM_ZANIKU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 1,0, false, 4)]
        public UInt32 PRIZNAK_KONTEXTU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 5)]
        public string RIZENI_ID_VZNIKU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 6)]
        public string RIZENI_ID_ZANIKU
        {
            get;
            set;
        }
    }
}
