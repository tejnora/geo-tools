using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    /*
    Nadzemní stavba spojená se zemí pevným základem, která je prostorově
    soustředěna a navenek uzavřena obvodovými stěnami a střešní
    konstrukcí.
     */
    [Serializable]
    public class VFKBUDTable : VFKDataTableBase
    {
        List<VFKBUDTableItem> _mVFKTableItems = new List<VFKBUDTableItem>();
        public void addTableData(VFKBUDTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKBUDTableItem> Items
        {
            get { return _mVFKTableItems; }
        }
    }
    [Serializable]
    public class VFKBUDTableItem : VFKDataTableBaseItemWithProp
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 1,0, true, 7)]
        public UInt32 TYPBUD_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 6,0, false, 8)]
        public UInt32 CAOBCE_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 4,0, false, 9)]
        public UInt32 CISLO_DOMOVNI
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 14,2, false, 10)]
        public double CENA_NEMOVITOSTI
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 4,0, false, 11)]
        public UInt32 ZPVYBU_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 12)]
        public UInt64 TEL_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 1, 0, true, 13)]
        public string DOCASNA_STAVBA
        {
            get; 
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 1, 0, true, 14)]
        public string JE_SOUCASTI
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30, 0, false, 15)]
        public string PS_ID
        {
            get;
            set;
        }

    }
}
