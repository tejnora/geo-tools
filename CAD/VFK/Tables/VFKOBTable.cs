using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    /*
    Obraz budovy je prvek prostorových dat, který obsahuje entity
    zobrazující budovu nově zapsanou v KN a další objekty typu
    budova zjištěné při obnově katastrálního operátu novým mapováním.
    Budovy jsou v katastru geometricky a polohově určeny obdobně jako
    parcely.
    Ve stávajících katastrálních mapách nelze jednoznačně určit,
    která část vnitřní kresby reprezentuje budovu. V tomto případě nebudou
    mít budovy zapsané v popisné části odpovídající obraz v entitě
    prvek katastrální mapy.
    Obvod budovy je liniový polohopisný prvek reprezentující vnější obvod
    budovy - zastavěné plochy.
    Značka druhu budovy je bodový popisný prvek reprezentující druh budovy
    podle tabulky 9.5. Stavební objekty v příloze Vyhlášky 190/96 Sb.
    Značka druhu budovy reprezentuje také druh pozemku - zastavěná
    plocha.
    */
    [Serializable]
    public class VFKOBTable : VFKDataTableBase
    {
        List<VFKOBTableItem> _mVFKTableItems = new List<VFKOBTableItem>();
        public void addTableData(VFKOBTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKOBTableItem> Items
        {
            get { return _mVFKTableItems; }
        }
    }
    [Serializable]
    public class VFKOBTableItem : VFKDataTableBaseItemWithProp
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
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10,2, false, 10)]
        public double VELIKOST
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 10,4, false, 11)]
        public double UHEL
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, true, 12)]
        public string BUD_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 10,0, true, 13)]
        public string OBRBUD_TYPE
        {
            get;
            set;
        }
    }
}
