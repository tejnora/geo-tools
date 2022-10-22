using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    /*
    Grafické vyjádření rozsahu práva, které omezuje vlastníka pozemku
    ve prospěch jiného. Zobrazení věcného břemene je liniový prvek,
    který musí být tvořen body polohopisu.
    */
    [Serializable]
    public class VFKZVBTable : VFKDataTableBase
    {
        List<VFKZVBTableItem> _mVFKTableItems = new List<VFKZVBTableItem>();
        public void addTableData(VFKZVBTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKZVBTableItem> Items
        {
            get { return _mVFKTableItems; }
        }
    }
    [Serializable]
    public class VFKZVBTableItem : VFKDataTableBaseItemWithProp
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10,0, true, 7)]
        public UInt32 TYPPPD_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 6,0, false, 8)]
        public UInt32 KATUZE_KOD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, false, 8)]
        public string HJPV_ID
        {
            get;
            set;
        }
    }
}
