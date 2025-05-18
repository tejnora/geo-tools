using System;
using System.Collections.Generic;
using System.Linq;
using CAD.GUI;
using System.Windows.Controls;
using CAD.Canvas;
using System.Windows;
using CAD.DTM.Elements;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CAD.DTM.Gui
{
    public partial class DtmPropPage : UserControl, IPropPage, INotifyPropertyChanged
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
        public void Load(IDrawObject drawObject)
        {
            var dtmElement = (IDtmDrawingElement)(drawObject);
            General.Load(dtmElement);
            SpolecneAtributy.Load(dtmElement.GetDtmElement);
            SpolecneAtributyZPS.Load(dtmElement.GetDtmElement);
            OnPropertyChanged("");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class GeneralTabData : DependencyObject
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
        public string Nazev { get; private set; }
        public string Zapis { get; private set; }
        public string Typ { get; private set; }
    }

    public class SpolecneAtributyTabData : DependencyObject
    {
        DtmElementSpolecneAtributy _owner;
        public void Load(IDtmElement element)
        {
            _owner = element.SpolecneAtributy;
        }
        public DateTime DatumVkladu => _owner.DatumVkladu;
        public DateTime DatumZmeny => _owner.DatumZmeny;
        public string ID => _owner.ID;
        public string IDEditora => _owner.IDEditora;
        public string IDZmeny => _owner.IDZmeny;
        public string PopisObjektu => _owner.PopisObjektu;
        public string VkladOsoba => _owner.VkladOsoba;
        public string ZmenaOsoba => _owner.ZmenaOsoba;
    }

    public class SpolecneAtributyZPSTabData : DependencyObject
    {
        DtmSpolecneAtributyZPS _owner;
        public void Load(IDtmElement element)
        {
            _owner = element.SpolecneAtributyZPS;
        }
        public int UrovenUmisteniObjektuZPS => _owner.UrovenUmisteniObjektuZPS;
        public int TridaPresnostiPoloha => _owner.TridaPresnostiPoloha;
        public int TridaPresnostiVyska => _owner.TridaPresnostiVyska;

        public string ZpusobPorizeniZPS
        {
            get
            {
                switch (_owner.ZpusobPorizeniZPS)
                {
                    case 1:
                        return "geodeticky - terestricky";
                    case 2:
                        return "geodeticky - fotogrammetricky";
                    case 3:
                        return "geodeticky - pozemním laserovým skenováním";
                    case 4:
                        return "přibližným zákresem";
                    case 5:
                        return "odvozením";
                    default:
                        return "nezjištěno";
                }
            }
        }

    }

}
