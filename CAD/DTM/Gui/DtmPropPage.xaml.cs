using System;
using CAD.GUI;
using CAD.Canvas;
using CAD.DTM.Elements;
using System.Text.RegularExpressions;
using CAD.Utils;
using System.ComponentModel;
using System.Windows;

namespace CAD.DTM.Gui
{
    public partial class DtmPropPage : IPropPage, INotifyPropertyChanged
    {
        public DtmPropPage()
        {
            InitializeComponent();
            DataContext = this;
            General = new GeneralTabData();
            SpolecneAtributy = new SpolecneAtributyTabData();
            SpolecneAtributyZPS = new SpolecneAtributyZPSTabData();
        }
        public GeneralTabData General { get; }
        public SpolecneAtributyTabData SpolecneAtributy { get; }
        public SpolecneAtributyZPSTabData SpolecneAtributyZPS { get; }

        public Visibility IsAdditionalPropertiesVisible { get; set; }

        public void Load(IDrawObject drawObject)
        {
            var dtmElement = (IDtmDrawingElement)(drawObject);
            General.Load(dtmElement);
            SpolecneAtributy.Load(dtmElement.GetDtmElement);
            SpolecneAtributyZPS.Load(dtmElement.GetDtmElement);

            var additionalProperties = dtmElement.GetDtmElement.AdditionalPropertiesGui;
            if (additionalProperties != null)
            {
                IsAdditionalPropertiesVisible = Visibility.Visible;
                additionalProperties.Load(_additionalProperties);
            }
            else
            {
                IsAdditionalPropertiesVisible = Visibility.Collapsed;
            }
            OnPropertyChanged("");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class GeneralTabData : ModelBase
    {
        public void Load(IDtmDrawingElement drawObject)
        {
            var nazevSplitted = Regex.Split(drawObject.Group.Name, @"(?<!^)(?=[A-Z])");
            Nazev = string.Join(" ", nazevSplitted);
            var element = drawObject.GetDtmElement;
            switch (element.EvaluateZapisObjektuForGui())
            {
                case 'i':
                    Zapis = "Novy";
                    break;
                case 'd':
                    Zapis = "Smazany";
                    break;
                case 'u':
                    Zapis = "Aktualizovany";
                    break;
                case 'r':
                    Zapis = "Referencni";
                    break;
            }
            Typ = element.ElementType.ToString();
        }
        string _nazev;
        public string Nazev
        {
            get => _nazev;
            private set => SetField(ref _nazev, value);
        }
        string _zapis;
        public string Zapis
        {
            get => _zapis;
            private set => SetField(ref _zapis, value);
        }

        string _typ;
        public string Typ { get => _typ; private set => SetField(ref _typ, value); }
    }

    public class SpolecneAtributyTabData : ModelBase
    {
        DtmElementSpolecneAtributy _owner;
        public void Load(IDtmElement element)
        {
            _owner = element.SpolecneAtributy;
        }
        public DateTime DatumVkladu
        {
            get => _owner.DatumVkladu;
            set => SetField(ref _owner.DatumVkladu, value);
        }
        public DateTime DatumZmeny
        {
            get => _owner.DatumZmeny;
            set => SetField(ref _owner.DatumZmeny, value);
        }
        public string ID
        {
            get => _owner.ID;
            set => SetField(ref _owner.ID, value);
        }
        public string IDEditora
        {
            get => _owner.IDEditora;
            set => SetField(ref _owner.IDEditora, value);
        }
        public string IDZmeny
        {
            get => _owner.IDZmeny;
            set => SetField(ref _owner.IDZmeny, value);
        }
        public string PopisObjektu
        {
            get => _owner.PopisObjektu;
            set => SetField(ref _owner.PopisObjektu, value);
        }
        public string VkladOsoba
        {
            get => _owner.VkladOsoba;
            set => SetField(ref _owner.VkladOsoba, value);
        }
        public string ZmenaOsoba
        {
            get => _owner.ZmenaOsoba;
            set => SetField(ref _owner.ZmenaOsoba, value);
        }
    }

    public class SpolecneAtributyZPSTabData : ModelBase
    {
        DtmSpolecneAtributyZPS _owner;
        public void Load(IDtmElement element)
        {
            IsVisible = element.SpolecneAtributyZPS == null ? Visibility.Collapsed : Visibility.Visible;
            _owner = element.SpolecneAtributyZPS ?? new DtmSpolecneAtributyZPS();
        }
        public Visibility IsVisible { get; private set; }

        public int UrovenUmisteniObjektuZPS
        {
            get => _owner.UrovenUmisteniObjektuZPS;
            set => SetField(ref _owner.UrovenUmisteniObjektuZPS, value);
        }
        public int TridaPresnostiPoloha
        {
            get => _owner.TridaPresnostiPoloha;
            set => SetField(ref _owner.TridaPresnostiPoloha, value);
        }
        public int TridaPresnostiVyska
        {
            get => _owner.TridaPresnostiVyska;
            set => SetField(ref _owner.TridaPresnostiVyska, value);
        }
        public int ZpusobPorizeniZPS
        {
            get => _owner.ZpusobPorizeniZPS;
            set => SetField(ref _owner.ZpusobPorizeniZPS, value);
        }
    }
}
