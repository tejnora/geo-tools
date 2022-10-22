using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    [Serializable]
    public class VFKHBPEJTable : VFKDataTableBase
    {
        public VFKHBPEJTable()
        {
            Items = new List<VFKHBPEJTableItem>();
        }
        public void addTableData(VFKHBPEJTableItem aValue)
        {
            Items.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKHBPEJTableItem> Items
        {
            get;
            private set;
        }
    }
    [Serializable]
    public class VFKHBPEJTableItem : VFKDataTableBaseItemWithProp
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10, 0, true, 7)]
        public UInt32 TYPPPD_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 5, 0, true, 8)]
        public string BPEJ_KOD_HRANICE_1
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 5, 0, false, 8)]
        public string BPEJ_KOD_HRANICE_2
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 6, 0, false, 8)]
        public string KATUZE_KOD
        {
            get;
            set;
        }
    }
}
