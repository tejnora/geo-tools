using System.Globalization;
using System.Windows.Forms;

namespace ContoursViewer
{
    public partial class ContoursIntervalDialog : Form
    {
        public double Interval { get; set; }
        public ContoursIntervalDialog(double minHeight, double maxHeight)
        {
            InitializeComponent();
            _minHeight.Text = minHeight.ToString(CultureInfo.InvariantCulture);
            _maxHeight.Text = maxHeight.ToString(CultureInfo.InvariantCulture);
            _elevationDifference.Text = (maxHeight - minHeight).ToString(CultureInfo.InvariantCulture);
            DialogResult = DialogResult.Cancel;
        }

        private void _ok_Click(object sender, System.EventArgs e)
        {
            double interval;
            if (double.TryParse(_intervalInput.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out interval))
            {
                Interval = interval;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
            MessageBox.Show(@"Interval value is not valid.", @"Invalid Input Value", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            _intervalInput.SelectAll();
            _intervalInput.Focus();
        }

        private void _cancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
