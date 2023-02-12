using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    /*
     Tabulka slouží k uložení informací o bonitních dílech parcely.
     Popisuje vztah mezi BPEJ a parcelou.
     */

    [Serializable]
    public class VFKBDPTable : VFKDataTableBase
    {
        List<VFKBDPTableItem> _mVFKTableItems = new List<VFKBDPTableItem>();

        public void addTableData(VFKBDPTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
        }

        public List<VFKBDPTableItem> Items => _mVFKTableItems;

        public void ReinitItems()
        {
            _mVFKTableItems = new List<VFKBDPTableItem>();
        }
    }

    [Serializable]
    public class VFKBDPTableItem : VFKDataTableBaseItem
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 2, 0, false, 0)]
        public int STAV_DAT
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Date, false, 1)]
        public DateTime DATUM_VZNIKU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Date, false, 2)]
        public DateTime DATUM_ZANIKU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 1, 0, false, 3)]
        public UInt32 PRIZNAK_KONTEXTU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30, 0, false, 4)]
        public string RIZENI_ID_VZNIKU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30, 0, false, 5)]
        public string RIZENI_ID_ZANIKU
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30, 0, true, 6)]
        public string PAR_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 5, 0, true, 7)]
        public string BPEJ_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 9, 0, true, 8)]
        public UInt64 VYMERA
        {
            get;
            set;
        }

    }
}
