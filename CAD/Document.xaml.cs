using System;
using System.Windows;
using System.Windows.Forms;
using CAD.Canvas;
using CAD.UITools;
using CAD.Canvas.DrawTools;
using System.IO;
using System.Linq;
using CAD.DTM;
using CAD.DTM.Gui;
using CAD.DTM.Validation;
using CAD.VFK;
using CAD.VFK.DrawTools;
using CAD.VFK.GUI;
using GeoBase.Localization;
using GeoBase.Utils;
using VFK.GUI;
using VFK;

namespace CAD
{
    public enum DocumentType
    {
        Simple,
        Vfk,
        Dtm
    }
    public partial class Document : ICanvasOwner
    {
        public Document(string aFileName, IMainWinInterface aOwner)
        {
            InitializeComponent();
            _owner = aOwner;
            Title = "<New Document>";
            FileName = string.Empty;
            DataModel = new DataModel();
            DataModel.ContentChanged += (_, _) => MarkDtmValidationStale();
            if (aFileName.Length > 0 && File.Exists(aFileName) && DataModel.Load(aFileName))
            {
                Title = aFileName;
                FileName = aFileName;
            }
            DataModel.AddDrawTool(DrawToolBar.MultiLine.Name, new LineEdit(false));
            DataModel.AddDrawTool(DrawToolBar.Line.Name, new LineEdit(true));
            DataModel.AddDrawTool(DrawToolBar.TextBox.Name, new TextBoxEdit());
            DataModel.AddDrawTool(DrawToolBar.Circle2Point.Name, new Circle(Arc.EArcType.Type2Point));
            DataModel.AddDrawTool(DrawToolBar.CircleCentrePoint.Name, new Circle(Arc.EArcType.TypeCenterRadius));
            DataModel.AddDrawTool(DrawToolBar.ActivePoint.Name, new ActivePointEdit());
            DataModel.AddDrawTool(VfkToolBar.VfkMultiLine.Name, new VfkMultiLine());
            DataModel.AddDrawTool(VfkToolBar.VfkActivePoint.Name, new VfkActivePoint());
            DataModel.AddDrawTool(VfkToolBar.VfkMark.Name, new VfkMarkEdit());
            DataModel.AddDrawTool(VfkToolBar.VfkText.Name, new VfkTextsEdit());
            DataModel.AddDrawTool(DtmToolBar.DtmMultiLine.Name, new DtmDrawingLineElement());
            DataModel.AddDrawTool(DtmToolBar.DtmPoint.Name, new DtmDrawingPointElement());
            DataModel.AddDrawTool(DtmToolBar.DtmDefinitionPoint.Name, new DtmDrawingDefinitionPointElement());
            DataModel.AddDrawTool(DtmToolBar.DtmArea.Name, new DtmDrawingPlochaElement());
            _Canvas.Construct(this, DataModel);
            _Canvas.HandleCreated += (_, _) => QueueInitialFitView();
            _Canvas.VisibleChanged += (_, _) => QueueInitialFitView();
            _Canvas.SizeChanged += (_, _) => QueueInitialFitView();

            _Canvas.RunningSnapsDefault = new Type[]
                {
                typeof(VertextSnapPoint),
                typeof(MidpointSnapPoint),
                typeof(IntersectSnapPoint),
                typeof(QuadrantSnapPoint),
                typeof(CenterSnapPoint),
                typeof(DivisionSnapPoint),
                };

            _Canvas.AddQuickSnapType(Keys.N, typeof(NearestSnapPoint));
            _Canvas.AddQuickSnapType(Keys.M, typeof(MidpointSnapPoint));
            _Canvas.AddQuickSnapType(Keys.I, typeof(IntersectSnapPoint));
            _Canvas.AddQuickSnapType(Keys.V, typeof(VertextSnapPoint));
            _Canvas.AddQuickSnapType(Keys.P, typeof(PerpendicularSnapPoint));
            _Canvas.AddQuickSnapType(Keys.Q, typeof(QuadrantSnapPoint));
            _Canvas.AddQuickSnapType(Keys.C, typeof(CenterSnapPoint));
            _Canvas.AddQuickSnapType(Keys.T, typeof(TangentSnapPoint));
            _Canvas.AddQuickSnapType(Keys.D, typeof(DivisionSnapPoint));

        }
        bool _initialFitViewPending;
        bool _initialFitViewQueued;
        internal void FitViewWhenReady()
        {
            _initialFitViewPending = true;
            QueueInitialFitView();
        }

