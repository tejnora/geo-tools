using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GeoBase.Localization;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;
using GeoHelper.Protocols;
using GeoHelper.Tables;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;

namespace GeoHelper.Calculations
{
    public partial class PolarMethodBatchDialog
    {
        public PolarMethodBatchDialog(string dialogName, IMainWindow aMainWindow)
            : base(dialogName, aMainWindow)
        {
            InitializeComponent();
            DisableDialogResize();
            DataContext = this;
            CalculationContext = new PolarMethodBatchContext();
            ProtocolContext = new PolarMethodBatchProtocolContext(CalculationContext);
        }

        List<TableMeasureListNode> _inputNodes;
        TableBase _outputTable;
        public PolarMethodBatchContext CalculationContext { get; set; }
        protected override CalculationContextBase CalculationContextBase { get { return CalculationContext; } }

        protected override void AddCalculatedPointIntoTable(CalculatedPointBase point)
        {
        }

        protected override void ResetCalculation(object sender, RoutedEventArgs e)
        {
            CalculationContext.SetDefaultValues();
            _inputNodes = null;
            _outputTable = null;
        }

        protected override void Calculate()
        {
            if (!string.IsNullOrEmpty(CalculationContext.InputFileName) &&
                !string.IsNullOrEmpty(CalculationContext.OutputFileName)) return;
            throw new PolarMethodBatchException("E35");
        }

        protected override void CalculatePoint()
        {
            CalculationContext.NumberOfDetailedPoints = 0;
            CalculationContext.NumberOfPointOfViews = 0;
            CalculationContext.NumberOfUnusedMeasuringPoints = 0;

            var pointOfViewNode = new List<TableMeasureListNode>();
            var doPointOfView = false;
            foreach (var measuringNode in _inputNodes)
            {
                if (measuringNode.PointType == TableMeasureListNode.PointTypes.PointOfView)
                {
                    if (doPointOfView)
                    {
                        if (CalculateBatch(pointOfViewNode))
                        {
                            CalculationContext.IncrementNumberOfPointOfViews();
                        }
                        pointOfViewNode.Clear();
                        doPointOfView = false;
                    }
                    if (CalculationContext.UseOnlySelectedNodes && !measuringNode.Selected)
                        continue;
                    doPointOfView = true;
                    pointOfViewNode.Add(measuringNode);
                }
                else if (doPointOfView)
                {
                    if (CalculationContext.UseOnlySelectedNodes && !measuringNode.Selected)
                        continue;
                    pointOfViewNode.Add(measuringNode);
                }
            }
            if (!doPointOfView) return;
            if (CalculateBatch(pointOfViewNode))
            {
                CalculationContext.IncrementNumberOfPointOfViews();
            }
        }

        void OnFileDrop(object sender, DragEventArgs e)
        {
            if (e.IsMeasureListTable())
            {
                _inputNodes = new List<TableMeasureListNode>(e.Table<MeasureListTable>().Nodes.Select(n => n as TableMeasureListNode));
                if (e.Table<MeasureListTable>().FileName == null)
                    CalculationContext.InputFileName = LanguageDictionary.Current.TranslateText("PolarMethodBatch.15");
                else
                    CalculationContext.InputFileName = e.Table<MeasureListTable>().FileName.FullName;
            }
            else if (e.IsCoordinateListTable())
            {
                var table = (DragDropDataObjectBase)e.Data.GetData(CoordinateListTable.TableName);
                _outputTable = (TableBase)table.MdiWindow;
                if (e.Table<CoordinateListTable>().FileName == null)
                    CalculationContext.OutputFileName = LanguageDictionary.Current.TranslateText("PolarMethodBatch.15");
                else
                    CalculationContext.OutputFileName = e.Table<CoordinateListTable>().FileName.FullName;
            }
        }

        void OnOutputFileDragEnter(object sender, DragEventArgs e)
        {
            if (e.IsCoordinateListTable()) e.Accept();
        }

        void OnInputFileDragEnter(object sender, DragEventArgs e)
        {
            if (e.IsMeasureListTable()) e.Accept();
        }

