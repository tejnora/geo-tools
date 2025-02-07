using System;
using System.Windows;
using System.Windows.Forms;
using CAD.Canvas;
using CAD.UITools;
using CAD.Canvas.DrawTools;
using System.IO;
using CAD.DTM;
using CAD.DTM.Gui;
using CAD.VFK;
using CAD.VFK.DrawTools;
using CAD.VFK.GUI;
using GeoBase.Localization;
using GeoBase.Utils;
using VFK.GUI;
using VFK;

namespace CAD
{
    public partial class Document : AvalonDock.DocumentContent, ICanvasOwner
    {
        public Document(string aFileName, IMainWinInterface aOwner)
        {
            InitializeComponent();
            _owner = aOwner;
            Title = "<New Document>";
            FileName = string.Empty;
            DataModel = new DataModel();
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
            DataModel.AddDrawTool(DtmToolBar.DtmMultiLine.Name, new DtmDrawingCurveElement());
            _Canvas.Construct(this, DataModel);

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

        public bool ImportDtm(string location)
        {
            var ctx = new DtmImportCtx();// { FileName = @"c:\Temp\JVF DTM\vydej_zps_ref.jvf.xml" };
            var dlg = new DtmImportDialog(ctx) { Owner = (MainWin)_owner };
            var result = dlg.DoModal();
            if (result == false)
                return false;
            try
            {
                DataModel.ImportDtm(ctx);
                CanvasCommand.CommandFitView();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ImportVfk(string aLocation)
        {
            var vfkDataContext = new VFKDataContext { FileName = aLocation };
            if (aLocation.Length == 0)
            {
                var dlg = new VFKImportDialog(vfkDataContext) { Owner = (MainWin)_owner };
                bool? result = dlg.DoModal();
                if (result == false)
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
            CanvasCommand.CommandFitView();
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
        public VFKMain GetIsVfkMain()
        {
            return DataModel.VfkMain;
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
    }
}
