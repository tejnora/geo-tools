using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    [Serializable]
    public class VFKOBPEJTable : VFKDataTableBase
    {
        public VFKOBPEJTable()
        {
            Items = new List<VFKOBPEJTableItem>();
        }
        public void addTableData(VFKOBPEJTableItem aValue)
        {
            Items.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKOBPEJTableItem> Items
        {
            get;
            private set;
        }
    }
    [Serializable]
    public class VFKOBPEJTableItem : VFKDataTableBaseItemWithProp
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10, 0, true, 7)]
        public UInt32 TYPPPD_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10, 2, false, 8)]
        public double SOURADNICE_Y
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10, 2, false, 9)]
        public double SOURADNICE_X
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 255, 0, true, 10)]
        public string TEXT
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10, 2, false, 11)]
        public double VELIKOST
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10, 4, false, 12)]
        public double UHEL
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 5, 0, true, 13)]
        public string BPEJ_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 6, 0, true, 14)]
        public string KATUZE_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 2, 0, true, 15)]
        public string VZTAZNY_BOD
        {
            get;
            set;
        }
    }
}
