using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Data;
using System.Windows.Input;
using CAD.Utils;
using GeoBase.Localization;
using GeoBase.Utils;

namespace VFK
{
    //Parcely - typ.
    [Serializable]
    enum D_PAR_TYPE
    {
        PKN,//Parcela KN.
        PZE//Parcela ZE.
    }
    [Serializable]
    public enum SouradnicovySystem
    {
        SJTSK=1,
        Gusterberg=2,
        SvatyStepan=3,
        Mistni=4
    }
    [Serializable]
    public enum TypGP
    {
        DKM,
        Ostatni
    }
    [Serializable]
    public enum PARAMETRY_LINE
    {
        empty,
        USECKA,//3-úsečka, daná 2 body
        LOMENA_CARA,//4-lomená čára, daná alespoň 2 body
        KRIVKA,//11-křivka, daná alespoň 2 body
        KRUZNICE3,//15-kružnice daná právě 3 body
        KRUZNICE_STRED,//15 r kružnice daná středem a poloměrem (r - v metrech na 2 desetinná místa)
        OBLOUK,//16 oblouk, daný právě 3 body
    }
    [Serializable]
    public class ScaleList
    {
        [Serializable]
        public struct ScaleListItem
        {
            public String ComboString
            {
            get;set;
            }
            public UInt32 ScaleNumber
            {
                get; set;
            }
        }

        public IList<ScaleListItem> ScaleListItems;
        private ScaleList()
        {
            ScaleListItems = new List<ScaleListItem>();
            ScaleListItems.Add(new ScaleListItem() { ComboString = "1:100", ScaleNumber = 100 });
            ScaleListItems.Add(new ScaleListItem() { ComboString = "1:250", ScaleNumber = 250 });
            ScaleListItems.Add(new ScaleListItem() { ComboString = "1:500", ScaleNumber = 500 });
            ScaleListItems.Add(new ScaleListItem() { ComboString = "1:720", ScaleNumber = 720 });
            ScaleListItems.Add(new ScaleListItem() { ComboString = "1:1000", ScaleNumber = 1000 });
            ScaleListItems.Add(new ScaleListItem() { ComboString = "1:1440", ScaleNumber = 1440 });
            ScaleListItems.Add(new ScaleListItem() { ComboString = "1:2000", ScaleNumber = 2000 });
            ScaleListItems.Add(new ScaleListItem() { ComboString = "1:2880", ScaleNumber = 2880 });
            ScaleListItems.Add(new ScaleListItem() { ComboString = "1:5000", ScaleNumber = 5000 });
        }
    }
    [Serializable]
    public class VFKDataContext : DataObjectBase<VFKDataContext>
    {
        public VFKDataContext()
            : base(null, new StreamingContext()) 
        {
            AlwaysInvokeNotifyChanged = true;
            LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
            IList<CilImportuEntry> list = new List<CilImportuEntry>
                                             {
                                                 new CilImportuEntry(dictionary.Translate("86", "Text", "DosavadniStav", typeof (string))as string,VFKMain.StavData.Pritomnoust),
                                                 new CilImportuEntry(dictionary.Translate("87", "Text", "NovyStav", typeof (string))as string,VFKMain.StavData.Budoucnost)
                                             };
            CilImportuEntrie = list[0];
            _cilImportuEntries = new CollectionView(list);
        }
        public VFKDataContext(SerializationInfo info, StreamingContext context)
            :base(info,context)
        {
            
        }
        protected override void ValidateFields()
        {
            LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
            string errorMesg =(string)dictionary.Translate("81", "Text", "PolozkaNemuzeBytPrazdna", typeof(string));
            if (string.IsNullOrEmpty(AuthorName)) SetFieldError(AuthorNameProperty, errorMesg);
            if (string.IsNullOrEmpty(FileName)) SetFieldError(FileNameProperty, errorMesg);
            if (FSU.ToString().Length != 6) SetFieldError(FSUProperty, (string)dictionary.Translate("82", "Text", "FSU musí mít 6 čísel", typeof(string)));
            if (string.IsNullOrEmpty(KatastralniPracoviste)) SetFieldError(KatastralniPracovisteProperty, errorMesg);
            if (string.IsNullOrEmpty(Obec)) SetFieldError(ObecProperty, errorMesg);
            if (string.IsNullOrEmpty(KatastralniUzemi)) SetFieldError(KatastralniUzemiProperty, errorMesg);
            if (string.IsNullOrEmpty(PoradoveCisloKU)) SetFieldError(PoradoveCisloKUProperty, errorMesg);
            if (!(CisloZPMZ>0 && CisloZPMZ <= 99999)) SetFieldError(CisloZPMZProperty, (string)dictionary.Translate("83", "Text", "ZPMZ musí mít 4 čísel", typeof(string)));
        }
        #region Property
        #region Cil importu Class Entry
        [Serializable]
        public class CilImportuEntry
        {
            public CilImportuEntry()
            {
            }
            public CilImportuEntry(string name, VFKMain.StavData cilImportuType)
            {
                Name = name;
                StavData = cilImportuType;
            }
            public string Name { get; set; }
            public enum CilImportuType
            {
                DosavadniStav=0,
                NovyStav=1
            }
            public VFKMain.StavData StavData
            {
                get;
                set;
            }
        }
        private readonly CollectionView _cilImportuEntries;
        public CollectionView CilImportuEntries
        {
            get { return _cilImportuEntries; }
        }

