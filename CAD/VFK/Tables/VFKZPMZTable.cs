using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    /*
    Záznam podrobného měření změn (ZPMZ) je doklad, do něhož se
    zaznamenávají výsledky zaměřování změn v terénu. Záznam podrobného
    měření se zhotovuje i k vyznačení takových změn, které nejsou spojené
    s měřením v terénu (slučování parcel, demolice budovy, doplnění
    pozemku dosud vedeného ve zjednodušené evidenci bez zaměřování
    v terénu), pak neobsahuje zápisník, nebo změn, které jsou spojené
    s měřením v terénu, ale nemění hranice pozemku nebo obvod budovy,
    např. určení hranice chráněného území nebo ochranného pásma a dalších
    prvků polohopisu (/39/ odst. 3.3., /3/ §66 - 68).
     */
    [Serializable]
    public class VFKZPMZTable : VFKDataTableBase
    {
        List<VFKZPMZTableItem> _mVFKTableItems = new List<VFKZPMZTableItem>();
        public void addTableData(VFKZPMZTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
        }
    }
    [Serializable]
    public class VFKZPMZTableItem : VFKDataTableBaseItem
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 6, 0, true, 0)]
        public UInt32 KATUZE_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 5, 0, true, 1)]
        public UInt32 CISLO_ZPMZ
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 2)]
        public string PPZ_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 1,0, true, 3)]
        public UInt32 STAV_ZPMZ
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 1,0, false, 4)]
        public string MERICKY_NACRT
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 1,0, false, 5)]
        public string ZAPISNIK_PODROB_MERENI
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 1,0, false, 6)]
        public string VYPOCET_PROTOKOL_VYMER
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 1,0, false, 7)]
        public UInt32 TYPSOS_KOD
        {
            get;
            set;
        }
    }
}
