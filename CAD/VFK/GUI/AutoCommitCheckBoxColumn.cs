namespace VFK.GUI
{
    
    using System.Windows;
    using System.Windows.Controls;

    
    public class AutoCommitCheckBoxColumn : DataGridCheckBoxColumn
    {
        
        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CommitCellEdit((FrameworkElement)sender);
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            CommitCellEdit((FrameworkElement)sender);
        }

        
        
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var checkBox = (CheckBox)base.GenerateEditingElement(cell, dataItem);

            checkBox.Checked += checkBox_Checked;
            checkBox.Unchecked += checkBox_Unchecked;

            return checkBox;
        }

            }
}