        void QueueInitialFitView()
        {
            if (!_initialFitViewPending || _initialFitViewQueued || _Canvas.IsDisposed) return;
            _initialFitViewQueued = true;
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() =>
            {
                _initialFitViewQueued = false;
                if (!_initialFitViewPending || _Canvas.IsDisposed || !_Canvas.IsHandleCreated || !_Canvas.Visible ||
                    _Canvas.ClientSize.Width <= 0 || _Canvas.ClientSize.Height <= 0) return;
                _initialFitViewPending = false;
                _Canvas.CommandFitView();
            }));
        }

        FrameworkElement _tabHeader;
        bool _middleButtonPressedOnTab;

        public override void OnApplyTemplate()
        {
            if (_tabHeader != null)
            {
                _tabHeader.PreviewMouseDown -= OnTabMouseDown;
                _tabHeader.PreviewMouseUp -= OnTabMouseUp;
                _tabHeader.MouseLeave -= OnTabMouseLeave;
            }
            _middleButtonPressedOnTab = false;
            base.OnApplyTemplate();
            _tabHeader = DragEnabledArea;
            if (_tabHeader != null)
            {
                _tabHeader.PreviewMouseDown += OnTabMouseDown;
                _tabHeader.PreviewMouseUp += OnTabMouseUp;
                _tabHeader.MouseLeave += OnTabMouseLeave;
            }
        }

        void OnTabMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton != System.Windows.Input.MouseButton.Middle) return;
            _middleButtonPressedOnTab = true;
            e.Handled = true;
        }

        void OnTabMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton != System.Windows.Input.MouseButton.Middle) return;
            var close = _middleButtonPressedOnTab;
            _middleButtonPressedOnTab = false;
            e.Handled = true;
            if (close && ContainerPane?.GetManager() != null) Close();
        }

        void OnTabMouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => _middleButtonPressedOnTab = false;

        private readonly IMainWinInterface _owner;
        public ICanvasCommand CanvasCommand
        {
            get { return _Canvas; }
        }
        public CanvasCtrl CanvasCtrl
        {
            get { return _Canvas; }
        }
        public string FileName
        {
            private set;
            get;
        }
        public DataModel DataModel
        {
            get;
            private set;
        }
        public void SetPositionInfo(UnitPoint unitpos)
        {
            _owner.SetPositionInfo(unitpos);
        }
        public void SetSnapInfo(ISnapPoint snap)
        {
            _owner.SetSnapInfo(snap);
        }
        public void UpdateToolBars(GeoCadRoutedCommand aDrawObjectId)
        {
            _owner.UpdateToolBars(aDrawObjectId);
        }
        public void Save()
        {
            if (FileName == null || FileName.Length == 0)
                SaveAs();
            else
                DataModel.Save(FileName);
        }
        public void SaveAs()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Geo cad files (*.gcad)|*.gcad",
                OverwritePrompt = true
            };
            if (FileName.Length > 0)
                dlg.FileName = FileName;
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                FileName = dlg.FileName;
                Title = FileName;
                DataModel.Save(FileName);
                Title = FileName;
            }
        }
        public void Export()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Drawing Interchange File Format (*.dxf)|*.dxf",
                OverwritePrompt = true
            };
            bool? result = dlg.ShowDialog();
            if (result == true)
                DataModel.Export(dlg.FileName, ExportType.Dxf);
        }
        public bool ImportVfk(string aLocation)
        {
            var vfkDataContext = new VFKDataContext { FileName = aLocation };
            if (aLocation.Length == 0)
            {
                var dlg = new VFKImportDialog(vfkDataContext) { Owner = (MainWin)_owner };
                bool? result = dlg.DoModal();
                if (result != true)
                    return false;
            }
            if (DataModel.IsImportedVfkFile() && LanguageDictionary.Current.ShowMessageBox("92", null, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return false;
            try
            {
                DataModel.OnImportVfk(vfkDataContext);
            }
            catch (Exception)
            {
                LanguageDictionary.Current.ShowMessageBox("110", null, MessageBoxButton.OK,
                                                  MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        public void ExportVfk()
        {
            var exportDialog = new VFKExportDialog(DataModel.VfkMain.VFKDataContext);
            bool? result = exportDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                try
                {
                    DataModel.VfkMain.ExportFile(exportDialog.FileName);
                }
                catch (ExportExcetiption ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Chyba pri exportu", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("Neočekávaná chyba", "Neočekávaná chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void RemoveVfk()
        {
            DataModel.OnRemoveVfkData();
        }
        public bool IsImportedVfk()
        {
            return DataModel.IsImportedVfkFile();
        }
        public void OnVfkEdidOfParcel()
        {
            var dialog = new VFKEdidOfParcel(DataModel.VfkMain);
            dialog.ShowDialog();
        }
        public DocumentType GetDocumentType()
        {
            if (DataModel.VfkMain != null)
                return DocumentType.Vfk;
            if (DataModel.DtmMain != null)
                return DocumentType.Dtm;
            return DocumentType.Simple;
        }
        public void OnGenerateSip()
        {
            var dialog = new GenerateSIP(DataModel.VfkMain.ParcelContext);
            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                DataModel.VfkMain.ParcelContext = dialog.Context;
            }
        }
        public void SelectAndScrollToActivePoint(VfkActivePoint activePoint)
        {
            _Canvas.CommandSelectVFKActiveObject(activePoint);
        }
        public void ExportDtm()
        {
            var ctx = new DtmExportCtx();
            var exportDialog = new DtmExportDialog(ctx) { Owner = (MainWin)_owner };
            var result = exportDialog.ShowDialog();
            if (!result.HasValue || !result.Value) return;
            _dtmValidationExportContext = ctx;
            var progress = new DtmExportProgress(ctx.FileName, () =>
            {
                var validation = new DtmValidator(DataModel.DtmMain, ctx).Validate();
                DtmValidationResult = validation;
                if (validation.HasErrors || IsDtmValidationVisible)
                    ((MainWin)_owner).ShowDtmValidationPanel(this);
                return validation;
            }, () => new DtmExporter(DataModel.DtmMain).CreateFile(ctx),
                () => DtmGadFilesValidator.Validate(ctx.OutputDirectory, ctx.GadFileNamePrefix, ctx.DescriptionInSketch),
                ctx.SignOutputDirectory ? () => new CAD.DTM.Signing.DtmDirectorySigner().SignAsync(
                    ctx.OutputDirectory, ctx.SigningSettings, ctx.CisloOvereni, ctx.DatumOvereni, ctx.ReplaceSignatureConfirmed) : null);
            try { new DtmExportProgressDialog(progress) { Owner = (MainWin)_owner }.ShowDialog(); }
            finally { if (ctx.SigningSettings != null) ctx.SigningSettings.Password = ""; }
        }
        public void ValidateDtm()
        {
            if (DataModel.DtmMain == null) return;
            DtmValidationResult = new DtmValidator(DataModel.DtmMain, _dtmValidationExportContext).Validate();
            ((MainWin)_owner).ShowDtmValidationPanel(this);
        }
        DtmExportCtx _dtmValidationExportContext;
        DtmValidationResult _dtmValidationResult;
        internal DtmValidationResult DtmValidationResult
        {
            get => _dtmValidationResult;
            private set
            {
                _dtmValidationResult = value;
                IsDtmValidationStale = false;
                DtmValidationStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        bool _isDtmValidationStale;
        internal bool IsDtmValidationStale
        {
            get => DataModel.DtmMain != null && (DtmValidationResult == null || _isDtmValidationStale);
            private set => _isDtmValidationStale = value;
        }
        internal event EventHandler DtmValidationStateChanged;
        internal void MarkDtmValidationStale()
        {
            if (DataModel.DtmMain == null || _isDtmValidationStale) return;
            IsDtmValidationStale = true;
            DtmValidationStateChanged?.Invoke(this, EventArgs.Empty);
        }
        internal bool IsDtmValidationVisible { get; set; } = GeoBase.SingletonsBase.Registry
            .getEntry(Registry.SubKey.kCurrentUser, DtmValidationPanel.VisibilitySettingKey).getBool(false);
        public void ToggleDtmValidationPanel() => ((MainWin)_owner).ToggleDtmValidationPanel(this);
        internal bool SelectDtmValidationObject(IDtmElement element)
        {
            var drawing = DataModel.Layers.OfType<DtmDrawingLayerMain>()
                .Select(layer => layer.FindObject(element)).FirstOrDefault(o => o != null);
            if (drawing == null) return false;
            if (DataModel.SelectedCount == 1 && DataModel.IsSelected(drawing) && DataModel.FocusetObject == drawing) return true;
            DataModel.ClearSelectedObjects();
            DataModel.AddSelectedObject(drawing);
            DataModel.FocusetObject = drawing;
            CanvasCommand.InvalidateAll();
            return true;
        }
        public bool ImportDtm(string location)
        {
            var ctx = new DtmImportCtx();
            var dlg = new DtmImportDialog(ctx) { Owner = (MainWin)_owner };
            var result = dlg.DoModal();
            if (result != true)
                return false;
            try
            {
                return DataModel.ImportDtm(ctx);
            }
            catch
            {
                return false;
            }
        }
        public bool ImportPointsDtm()
        {
            var ctx = new DtmImportPointsCtx();
            var dlg = new DtmImportPointsDialog(ctx) { Owner = (MainWin)_owner };
            var result = dlg.DoModal();
            if (result == false)
                return false;
            try
            {
                DataModel.ImportPointsDtm(ctx);
                CanvasCommand.CommandFitView();
                return true;
            }
            catch (Exception ex)
            {
                var resourceParams = new ResourceParams();
                resourceParams.Add("msg", ex.Message);
                LanguageDictionary.Current.ShowMessageBox("163", resourceParams, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void ShowIdetickeBodyMapovaniDialog()
        {
            var ctx = new DtmIdentickeBodyMapovaniCtx(DataModel.DtmMain);
            var dlg = new DtmIdentickeBodyMapovaniDialog(ctx);
            var result = dlg.DoModal();
        }
    }
}
