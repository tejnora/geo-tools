using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    /*
    Body bodových polí jsou zobrazeny v katastrální mapě dekadických
    měřítek a v DKM. Zobrazení bodů bodového pole v KM-D je problémem
    vymezení obsahu KM-D.
    Číslo bodu je textový popisný prvek, který zobrazuje číslo bodu
    bodového pole.
    Značka bodu je bodový polohopisný prvek, který zobrazuje bod bodového
    pole.
    */
    [Serializable]
    public class VFKOBBPTable : VFKDataTableBase
    {
        List<VFKOBBPTableItem> _mVFKTableItems = new List<VFKOBBPTableItem>();
        public void addTableData(VFKOBBPTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKOBBPTableItem> Items
        {
            get { return _mVFKTableItems; }
        }
    }
    [Serializable]
    public class VFKOBBPTableItem : VFKDataTableBaseItemWithProp
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
        public string OBBP_TYPE
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
    }
}
