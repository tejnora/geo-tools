using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AvalonDock;
using CAD.Canvas.Layers;
using CAD.GUI;
using CAD.UITools;
using CAD.Utils;
using CAD.Canvas;
using CAD.BackgrounImages;
using GeoBase.Localization;
using GeoBase.Utils;
using VFK.GUI;
using Registry = GeoBase.Utils.Registry;
using ProgramOption = GeoBase.Utils.ProgramOption;

namespace CAD
{
    public partial class MainWin : Window, IMainWinInterface
    {
        
        public MainWin()
        {
            InitLanguages();
            InitializeComponent();
            InitLayout();
            InitToolBars();
        }

        
        
        private GeoCadToolBarManager _toolBarManager;
        private bool _lockUpdateToolBar;

        
        
        private void DockingManagerControl_Loaded(object sender, EventArgs arg)
        {
            ProgramOption po = Singletons.Registry.getEntry(Registry.SubKey.kCurrentUser, "MainWindowDockPosAndSize");
            if (po.isString())
            {
                using (System.IO.StringReader reader = new System.IO.StringReader(po.getString()))
                {
                    _dockingManager.RestoreLayout(reader);
                }
            }
        }

        private Document GetDocument()
        {
            if (_dockingManager == null || _dockingManager.ActiveDocument == null)
                return null;
            if (_dockingManager.ActiveDocument is Document)
                return ((Document)_dockingManager.ActiveDocument);
            return null;
        }

        private void OnMainDocumentChanged(object sender, EventArgs e)
        {
            Document doc = GetDocument();
            _toolBarManager.Document = GetDocument();
            if (doc != null)
            {
                if (doc.GetIsVfkMain() != null)
                {
                    _seznamSouradnic.SetDocument(doc);
                    return;
                }
            }

            _seznamSouradnic.SetDocument(null);
        }

        
        
        
        private void OnAddRaster(object sender, EventArgs e)
        {
            Document doc = GetDocument();
            if (doc == null) return;
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CIT (only 24 code supported) (*.cit)|*.cit"
            };
            Nullable<bool> result = dlg.ShowDialog(this);
            if (result == true)
            {
                var iibl = doc.DataModel.ImageBackgrounLayer as IImageBackgrounLayer;
                if (iibl != null) iibl.AddImageObject(new BackgrounImage(dlg.FileName));
                doc.CanvasCommand.InvalidateAll();
            }
        }

        private void OnRemoveRaster(object sender, EventArgs e)
        {
            Document doc = GetDocument();
            if (doc == null) return;
            IImageBackgrounLayer iibl = doc.DataModel.ImageBackgrounLayer as IImageBackgrounLayer;
            if (iibl != null)
            {
                if (iibl.getImageObjectCount() == 0)
                    return;
                if (iibl.getImageObjectCount() == 1)
                {
                    iibl.DeleteImageObject(iibl.getImageObject(0));
                    doc.CanvasCommand.InvalidateAll();
                }
            }
        }

        private void OnManageRaster(object sender, EventArgs e)
        {
            Document doc = GetDocument();
            if (doc == null) return;
        }

        
        
        private void OnImportVfk(object aSender, EventArgs aArg)
        {
            Document doc = GetDocument();
            if (doc == null)
            {
                DocumentNew(string.Empty);
                doc = GetDocument();
            }

            if (doc.ImportVfk(string.Empty))
                OnMainDocumentChanged(this, EventArgs.Empty);
        }

