using System.Linq;
using System.Windows.Controls;
namespace CAD.GUI
{
    public class IntegerTextBox : TextBox
    {
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            Text = new string(Text.Where(c => char.IsDigit(c)).ToArray());
            SelectionStart = Text.Length;
            base.OnTextChanged(e);
        }
    }
}
