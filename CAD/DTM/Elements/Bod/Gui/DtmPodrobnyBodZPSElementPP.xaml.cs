using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using CAD.Utils;
using DxfLibrary;


namespace CAD.DTM.Elements.Bod.Gui
{
    public partial class DtmPodrobnyBodZPSElementPP : UserControl
    {
        DtmPodrobnyBodZPSElementPPData _data = new DtmPodrobnyBodZPSElementPPData();
        public DtmPodrobnyBodZPSElementPP()
        {
            InitializeComponent();
            DataContext = _data;
        }
        public void SetElement(DtmPodrobnyBodZPSElement element)
        {
            _data.SetElement(element);
        }
        void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }

    class DtmPodrobnyBodZPSElementPPData
        : ModelBase
    {
        DtmPodrobnyBodZPSElement _element;
        bool _loading = false;
        public void SetElement(DtmPodrobnyBodZPSElement element)
        {
            _loading = true;
            _element = element;
            CisloBodu = _element.CisloBodu;
            _loading = false;
        }
        public void Save()
        {
            if (_loading) return;
            _element.CisloBodu = CisloBodu;
        }
        string _cisloBodu;
        public string CisloBodu
        {
            get => _cisloBodu;
            set
            {
                SetField(ref _cisloBodu, value);
                Save();
            }
        }
    }
}
