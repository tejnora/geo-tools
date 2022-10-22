using System;
using System.Windows;
using GeoBase.Localization;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Methods;
using GeoCalculations.Points;
using GeoHelper.Protocols;
using Microsoft.Win32;

namespace GeoHelper.Calculations
{
    public partial class PolygonTraverseDialog : IPointExport
    {
        PolygonCalculatedPoints _calculatedPoints;

        bool _hightCalculation;

        public PolygonTraverseDialog(string dialogName, IMainWindow aMainWindow)
            : base(dialogName, aMainWindow)
        {
            InitializeComponent();
            DataContext = this;
            CalculationContext = new PolygonTraverseContext();
            InitAdditionalControls();
            ProtocolContext = new PolygonTraverseProtocolContext();
        }

        public PolygonTraverseContext CalculationContext { get; set; }
        protected override CalculationContextBase CalculationContextBase { get { return CalculationContext; } }

        protected override void ResetCalculation(object sender, RoutedEventArgs e)
        {
            InitAdditionalControls();
        }

        void InitAdditionalControls()
        {
            _beginPoint.Point = CalculationContext.BeginPoint;
            _endPoint.Point = CalculationContext.EndPoint;
            _beginOrientation.Orientation = CalculationContext.BeginOrientationContext;
            _endOrientation.Orientation = CalculationContext.EndOrientationContext;
            _measuringData.CalculationContext = CalculationContext.MeasuredContext;
        }

        protected override void Calculate()
        {
            PolygonTraverseMethod.PolygonTraverse(CalculationContext, ref _calculatedPoints);
            if (HightCalculation)
                PolygonTraverseMethod.PolygonTraversHeight(_calculatedPoints, _beginPoint.Point, _endPoint.Point, CalculationContext.MeasuredContext.TableNodes);
            OnPropertyChanged("CalculatedPoints");
        }

        protected override void CalculatePoint()
        {
            //PolygonCalculatedPoints
        }

        public bool HightCalculation
        {
            get { return _hightCalculation; }
            set
            {
                _hightCalculation = value;
                OnPropertyChanged("HightCalculation");
            }
        }

        public void Export(IPointExporter pointExporter)
        {
            pointExporter.AddSettingValue("Type", "3D");
        }

        public void SettingCallback(string paramName, string value)
        {
            switch (paramName)
            {
                case "Type":
                    {
                        if (value == "3D")
                            HightCalculation = true;
                    }
                    break;
            }
        }

        void OnPolygonTraverseLoad(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Plygon (*.pol,)|*.pol|" + "All files (*.*)|*.*" };
            if (!dlg.ShowDialog(this).GetValueOrDefault(false)) return;
            try
            {
                ResetCalculation(this, null);
                var importer = new PolygonTraversImporter { SettingCallback = SettingCallback };
                importer.ImportFromFile(dlg.FileName, CalculationContext);
            }
            catch (Exception)
            {
                ResetCalculation(this, null);
                LanguageDictionary.Current.ShowMessageBox("456", null, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void OnPolygonTraverseSave(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "Plygon (*.pol,)|*.pol" };
            if (!dlg.ShowDialog(this).GetValueOrDefault(false)) return;
            try
            {
                var exporter = new CalculationExporter { Settings = this };
                exporter.ExportToFile(dlg.FileName, CalculationContext);
            }
            catch
            {
                LanguageDictionary.Current.ShowMessageBox("457", null, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}