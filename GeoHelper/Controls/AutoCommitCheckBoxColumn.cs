using System;
using System.Windows;
using System.Windows.Controls;

namespace GeoHelper.Tabulky
{
    public class AutoCommitCheckBoxColumn : DataGridCheckBoxColumn
    {
        void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CommitCellEdit((FrameworkElement) sender);
            DataGridOwner.CommitEdit(DataGridEditingUnit.Row, true);
            UpdateCallBack.Invoke(this, new EventArgs());
        }

        void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            CommitCellEdit((FrameworkElement) sender);
            DataGridOwner.CommitEdit(DataGridEditingUnit.Row, true);
            UpdateCallBack.Invoke(this, new EventArgs());
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var checkBox = (CheckBox) base.GenerateEditingElement(cell, dataItem);

            checkBox.Checked += checkBox_Checked;
            checkBox.Unchecked += checkBox_Unchecked;

            return checkBox;
        }

        public event EventHandler UpdateCallBack;
    }
}