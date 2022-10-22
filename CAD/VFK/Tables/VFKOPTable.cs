using System;
using System.Collections.Generic;

namespace VFK.Tables
{
    /*
    Prvek prostorových dat, který obsahuje entity reprezentující parcelu v
    katastrální mapě. Parcela je pozemek, který je geometricky a polohově
    určen, zobrazen v katastrální mapě a označen parcelním číslem /2/.
    Parcelní číslo je textový popisný prvek, který zobrazuje atributy
    týkající se parcelního čísla entity PARCELA. Textový prvek je umístěn
    uvnitř parcely a reprezentuje její definiční bod. Parcelní číslo je
    vázáno na katastrální území.
    Značka druhu pozemku je bodový popisný prvek, který reprezentuje
    atribut druh pozemku a způsob využití nemovitostí entity PARCELA.
    Mapová značka se umísťuje uprostřed plochy parcely pokud možno nad
    parcelním číslem, u rozsáhlých nebo členitých parcel může být značka
    umístěna vícekrát, u parcel malé plochy může být vynechána.
    Popisné parcelní číslo je textový popisný prvek, který se umístí
    poblíž parcely tehdy, když se do parcely nevejde parcelní číslo.
    Vynášecí čára a šipka k popisnému parcelnímu číslu vysvětlují
    umístění popisného parcelního čísla.
    */
    [Serializable]
    public class VFKOPTable : VFKDataTableBase
    {
        List<VFKOPTableItem> _mVFKTableItems = new List<VFKOPTableItem>();
        public void addTableData(VFKOPTableItem aValue)
        {
            _mVFKTableItems.Add(aValue);
            UpdateMaxValue(aValue.ID);
        }
        public List<VFKOPTableItem> Items
        {
            get { return _mVFKTableItems; }
        }
    }
    [Serializable]
    public class VFKOPTableItem : VFKDataTableBaseItemWithProp
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
        [VFKDefinitionAttribute(DefinitionFieldType.Number, 30,0, true, 13)]
        public string PAR_ID
        {
            get;
            set;
        }
        [VFKDefinitionAttribute(DefinitionFieldType.Text, 10,0, true, 14)]
        public string OPAR_TYPE
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
