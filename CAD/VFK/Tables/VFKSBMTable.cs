using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VFK.Tables
{
    /*
    Entita popisuje vazbu mezi body, jejichž spojením vzniká liniový nepolohopisný
    prvek katastrální mapy.
    */
    [Serializable]
    public class VFKSBMTable : VFKDataTableBase
    {
        List<VFKSBMTableItem> _mVFKTableItems = new List<VFKSBMTableItem>();
        public void addTableData(VFKSBMTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
        }
        public List<VFKSBMTableItem> Items
        {
            get { return _mVFKTableItems; }
        }
    }
    [Serializable]
    public class VFKSBMTableItem : VFKDataTableBaseItem
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Date,false, 7)]
        public DateTime DATUM_VZNIKU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Date, false, 8)]
        public DateTime DATUM_ZANIKU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 38,0, true, 9)]
        public UInt64 PORADOVE_CISLO_BODU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10,2, true, 10)]
        public double SOURADNICE_Y
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10,2, true, 11)]
        public double SOURADNICE_X
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 12)]
        public string OP_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 13)]
        public string DPM_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 14)]
        public string HBPEJ_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 100,0, false, 15)]
        public string PARAMETRY_SPOJENI
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 1,0, false, 16)]
        public UInt32 PRIZNAK_KONTEXTU
        {
            get;
            set;
        }

    }
}
