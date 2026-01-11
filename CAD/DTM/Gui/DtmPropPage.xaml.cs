using System;
using CAD.GUI;
using CAD.Canvas;
using CAD.DTM.Elements;
using System.Text.RegularExpressions;
using CAD.Utils;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CAD.DTM.Elements.GUI;

namespace CAD.DTM.Gui
{
    public partial class DtmPropPage : IPropPage, INotifyPropertyChanged
    {
        DtmCustomElementProperties _customProperties = new DtmCustomElementProperties();
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

        public Visibility CustomPropertiesVisibility { get; set; }

        public void Load(IDrawObject drawObject)
        {
            var dtmElement = (IDtmDrawingElement)(drawObject);
            General.Load(dtmElement);
            SpolecneAtributy.Load(dtmElement.GetDtmElement);
            SpolecneAtributyZPS.Load(dtmElement.GetDtmElement);
            _customProperties.Clear();
            dtmElement.GetDtmElement.InitGUICustomProperties(_customProperties);
            if (_customProperties.Properties.Count > 0)
            {
                CustomPropertiesVisibility = Visibility.Visible;
                InitCustomProperties();
            }
            else
            {
                CustomPropertiesVisibility = Visibility.Collapsed;
            }
            OnPropertyChanged("");
        }

        public void InvalidateCanvas()
        {
            //todo
        }

        public void InitCustomProperties()
        {
            for (var i = _customPropertiesGrid.RowDefinitions.Count; i < _customProperties.Properties.Count; i++)
            {
                _customPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            _customPropertiesGrid.Children.Clear();
            var counter = -1;
            foreach (var property in _customProperties.Properties)
            {
                ++counter;
                property.PropPage = this;
                var nameLabel = new Label { Content = property.Name };
                Grid.SetRow(nameLabel, counter);
                Grid.SetColumn(nameLabel, 0);
                _customPropertiesGrid.Children.Add(nameLabel);
                var editControl = property.GetEditControl();
                Grid.SetRow(editControl, counter);
                Grid.SetColumn(editControl, 1);
                editControl.IsEnabled = General.IsEditable;
                _customPropertiesGrid.Children.Add(editControl);
            }
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
                    IsEditable = true;
                    break;
                case 'd':
                    Zapis = "Smazany";
                    IsEditable = false;
                    break;
                case 'u':
                    Zapis = "Aktualizovany";
                    IsEditable = false;
                    break;
                case 'r':
                    Zapis = "Referencni";
                    IsEditable = false;
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

        bool _isEditable = true;
        public bool IsEditable
        {
            get => _isEditable;
            private set => SetField(ref _isEditable, value);
        }
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
