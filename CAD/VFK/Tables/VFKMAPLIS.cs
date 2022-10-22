using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    [Serializable]
    public class VFKMAPLISTable : VFKDataTableBase
    {
        List<VFKMAPLISTableItem> _mVFKTableItems = new List<VFKMAPLISTableItem>();
        public void addTableData(VFKMAPLISTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKMAPLISTableItem> Items
        {
            get { return _mVFKTableItems; }
        }
    }
    [Serializable]
    public class VFKMAPLISTableItem : VFKDataTableBaseItem
    {
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30, 0, true, 0)]
        public string ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 100, 0, true, 1)]
        public string OZNACENI_MAPOVEHO_LISTU
        {
            get; set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Date, false, 2)]
        public DateTime PLATNOST_OD
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Date, false, 3)]
        public DateTime PLATNOST_DO
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 5, 0, false, 4)]
        public string MAPA
        {
            get;
            set;
        }
    }
}
