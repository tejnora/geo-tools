using System.Diagnostics;

namespace GeoHelper.Tables.AdditionalGui
{
    public partial class CoordinateTableSttingsDialog
    {
        public CoordinateTableSttingsDialog(TableBase tableBase)
            : base("CoordinateTableSttingsDialog", true)
        {
            InitializeComponent();
            DisableDialogResize();
            DataContext = this;
            ColumnVisibility = tableBase.ColumnVisibility;
            DoubleCoordinate = tableBase is DoubleCoordinateListTable;
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

        public bool CoordinateX
        {
            get { return GetIsSet(ColumnVisibility.CoordinateX); }
            set { Set(ColumnVisibility.CoordinateX, value); }
        }

        public bool CoordinateY
        {
            get { return GetIsSet(ColumnVisibility.CoordinateY); }
            set { Set(ColumnVisibility.CoordinateY, value); }
        }

        public bool CoordinateZ
        {
            get { return GetIsSet(ColumnVisibility.CoordinateZ); }
            set { Set(ColumnVisibility.CoordinateZ, value); }
        }

        public bool QualityCode
        {
            get { return GetIsSet(ColumnVisibility.Quality); }
            set { Set(ColumnVisibility.Quality, value); }
        }

        public bool Description
        {
            get { return GetIsSet(ColumnVisibility.Description); }
            set { Set(ColumnVisibility.Description, value); }
        }

        public bool SpolX
        {
            get { return GetIsSet(ColumnVisibility.SpolX); }
            set { Set(ColumnVisibility.SpolX, value); }
        }

        public bool SpolY
        {
            get { return GetIsSet(ColumnVisibility.SpolY); }
            set { Set(ColumnVisibility.SpolY, value); }
        }

        public bool SpolQuality
        {
            get { return GetIsSet(ColumnVisibility.SpolQuality); }
            set { Set(ColumnVisibility.SpolQuality, value); }
        }

        public bool DoubleCoordinate { get; set; }

        public bool Coordinate
        {
            get { return !DoubleCoordinate; }
            set { Debug.Assert(false); }
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