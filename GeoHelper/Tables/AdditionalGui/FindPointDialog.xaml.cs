using System.Windows.Input;

namespace GeoHelper.Tables.AdditionalGui
{
    public partial class FindPointDialog
    {
        string _prevString;

        public FindPointDialog(TableBase table)
            : base("TableMenuFindPoint", true)
        {
            InitializeComponent();
            DisableDialogResize();
            _tableBase = table;
            _prevString = string.Empty;
        }

        protected override void OnKeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyUp(sender, e);
            if (e.Key == Key.Down)
            {
                _tableBase.SelectNodeNext();
                return;
            }
            if (_prevString == _textBox.Text) return;
            _prevString = _textBox.Text;
            _tableBase.SelectNodeWith(_textBox.Text);
        }

        readonly TableBase _tableBase;
    }
}