        bool CalculateBatch(IList<TableMeasureListNode> measuringNodes)
        {
            if (measuringNodes.Count == 0) return true;
            var pointOfViewCoordinates = (TableCoordinateListNode)_outputTable.GetNode(measuringNodes[0].NumberWithPrefix);
            if (pointOfViewCoordinates == null)
            {
                if (CalculationContext.CalculateFreePointOfView)
                {
                    if (!DoPointOfViewsCalculation(measuringNodes, out pointOfViewCoordinates))
                        return false;
                }
                else
                    return false;
            }
            return DoPolarMethodCalculation(measuringNodes, pointOfViewCoordinates);
        }

        bool DoPointOfViewsCalculation(IList<TableMeasureListNode> measuringNodes, out TableCoordinateListNode pointOfViewCoordinates)
        {
            pointOfViewCoordinates = default(TableCoordinateListNode);
            var freePointOfViewDialog = new FreePointOfViewDialog("VypoctyVolneStanoviskoMetoda", MainWindow, true);
            freePointOfViewDialog.CalculatedPoint.LoadValues(new MeasurePointAdapter(measuringNodes[0]));
            foreach (var measuringNode in measuringNodes)
            {
                if (measuringNode.PointType != TableMeasureListNode.PointTypes.Orientation) continue;
                var TableCoordinateListNode = _outputTable.GetNode(measuringNode.NumberWithPrefix);
                if (TableCoordinateListNode == null)
                    continue;
                var oNode = new OrientationPoint();
                oNode.LoadValues(new MeasurePointAdapter(measuringNode));
                oNode.LoadValues(new CoordinatePointAdapter(TableCoordinateListNode as TableCoordinateListNode));
                freePointOfViewDialog.CalculationContext.TableNodes.Add(oNode);
            }
            freePointOfViewDialog.TryToCalculate(true);
            freePointOfViewDialog.TryCalculatePoints(true);
            if (!freePointOfViewDialog.ShowDialog().GetValueOrDefault(false))
                throw new SilentAbortCalculation();
            if(!freePointOfViewDialog.CalculatedPointAddedIntoTable && !freePointOfViewDialog.TryCalculatePoints(false))
                return false;
            pointOfViewCoordinates = (TableCoordinateListNode)_outputTable.GetNode(measuringNodes[0].NumberWithPrefix);
            CalculationContext.PolarMethodResults.Add(freePointOfViewDialog.GetProtocolSimpleText());
            return true;
        }

        bool DoPolarMethodCalculation(IList<TableMeasureListNode> measuringNodes, TableCoordinateListNode pointOfViewCoordinates)
        {
            //pointOfView
            var polarMethodDialog = new PolarMethodDialog("VypoctyVypoctyPolarniMetoda", MainWindow, true);
            polarMethodDialog.CalculationContext.PointOfView.LoadValues(new CoordinatePointAdapter(pointOfViewCoordinates));
            //orientation
            foreach (var measuringNode in measuringNodes)
            {
                if (measuringNode.PointType != TableMeasureListNode.PointTypes.Orientation) continue;
                var node = _outputTable.GetNode(measuringNode.NumberWithPrefix);
                if (node == null)
                {
                    CalculationContext.IncrementNumberOfUnusedMeasuringPoints();
                    continue;
                }
                var oNode = new OrientationPoint();
                oNode.LoadValues(new MeasurePointAdapter(measuringNode));
                oNode.LoadValues(new CoordinatePointAdapter(node as TableCoordinateListNode));
                polarMethodDialog.CalculationContext.Orientation.TableNodes.Add(oNode);
            }
            polarMethodDialog.TryToCalculate(false);
            if (polarMethodDialog.CalculationContext.Orientation.TableNodes.Count < 2 || CalculationContext.EditOrientation)
            {
                if (!polarMethodDialog.ShowDialog().GetValueOrDefault(false))
                    throw new SilentAbortCalculation();
            }
            //calculation points
            foreach (var measuringNode in measuringNodes.Where(measuringNode => measuringNode.PointType == TableMeasureListNode.PointTypes.MeasuringPoint))
            {
                polarMethodDialog.CalculatedPoint.LoadValues(new MeasurePointAdapter(measuringNode));
                if (polarMethodDialog.TryCalculatePoints(false))
                {
                    CalculationContext.IncrementNumberOfDetailedPoints();
                }
            }
            CalculationContext.PolarMethodResults.Add(polarMethodDialog.GetProtocolSimpleText());
            return true;
        }
    }
}