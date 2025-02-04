using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using CAD.VFK.DrawTools;
using GeoBase.Localization;
using GeoBase.Utils;
using VFK;
using MessageBox = System.Windows.Forms.MessageBox;
using SaveFileDialog=Microsoft.Win32.SaveFileDialog;

namespace CAD.VFK.GUI
{
    public partial class SeznamSouradnic
    {
                public SeznamSouradnic()
        {
            InitializeComponent();
            ActivePoints = null;
        }
                        Document _mCurrentDocument;
        public VfkActivePointCollection ActivePoints
        {
            get;
            set;
        }
                        public void SetDocument(Document aDoc)
        {
            _mCurrentDocument = aDoc;
            _mDataGrid.DataContext = null;
            if (aDoc != null)
            {
                ActivePoints = aDoc.DataModel.VfkActivePoints;
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(DataSourceRefreshHack));
            }
            else
            {
                ActivePoints = null;
            }
        }
        private void DataSourceRefreshHack()
        {
            _mDataGrid.DataContext = this; 
        }
        public void OnExport(object sender, EventArgs args)
        {
            if (ActivePoints == null) return;
            ExportSouradnicDialog dlg = new ExportSouradnicDialog(ActivePoints);
            dlg.ShowDialog();
        }
        public void OnLoadFromFile(object sender, EventArgs args)
        {
            if (ActivePoints == null) return;
            var dialog = new ImportSouradnicDialog(ActivePoints, _mCurrentDocument);
            dialog.ShowDialog();
        }

        private void DockableContent_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VfkActivePoint vfkActivePoint = _mDataGrid.SelectedItem as VfkActivePoint;
            if(vfkActivePoint!=null)
                _mCurrentDocument.SelectAndScrollToActivePoint(vfkActivePoint);
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UIElement reference = sender as UIElement;
            if (reference != null)
            {
                DependencyObject element = reference.InputHitTest(e.GetPosition(_mDataGrid))
                             as DependencyObject;
                while (element!=null)
                {
                    element = UIHelpers.GetParentObject(element);
                    if (element is DataGridRowHeader && _mDataGrid.SelectedItems.Count==1)
                    {
                        var item = _mDataGrid.SelectedItems[0] as VfkActivePoint;
                        if (item != null)
                        {
                            SouradniceDialog dlg = new SouradniceDialog(item.VfkItem);
                            if(dlg.ShowDialog().GetValueOrDefault(false))
                            {
                                if(!dlg.SouradniceDataContext.AssigneInto(item.VfkItem))
                                {
                                    LanguageDictionary.Current.ShowMessageBox("132", null, MessageBoxButton.OK,
                                                     MessageBoxImage.Warning);
                                }
                                item.OnPropertyChangedAll();
                            }
                        }
                    }
                }
            }
        }
        
    }
}
