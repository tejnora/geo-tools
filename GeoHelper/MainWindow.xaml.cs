using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using GeoBase.Localization;
using GeoBase.Utils;
using GeoCalculations.MethodPoints;
using GeoHelper.Calculations;
using GeoHelper.Controls;
using GeoHelper.FileExport;
using GeoHelper.FileParses;
using GeoHelper.Options;
using GeoHelper.Printing;
using GeoHelper.Protocols;
using GeoHelper.Tables;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Tabulky;
using GeoHelper.Tools;
using GeoHelper.Utils;
using Microsoft.Win32;
using Utils.PartialStream;
using WPF.MDI;
using Registry = GeoBase.Utils.Registry;

namespace GeoHelper
{
    public partial class MainWindow : INotifyPropertyChanged, IMainWindow
    {
        public MainWindow()
        {
            LanguageDictionary.RegisterDictionary(CultureInfo.GetCultureInfo("cs-CZ"),
                                                  new XmlLanguageDictionary("Localization/cs-CZ.xml",
                                                                            "Localization/CZ/LocalizationSettings.xml"));
            LanguageContext.Instance.Culture = CultureInfo.GetCultureInfo("cs-CZ");
            InitializeComponent();
            _recentFileList.UseRegistryPersister(Singletons.MyRegistry, "RecentFiles");
            _recentFileList.MaxPathLength = 100;
            _recentFileList.DisplayPathWithExt = true;
            DataContext = this;
            LoadPosAndSize();
            Container.Theme = MdiContainer.ThemeType.Aero;
            Loaded += OnLoaded;
        }

        void OnClosing(object sender, EventArgs args)
        {
            SavePosAndSize();
            SaveChildrenWindws();
            SaveVypctyDialogs();
            foreach (MdiChild child in Container.Children)
            {
                AskForSaveWhenClosingMdi(child);
            }
            Singletons.MyRegistry.Save();
        }

        void OnLoaded(object sender, EventArgs args)
        {
            LoadChildrenWindows();
            LoadVypoctyDialogs();
        }

        MdiChild _currentMdi;

        public bool CanSaveAs
        {
            get { return Container.Children.Count > 0; }
            set { }
        }

        public bool CanSave
        {
            get
            {
                if (_currentMdi == null || Container.Children.Count == 0)
                    return false;
                var ls = _currentMdi.Content as ILoadSaveMdi;
                if (ls == null || ls.FileName == null)
                    return false;
                return true;
            }
            set { }
        }

        void OnNewFile(object sender, EventArgs e)
        {
            var dlg = new NewFileDialog();
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                UIElement content;
                object title;
                switch (dlg.FileType)
                {
                    case FileTypes.SeznamSouradnic:
                        title = dictionary.Translate("49", "Text", "SeznamSouradnic", typeof(string));
                        content = new CoordinateListTable(this);
                        break;
                    case FileTypes.SeznamSouradnicDvoji:
                        content = new DoubleCoordinateListTable(this);
                        title = dictionary.Translate("48", "Text", "SeznamSouradnicDvoji", typeof(string));
                        break;
                    case FileTypes.MerenaDataPolarni:
                        content = new MeasureListTable(this);
                        title = dictionary.Translate("5", "Text", "MeasurementsListPolarMethod", typeof(string));
                        break;
                    case FileTypes.Protokol:
                        if (_protokol != null)
                        {
                            object res = dictionary.Translate("102", "Text", "Může být otevřen pouze jeden protokol.",
                                                              typeof(string));
                            object res1 = dictionary.Translate("57", "Text", "Pozor", typeof(string));
                            MessageBox.Show((string)res, (string)res1, MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        _protokol = new Protokol(this);
                        content = _protokol;
                        title = dictionary.Translate("101", "Title", "Protokol", typeof(string));
                        break;
                    default:
                        Debug.Assert(false);
                        return;
                }
                Container.Children.Add(new MdiChild
                                           {
                                               Title = (string)title,
                                               Content = content,
                                               Width = 714,
                                               Height = 734,
                                               Margin = new Thickness(20, 100, 0, 0)
                                           });
                SetModifiedFlag(content);
            }
        }

        void OnOpenRecentFile(object sender, RecentFileList.MenuClickEventArgs args)
        {
            OpenFileCore(new FileInfo(args.Filepath), args.FilterIndex);
        }

        void OnOpenFile(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Všechny (*.s,*.sdv,*.mpl,*.crd,*.mes)|*.s;*.mpl;*.sdv;*.crd;*.mes|" + //1
                         "Raw files (*.raw)|*.raw|" + //2
                         "Souřadnice YXZ(*.txt)|*.txt|" + //3
                         "Protokol (*.prot)|*.prot|" + //4
                         "Uzivatelský format (*.*)|*.*|" + //5
                         "Groma crd format (*.crd)|*.crd|" +//6
                         "Groma mes format (*.mes)|*.mes|" +//7
                         "All files (*.*)|*.*"; //8
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                OpenFileCore(new FileInfo(dlg.FileName), dlg.FilterIndex);
            }
        }

