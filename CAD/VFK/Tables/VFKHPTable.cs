using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    /*
    Hranice parcely je liniový polohopisný prvek, který je tvořen
    spojením bodů polohopisu. Každý liniový prvek vymezuje hranici
    mezi dvěma parcelami (pokud netvoří hranici státu). Liniové prvky
    vymezující hranice parcel tvoří areálový graf:
    Poznámka: využitím sloupců PAR_ID1 a PAR_ID2 datového bloku HP (hranice parcel)
    lze získat k dané parcele seznam okolních parcel.
    */
    [Serializable]
    public class VFKHPTable : VFKDataTableBase
    {
        List<VFKHPTableItem> _mVFKTableItems = new List<VFKHPTableItem>();
        public void addTableData(VFKHPTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKHPTableItem> Items
        {
            get { return _mVFKTableItems; }
        }
    }
    [Serializable]
    public class VFKHPTableItem : VFKDataTableBaseItemWithProp
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10,0, true, 7)]
        public UInt32 TYPPPD_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, true, 8)]
        public string PAR_ID_1
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 9)]
        public string PAR_ID_2
        {
            get;
            set;
        }
    }
}
