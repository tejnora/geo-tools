using CAD.Utils;
using System.Globalization;
using System.Windows.Controls;
namespace CAD.DTM.Elements.Bod.Gui
{
    public partial class DtmZarizeniVodovodniPripojkyBodPP
        : UserControl
    {
        DtmZarizeniVodovodniPripojkyBodPPData _data = new DtmZarizeniVodovodniPripojkyBodPPData();
        public DtmZarizeniVodovodniPripojkyBodPP()
        {
            InitializeComponent();
            DataContext = _data;
        }
        public void SetElement(DtmZarizeniVodovodniPripojkyBodElement element)
        {
            _data.SetElement(element);
        }
    }

    class DtmZarizeniVodovodniPripojkyBodPPData
    : ModelBase
    {
        DtmZarizeniVodovodniPripojkyBodElement _element;
        bool _loading = false;
        public void SetElement(DtmZarizeniVodovodniPripojkyBodElement element)
        {
            _loading = true;
            _element = element;
            TypVodovodniPripojky = ((int)_element.TypZarizeniVodovodniPripojky).ToString();
            _loading = false;
        }
        public void Save()
        {
            if (_loading) return;
            _element.TypZarizeniVodovodniPripojky = (DtmTypZarizeniVodovodniPripojkyEnum)int.Parse(TypVodovodniPripojky, CultureInfo.InvariantCulture);
        }
        string _typVodovodniPripojky;
        public string TypVodovodniPripojky
        {
            get => _typVodovodniPripojky;
            set
            {
                SetField(ref _typVodovodniPripojky, value);
                Save();
            }
        }
    }

}
