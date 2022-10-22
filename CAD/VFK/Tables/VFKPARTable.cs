using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    [Serializable]
    public class VFKPARTable : VFKDataTableBase
    {
        List<VFKPARTableItem> _mVFKTableItems = new List<VFKPARTableItem>();
        public void addTableData(VFKPARTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKPARTableItem> Items
        {
            get { return _mVFKTableItems; }
        }
    }
    [Serializable]
    public class VFKPARTableItem : VFKDataTableBaseItemWithProp
    {
        public VFKPARTableItem()
        {
            isEdited = false;
        }
        #region VFKDefinition
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30, 0, false, 7)]
        public string PKN_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 10, 0, true, 8)]
        public string PAR_TYPE
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 6, 0, true, 9)]
        public UInt32 KATUZE_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 6, 0, false, 10)]
        public UInt32 KATUZE_KOD_PUV
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 1, 0, true, 11)]
        public UInt32 DRUH_CISLOVANI_PAR
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 5, 0, true, 12)]
        public UInt32 KMENOVE_CISLO_PAR
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 1, 0, false, 13)]
        public UInt32 ZDPAZE_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 3, 0, false, 14)]
        public UInt32 PODDELENI_CISLA_PAR
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 1, 0, false, 15)]
        public UInt32 DIL_PARCELY
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30, 0, false, 16)]
        public string MAPLIS_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 1, 0, false, 17)]
        public UInt32 ZPURVY_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 2, 0, false, 18)]
        public UInt32 DRUPOZ_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 4, 0, false, 19)]
        public UInt32 ZPVYPA_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 1, 0, false, 20)]
        public UInt32 TYP_PARCELY
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 9, 0, true, 21)]
        public UInt32 VYMERA_PARCELY
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 14, 2, false, 22)]
        public UInt64 CENA_NEMOVITOSTI
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 100, 0, false, 23)]
        public string DEFINICNI_BOD_PAR
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30, 0, false, 24)]
        public string TEL_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30, 0, false, 25)]
        public string PAR_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30, 0, false, 26)]
        public string BUD_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 1, 0, true, 27)]
        public string IDENT_BUD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 1, 0, true, 28)]
        public string SOUCASTI
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30, 0, false, 29)]
        public string PS_ID
        {
            get;
            set;
        }

        [VFKDefinitionAttribute(DefinitionFieldType.Text, 1, 0, true, 30)]
        public string IDENT_PS
        {
            get;
            set;
        }
        #endregion
        public bool isEdited
        {
            get;
            set;
        }
    }
}
