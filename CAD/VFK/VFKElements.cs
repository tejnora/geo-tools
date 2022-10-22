using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media.Imaging;
using CAD.Canvas;
using CAD.VFK;
using GeoBase.Localization;
using GeoBase.Utils;
using BaseObject = CAD.Utils.BaseObject;

namespace VFK
{
    #region Enums
    public enum ObbpType
    {
        CB,     //Číslo bodu.                    --Cisla
        ZB      //Značka bodu.
    }
    public enum ObrBudType
    {
        ZDB,
        OB
    }
    public enum BlockNvf
    {
        empty = 0,
        OP = 1,
        DPM = 2,
        OBBP = 3,
        HP = 4,
        ZVB = 5,
        OB = 6,
        HBPEJ = 7,
        OBPEJ = 8
    }
    public enum ElementGroupType
    {
        epmpty,
        LINES,
        MARKS,
        NUMBERS,
        UN_IMPLEMENTED
    }

    public enum DpmType
    {
        None,
        HCHU,
        HOCHP,
        MEZ,
        LDPP,
        LPP,
        TPP,
        BDPP,
        BPP
    }
    #endregion
    [Serializable]
    public class VfkElement : DataObjectBase<VfkElement>
    {
        #region Constructor
        public VfkElement()
            : base(null, new StreamingContext())
        {
            BlockNvf = BlockNvf.empty;
            Prislusnost = false;

        }
        #endregion
        #region Class ElementDrawInfo
        public class ElementDrawInfo : BaseObject
        {
            #region Constructor
            public ElementDrawInfo()
            {
                Color = Color.Black;
                Width = 1.0;
                LayerName = "1";
                VyskaTextu = 1.7;
                FontName = "Arial";
            }
            public ElementDrawInfo(string parseString)
            {
                string[] items = parseString.Split(' ');
                FontName = string.Empty;
                LayerName = items[0];
                Int32 colorIndex;
                if (Int32.TryParse(items[1], out colorIndex) && colorIndex < DxfLibrary.Colors.DxfColors.Length)
                    Color = DefaultColors.GetColor(colorIndex);
                else
                    Color = Color.Black;
                _type = int.Parse(items[2]);
                Width = double.Parse(items[3], CultureInfo.InvariantCulture);
                if (items.Length == 4)
                    return;
                VyskaTextu = double.Parse(items[4], CultureInfo.InvariantCulture);
                if (items.Length == 5)
                    return;
                //font item[5]
                if (items.Length == 6)
                    return;
                VztaznyBod = int.Parse(items[6]);
                if (items.Length == 7)
                    return;
                SirkaText = double.Parse(items[7], CultureInfo.InvariantCulture);
            }
            public ElementDrawInfo(BinaryReader aR)
            {
                LoadFromDefaultFile(aR);
            }
            #endregion
            #region Property
            private Color _color;
            public Color Color
            {
                get { return _color; }
                set { _color = value; OnPropertyChanged("Color"); }
            }
            private double _width;
            public double Width
            {
                get { return _width; }
                set { _width = value; OnPropertyChanged("Width"); }
            }
            private string _layername;
            public string LayerName
            {
                get { return _layername; }
                set { _layername = value; OnPropertyChanged("LayerName"); }
            }
            private int _type;
            private double _vyskaTextu;
            public double VyskaTextu
            {
                get { return _vyskaTextu; }
                set { _vyskaTextu = value; OnPropertyChanged("VyskaTextu"); }
            }
            private int _vztaznyBod;
            public int VztaznyBod
            {
                get { return _vztaznyBod; }
                set { _vztaznyBod = value; OnPropertyChanged("VztaznyBod"); }
            }
            private double _sirkaText;
            public double SirkaText
            {
                get { return _sirkaText; }
                set { _sirkaText = value; OnPropertyChanged("SirkaText"); }
            }
            private string _fontName;
            public string FontName
            {
                get { return _fontName; }
                set { _fontName = value; OnPropertyChanged("FontName"); }
            }
            #endregion
            #region Methods
            public void LoadFromDefaultFile(BinaryReader aR)
            {
                LayerName = string.Format("{0}", aR.ReadInt32());
                var colorIndex = aR.ReadInt32();
                if (colorIndex < DxfLibrary.Colors.DxfColors.Length)
                    Color = DefaultColors.GetColor(colorIndex);
                else
                    Color = Color.Black;
                _type = aR.ReadInt32();
                Width = aR.ReadDouble();
                VyskaTextu = aR.ReadDouble();
                FontName = aR.ReadString();
                VztaznyBod = aR.ReadInt32();
                SirkaText = aR.ReadDouble();
            }
            #endregion
        }
        #endregion
        #region Properties
        private string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged("Description"); }
        }
        private UInt32 _typPdKod;
        public UInt32 TYPPPD_KOD
        {
            get { return _typPdKod; }
            set { _typPdKod = value; OnPropertyChanged("TYPPPD_KOD"); }
        }
        public bool Prislusnost
        {
            get;
            set;
        }
        public ElementDrawInfo MinulyStav
        {
            get;
            set;
        }
        public ElementDrawInfo PritomnyStav
        {
            get;
            set;
        }
        public ElementDrawInfo BudouciStav
        {
            get;
            set;
        }
        public DpmType DpmType
        {
            get
            {
                if (TYPPPD_KOD == 22300)
                    return DpmType.HCHU;
                if (TYPPPD_KOD == 22400)
                    return DpmType.HOCHP;
                if (TYPPPD_KOD == 105)
                    return DpmType.MEZ;
                if (TYPPPD_KOD == 1060)
                    return DpmType.BPP;
                if (TypPrvku == 1 && Polohopis)
                    return DpmType.LDPP;
                if (TypPrvku == 1 && !Polohopis)
                    return DpmType.LPP;
                if (TypPrvku == 4)
                    return DpmType.TPP;
                if (TypPrvku == 7 && Polohopis)
                    return DpmType.BDPP;
                if (TypPrvku == 7 && !Polohopis)
                    return DpmType.BPP;
                return DpmType.None;
            }
        }
        public string OparType
        {
            get
            {
                if (TYPPPD_KOD >= 301 && TYPPPD_KOD <= 316 || TYPPPD_KOD == 701 ||
                    TYPPPD_KOD == 703 || TYPPPD_KOD == 802 || TYPPPD_KOD == 803 || TYPPPD_KOD == 804
                    || TYPPPD_KOD == 319)
                    return "ZDP";
                switch (TYPPPD_KOD)
                {
                    case 18:
                    case 28:
                    case 29:
                    case 39:
                        return "PC";
                    case 1032:
                    case 1033:
                        return "SPC";
                    case 1018:
                        return "PPC";
                    default:
                        throw new UnExpectException();
                }
            }
        }
        public ObbpType ObbpType
        {
            get
            {
                switch (TYPPPD_KOD)
                {
                    case 1016:
                    case 1027:
                        return ObbpType.CB;
                    case 101:
                    case 102:
                    case 103:
                    case 104:
                        return ObbpType.ZB;
                    default:
                        throw new UnExpectException();

                }
            }
            set { throw new UnExpectException(); }
        }
        public BlockNvf BlockNvf
        {
            get;
            set;
        }
        public bool SnapToSobr
        {
            get;
            set;
        }
        public ObrBudType ObrBudType
        {
            get
            {
                if (TYPPPD_KOD >= 21700 && TYPPPD_KOD <= 21799)
                    return ObrBudType.OB;
                switch (TYPPPD_KOD)
                {
                    case 404:
                    case 405:
                    case 424:
                        return ObrBudType.ZDB;
                    default:
                        throw new UnExpectException();
                }
            }
            set { throw new UnExpectException(); }
        }
        public Int32 TypPrvku
        {
            get;
            set;
        }
        public bool Polohopis
        {
            get;
            set;
        }
        public VfkElementSubGroup Owner
        { get; set; }
        #endregion
        #region Methods
        public override string ToString()
        {
            return Description;
        }
        #endregion
    }
    [Serializable]
    public class VfkElementSubGroup : DataObjectBase<VfkElementSubGroup>, IEnumerable
    {
        #region Constructor
        public VfkElementSubGroup()
            : base(null, new StreamingContext())
        {
            Elements = new ObservableCollection<VfkElement>();
        }
        #endregion
        #region Property
        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged("Description"); }
        }
        public UInt32 GroupId
        {
            get;
            set;
        }
        public readonly PropertyData _elementsProperty = RegisterProperty("Elements", typeof(ObservableCollection<VfkElement>), null);
        public ObservableCollection<VfkElement> Elements
        {
            get { return GetValue<ObservableCollection<VfkElement>>(_elementsProperty); }
            set { SetValue(_elementsProperty, value); }
        }
        public readonly PropertyData _selectedElementProperty = RegisterProperty("SelectedElement", typeof(VfkElement), null);
        public VfkElement SelectedElement
        {
            get { return GetValue<VfkElement>(_selectedElementProperty); }
            set { SetValue(_selectedElementProperty, value); }
        }
        public VfkElementGroup Owner
        {
            get;
            set;
        }
        public VfkElement DefaultElement
        {
            get
            {
                if (Elements.Count > 0)
                    return Elements[0];
                return null;
            }
        }
        #endregion
        #region IEnumerable
        public IEnumerator GetEnumerator()
        {
            foreach (var element in Elements)
                yield return element;
        }
        #endregion
        #region Methods
        public void Add(VfkElement element)
        {
            Elements.Add(element);
            element.Owner = this;
        }
        #endregion
    }
    [Serializable]
    public class VfkElementGroup : DataObjectBase<VfkElementGroup>, IEnumerable
    {
        public enum Types
        {
            Lines,
            Marks,
            Numbers
        }
        #region Constructor
        public VfkElementGroup(Types type)
            : base(null, new StreamingContext())
        {
            SubGroupItems = new ObservableCollection<VfkElementSubGroup>();
            Type = type;
        }
        #endregion
        #region Property
        public ObservableCollection<VfkElementSubGroup> SubGroupItems
        {
            get;
            private set;
        }

        private string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    switch (Type)
                    {
                        case Types.Lines:
                            _name = LanguageDictionary.Current.Translate<string>("118", "Lines");
                            break;
                        case Types.Marks:
                            _name = LanguageDictionary.Current.Translate<string>("118", "Marks");
                            break;
                        case Types.Numbers:
                            _name = LanguageDictionary.Current.Translate<string>("118", "Numbers");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                return _name;
            }
        }
        public VfkElementSubGroup DefaultElementSubGroup
        {
            get
            {
                if (SubGroupItems.Count > 0)
                    return SubGroupItems[0];
                return null;
            }
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get
            {
                if (_image == null)
                {
                    _image = new BitmapImage();
                    _image.BeginInit();
                    switch (Type)
                    {
                        case Types.Lines:
                            _image.UriSource = new Uri("pack://application:,,,/Icons/VfkLine.png", UriKind.Absolute);
                            break;
                        case Types.Marks:
                            _image.UriSource = new Uri("pack://application:,,,/Icons/VfkMarks.png", UriKind.Absolute);
                            break;
                        case Types.Numbers:
                            _image.UriSource = new Uri("pack://application:,,,/Icons/VfkText.png", UriKind.Absolute);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    _image.EndInit();
                }
                return _image;
            }
        }
        public Types Type
        { get; private set; }
        #endregion
        #region Methos
        public void Add(VfkElementSubGroup subGroup)
        {
            SubGroupItems.Add(subGroup);
            subGroup.Owner = this;
            subGroup.SelectedElement = subGroup.DefaultElement;
        }
        #endregion
        #region IEnumerable
        public IEnumerator GetEnumerator()
        {
            foreach (var subGroup in SubGroupItems)
                yield return subGroup;
        }
        #endregion
    }

    public class VfkElements : DataObjectBase<VfkElements>
    {
        #region Constructor
        private VfkElements()
            : base(null, new StreamingContext())
        {
            LoadVfkElements();
            return;
        }
        #endregion
        #region Property
        public VfkElementGroup VfkLineElementGroup;
        public VfkElementGroup VfkMarkElementGroup;
        public VfkElementGroup VfkNumberElementGroup;

        public readonly PropertyData _elementSubGroupsProperty = RegisterProperty("SubGroups", typeof(ObservableCollection<VfkElementSubGroup>), null);
        public ObservableCollection<VfkElementSubGroup> SubGroups
        {
            get { return GetValue<ObservableCollection<VfkElementSubGroup>>(_elementSubGroupsProperty); }
            set { SetValue(_elementSubGroupsProperty, value); }
        }
        public readonly PropertyData _selectedSubGroupProperty = RegisterProperty("SelectedSubGroup", typeof(VfkElementSubGroup), null);
        public VfkElementSubGroup SelectedSubGroup
        {
            get { return GetValue<VfkElementSubGroup>(_selectedSubGroupProperty); }
            set
            {
                SetValue(_selectedSubGroupProperty, value);
                if (value != null)
                    value.SelectedElement = value.DefaultElement;
            }
        }
        #endregion
        #region Methods
        public void LoadVfkElements()
        {
            VfkLineElementGroup = new VfkElementGroup(VfkElementGroup.Types.Lines);
            VfkMarkElementGroup = new VfkElementGroup(VfkElementGroup.Types.Marks);
            VfkNumberElementGroup = new VfkElementGroup(VfkElementGroup.Types.Numbers);
            String appDir = AppDomain.CurrentDomain.BaseDirectory;
            const string fileName = "SC_T_PRV.dat";
            var location = appDir + "Data\\" + fileName;
            var lineElements = new List<UInt32> { 1, 4, 5, 9, 10, 11, 12, 13, 14, 15, 1002, 1006 };
            var markElements = new List<UInt32> { 2, 3, 6, 1007, 1005 };
            var numberElements = new List<UInt32> { 7, 8, 1015 };
            try
            {
                BinaryReader reader = new BinaryReader(File.OpenRead(location));
                Int32 lastGroupNumber = reader.ReadInt32();
                while (true)
                {
                    VfkElementSubGroup subGroup = new VfkElementSubGroup();
                    subGroup.GroupId = (UInt32)reader.ReadInt32();
                    switch (subGroup.GroupId)
                    {
                        case 1:
                            subGroup.Description = "Hranice parcely";
                            break;
                        case 2:
                            subGroup.Description = "Ostatní zařazené";
                            break;
                        case 3:
                            subGroup.Description = "Druhy pozemků";
                            break;
                        case 4:
                            subGroup.Description = "Vnitřní kresba";
                            break;
                        case 5:
                        case 1005:
                            subGroup.Description = "Ostatní";
                            break;
                        case 6:
                        case 1006:
                            subGroup.Description = "Budova";
                            break;
                        case 7:
                        case 1007:
                        case 1015:
                            subGroup.Description = "PBPP";
                            break;
                        case 8:
                            subGroup.Description = "Místopisné názvy";
                            break;
                        case 9:
                            subGroup.Description = "Hranice katastrálního území";
                            break;
                        case 10:
                            subGroup.Description = "Hranice obce";
                            break;
                        case 11:
                            subGroup.Description = "Hranice okresu";
                            break;
                        case 12:
                            subGroup.Description = "Hranice kraje";
                            break;
                        case 13:
                            subGroup.Description = "Hranice státní";
                            break;
                        case 14:
                            subGroup.Description = "Hranice chráněného území";
                            break;
                        case 15:
                            subGroup.Description = "BPEJ";
                            break;
                        case 1002:
                            subGroup.Description = "Parcelní Číslo";
                            break;
                    }
                    Int32 count = reader.ReadInt32();
                    for (Int32 i = 0; i < count; i++)
                    {
                        VfkElement element = new VfkElement();
                        element.TYPPPD_KOD = (UInt32)reader.ReadInt32();
                        element.Description = reader.ReadString();
                        /*string clenKod = */
                        reader.ReadString();
                        /*Int32 tumKod = */
                        reader.ReadInt32();
                        /*bool prislusnost = */
                        reader.ReadBoolean();
                        /*bool krivka = */
                        reader.ReadBoolean();
                        element.TypPrvku = reader.ReadInt32();
                        element.Polohopis = reader.ReadBoolean();
                        element.MinulyStav = new VfkElement.ElementDrawInfo(reader);
                        element.PritomnyStav = new VfkElement.ElementDrawInfo(reader);
                        element.BudouciStav = new VfkElement.ElementDrawInfo(reader);
                        element.BlockNvf = (BlockNvf)reader.ReadInt32();
                        subGroup.Add(element);
                    }
                    if (lineElements.Contains(subGroup.GroupId))
                        VfkLineElementGroup.Add(subGroup);
                    else if (markElements.Contains(subGroup.GroupId))
                        VfkMarkElementGroup.Add(subGroup);
                    else if (numberElements.Contains(subGroup.GroupId))
                        VfkNumberElementGroup.Add(subGroup);
                    if (lastGroupNumber == subGroup.GroupId)
                        break;
                }
            }
            catch (Exception)
            {
                LanguageDictionary.Current.ShowMessageBox("109", null, MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.MainWindow.Close();
            }
            #region Lines Additional
            VfkElementSubGroup elementSubGroup = new VfkElementSubGroup { Description = "věcné břemeno", GroupId = 101 };
            elementSubGroup.Add(new VfkElement
                                    {
                                        TYPPPD_KOD = 23700,
                                        Description = "věcné břemeno",
                                        Prislusnost = true,
                                        BlockNvf = BlockNvf.ZVB,
                                        MinulyStav = new VfkElement.ElementDrawInfo("1 148 1 0.1"),
                                        PritomnyStav = new VfkElement.ElementDrawInfo("1 148 1 0.1"),
                                        BudouciStav = new VfkElement.ElementDrawInfo("1 148 1 0.1")
                                    });
            elementSubGroup.Add(new VfkElement
                                    {
                                        TYPPPD_KOD = 23100,
                                        Description = "první břemeno",
                                        Prislusnost = true,
                                        BlockNvf = BlockNvf.ZVB,
                                        MinulyStav = new VfkElement.ElementDrawInfo("1 148 1 0.1"),
                                        PritomnyStav = new VfkElement.ElementDrawInfo("1 148 1 0.1"),
                                        BudouciStav = new VfkElement.ElementDrawInfo("1 148 1 0.1")
                                    });
            elementSubGroup.Add(new VfkElement
                                    {
                                        TYPPPD_KOD = 23200,
                                        Description = "druhé břemeno",
                                        Prislusnost = true,
                                        BlockNvf = BlockNvf.ZVB,
                                        MinulyStav = new VfkElement.ElementDrawInfo("1 148 1 0.1"),
                                        PritomnyStav = new VfkElement.ElementDrawInfo("1 148 1 0.1"),
                                        BudouciStav = new VfkElement.ElementDrawInfo("1 148 1 0.1")
                                    });
            elementSubGroup.Add(new VfkElement
                                    {
                                        TYPPPD_KOD = 23300,
                                        Description = "třetí břemeno",
                                        Prislusnost = true,
                                        BlockNvf = BlockNvf.ZVB,
                                        MinulyStav = new VfkElement.ElementDrawInfo("1 148 1 0.1"),
                                        PritomnyStav = new VfkElement.ElementDrawInfo("1 148 1 0.1"),
                                        BudouciStav = new VfkElement.ElementDrawInfo("1 148 1 0.1")
                                    });
            elementSubGroup.Add(new VfkElement
                                    {
                                        TYPPPD_KOD = 23400,
                                        Description = "čtvrté břemeno",
                                        Prislusnost = true,
                                        BlockNvf = BlockNvf.ZVB,
                                        MinulyStav = new VfkElement.ElementDrawInfo("1 148 1 0.1"),
                                        PritomnyStav = new VfkElement.ElementDrawInfo("1 148 1 0.1"),
                                        BudouciStav = new VfkElement.ElementDrawInfo("1 148 1 0.1")
                                    });
            VfkLineElementGroup.Add(elementSubGroup);
            //mistopisne nazvy
            #endregion
            #region Texts Additional
            elementSubGroup = new VfkElementSubGroup { Description = "Parcelní číslo", GroupId = 101 };
            elementSubGroup.Add(new VfkElement
            {
                TYPPPD_KOD = 18,
                Description = "Číslo (def.bod) pozemkové parcely",
                BlockNvf = BlockNvf.OP,
                MinulyStav = new VfkElement.ElementDrawInfo("2 148 0 0 1.7 23 2 1.5"),
                PritomnyStav = new VfkElement.ElementDrawInfo("2 0 0 0 1.7 23 2 1.5"),
                BudouciStav = new VfkElement.ElementDrawInfo("2 4 0 0 1.7 23 2 1.5")
            });
            elementSubGroup.Add(new VfkElement
            {
                TYPPPD_KOD = 29,
                Description = "Číslo (def.bod) pozemkové parcely zjednodušeným způsobem",
                BlockNvf = BlockNvf.OP,
                MinulyStav = new VfkElement.ElementDrawInfo("2 148 0 0 1.7 23 2 1.5"),
                PritomnyStav = new VfkElement.ElementDrawInfo("2 0 0 0 1.7 23 2 1.5"),
                BudouciStav = new VfkElement.ElementDrawInfo("2 4 0 0 1.7 23 2 1.5")
            });
            elementSubGroup.Add(new VfkElement
            {
                TYPPPD_KOD = 28,
                Description = "Číslo (def.bod) stavební parcely",
                BlockNvf = BlockNvf.OP,

                MinulyStav = new VfkElement.ElementDrawInfo("2 148 0 0 1.7 23 2 1.5"),
                PritomnyStav = new VfkElement.ElementDrawInfo("2 0 0 0 1.7 23 2 1.5"),
                BudouciStav = new VfkElement.ElementDrawInfo("2 4 0 0 1.7 23 2 1.5")
            });
            elementSubGroup.Add(new VfkElement
            {
                TYPPPD_KOD = 39,
                Description = "Číslo (def.bod) stavební parcely zjednodušeným způsobem",
                BlockNvf = BlockNvf.OP,
                MinulyStav = new VfkElement.ElementDrawInfo("2 148 0 0 1.7 23 2 1.5"),
                PritomnyStav = new VfkElement.ElementDrawInfo("2 0 0 0 1.7 23 2 1.5"),
                BudouciStav = new VfkElement.ElementDrawInfo("2 4 0 0 1.7 23 2 1.5")
            });
            elementSubGroup.Add(new VfkElement
            {
                TYPPPD_KOD = 1018,
                Description = "Popisné parcelní číslo",
                BlockNvf = BlockNvf.OP,
                MinulyStav = new VfkElement.ElementDrawInfo("2 148 0 0 1.7 23 2 1.5"),
                PritomnyStav = new VfkElement.ElementDrawInfo("2 0 0 0 1.7 23 2 1.5"),
                BudouciStav = new VfkElement.ElementDrawInfo("2 4 0 0 1.7 23 2 1.5")
            });
            VfkNumberElementGroup.Add(elementSubGroup);
            elementSubGroup = new VfkElementSubGroup { Description = "Ostatní nezařazené", GroupId = 102 };
            elementSubGroup.Add(new VfkElement
            {
                TYPPPD_KOD = 1019,
                Description = "Popisné parcelní číslo (volné)",
                BlockNvf = BlockNvf.DPM,
                MinulyStav = new VfkElement.ElementDrawInfo("7 144 0 0 1.7 23 2 1.5"),
                PritomnyStav = new VfkElement.ElementDrawInfo("7 0 0 0 1.7 23 2 1.5"),
                BudouciStav = new VfkElement.ElementDrawInfo("7 0 0 0 1.7 23 2 1.5")
            });
            VfkNumberElementGroup.Add(elementSubGroup);
            var mistopisneNazvy = (from n in VfkNumberElementGroup.SubGroupItems where n.GroupId == 8 select n).ToList();
            mistopisneNazvy[0].Add(new VfkElement
            {
                TYPPPD_KOD = 1006,
                Description = "Název místní části",
                BlockNvf = BlockNvf.DPM,
                TypPrvku = 4,
                MinulyStav = new VfkElement.ElementDrawInfo("7 144 0 0 1.7 23 2 1.5"),
                PritomnyStav = new VfkElement.ElementDrawInfo("7 0 0 0 1.7 23 2 1.5"),
                BudouciStav = new VfkElement.ElementDrawInfo("7 0 0 0 1.7 23 2 1.5")
            });
            #endregion
            #region Marks
            var group = from n in VfkMarkElementGroup.SubGroupItems where n.GroupId == 3 select n;
            if (group.Any())
                elementSubGroup = group.First();
            else
            {
                elementSubGroup = new VfkElementSubGroup { Description = "Druhy pozemků", GroupId = 101 };
                VfkMarkElementGroup.Add(elementSubGroup);
            }
            elementSubGroup.Add(new VfkElement
            {
                TYPPPD_KOD = 319,
                Description = "Společný dvůr, zbořeniště",
                BlockNvf = BlockNvf.OP,
                MinulyStav = new VfkElement.ElementDrawInfo("2 148 0 0 1.7 23 2 1.5"),
                PritomnyStav = new VfkElement.ElementDrawInfo("2 0 0 0 1.7 23 2 1.5"),
                BudouciStav = new VfkElement.ElementDrawInfo("2 4 0 0 1.7 23 2 1.5")
            });
            elementSubGroup.Add(new VfkElement
            {
                TYPPPD_KOD = 1060,
                Description = "Symbol vodního toku užšího než 2m",
                BlockNvf = BlockNvf.DPM,
                MinulyStav = new VfkElement.ElementDrawInfo("2 148 0 0 1.7 23 2 1.5"),
                PritomnyStav = new VfkElement.ElementDrawInfo("2 0 0 0 1.7 23 2 1.5"),
                BudouciStav = new VfkElement.ElementDrawInfo("2 4 0 0 1.7 23 2 1.5")
            });
            elementSubGroup.Add(new VfkElement
            {
                TYPPPD_KOD = 424,
                Description = "Vodní dílo",
                BlockNvf = BlockNvf.OB,
                MinulyStav = new VfkElement.ElementDrawInfo("2 148 0 0 1.7 23 2 1.5"),
                PritomnyStav = new VfkElement.ElementDrawInfo("2 0 0 0 1.7 23 2 1.5"),
                BudouciStav = new VfkElement.ElementDrawInfo("2 4 0 0 1.7 23 2 1.5")
            });
            #endregion
        }
        private VfkElement _prevVfkElement;
        public VfkElement GetElement(UInt32 aTypeId)
        {
            if (_prevVfkElement == null || _prevVfkElement.TYPPPD_KOD != aTypeId)
            {
                _prevVfkElement = null;
                foreach (VfkElementSubGroup gg in VfkLineElementGroup)
                    foreach (VfkElement e in gg)
                    {
                        if (e.TYPPPD_KOD == aTypeId)
                        {
                            _prevVfkElement = e;
                            return _prevVfkElement;
                        }
                    }
                foreach (VfkElementSubGroup gg in VfkMarkElementGroup)
                    foreach (VfkElement e in gg)
                    {
                        if (e.TYPPPD_KOD == aTypeId)
                        {
                            _prevVfkElement = e;
                            return _prevVfkElement;
                        }
                    }
                foreach (VfkElementSubGroup gg in VfkNumberElementGroup)
                    foreach (VfkElement e in gg)
                    {
                        if (e.TYPPPD_KOD == aTypeId)
                        {
                            _prevVfkElement = e;
                            return _prevVfkElement;
                        }
                    }
                throw new UnExpectException();
            }
            return _prevVfkElement;
        }
        private Dictionary<UInt32, ElementGroupType> _elementGroupCache = new Dictionary<uint, ElementGroupType>();
        public ElementGroupType GetElemntGroup(UInt32 aTypeId)
        {
            if (_elementGroupCache.ContainsKey(aTypeId))
            {
                return _elementGroupCache[aTypeId];
            }
            if (aTypeId == 1032 || aTypeId == 21700)
            {
                return ElementGroupType.UN_IMPLEMENTED;
            }
            foreach (VfkElementSubGroup gg in VfkLineElementGroup)
                foreach (VfkElement e in gg)
                {
                    if (e.TYPPPD_KOD == aTypeId)
                    {
                        _elementGroupCache[aTypeId] = ElementGroupType.LINES;
                        return ElementGroupType.LINES;
                    }
                }
            foreach (VfkElementSubGroup gg in VfkMarkElementGroup)
                foreach (VfkElement e in gg)
                {
                    if (e.TYPPPD_KOD == aTypeId)
                    {
                        _elementGroupCache[aTypeId] = ElementGroupType.MARKS;
                        return ElementGroupType.MARKS;
                    }
                }
            foreach (VfkElementSubGroup gg in VfkNumberElementGroup)
                foreach (VfkElement e in gg)
                {
                    if (e.TYPPPD_KOD == aTypeId)
                    {
                        _elementGroupCache[aTypeId] = ElementGroupType.NUMBERS;
                        return ElementGroupType.NUMBERS;
                    }
                }
            throw new UnExpectException();
        }
        #endregion
    }
}
