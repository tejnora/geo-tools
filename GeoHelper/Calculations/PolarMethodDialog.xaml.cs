using System;
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
    public partial class PolarMethodDialog
    {
        readonly bool _isFromBatch;

        public PolarMethodDialog(string dialogName, IMainWindow aMainWindow)
            : base(dialogName, aMainWindow)
        {
            _isFromBatch = false;
            InitializeComponent();
            DataContext = this;
            Loaded += OnLoaded;
            CalculationContext = new PolarContex();
            CalculatedPoint = new CalculatedPointBase();
            ProtocolContext = new PolarMethodProtocolContext(CalculationContext, false);
        }

        public PolarMethodDialog(string dialogName, IMainWindow aMainWindow, bool isFromBatch)
            : base(dialogName, aMainWindow)
        {
            _isFromBatch = isFromBatch;
            InitializeComponent();
            DataContext = this;
            Loaded += OnLoaded;
            CalculationContext = new PolarContex();
            CalculatedPoint = new CalculatedPointBase();
            ProtocolContext = new PolarMethodProtocolContext(CalculationContext, true);
        }

        public PolarContex CalculationContext { get; set; }
        protected override CalculationContextBase CalculationContextBase { get { return CalculationContext; } }

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
            _pointCalculationTab.Visibility = Visibility.Hidden;
        }


        public void OnAddOrientationNode(object sender, EventArgs args)
        {
            if (CalculationContext.Orientation.AddNode())
                TryToCalculate(true);
        }

        public void OnRemoveOrientationNode(object sender, EventArgs arg)
        {
            CalculationContext.Orientation.RemoveNode((OrientationPoint)_dataGrid.SelectedItem);
            TryToCalculate(true);
        }

        public void OnOrientaionTableSelectionChanged(object sender, EventArgs args)
        {
            CalculationContext.Orientation.SelectionChanged(_dataGrid.SelectedItem);
        }

        protected override void ResetCalculation(object sender, RoutedEventArgs e)
        {
            CalculationContext.SetDefaultValues();
            CalculatedPoint.SetDefaultValues();
        }

        protected override void Calculate()
        {
            SimpleCalculation.CalculateOrientationMovement(CalculationContext);
        }

        protected override void CalculatePoint()
        {
            PolarMethod.Calculate(CalculationContext, CalculatedPoint);
        }

        void OnPointOfViewDragEnter(object sender, DragEventArgs args)
        {
            if (args.IsCoordinateListTable())
                args.Accept();
            else if (args.IsMeasureListTable() && args.TableNode<TableMeasureListNode>().PointType == TableMeasureListNode.PointTypes.PointOfView)
                args.Accept();
        }

        void OnPointOfViewDrop(object sender, DragEventArgs args)
        {
            if (args.IsCoordinateListTable())
            {
                CalculationContext.PointOfView.LoadValues(new CoordinatePointAdapter(args.TableNode<TableCoordinateListNode>()));
            }
            else if (args.IsMeasureListTable())
            {
                var selectedNode = args.TableNode<TableMeasureListNode>();
                if (selectedNode.PointType == TableMeasureListNode.PointTypes.PointOfView)
                {
                    CalculationContext.PointOfView.LoadValues(new MeasurePointAdapter(selectedNode));
                    var coordinates = MainWindow.FindNodeFromTables(selectedNode.NumberWithPrefix, TableType.Souradnice);
                    if (coordinates != null)
                        CalculationContext.PointOfView.LoadValues(new CoordinatePointAdapter((TableCoordinateListNode)coordinates));
                }
            }
        }

        void OnOrientationDragEnter(object sender, DragEventArgs args)
        {
            if (args.IsCoordinateListTable() || args.IsMeasureListTable())
                args.Accept();
        }

        void OnOrientationDrop(object sender, DragEventArgs args)
        {
            if (args.IsCoordinateListTable())
            {
                CalculationContext.Orientation.Node.LoadValues(new CoordinatePointAdapter(args.TableNode<TableCoordinateListNode>()));
                TryToCalculate(true);
                return;
            }

            if (!args.IsMeasureListTable()) return;
            var selectedNode = args.TableNode<TableMeasureListNode>();
            if (selectedNode.PointType == TableMeasureListNode.PointTypes.PointOfView)
            {
                CalculationContext.Orientation.TableNodes.Clear();
                var table = args.Table<TableBase>();
                var start = false;
                foreach (var nodesBase in table.Nodes)
                {
                    var node = (TableMeasureListNode)nodesBase;
                    if (node == selectedNode)
                        start = true;
                    else if (start)
                    {
                        if (node.PointType == TableMeasureListNode.PointTypes.PointOfView)
                            break;
                        if (node.PointType == TableMeasureListNode.PointTypes.MeasuringPoint)
                            continue;
                        var coordinate = MainWindow.FindNodeFromTables(node.NumberWithPrefix, TableType.Souradnice);
                        if (coordinate == null) continue;
                        var newNode = new OrientationPoint();
                        newNode.LoadValues(new MeasurePointAdapter(node));
                        newNode.LoadValues(new CoordinatePointAdapter((TableCoordinateListNode)coordinate));
                        CalculationContext.Orientation.TableNodes.Add(newNode);
                    }
                }
                CalculationContext.PointOfView.LoadValues(new MeasurePointAdapter(selectedNode));
                var pointOfViewCoordinates = MainWindow.FindNodeFromTables(selectedNode.NumberWithPrefix, TableType.Souradnice);
                if (pointOfViewCoordinates != null)
                    CalculationContext.PointOfView.LoadValues(new CoordinatePointAdapter(pointOfViewCoordinates as TableCoordinateListNode));
                CalculationContext.Orientation.Node = new OrientationPoint();
                TryToCalculate(true);
            }
            else
            {
                CalculationContext.Orientation.Node.LoadValues(new MeasurePointAdapter(selectedNode));
                var coordinates = MainWindow.FindNodeFromTables(selectedNode.NumberWithPrefix, TableType.Souradnice);
                if (coordinates != null)
                    CalculationContext.Orientation.Node.LoadValues(new CoordinatePointAdapter(coordinates as TableCoordinateListNode));
            }
        }

        void OnMeasuredValuesDragEnter(object sender, DragEventArgs args)
        {
            if (args.IsMeasureListTable()) args.Accept();
        }

        void OnMeasuredValuesDrop(object sender, DragEventArgs args)
        {
            if (!args.IsMeasureListTable()) return;
            CalculatedPoint.SetDefaultValues();
            CalculatedPoint.LoadValues(new MeasurePointAdapter(args.TableNode<TableMeasureListNode>()));
            TryCalculatePoints(false);
        }
    }
}
