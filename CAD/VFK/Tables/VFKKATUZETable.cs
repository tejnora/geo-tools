using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    /*
    Centrálně spravovaný číselík všech katastrálních uzemí (číselník obsahuje existující i
    zrušená k.ú.). Exportovaný datový blok obsahuje pouze vázané věty na
    PAR.KATUZE_KOD, PAR.KATUZE_KOD_PUV, TEL.KATUZE_KOD, RIZKU.KATUZE_KOD,
    SOBR.KATUZE_KOD, HBPEJ.KATUZE_KOD, OBPEJ.KATUZE_KOD, ZPMZ.KATUZE_KOD,
    NZZP.ZPMZ_KATUZE_KOD, SPOL.KATUZE_KOD, REZE.KATUZE_KOD, RECI.KATUZE_KOD,
    DOCI.KATUZE_KOD, DOHICI.KATUZE_KOD
     */
    [Serializable]
    public class VFKKATUZETable : VFKDataTableBase
    {
        List<VFKKATUZETableItem> _mVFKTableItems = new List<VFKKATUZETableItem>();
        public void addTableData(VFKKATUZETableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
        }
        public List<VFKKATUZETableItem> Items
        {
            get { return _mVFKTableItems; }
        }
    }
    [Serializable]
    public class VFKKATUZETableItem : VFKDataTableBaseItem
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 6,0, true, 0)]
        public UInt32 KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 6,0, true, 1)]
        public UInt32 OBCE_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 48,0, true, 2)]
        public string NAZEV
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Date, false, 3)]
        public DateTime PLATNOST_OD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Date, false, 4)]
        public DateTime PLATNOST_DO
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 3, 0, false, 5)]
        public UInt32 CISLO
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 1, 0, true, 5)]
        public UInt32 CISELNA_RADA
        {
            get;
            set;
        }
    }
}