        void OpenFileCore(FileInfo fileName, int filterIndex)
        {
            var allFilesIdx = 8;
            if (filterIndex == -1)
                filterIndex = allFilesIdx;
            try
            {
                var dictionary = LanguageConverter.ResolveDictionary();
                if (fileName.Extension == ".s" && (filterIndex == allFilesIdx || filterIndex == 1))
                {
                    var ss = new CoordinateListTable(this) { FileName = fileName };
                    var title = dictionary.Translate("49", "Text", "SeznamSouradnic", typeof(string));
                    var mdi = new MdiChild
                            {
                                Title = (string)title + " - " + fileName.FullName,
                                Content = ss,
                                Width = 714,
                                Height = 734,
                            };
                    Stream stream = File.Open(fileName.FullName, FileMode.Open);
                    LoadWindowInfo(stream, mdi);
                    ss.Deserialize(stream);
                    Container.Children.Add(mdi);
                }
                else if (fileName.Extension == ".sdv" && (filterIndex == allFilesIdx || filterIndex == 1))
                {
                    var ss = new DoubleCoordinateListTable(this) { FileName = fileName };
                    object title = dictionary.Translate("48", "Text", "SeznamSouradnicDvoji", typeof(string));
                    var mdi =
                        new MdiChild
                            {
                                Title = (string)title + " - " + fileName.FullName,
                                Content = ss,
                                Width = 714,
                                Height = 734,
                            };
                    Stream stream = File.Open(fileName.FullName, FileMode.Open);
                    LoadWindowInfo(stream, mdi);
                    ss.Deserialize(stream);
                    Container.Children.Add(mdi);
                }
                else if (fileName.Extension == ".mpl" && (filterIndex == allFilesIdx || filterIndex == 1))
                {
                    var mlpm = new MeasureListTable(this) { FileName = fileName };
                    var title = dictionary.Translate("5", "Text", "MeasurementsListPolarMethod", typeof(string));
                    var mdi =
                        new MdiChild
                            {
                                Title = (string)title + " - " + fileName.FullName,
                                Content = mlpm,
                                Width = 714,
                                Height = 734,
                            };
                    Stream stream = File.Open(fileName.FullName, FileMode.Open);
                    LoadWindowInfo(stream, mdi);
                    mlpm.Deserialize(stream);
                    Container.Children.Add(mdi);
                }
                else if (fileName.Extension == ".raw" && (filterIndex == allFilesIdx || filterIndex == 2))
                {
                    var parser = new NiconRowParser();
                    parser.ParseFile(fileName);
                    object title = dictionary.Translate("5", "Text", "MeasurementsListPolarMethod", typeof(string));
                    Container.Children.Add(new MdiChild
                                               {
                                                   Title = (string)title + " - " + fileName.FullName,
                                                   Content = new MeasureListTable(this, parser.Records),
                                                   Width = 714,
                                                   Height = 734,
                                               });
                }
                else if (fileName.Extension == ".txt" && (filterIndex == 3))
                {
                    var parser = new YXZTextParser();
                    parser.ParseFile(fileName);
                    object title = dictionary.Translate("33", "Text", "Souracnice", typeof(string));
                    Container.Children.Add(new MdiChild
                                               {
                                                   Title = (string)title + " - " + filterIndex,
                                                   Content = new CoordinateListTable(parser.Nodes, this),
                                                   Width = 714,
                                                   Height = 734,
                                               });
                }
                else if (fileName.Extension == ".prot" && (filterIndex == allFilesIdx || filterIndex == 4))
                {
                    if (_protokol != null)
                    {
                        var res = dictionary.Translate("102", "Text", "Může být otevřen pouze jeden protokol.", typeof(string));
                        var res1 = dictionary.Translate("57", "Text", "Pozor", typeof(string));
                        MessageBox.Show((string)res, (string)res1, MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    _protokol = new Protokol(this) { FileName = fileName };
                    object title = dictionary.Translate("101", "Title", "Protokol", typeof(string));
                    var mdi =
                        new MdiChild
                            {
                                Title = (string)title + " - " + fileName.FullName,
                                Content = _protokol,
                                Width = 714,
                                Height = 734,
                            };
                    try
                    {
                        Stream stream = File.Open(fileName.FullName, FileMode.Open);
                        LoadWindowInfo(stream, mdi);
                        _protokol.Deserialize(stream);
                    }
                    catch (Exception)
                    {
                        _protokol = null;
                        throw;
                    }
                    Container.Children.Add(mdi);
                }
                else if (filterIndex == 5 || filterIndex == allFilesIdx)
                {
                    var dialog = new CustomImportDialog();
                    dialog.Owner = this;
                    dialog.ShowDialog();
                    if (!dialog.DialogResult.GetValueOrDefault(false))
                        return;
                    String parserPattern = dialog.ParserPattern;
                    if (parserPattern.Length == 0)
                    {
                        object res = dictionary.Translate("252", "Text", "Pattern nemuze byt prazdny.", typeof(string));
                        throw new Exception((string)res);
                    }
                    UserFormatParser parser = null;
                    switch (dialog.FileType)
                    {
                        case FileTypes.SeznamSouradnic:
                            parser = new SSUserFormatParser();
                            break;
                        case FileTypes.SeznamSouradnicDvoji:
                            parser = new SSDUserFormatParser();
                            break;
                        case FileTypes.MerenaDataPolarni:
                            parser = new MLPMUserFormatParser();
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    parser.ParserPatter = parserPattern;
                    parser.ParseFile(fileName);
                    string title = string.Empty;
                    UIElement content = null;
                    switch (dialog.FileType)
                    {
                        case FileTypes.SeznamSouradnic:
                            title = (string)dictionary.Translate("33", "Text", "Souracnice", typeof(string));
                            content = new CoordinateListTable(parser.Nodes, this);
                            break;
                        case FileTypes.SeznamSouradnicDvoji:
                            title = (string)dictionary.Translate("48", "Text", "SeznamSouradnicDvoji", typeof(string));
                            content = new DoubleCoordinateListTable(this, parser.Nodes, false);
                            break;
                        case FileTypes.MerenaDataPolarni:
                            title =
                                (string)
                                dictionary.Translate("5", "Text", "MeasurementsListPolarMethod", typeof(string));
                            content = new MeasureListTable(this, parser.Nodes);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    Container.Children.Add(new MdiChild
                                               {
                                                   Title = title,
                                                   Content = content,
                                                   Width = 714,
                                                   Height = 734,
                                               });
                    filterIndex = -1;
                }
                else if (fileName.Extension == ".crd" && (filterIndex == allFilesIdx || filterIndex == 6 || filterIndex == 1))
                {
                    var parser = new GromaCrdFileParser();
                    parser.ParseFile(fileName);
                    var nodes = new CoordinateListTable(parser.Nodes, this);
                    var mdi = new MdiChild
                        {
                            Title = dictionary.Translate<string>("49", "Text") + " - " + fileName.FullName,
                            Content = nodes,
                            Width = 714,
                            Height = 734,
                        };
                    Container.Children.Add(mdi);
                }
                else if (fileName.Extension == ".mes" && (filterIndex == allFilesIdx || filterIndex == 7 || filterIndex == 1))
                {
                    var parser = new GromaMesFileParser();
                    parser.ParseFile(fileName);
                    var table = new MeasureListTable(this, parser.Nodes) { FileName = fileName };
                    var mdi = new MdiChild
                    {
                        Title = dictionary.Translate<string>("5", "Text") + " - " + fileName.FullName,
                        Content = table,
                        Width = 714,
                        Height = 734,
                    };
                    Container.Children.Add(mdi);
                }
                else
                {
                    object res = dictionary.Translate("3", "Text", "File can not be opened.", typeof(string));
                    MessageBox.Show((string)res, (string)res, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                    ;
                }
                if (filterIndex != -1)
                    _recentFileList.InsertFile(fileName.FullName, filterIndex);
            }
            catch (ParseException ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            OnPropertyChanged("CanSave");
        }

        void OnSaveFile(object sender, RoutedEventArgs args)
        {
            var ls = _currentMdi.Content as ILoadSaveMdi;
            if (ls.FileStream != null)
            {
                OnSaveFile(ls.FileName.FullName);
                ls.RestModifedFlag();
                if (_currentMdi.Title.Length > 0 && _currentMdi.Title.Substring(_currentMdi.Title.Length - 1) == "*")
                {
                    _currentMdi.Title = _currentMdi.Title.Substring(0, _currentMdi.Title.Length - 1);
                }
            }
            else
            {
                OnSaveFileAs(null, null);
            }
        }

        void OnSaveFileAs(object sender, RoutedEventArgs args)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = (_currentMdi.Content as ILoadSaveMdi).SaveDialogFilter +
                         "|Uzivatelský format (*.*)|*.*";
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                if (dlg.FilterIndex == 1 && OnSaveFile(dlg.FileName))
                {
                    var title = string.Empty;
                    var dictionary = LanguageConverter.ResolveDictionary();
                    if (_currentMdi.Content is MeasureListTable)
                        title = dictionary.Translate<string>("5", "Text");
                    else if (_currentMdi.Content is CoordinateListTable)
                        title = dictionary.Translate<string>("33", "Text");
                    else if (_currentMdi.Content is DoubleCoordinateListTable)
                        title = dictionary.Translate<string>("48", "Text");
                    else if (_currentMdi.Content is Protokol)
                        title = dictionary.Translate<string>("101", "Title");
                    else
                        throw new NotImplementedException();
                    title += " - " + dlg.FileName;
                    _currentMdi.Title = title;
                    (_currentMdi.Content as ILoadSaveMdi).RestModifedFlag();
                    _recentFileList.InsertFile(dlg.FileName, -1);
                    OnPropertyChanged("CanSave");
                    OnPropertyChanged("CanSaveAs");
                }
                else
                {
                    try
                    {
                        var dialog = new CustomExportDialog();
                        dialog.ShowDialog();
                        if (!dialog.DialogResult.GetValueOrDefault(false))
                            return;
                        var export = new UserExportToTextFile(dlg.FileName, dialog.ParserPattern,
                                                              _currentMdi.Content as TableBase,
                                                              dialog.UndefinedReplaceZero);
                        export.Export();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        bool OnSaveFile(string location)
        {
            try
            {
                var saveIface = _currentMdi.Content as ILoadSaveMdi;

                Stream stream = null;
                var fileInfo = new FileInfo(location);
                string backupPath = string.Empty;
                if (saveIface.FileStream != null)
                {
                    stream = saveIface.FileStream;
                    stream.Seek(0, SeekOrigin.Begin);
                    backupPath = saveIface.FileName.FullName + ".bak";
                    using (Stream backup = File.Create(backupPath))
                    {
                        stream.CopyTo(backup);
                    }
                }
                else
                {
                    stream = File.Create(fileInfo.FullName);
                    saveIface.FileStream = stream;
                    saveIface.FileName = fileInfo;
                }
                stream.Seek(0, SeekOrigin.Begin);
                SaveWindowInfo(stream, _currentMdi);
                saveIface.Serialize(stream);
                stream.Flush();
                if (backupPath.Length > 0)
                {
                    File.Delete(backupPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        void SaveWindowInfo(Stream stream, MdiChild child)
        {
            stream.Seek(0, SeekOrigin.Begin);
            Stream windowStream = PartialStreamFactory.CreateWritePartialStream(stream);
            windowStream.WriteInt32(1);
            child.Save(windowStream);
            windowStream.Dispose();
        }

        void LoadWindowInfo(Stream stream, MdiChild child)
        {
            Stream windoStream = PartialStreamFactory.CreateReadPartialStream(stream);
            windoStream.ReadInt32();
            child.Load(windoStream);
            windoStream.Dispose();
        }

        public void OnMdiFocusChanged(object sender, FocusedChangedEventArgs arg)
        {
            _currentMdi = arg.MdiChild;
            if (_currentMdi.Content is IEditMenu)
            {
                (_currentMdi.Content as IEditMenu).FillMenuItems(_tableMenu);
            }
            else
            {
                _tableMenu.Visibility = Visibility.Collapsed;
            }
            OnPropertyChanged("CanSave");
            OnPropertyChanged("CanSaveAs");
            OnPropertyChanged("CanPrint");
        }

        public void OnRemoveChildern(object sender, EventArgs args)
        {
            if (_currentMdi == null) return;
            if (_currentMdi.Content != null)
                AskForSaveWhenClosingMdi(_currentMdi);
            if (_currentMdi.Content == _protokol)
            {
                _protokol = null;
            }
            if (_currentMdi != null && _currentMdi.Content is IDisposable)
                (_currentMdi.Content as IDisposable).Dispose();
            _currentMdi = null;
            _tableMenu.Visibility = Visibility.Collapsed;
            OnPropertyChanged("CanSave");
            OnPropertyChanged("CanSaveAs");
            OnPropertyChanged("CanPrint");
        }

        public void OnExit(object sender, EventArgs args)
        {
            Close();
        }

        readonly Dictionary<string, Window> _otevereneVypocty = new Dictionary<string, Window>();
        FileTransferDialog _predosSouboruDialogDialog;

        public bool CanPrint
        {
            get
            {
                if (_currentMdi != null && _currentMdi.Content is IPrinting)
                    return true;
                return false;
            }
        }

        void OpenWindow(string name, Type windowType)
        {
            if (_otevereneVypocty.ContainsKey(name))
            {
                (_otevereneVypocty[name]).Activate();
                return;
            }
            var args = new object[2];
            args[0] = name;
            args[1] = this;
            _otevereneVypocty[name] = (Window)Activator.CreateInstance(windowType, args);
            _otevereneVypocty[name].Owner = this;
            _otevereneVypocty[name].Show();
        }

        void OnVypoctyOrtogonalniMetoda(object sender, EventArgs args)
        {
            OpenWindow("VypoctyOrtogonalniMetoda", typeof(OrtogonalMethodDialog));
        }

        void OnVypoctyPolarniMetodaDavkou(object sender, EventArgs args)
        {
            OpenWindow("OnVypoctyPolarniMetodaDavkou", typeof(PolarMethodBatchDialog));
        }

        void OnVypoctyPolarniMetoda(object sender, EventArgs args)
        {
            OpenWindow("VypoctyVypoctyPolarniMetoda", typeof(PolarMethodDialog));
        }

        void OnVolneStanoviskoMetoda(object sender, EventArgs args)
        {
            OpenWindow("VypoctyVolneStanoviskoMetoda", typeof(FreePointOfViewDialog));
        }

        void OnProtinaniZDelek(object sender, EventArgs args)
        {
            OpenWindow("VypoctyProtinaniZDelek", typeof(LengthIntersectionDialog));
        }

        void OnProtinaniZeSmeru(object sender, EventArgs args)
        {
            OpenWindow("VypoctyProtinaniZeSmeru", typeof(DirectionIntersectionDialog));
        }

        void OnPrusecikPrimek(object sender, EventArgs args)
        {
            OpenWindow("VypoctyPrusecikPrimek", typeof(LinesIntersectionDialog));
        }

        void OnPolygonovyPorad(object sender, EventArgs args)
        {
            OpenWindow("VypoctyPolygonovyPorad", typeof(PolygonTraverseDialog));
        }

        void OnKonstrukcniOdmerne(object sender, ExecutedRoutedEventArgs e)
        {
            OpenWindow("VypocetKonstrukcniOdmerne", typeof(ConstructionDistanceDialog));
        }

        void OnVypoctyKontrolniOdmerne(object sender, ExecutedRoutedEventArgs e)
        {
            OpenWindow("VypocetKontrolniOdmerne", typeof(ControlDistanceDialog));
        }

        void OnVypoctySmernikADelka(object sender, ExecutedRoutedEventArgs e)
        {
            OpenWindow("VypoctySmernikADelka", typeof(DirectionAzimutAndLengthDialog));
        }

        void OnVypoctyTrasformaceSouradnice(object sender, ExecutedRoutedEventArgs e)
        {
            OpenWindow("VypoctyTrasformaceSouradnice", typeof(TransformCoordinatesDialog));
        }

        void OnPrinting(object sender, EventArgs args)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog().GetValueOrDefault(false))
            {
                var paginator = new PrintingPaginator(_currentMdi.Content as IPrinting, printDialog.PrintSetting);
                var ls = _currentMdi.Content as ILoadSaveMdi;
                if (ls.FileName != null)
                    printDialog.PrintDocument(paginator, ls.FileName.FullName);
                else
                    printDialog.PrintDocument(paginator, "New document");
            }
        }

        void OnOptions(object sender, EventArgs args)
        {
            var dlg = new OptionsDialog(new OptionsDialogContext());
            dlg.Owner = this;
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                dlg.OptionsDialogContext.SaveToRegistry();
            }
        }

        void OnCopy(object sender, EventArgs args)
        {
            (_currentMdi.Content as IManipulateItems).Copy();
        }

        void CanCopy(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = false;
            if (_currentMdi != null && _currentMdi.Content is IManipulateItems)
            {
                var man = _currentMdi.Content as IManipulateItems;
                args.CanExecute = man.CanCopy();
            }
            args.Handled = true;
        }

        void OnCut(object sender, EventArgs args)
        {
            (_currentMdi.Content as IManipulateItems).Cut();
        }

        void CanCut(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = false;
            if (_currentMdi != null && _currentMdi.Content is IManipulateItems)
            {
                var man = _currentMdi.Content as IManipulateItems;
                args.CanExecute = man.CanCut();
            }
            args.Handled = true;
        }

        void OnPaste(object sender, EventArgs args)
        {
            (_currentMdi.Content as IManipulateItems).Paste();
        }

        void CanPaste(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = false;
            if (_currentMdi != null && _currentMdi.Content is IManipulateItems)
            {
                var man = _currentMdi.Content as IManipulateItems;
                args.CanExecute = man.CanPaste();
            }
            args.Handled = true;
        }

        void OnDelete(object sender, EventArgs args)
        {
            (_currentMdi.Content as IManipulateItems).Delete(false);
        }

        void CanDelete(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = false;
            if (_currentMdi != null && _currentMdi.Content is IManipulateItems)
            {
                var man = _currentMdi.Content as IManipulateItems;
                args.CanExecute = man.CanDelete();
            }
            args.Handled = true;
        }

        void OnUndo(object sender, EventArgs args)
        {
            (_currentMdi.Content as IManipulateItems).Undo();
        }

        void CanUndo(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = false;
            if (_currentMdi != null && _currentMdi.Content is IManipulateItems)
            {
                var man = _currentMdi.Content as IManipulateItems;
                args.CanExecute = man.CanUndo();
            }
            args.Handled = true;
        }

        void OnRedo(object sender, EventArgs args)
        {
            (_currentMdi.Content as IManipulateItems).Redo();
        }

        void CanRedo(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = false;
            if (_currentMdi != null && _currentMdi.Content is IManipulateItems)
            {
                var man = _currentMdi.Content as IManipulateItems;
                args.CanExecute = man.CanRedo();
            }
            args.Handled = true;
        }

        void OnPrenosSoboru(object sender, EventArgs args)
        {
            if (_predosSouboruDialogDialog == null || !_predosSouboruDialogDialog.IsVisible)
            {
                _predosSouboruDialogDialog = new FileTransferDialog(this);
                _predosSouboruDialogDialog.Owner = this;
                _predosSouboruDialogDialog.Show();
            }
            else
            {
                _predosSouboruDialogDialog.Activate();
            }
        }

        void OnAktivniSeznamSouradnic(object sender, ExecutedRoutedEventArgs e)
        {
            List<Tuple<string, TableBase>> list = GetTabulkySouradnic();
            var ass = new ActiveListOfCoordinates();
            ass.Init(list, _aktivniCoordinateListTable);
            if (ass.ShowDialog().GetValueOrDefault(false))
            {
                _aktivniCoordinateListTable = ass.GetSelectedTable() as CoordinateListTable;
            }
        }

        CoordinateListTable _aktivniCoordinateListTable;
        Protokol _protokol;

        public void CreateWindowWithDoubleCoordinates(List<TableNodesBase> aFromNode)
        {
            var ssd = new DoubleCoordinateListTable(this, aFromNode, true);
            LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
            object title = dictionary.Translate("48", "Text", "SeznamSouradnicDvoji", typeof(string));
            Container.Children.Add(new MdiChild
                                       {
                                           Title = (string)title,
                                           Content = ssd,
                                           Width = 714,
                                           Height = 734,
                                       });
        }


        public bool AddNewNodeIntoTable(Type tableType, CalculatedPointBase newPoint, IProtocolContext protocolContex)
        {
            if (_currentMdi != null && _currentMdi.Content.GetType() == tableType)
            {
                var mdi = (IManipulateItems)_currentMdi.Content;
                return mdi.AddNewNode(newPoint, protocolContex);
            }
            foreach (var child in Container.Children)
            {
                if (child.Content.GetType() != tableType) continue;
                var mdi = (IManipulateItems)child.Content;
                return mdi.AddNewNode(newPoint, protocolContex);
            }
            return false;
        }

        public void ReleaseCalculationMethod(string name)
        {
            _otevereneVypocty.Remove(name);
        }

        public void SetModifiedFlag(object table)
        {
            foreach (var child in Container.Children.Where(child => child.Content == table))
            {
                child.Title += " *";
                return;
            }
        }

        public TableNodesBase FindNodeFromTables(string uplneCislo, TableType tableType)
        {
            Debug.Assert(tableType == TableType.Souradnice);
            return FindNodeFromSeznamSouradnice(uplneCislo);
        }

        public MdiChild GetMdiWindow(object tableContent)
        {
            foreach (MdiChild child in Container.Children)
            {
                if (child.Content == tableContent)
                    return child;
            }
            return null;
        }

        public void AddTable(UIElement table)
        {
            try
            {
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                if (table is MeasureListTable)
                {
                    object title = dictionary.Translate("5", "Text", "MeasurementsListPolarMethod", typeof(string));
                    Container.Children.Add(new MdiChild
                                               {
                                                   Title = (string)title,
                                                   Content = table,
                                                   Width = 714,
                                                   Height = 734,
                                               });
                }
                else if (table is CoordinateListTable)
                {
                    object title = dictionary.Translate("33", "Text", "Souracnice", typeof(string));
                    Container.Children.Add(new MdiChild
                                               {
                                                   Title = (string)title,
                                                   Content = table,
                                                   Width = 714,
                                                   Height = 734,
                                               });
                }
                else
                {
                    Debug.Assert(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AppendTextIntoProtocol(FlowDocument document)
        {
            if (_protokol == null)
            {
                var dictionary = LanguageConverter.ResolveDictionary();
                _protokol = new Protokol(this);
                var title = dictionary.Translate("101", "Title", "Protokol", typeof(string));
                Container.Children.Add(new MdiChild
                {
                    Title = (string)title,
                    Content = _protokol,
                    Width = 714,
                    Height = 734,
                });
            }
            _protokol.AppendText(document);

        }

        TableNodesBase FindNodeFromSeznamSouradnice(string uplneCislo)
        {
            if (_aktivniCoordinateListTable != null)
            {
                if (!GetIsTableExist(_aktivniCoordinateListTable))
                    _aktivniCoordinateListTable = null;
            }
            if (_aktivniCoordinateListTable == null)
            {
                List<Tuple<string, TableBase>> list = GetTabulkySouradnic();
                if (list.Count == 0) return null;
                if (list.Count == 1)
                    _aktivniCoordinateListTable = list[0].Item2 as CoordinateListTable;
                else
                {
                    var ass = new ActiveListOfCoordinates();
                    ass.Init(list, _aktivniCoordinateListTable);
                    if (ass.ShowDialog().GetValueOrDefault(false))
                    {
                        _aktivniCoordinateListTable = ass.GetSelectedTable() as CoordinateListTable;
                    }
                }
            }
            if (_aktivniCoordinateListTable != null)
                return _aktivniCoordinateListTable.GetNode(uplneCislo);
            return null;
        }

        List<Tuple<string, TableBase>> GetTabulkySouradnic()
        {
            var list = new List<Tuple<string, TableBase>>();
            foreach (MdiChild child in Container.Children)
            {
                if (child.Content is CoordinateListTable)
                {
                    var table = child.Content as CoordinateListTable;
                    LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                    var fileName = dictionary.Translate<string>("473", "Text");
                    if (table.FileName != null)
                        fileName = table.FileName.FullName;
                    list.Add(new Tuple<string, TableBase>(fileName, table));
                }
            }
            return list;
        }

        bool GetIsTableExist(TableBase table)
        {
            foreach (MdiChild child in Container.Children)
            {
                if (child.Content == table)
                    return true;
            }
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void LoadPosAndSize()
        {
            ProgramOption po = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "MainWindowPos");
            if (po.isPoint())
            {
                Left = po.getPoint().X;
                Top = po.getPoint().Y;
            }
            po = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "MainWindowState");
            if (po.isBool())
            {
                if (po.getBool())
                {
                    WindowState = WindowState.Maximized;
                }
            }
            po = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "MainWindowSize");
            if (po.isPoint())
            {
                Width = po.getPoint().X;
                Height = po.getPoint().Y;
            }
        }

        void SavePosAndSize()
        {
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "MainWindowPos",
                                           new ProgramOption(new Point(Left, Top)));
            if (WindowState != WindowState.Maximized)
            {
                Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "MainWindowSize",
                                               new ProgramOption(new Point(ActualWidth, ActualHeight)));
            }
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "MainWindowState",
                                           new ProgramOption(WindowState == WindowState.Maximized));
        }

        void LoadVypoctyDialogs()
        {
            ProgramOption po = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "VypoctyDialogs");
            string vypocty = po.getString(string.Empty);
            if (string.IsNullOrEmpty(vypocty)) return;
            foreach (string dialog in vypocty.Split(';'))
            {
                string[] param = dialog.Split('#');
                if (param.Length != 2)
                    continue;
                try
                {
                    Type type = Type.GetType(param[1]);
                    OpenWindow(param[0], type);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        void SaveVypctyDialogs()
        {
            string vypocty = string.Empty;
            foreach (var vypocet in _otevereneVypocty)
            {
                vypocty += vypocet.Key + '#' + vypocet.Value.GetType() + ";";
            }
            if (vypocty.Length != 0)
                vypocty = vypocty.Substring(0, vypocty.Length - 1);
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "VypoctyDialogs", new ProgramOption(vypocty));
        }

        void SaveChildrenWindws()
        {
            string files = string.Empty;
            foreach (var child in Container.Children)
            {
                var ld = (ILoadSaveMdi)child.Content;
                if (ld.FileName == null) continue;
                if (!string.IsNullOrEmpty(ld.FileName.FullName))
                {
                    files += ld.FileName.FullName + "?";
                }
            }
            if (files.Length > 0)
                files = files.Substring(0, files.Length - 1);
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "MainWindowOpenFiles", new ProgramOption(files));
        }

        void LoadChildrenWindows()
        {
            var filesString = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "MainWindowOpenFiles").getString(string.Empty);
            if (filesString.Length == 0) return;
            _recentFileList.DisalbeInsertFile = true;
            var files = filesString.Split('?');
            foreach (var file in files)
            {
                OpenFileCore(new FileInfo(file), -1);
            }
            _recentFileList.DisalbeInsertFile = false;
        }

        void AskForSaveWhenClosingMdi(MdiChild child)
        {
            var ls = child.Content as ILoadSaveMdi;
            if (ls != null && ls.GetIsModifed())
            {
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                MessageBoxResult result;
                if (ls is TableBase)
                {
                    var par = new ResourceParams();
                    par.Add("tableName", child.Title);
                    result = MessageBox.Show(dictionary.Translate("258", "Text", par),
                                             dictionary.Translate<string>("258", "Caption"),
                                             MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                }
                else if (ls is Protokol)
                {
                    result = MessageBox.Show(dictionary.Translate<string>("259", "Text"),
                                             dictionary.Translate<string>("259", "Caption"),
                                             MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                }
                else
                {
                    Debug.Assert(false);
                    return;
                }
                if (result == MessageBoxResult.OK)
                {
                    MdiChild backUp = _currentMdi;
                    _currentMdi = child;
                    OnSaveFile(this, null);
                    _currentMdi = backUp;
                }
            }
            if (ls == _aktivniCoordinateListTable)
                _aktivniCoordinateListTable = null;
        }

        void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}