        void OnCanImportVfk(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        void OnImportDtm(object aSender, EventArgs aArg)
        {
            var doc = GetDocument();
            if (doc == null)
            {
                DocumentNew(string.Empty);
                doc = GetDocument();
            }

            if (doc.ImportDtm(string.Empty))
                OnMainDocumentChanged(this, EventArgs.Empty);
        }

        void CanImportDtm(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void OnExportVfk(object aSender, EventArgs aArg)
        {
            Document doc = GetDocument();
            if (doc == null) return;
            doc.ExportVfk();
        }

        private void OnCanExportVfk(object sender, CanExecuteRoutedEventArgs args)
        {
            Document doc = GetDocument();
            args.CanExecute = doc != null && doc.IsImportedVfk();
        }

        private void OnRemoveVFKData(object aSender, EventArgs aArg)
        {
            Document doc = GetDocument();
            if (doc == null) return;
            doc.RemoveVfk();
            _seznamSouradnic.SetDocument(null);
        }

        private void OnCanRemoveVFKData(object sender, CanExecuteRoutedEventArgs args)
        {
            Document doc = GetDocument();
            args.CanExecute = doc != null && doc.IsImportedVfk();
        }

        private void OnVFKEdidOfParcel(object aSender, EventArgs aArgs)
        {
            Document doc = GetDocument();
            doc.OnVfkEdidOfParcel();
        }

        private void OnCanVFKEdidOfParcel(object sender, CanExecuteRoutedEventArgs args)
        {
            Document doc = GetDocument();
            args.CanExecute = doc != null && doc.IsImportedVfk();
        }

        private void OnSeznamSouradnicVFK(object aSender, EventArgs aArgs)
        {
            _dockingManager.Show(_seznamSouradnic);
        }

        private void OnCanSeznamSouradnicVFK(object sender, CanExecuteRoutedEventArgs args)
        {
            Document doc = GetDocument();
            args.CanExecute = doc != null && doc.IsImportedVfk();
        }

        private void OnSetElements(object aSender, EventArgs aArgs)
        {
            ElementsProperties prop = new ElementsProperties();
            prop.Show();
        }

        private void OnGenerateSIP(object aSender, EventArgs aArgs)
        {
            Document doc = GetDocument();
            doc.OnGenerateSip();
        }

        private void OnCanGenerateSIP(object sender, CanExecuteRoutedEventArgs args)
        {
            Document doc = GetDocument();
            args.CanExecute = doc != null && doc.IsImportedVfk();
        }

        
        
        private void onSavePosAndSize(object sender, EventArgs arg)
        {
            Singletons.Registry.setEntry(Registry.SubKey.kCurrentUser, "MainWindowPos",
                new ProgramOption(new Point(Left, Top)));
            if (WindowState != WindowState.Maximized)
            {
                Singletons.Registry.setEntry(Registry.SubKey.kCurrentUser, "MainWindowSize",
                    new ProgramOption(new Point(ActualWidth, ActualHeight)));
            }

            Singletons.Registry.setEntry(Registry.SubKey.kCurrentUser, "MainWindowState",
                new ProgramOption(WindowState == WindowState.Maximized));
            StringBuilder builder = new StringBuilder();
            using (System.IO.StringWriter writer = new System.IO.StringWriter((builder)))
            {
                _dockingManager.SaveLayout(writer);
            }

            Singletons.Registry.setEntry(Registry.SubKey.kCurrentUser, "MainWindowDockPosAndSize",
                new ProgramOption(builder.ToString()));
        }

        
        
        private void OnAboutBox(object sender, RoutedEventArgs e)
        {
            AboutBox aboutBox = new AboutBox(this);
            aboutBox.ShowDialog();
        }

        
        
        
        public void SetPositionInfo(UnitPoint aUnitpos)
        {
            Document document = GetDocument();
            if (document == null) return;
            string text = "[" + aUnitpos.X.ToString("#.000") + ", " + aUnitpos.Y.ToString("#.000") + "]";
            _statusBarInfoPos.Text = text;
            string s = string.Empty;
            if (document.DataModel.SelectedCount == 1 || document.CanvasCtrl.NewObject != null)
            {
                IDrawObject obj = document.DataModel.GetFirstSelected();
                if (obj == null)
                    obj = document.CanvasCtrl.NewObject;
                if (obj != null)
                    s = obj.GetInfoAsString();
            }

            _drawInfoLabel.Text = s;
        }

        public void SetSnapInfo(ISnapPoint aSnap)
        {
            string snapHint = string.Empty;
            if (aSnap != null)
                snapHint = string.Format("Snap@{0}, {1}", aSnap.SnapPoint.PosAsString(), aSnap.GetType());
            _snapBarInfoLabel.Text = snapHint;
        }

        public void DocumentNew(string aName)
        {
            Document doc = new Document(aName, this);
            doc.InfoTip = "Info tipo for " + doc.Title;
            doc.ContentTypeDescription = "Sample document";
            _dockingManager.MainDocumentPane.Items.Add(doc);
            doc.CanvasCommand.CommandFitView();
        }

        public void CloseDocument()
        {
            ((DocumentContent)_dockingManager.ActiveDocument).Close();
        }

        public void UpdateToolBars(GeoCadRoutedCommand command)
        {
            if (_lockUpdateToolBar) return;
            _lockUpdateToolBar = true;
            _toolBarManager.Command = command;
            _lockUpdateToolBar = false;
        }

        
        
        private void InitLanguages()
        {
            LanguageDictionary.RegisterDictionary(
                CultureInfo.GetCultureInfo("cs-CZ"),
                new XmlLanguageDictionary("Languages/cs-CZ.xml", string.Empty));
            LanguageContext.Instance.Culture = CultureInfo.GetCultureInfo("cs-CZ");
        }

        private void InitLayout()
        {
            ProgramOption po = Singletons.Registry.getEntry(Registry.SubKey.kCurrentUser, "MainWindowPos");
            if (po.isPoint())
            {
                Left = po.getPoint().X;
                Top = po.getPoint().Y;
            }

            po = Singletons.Registry.getEntry(Registry.SubKey.kCurrentUser, "MainWindowState");
            if (po.isBool())
            {
                if (po.getBool())
                {
                    WindowState = WindowState.Maximized;
                }
            }

            po = Singletons.Registry.getEntry(Registry.SubKey.kCurrentUser, "MainWindowSize");
            if (po.isPoint())
            {
                Width = po.getPoint().X;
                Height = po.getPoint().Y;
            }
        }

        private void InitToolBars()
        {
            _toolBarManager = new GeoCadToolBarManager(this);
            _toolBarManager.Document = GetDocument();
            _toolBarManager.RegisterToolBar(_toolBarTrayLeft, new DrawToolBar());
            _toolBarManager.RegisterToolBar(_toolBarTrayTop, new DocumentToolBar());
            _toolBarManager.RegisterToolBar(_toolBarTrayTop, new EditToolBar());
            _toolBarManager.RegisterToolBar(_toolBarTrayTop, new AttributesToolBar());
            _toolBarManager.RegisterToolBar(_toolBarTrayTop, new VfkToolBar());
            _toolBarManager.NotifyDocumentChanged();
            _toolBarManager.MergeCommandBindings(CommandBindings);
            _toolBarManager.MergeInputBindings(InputBindings);
            _toolBarManager.ToolChanged += OnToolChanged;
        }

        private void OnToolChanged(GeoCadRoutedCommand command)
        {
            switch (command.CommandType)
            {
                case GeoCadRoutedCommand.CommandTypes.None:
                    break;
                case GeoCadRoutedCommand.CommandTypes.DrawTool:
                    GetDocument().CanvasCommand.CommandSelectDrawTool(command);
                    break;
                case GeoCadRoutedCommand.CommandTypes.EditTool:
                    GetDocument().CanvasCommand.CommandEdit(command);
                    break;
                case GeoCadRoutedCommand.CommandTypes.Select:
                    GetDocument().CanvasCommand.CommandEscape(false);
                    break;
                case GeoCadRoutedCommand.CommandTypes.Pan:
                    GetDocument().CanvasCommand.CommandPan();
                    break;
                case GeoCadRoutedCommand.CommandTypes.Move:
                    GetDocument().CanvasCommand.CommandMove(false);
                    break;
                case GeoCadRoutedCommand.CommandTypes.InfoTool:
                    GetDocument().CanvasCommand.CommandInfoTool(command);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

            }
}