namespace GeoHelper.Controls
{
    public enum FileTypes
    {
        SeznamSouradnic,
        SeznamSouradnicDvoji,
        MerenaDataPolarni,
        Protokol
    }

    public partial class NewFileDialog
    {
        FileTypes _fileType;

        public NewFileDialog()
            : base("NewFileDialog")
        {
            InitializeComponent();
            FileType = FileTypes.SeznamSouradnic;
            DataContext = this;
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
    }
}