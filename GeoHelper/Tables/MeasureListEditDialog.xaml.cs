using GeoHelper.Tables.TableNodes;

namespace GeoHelper.Tables
{
    public partial class MeasureListEditDialog
    {
        public MeasureListEditDialog(TableMeasureListNode aTableMeasureListNode)
            : base("MeasureListEditDialog", true)
        {
            InitializeComponent();
            DataContext = aTableMeasureListNode;
        }
    }
}