using GeoBase.Gui;
using GeoHelper.Options;

namespace GeoHelper.Controls
{
    public partial class CustomImportDialog : DialogBase
    {
        public CustomImportDialog()
        {
            InitializeComponent();
            DataContext = this;
            ComboContext = new FileInputOutputOptionsContext();
            ComboContext.LoadFromRegistry();
            FileType = FileTypes.SeznamSouradnic;
        }

        FileInputOutputOptionsContext _comboContext;

        FileTypes _fileType;

        public FileInputOutputOptionsContext ComboContext
        {
            get { return _comboContext; }
            set
            {
                _comboContext = value;
                OnPropertyChanged("ComboContext");
            }
        }

        public FileTypes FileType
        {
            get { return _fileType; }
            set
            {
                _fileType = value;
                OnPropertyChanged("FileType");
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
    }
}