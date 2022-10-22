using GeoHelper.Tables.TableNodes;

namespace GeoHelper.Tables
{
    public partial class DoubleCoordinateEditDialog
    {
        public DoubleCoordinateEditDialog(TableDoubleCoordinateListNode tableDoubleCoordinateListNode)
            : base("DoubleCoordinateEditDialog")
        {
            InitializeComponent();
            DataContext = DataContent = tableDoubleCoordinateListNode;
        }

        public TableDoubleCoordinateListNode DataContent { get; set; }
    }
}