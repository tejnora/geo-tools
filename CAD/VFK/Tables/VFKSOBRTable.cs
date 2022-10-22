using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VFK.Tables
{
    /*
    Tabulka obsahuje body polohopisu - čísla bodů a souřadnice obrazu v mapě.
    */
    [Serializable]
    public class VFKSOBRTable : VFKDataTableBase
    {
        List<VFKSOBRTableItem> _mVFKTableItems = new List<VFKSOBRTableItem>();
        public void addTableData(VFKSOBRTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKSOBRTableItem> Items
        {
            get { return _mVFKTableItems; }
        }
    }
    [Serializable]
    public class VFKSOBRTableItem : VFKDataTableBaseItem
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, true, 0)]
        public string ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 2,0, false, 1)]
        public Int32 STAV_DAT
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 6,0, false, 2)]
        public UInt32 KATUZE_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 5,0, false, 3)]
        public UInt32 CISLO_ZPMZ
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 4,0, false, 4)]
        public UInt32 CISLO_TL
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 12,0, true, 5)]
        public UInt64 CISLO_BODU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 12,0, false, 6)]
        public UInt64 UPLNE_CISLO
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10,2, true, 7)]
        public double SOURADNICE_Y
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10,2, true, 8)]
        public double SOURADNICE_X
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 2,0, false, 9)]
        public UInt32 KODCHB_KOD
        {
            get;
            set;
        }
    }
}
