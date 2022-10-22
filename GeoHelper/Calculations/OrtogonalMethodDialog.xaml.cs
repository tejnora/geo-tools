using System;
using System.Windows;
using GeoBase.Localization;
using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoHelper.ExtensionMethods;
using GeoHelper.Protocols;
using GeoHelper.Tables;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;

namespace GeoHelper.Calculations
{
    public partial class OrtogonalMethodDialog
    {
        public OrtogonalMethodDialog(string dialogName, IMainWindow aMainWindow)
            : base(dialogName, aMainWindow)
        {
            InitializeComponent();
            DisableDialogResize();
            DataContext = this;
            CalculationContext = new OrtogonalContext();
            CalculatedPoint = new CalculatedPointBase();
            ProtocolContext = new OrtogonalMethodProtocolContext(CalculationContext);

        }

        public OrtogonalContext CalculationContext { get; set; }
        protected override CalculationContextBase CalculationContextBase { get { return CalculationContext; } }

        protected override void AddCalculatedPointIntoTable(CalculatedPointBase point)
        {
            MainWindow.AddNewNodeIntoTable(typeof(CoordinateListTable), point, ProtocolContext);
            CalculatedPoint.IncrementNumber();
        }

        public string PointOfViewCorrection
        {
            get
            {
                var dictionary = LanguageConverter.ResolveDictionary();
                var correction = (string)dictionary.Translate("OrtogonalMethodDialog.25", "Text", "Oprava:", typeof(string));
                if (!double.IsNaN(CalculationContext.StationingCorrection))
                    correction += CalculationContext.StationingCorrection.ToDelka(false);
                return correction;
            }
        }

        void OnAdd(object sender, EventArgs args)
        {
            if (CalculationContext.AddNode())
                TryToCalculate(true);
            OnPropertyChanged("PointOfViewCorrection");
        }

        void OnRemove(object sender, EventArgs args)
        {
            CalculationContext.RemoveNode((PointBaseEx)_dataGrid.SelectedItem);
            TryToCalculate(true);
            OnPropertyChanged("PointOfViewCorrection");
        }

        void OnKey(object sender, EventArgs args)
        {
        }

        void OnDataGridOnSelectionChanged(object sender, EventArgs args)
        {
            CalculationContext.SelectionChanged(_dataGrid.SelectedItem);
        }

        protected override void ResetCalculation(object sender, RoutedEventArgs e)
        {
            CalculationContext.SetDefaultValues();
            CalculatedPoint.SetDefaultValues();
            OnPropertyChanged("PointOfViewCorrection");
        }

        protected override void Calculate()
        {
            OrthogonalMethod.Calculate(CalculationContext);
        }

        protected override void CalculatePoint()
        {
            OrthogonalMethod.CalculatePoint(CalculationContext, CalculatedPoint);
        }

        void OnIdenticallyPointsDragEnter(object sender, DragEventArgs args)
        {
            if (args.IsCoordinateListTable())
                args.Accept();
        }

        void OnIdenticallyPointsDrop(object sender, DragEventArgs args)
        {
            CalculationContext.Node.LoadValues(new CoordinatePointAdapter(args.TableNode<TableCoordinateListNode>()));
            if (CalculationContext.TableNodes.Count > 0)
            {
                var lastNode = CalculationContext.TableNodes[CalculationContext.TableNodes.Count - 1];
                CalculationContext.Node.Stationing = SimpleCalculation.CalculateDistance(new GeoCalculations.BasicTypes.Point(lastNode.X, lastNode.Y), new GeoCalculations.BasicTypes.Point(CalculationContext.Node.X, CalculationContext.Node.Y));
            }
            else
                CalculationContext.Node.Stationing = 0;
            CalculationContext.Node.Vertical = 0;
        }
    }
}