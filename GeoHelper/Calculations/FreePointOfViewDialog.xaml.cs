using System;
using System.Linq;
using System.Windows;
using GeoBase.Gui.Buttons;
using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoHelper.Protocols;
using GeoHelper.Tables;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;

namespace GeoHelper.Calculations
{
    public partial class FreePointOfViewDialog
    {
        readonly bool _isFromBatch;
        public FreePointOfViewDialog(string dialogName, IMainWindow aMainWindow)
            : base(dialogName, aMainWindow)
        {
            InitializeComponent();
            _isFromBatch = false;
            DataContext = this;
            CalculationContext = new OrientationContext();
            CalculatedPoint = new CalculatedPointBase();
            ProtocolContext = new FreePointOfViewProtocolContext { PointOfView = CalculatedPoint, Orientation = CalculationContext, IsNotFromBatch = true };
        }

        public FreePointOfViewDialog(string dialogName, IMainWindow aMainWindow, bool isFromBatch)
            : base(dialogName, aMainWindow)
        {
            _isFromBatch = isFromBatch;
            InitializeComponent();
            DataContext = this;
            CalculationContext = new OrientationContext();
            CalculatedPoint = new CalculatedPointBase();
            ProtocolContext = new FreePointOfViewProtocolContext { PointOfView = CalculatedPoint, Orientation = CalculationContext, IsNotFromBatch = !isFromBatch };
        }

        public override void EndInit()
        {
            base.EndInit();
            if (!_isFromBatch) return;
            _buttonsStackPanel.Children.Clear();
            var okButton = new OkButton { IsDefault = true };
            okButton.Click += OnOkButtonClick;
            _buttonsStackPanel.Children.Insert(0, okButton);
            var cancelButton = new CancelButton();
            cancelButton.Click += OnCancelButtonClick;
            _buttonsStackPanel.Children.Insert(1, cancelButton);
            var calculateButton = new CalculateButton();
            calculateButton.Click += Calculate;
            _buttonsStackPanel.Children.Add(calculateButton);
            _calculatedPointPrefix.IsEnabled = false;
            _calculatedPointNumber.IsEnabled = false;
        }

        public bool CalculatedPointAddedIntoTable { private set; get; }

        public OrientationContext CalculationContext { get; set; }
        protected override CalculationContextBase CalculationContextBase { get { return CalculationContext; } }

        protected override void AddCalculatedPointIntoTable(CalculatedPointBase point)
        {
            CalculatedPointAddedIntoTable = true;
            base.AddCalculatedPointIntoTable(point);
        }

        protected override void Calculate()
        {
            FreePointOfViewMethod.Calculate(CalculationContext);
        }

        protected override void CalculatePoint()
        {
            FreePointOfViewMethod.CalculatePoint(CalculationContext, CalculatedPoint);
        }

        protected override void ResetCalculation(object sender, RoutedEventArgs e)
        {
            CalculationContext.SetDefaultValues();
            CalculatedPoint.SetDefaultValues();
        }

        public void OnOrientaionTableSelectionChanged(object sender, EventArgs args)
        {
            CalculationContext.SelectionChanged(_dataGrid.SelectedItem);
        }

        void OnOrientationDragEnter(object sender, DragEventArgs args)
        {
            if (args.IsCoordinateListTable()) args.Accept();
            if (args.IsMeasureListTable()) args.Accept();
        }

        void OnOrientationDrop(object sender, DragEventArgs args)
        {
            if (args.IsCoordinateListTable())
            {
                CalculationContext.Node.LoadValues(new CoordinatePointAdapter(args.TableNode<TableCoordinateListNode>()));
            }
            else if (args.IsMeasureListTable())
            {
                var measuringNode = args.TableNode<TableMeasureListNode>();
                if (measuringNode.PointType == TableMeasureListNode.PointTypes.PointOfView)
                {
                    CalculationContext.SetDefaultValues();
                    var table = args.Table<MeasureListTable>();
                    var start = false;
                    foreach (var node in table.Nodes.Cast<TableMeasureListNode>())
                    {
                        if (node == measuringNode)
                            start = true;
                        else if (start)
                        {
                            if (node.PointType == TableMeasureListNode.PointTypes.PointOfView)
                                break;
                            if (node.PointType == TableMeasureListNode.PointTypes.MeasuringPoint)
                                continue;
                            var pointCoordinates = MainWindow.FindNodeFromTables(node.NumberWithPrefix, TableType.Souradnice);
                            if (pointCoordinates == null)
                                continue;
                            var newNode = new OrientationPoint();
                            newNode.LoadValues(new MeasurePointAdapter(node));
                            newNode.LoadValues(new CoordinatePointAdapter(pointCoordinates as TableCoordinateListNode));
                            CalculationContext.TableNodes.Add(newNode);
                        }
                    }
                    CalculationContext.Node.SetDefaultValues();
                    CalculatedPoint.LoadValues(new MeasurePointAdapter(measuringNode));
                    TryToCalculate(true);
                    TryCalculatePoints(false);
                }
                else
                {
                    CalculationContext.Node.LoadValues(new MeasurePointAdapter(measuringNode));
                }
            }
            args.Handled = true;
        }

        void OnAddIntoTable(object sender, RoutedEventArgs e)
        {
            CalculationContext.AddNode();
            TryToCalculate(true);
        }

        void OnRemoveFromTable(object sender, RoutedEventArgs e)
        {
            CalculationContext.RemoveNode(_dataGrid.SelectedItem as OrientationPoint);
            TryToCalculate(true);
        }
    }
}
