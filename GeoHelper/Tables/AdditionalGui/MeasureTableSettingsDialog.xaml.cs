namespace GeoHelper.Tables.AdditionalGui
{
    public partial class MeasureTableSettingsDialog
    {
        public MeasureTableSettingsDialog(TableBase tableBase)
            : base("MeasureTableSettingsDialog", true)
        {
            InitializeComponent();
            DisableDialogResize();
            DataContext = this;
            ColumnVisibility = tableBase.ColumnVisibility;
        }

        public ColumnVisibility ColumnVisibility { get; private set; }

        public bool Prefix
        {
            get { return GetIsSet(ColumnVisibility.Prefix); }
            set { Set(ColumnVisibility.Prefix, value); }
        }

        public bool Number
        {
            get { return GetIsSet(ColumnVisibility.Number); }
            set { Set(ColumnVisibility.Number, value); }
        }

        public bool Hz
        {
            get { return GetIsSet(ColumnVisibility.Hz); }
            set { Set(ColumnVisibility.Hz, value); }
        }

        public bool ZenitAgnle
        {
            get { return GetIsSet(ColumnVisibility.ZenitAngle); }
            set { Set(ColumnVisibility.ZenitAngle, value); }
        }

        public bool dH
        {
            get { return GetIsSet(ColumnVisibility.dH); }
            set { Set(ColumnVisibility.dH, value); }
        }

        public bool Signal
        {
            get { return GetIsSet(ColumnVisibility.Signal); }
            set { Set(ColumnVisibility.Signal, value); }
        }

        public bool Description
        {
            get { return GetIsSet(ColumnVisibility.Description); }
            set { Set(ColumnVisibility.Description, value); }
        }

        public bool HorizontalDistance
        {
            get { return GetIsSet(ColumnVisibility.HorizontalDistance); }
            set { Set(ColumnVisibility.HorizontalDistance, value); }
        }

        bool GetIsSet(ColumnVisibility column)
        {
            return (ColumnVisibility & column) == column;
        }

        void Set(ColumnVisibility column, bool value)
        {
            if (value)
                ColumnVisibility |= column;
            else
            {
                ColumnVisibility ^= column;
            }
        }
    }
}