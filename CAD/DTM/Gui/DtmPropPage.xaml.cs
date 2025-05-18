using System;
using System.Linq;
using CAD.GUI;
using System.Windows.Controls;
using CAD.Canvas;
using System.Windows;
using CAD.DTM.Elements;
using System.Text.RegularExpressions;

namespace CAD.DTM.Gui
{
    public partial class DtmPropPage : UserControl, IPropPage
    {
        public DtmPropPage()
        {
            InitializeComponent();
            DataContext = this;
            General = new GeneralTabData();
            SpolecneAtributy = new SpolecneAtributyTabData();
        }

        public GeneralTabData General { get; }
        public SpolecneAtributyTabData SpolecneAtributy { get; }
        public void Load(IDrawObject drawObject)
        {
            var dtmElement = (IDtmDrawingElement)(drawObject);
            General.Load(dtmElement);
            SpolecneAtributy.Load(dtmElement.GetDtmElement);
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

}
