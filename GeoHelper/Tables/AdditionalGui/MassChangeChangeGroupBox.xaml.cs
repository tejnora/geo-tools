namespace GeoHelper.Tables.AdditionalGui
{
    public partial class MassChangeChangeGroupBox
    {
        public enum OperationTypes
        {
            All,
            Selected
        }

        public MassChangeChangeGroupBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        OperationTypes _operationType = OperationTypes.All;
        public OperationTypes OperationType
        {
            get { return _operationType; }
            set
            {
                _operationType = value;
                OnPropertyChanged("OperationType");
            }
        }
    }
}