        #endregion
        public readonly PropertyData CilImportuProperty = RegisterProperty("CilImportuEntrie", typeof(CilImportuEntry), new CilImportuEntry());
        public CilImportuEntry CilImportuEntrie
        {
            get { return GetValue<CilImportuEntry>(CilImportuProperty); }
            set { SetValue(CilImportuProperty, value); }
        }

        public readonly PropertyData AuthorNameProperty = RegisterProperty("AuthorName", typeof(string), string.Empty);
        public string AuthorName
        {
            get{return GetValue<string>(AuthorNameProperty);}
            set{SetValue(AuthorNameProperty, value);}
        }

        public readonly PropertyData FileNameProperty = RegisterProperty("FileName", typeof(string), string.Empty);
        public string FileName
        {
            get{return GetValue<string>(FileNameProperty);}
            set { SetValue(FileNameProperty, value); }
        }

        public readonly PropertyData FSUProperty = RegisterProperty("FSU", typeof(UInt32), 0);
        public UInt32 FSU
        {
            get { return GetValue<UInt32>(FSUProperty); }
            set { SetValue(FSUProperty, value); }
        }

        public readonly PropertyData KatastralniPracovisteProperty = RegisterProperty("KatastralniPracoviste", typeof(string), string.Empty);
        public string KatastralniPracoviste
        {
            get { return GetValue<string>(KatastralniPracovisteProperty); }
            set { SetValue(KatastralniPracovisteProperty, value); }
        }

        public readonly PropertyData ObecProperty = RegisterProperty("Obec", typeof(string), string.Empty);
        public string Obec
        {
            get { return GetValue<string>(ObecProperty); }
            set { SetValue(ObecProperty, value); }
        }

        public readonly PropertyData KatastralniUzemiProperty = RegisterProperty("KatastralniUzemi", typeof(string), string.Empty);
        public string KatastralniUzemi
        {
            get { return GetValue<string>(KatastralniUzemiProperty); }
            set { SetValue(KatastralniUzemiProperty, value); }
        }

        public readonly PropertyData PoradoveCisloKUProperty = RegisterProperty("PoradoveCisloKU", typeof(string), string.Empty);
        public string PoradoveCisloKU
        {
            get { return GetValue<string>(PoradoveCisloKUProperty); }
            set { SetValue(PoradoveCisloKUProperty, value); }
        }

        public readonly PropertyData CiselnaRadaProperty = RegisterProperty("CiselnaRada", typeof(UInt32), 0);
        public UInt32 CiselnaRada
        {
            get { return GetValue<UInt32>(CiselnaRadaProperty); }
            set { SetValue(CiselnaRadaProperty, value); }
        }

        public readonly PropertyData VztazneMeritkoProperty = RegisterProperty("VztazneMeritko", typeof(ScaleList.ScaleListItem), Singletons.ScaleList.ScaleListItems[7]);
        public ScaleList.ScaleListItem VztazneMeritko
        {
            get { return GetValue<ScaleList.ScaleListItem>(VztazneMeritkoProperty); }
            set { SetValue(VztazneMeritkoProperty, value); }
        }

        public readonly PropertyData SouradnicovySystemProperty = RegisterProperty("SouradnicovySystem", typeof(SouradnicovySystem), SouradnicovySystem.SJTSK);
        public SouradnicovySystem SouradnicovySystem
        {
            get { return GetValue<SouradnicovySystem>(SouradnicovySystemProperty); }
            set { SetValue(SouradnicovySystemProperty, value); }
        }

        public readonly PropertyData TypGPProperty = RegisterProperty("TypGP", typeof(TypGP), TypGP.DKM);
        public TypGP TypGP
        {
            get { return GetValue<TypGP>(TypGPProperty); }
            set { SetValue(TypGPProperty, value); }
        }

        public readonly PropertyData CisloZPMZProperty = RegisterProperty("CisloZPMZ", typeof(UInt32), 0);
        public UInt32 CisloZPMZ
        {
            get { return GetValue<UInt32>(CisloZPMZProperty); }
            set { SetValue(CisloZPMZProperty, value); }
        }
        #endregion
    }
}
