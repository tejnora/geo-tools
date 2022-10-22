using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    /*
    Vazební tabulka přiřazující způsob ochrany nemovitosti ke konkrétní parcele,
    budově nebo jednotce.
    */
    [Serializable]
    public class VFKRZOTable : VFKDataTableBase
    {
        List<VFKRZOTableItem> _mVFKTableItems = new List<VFKRZOTableItem>();
        public void addTableData(VFKRZOTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
    }
    [Serializable]
    public class VFKRZOTableItem : VFKDataTableBaseItemWithProp
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 4,0, true, 7)]
        public UInt32 ZPOCHR_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 8)]
        public string PAR_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 9)]
        public string BUD_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 10)]
        public string JED_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 30, 0, true, 30)]
        public string PS_ID
        {
            get;
            set;
        }
    }
}
