using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    [Serializable]
    public class VFKTELTable : VFKDataTableBase
    {
        List<VFKTELTableItem> _mVFKTableItems = new List<VFKTELTableItem>();
        public void addTableData(VFKTELTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKTELTableItem> Items
        {
            get { return _mVFKTableItems; }
        }
    }
    [Serializable]
    public class VFKTELTableItem : VFKDataTableBaseItemWithProp 
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 6, 0, true, 0)]
        public string KATUZE_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 6, 0, false, 0)]
        public string CISLO_TEL
        {
            get;
            set;
        }
    }
}
