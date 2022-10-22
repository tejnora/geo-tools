using System.Windows.Input;
using GeoHelper.Tables.TableNodes;

namespace GeoHelper.Tables
{
    public partial class CoordinateListEditDialog
    {
        public CoordinateListEditDialog(TableCoordinateListNode a)
            : base("CoordinateListEditDialog")
        {
            InitializeComponent();
            DataContext = DataContent = a;
        }

        public TableCoordinateListNode DataContent { get; set; }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
                return;
            }
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
                Close();
                return;
            }
            base.OnKeyDown(e);
        }
    }
}