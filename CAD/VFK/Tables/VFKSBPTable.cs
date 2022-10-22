using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    /*
    Entita popisuje vazbu mezi podrobnými body, jejichž
    spojením vzniká liniový polohopisný prvek katastrální mapy.
     */
    [Serializable]
    public class VFKSBPTable : VFKDataTableBase
    {
        List<VFKSBPTableItem> _mVFKTableItems = new List<VFKSBPTableItem>();
        public void addTableData(VFKSBPTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKSBPTableItem> Items
        {
            get { return _mVFKTableItems; }
        }
    }
    [Serializable]
    public class VFKSBPTableItem : VFKDataTableBaseItemWithProp
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, true, 7)]
        public string BP_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 38,0, true, 8)]
        public UInt64 PORADOVE_CISLO_BODU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 9)]
        public string OB_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 10)]
        public string HP_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 11)]
        public string DPM_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 100,0, false, 12)]
        public string PARAMETRY_SPOJENI
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false,13)] //should be true, but now is false
        public string ZVB_ID
        {
            get;
            set;
        }
    }
}
