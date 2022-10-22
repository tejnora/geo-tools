using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GeoBase.Localization;
using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoHelper.Protocols;
using GeoHelper.Tables;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;

namespace GeoHelper.Calculations
{
    public partial class DirectionIntersectionDialog
    {
        readonly LanguageDictionary _dictionary = LanguageConverter.ResolveDictionary();
        bool _pointOfViewA = true;

        public DirectionIntersectionDialog(string dialogName, IMainWindow mainWindow)
            : base(dialogName, mainWindow)
        {
            InitializeComponent();
            CalculationContext = new DirectionIntersectionContext();
            CalculatedPoint = new DirectionIntersectionCalculatedPoint();
            ProtocolContext = new DirectionIntersectionProtocolContext(CalculationContext);
            DataContext = this;
        }

        public DirectionIntersectionContext CalculationContext { get; set; }
        protected override CalculationContextBase CalculationContextBase { get { return CalculationContext; } }
        public PolarContex SelectedPointOfView
        {
            get
            {
                return _pointOfViewA ? CalculationContext.PointOfViewA : CalculationContext.PointOfViewB;
            }
        }

        protected override void ResetCalculation(object sender, RoutedEventArgs e)
        {
            CalculationContext.SetDefaultValues();
            CalculatedPoint.SetDefaultValues();
            OnPropertyChanged("");
        }

        public void OnAddOrientationNode(object sender, EventArgs args)
        {
            if (SelectedPointOfView.Orientation.AddNode())
                TryToCalculate(true);
        }

        public void OnRemoveOrientationNode(object sender, EventArgs arg)
        {
            SelectedPointOfView.Orientation.RemoveNode((OrientationPoint)_dataGrid.SelectedItem);
            TryToCalculate(true);
        }

        public string PointOfViewButtonContent
        {
            get
            {
                var result = (string)_dictionary.Translate("DirectionIntersectionDialog.2", "Header", "PointOfView", typeof(string));
                if (!_pointOfViewA)
                    return result + " A";
                return result + " B";
            }
        }

        public string SelectedPointOfViewLabel
        {
            get
            {
                var result = (string)_dictionary.Translate("DirectionIntersectionDialog.2", "Header", "PointOfView", typeof(string));
                if (_pointOfViewA)
                    return result + " A";
                return result + " B";
            }
        }

        protected override void Calculate()
        {
            DirectionIntersectionMethod.Calculate(CalculationContext, (DirectionIntersectionCalculatedPoint)CalculatedPoint);
        }

        protected override void CalculatePoint()
        {
            DirectionIntersectionMethod.Calculate(CalculationContext, (DirectionIntersectionCalculatedPoint)CalculatedPoint);
        }

        void OnChnagePointOfView(object sender, EventArgs args)
        {
            _pointOfViewA = !_pointOfViewA;
            OnPropertyChanged("");
        }

        void OnOrientationPreviewDragEnter(object sender, DragEventArgs args)
        {
            if (args.IsCoordinateListTable() || args.IsMeasureListTable())
                args.Accept();
        }

        void OnPointOfViewPreviewDragEnter(object sender, DragEventArgs args)
        {
            if (args.IsCoordinateListTable())
                args.Accept();
        }

        void OnPointOfViewDrop(object sender, DragEventArgs args)
        {
            if (!args.IsCoordinateListTable()) return;
            SelectedPointOfView.PointOfView.LoadValues(new CoordinatePointAdapter(args.TableNode<TableCoordinateListNode>()));
            args.Handled = true;
        }

        void OnOrientationDrop(object sender, DragEventArgs args)
        {
            if (args.IsCoordinateListTable())
            {
                SelectedPointOfView.Orientation.Node.LoadValues(new CoordinatePointAdapter(args.TableNode<TableCoordinateListNode>()));
                args.Handled = true;
                return;
            }
            if (!args.IsMeasureListTable()) return;
            var dropedNode = args.TableNode<TableMeasureListNode>();
            switch (dropedNode.PointType)
            {
                case TableMeasureListNode.PointTypes.PointOfView:
                    LoadValuesFromPointOfView(dropedNode, args.Table<TableBase>());
                    break;
                case TableMeasureListNode.PointTypes.Orientation:
                    LoadValuesFromOrientation(dropedNode);
                    break;
                case TableMeasureListNode.PointTypes.MeasuringPoint:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            OnPropertyChanged("");
            args.Handled = true;
        }

        void LoadValuesFromOrientation(TableMeasureListNode tableMeasureListNode)
        {
            SelectedPointOfView.Orientation.Node.LoadValues(new MeasurePointAdapter(tableMeasureListNode));
            var cordinates = MainWindow.FindNodeFromTables(tableMeasureListNode.NumberWithPrefix, TableType.Souradnice);
            if (cordinates != null)
                SelectedPointOfView.Orientation.Node.LoadValues(new CoordinatePointAdapter(cordinates as TableCoordinateListNode));
        }

        void LoadValuesFromPointOfView(TableNodesBase pointOfViewNode, TableBase tableBase)
        {
            var cordinates = MainWindow.FindNodeFromTables(pointOfViewNode.NumberWithPrefix, TableType.Souradnice);
            if (cordinates != null)
                SelectedPointOfView.PointOfView.LoadValues(new CoordinatePointAdapter(cordinates as TableCoordinateListNode));
            var start = false;
            foreach (var node in tableBase.Nodes.Cast<TableMeasureListNode>())
            {
                if (node == pointOfViewNode)
                    start = true;
                else if (start)
                {
                    if (node.PointType == TableMeasureListNode.PointTypes.PointOfView)
                        break;
                    if (node.PointType == TableMeasureListNode.PointTypes.MeasuringPoint)
                        continue;
                    LoadValuesFromOrientation(node);
                    if (double.IsNaN(SelectedPointOfView.Orientation.Node.Hz))
                        SelectedPointOfView.Orientation.Node.Hz = 0;
                    SelectedPointOfView.Orientation.AddNode();
                    SelectedPointOfView.Orientation.Node.SetDefaultValues();
                }
            }
        }

        void OnCalculatedPointPreviewDragEnter(object sender, DragEventArgs args)
        {
            if (args.IsMeasureListTable())
                args.Accept();
        }

        void OnCalculatedPointDropFromA(object sender, DragEventArgs args)
        {
            if (!args.IsMeasureListTable()) return;
            ((DirectionIntersectionCalculatedPoint)CalculatedPoint).DirectionFromA = LoadCalculatedPoint(args);
        }

        void OnCalculatedPointDropFromB(object sender, DragEventArgs args)
        {
            if (!args.IsMeasureListTable()) return;
            ((DirectionIntersectionCalculatedPoint)CalculatedPoint).DirectionFromB = LoadCalculatedPoint(args);
        }

        double LoadCalculatedPoint(DragEventArgs args)
        {
            var measuringNode = args.TableNode<TableMeasureListNode>();
            if (string.IsNullOrEmpty(CalculatedPoint.Number))
                CalculatedPoint.LoadValues(new MeasurePointAdapter(measuringNode));
            args.Handled = true;
            return measuringNode.Hz;
        }

        void OnOrientaionTableSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPointOfView.Orientation.SelectionChanged(_dataGrid.SelectedItem);
        }
    }
}