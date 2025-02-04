using System;
using System.Windows;
using System.Windows.Input;

namespace CAD.UITools
{
    public partial class DocumentToolBar : GeoCadToolBar
    {
                public static GeoCadRoutedCommand NewDocumnet = new GeoCadRoutedCommand("NewDocumnet", typeof(MainWin),GeoCadRoutedCommand.CommandTypes.None);
        public static GeoCadRoutedCommand OpenDocument = new GeoCadRoutedCommand("OpenDocument", typeof(MainWin), GeoCadRoutedCommand.CommandTypes.None);
        public static GeoCadRoutedCommand SaveDocument = new GeoCadRoutedCommand("SaveDocument", typeof(MainWin), GeoCadRoutedCommand.CommandTypes.None);
        public static GeoCadRoutedCommand SaveDocumentAs = new GeoCadRoutedCommand("SaveDocumentAs", typeof(MainWin), GeoCadRoutedCommand.CommandTypes.None);
        public static GeoCadRoutedCommand CloseDocument = new GeoCadRoutedCommand("CloseDocument", typeof(MainWin), GeoCadRoutedCommand.CommandTypes.None);
        public static GeoCadRoutedCommand ExportDocumnet = new GeoCadRoutedCommand("ExportDocumnet", typeof(MainWin), GeoCadRoutedCommand.CommandTypes.None);
                        public DocumentToolBar()
        {
            InitializeComponent();
        }
                        private void OnNewDocument(object sender, ExecutedRoutedEventArgs e)
        {
            ToolBarManager.Owner.DocumentNew(string.Empty);
        }

        private void OnOpenDocument(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Geo cad files (*.gcad)|*.gcad";
            Nullable<bool> result = dlg.ShowDialog(Application.Current.MainWindow);
            if (result == true)
            {
                ToolBarManager.Owner.DocumentNew(dlg.FileName);
            }
        }

        private void OnCanSaveDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ToolBarManager!=null && ToolBarManager.Document != null && ToolBarManager.Document.FileName != string.Empty;
        }

        private void onSaveDocument(object sender, ExecutedRoutedEventArgs e)
        {
            ToolBarManager.Document.Save();
        }

        private void OnCanSaveDocumentAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ToolBarManager != null && ToolBarManager.Document != null;
        }

        private void OnSaveDocumentAs(object sender, ExecutedRoutedEventArgs e)
        {
            ToolBarManager.Document.SaveAs();
        }

        private void OnCanCloseDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ToolBarManager != null && ToolBarManager.Document != null;
        }

        private void OnCloseDocument(object sender, ExecutedRoutedEventArgs e)
        {
            ToolBarManager.Owner.CloseDocument();
        }
        private void OnExportDocumnet(object sender, ExecutedRoutedEventArgs e)
        {
            ToolBarManager.Document.Export();
        }

        private void OnCanExportDocumnet(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ToolBarManager != null && ToolBarManager.Document != null;
        }
            }
}
