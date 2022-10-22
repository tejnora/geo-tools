using GeoBase.Gui;
using GeoHelper.Options;

namespace GeoHelper.Controls
{
    public partial class CustomExportDialog : DialogBase
    {
        public CustomExportDialog()
        {
            InitializeComponent();
            DataContext = this;
            ComboContext = new FileInputOutputOptionsContext();
            ComboContext.LoadFromRegistry();
        }

        FileInputOutputOptionsContext _comboContext;

        public FileInputOutputOptionsContext ComboContext
        {
            get { return _comboContext; }
            set
            {
                _comboContext = value;
                OnPropertyChanged("ComboContext");
            }
        }

        public string ParserPattern
        {
            get
            {
                if (_pattern.SelectedItem == null)
                    return string.Empty;
                return ((FileInputOutputOptionsContext.OFItem) _pattern.SelectedItem).Format;
            }
            private set { }
        }

        public bool UndefinedReplaceZero
        {
            get
            {
                if (_pattern.SelectedItem == null)
                    return false;
                return ((FileInputOutputOptionsContext.OFItem) _pattern.SelectedItem).NedefinovaneNahraditNulami;
            }
            private set { }
        }
    }
}