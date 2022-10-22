using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    /*
    Tabulka obsahuje polohopisné a popisné entity katastrální mapy, které
    nezobrazují popisné údaje katastru.
    */
    [Serializable]
    public class VFKDPMTable : VFKDataTableBase
    {
        public VFKDPMTable()
        {
            Items = new List<VFKDPMTableItem>();
        }
        public void addTableData(VFKDPMTableItem aValue)
        {
            Items.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKDPMTableItem> Items
        {
            get;
            private set;
        }
    }
    [Serializable]
    public class VFKDPMTableItem : VFKDataTableBaseItemWithProp
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10,0, true, 7)]
        public UInt32 TYPPPD_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10,2, false, 8)]
        public double SOURADNICE_Y
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10,2, false, 9)]
        public double SOURADNICE_X
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 255,0, false, 10)]
        public string TEXT
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10,2, false, 11)]
        public double VELIKOST
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10,4, false, 12)]
        public double UHEL
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 13)]
        public string BP_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 10,0, true, 14)]
        public string DPM_TYPE
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 2,0, false, 15)]
        public UInt32 VZTAZNY_BOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 6,0, true, 16)]
        public UInt32 KATUZE_KOD
        {
            get;
            set;
        }
    }
}
