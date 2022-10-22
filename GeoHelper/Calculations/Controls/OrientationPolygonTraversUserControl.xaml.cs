using System;
using System.ComponentModel;
using System.Windows;
using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoHelper.Tables;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;

namespace GeoHelper.Calculations.Controls
{
    public partial class OrientationPolygonTraversUserControl
    {
        public OrientationPolygonTraversUserControl()
        {
            InitializeComponent();
            DataContext = this;
        }
        public OrientationContext Orientation { get; set; }

        public bool CanAddTableItem
        {
            get { return Orientation.Node != null; }
        }

        public bool CanRemoveTableItem
        {
            get { return Orientation.TableNodes.Count > 0; }
        }

        public IMainWindow Owner { get; set; }

        public void OnAddIntoTable(object sender, EventArgs args)
        {
            Orientation.TableNodes.Add((OrientationPoint)Orientation.Node.Clone());
        }

        public void OnRemoveFromTable(object sender, EventArgs arg)
        {
            Orientation.TableNodes.Remove((OrientationPoint)_dataGrid.SelectedItem);
        }

        public void OnOrientaceTableSelectionChanged(object sender, EventArgs args)
        {
            if (_dataGrid.SelectedItem != null)
                Orientation.Node = (OrientationPoint)((OrientationPoint)_dataGrid.SelectedItem).Clone();
            OnPropertyChanged("CanRemoveTableItem");
            OnPropertyChanged("CanAddTableItem");
        }

        public void OnOrienteceSourceUpdated(object sender, PropertyChangedEventArgs args)
        {
            OnPropertyChanged("CanRemoveTableItem");
            OnPropertyChanged("CanAddTableItem");
        }

        void OnOrientaceDragEnter(object sender, DragEventArgs args)
        {
            if (!args.Data.GetDataPresent("SeznamSouradnicTabulka") && !args.Data.GetDataPresent("SeznamMereniTabulka"))
                return;
            args.Effects = DragDropEffects.Copy;
            args.Handled = true;
        }

        void OnOrientaceDrop(object sender, DragEventArgs args)
        {
            if (args.Data.GetDataPresent("SeznamSouradnicTabulka"))
            {
                var table = (DragDropDataObjectBase)args.Data.GetData("SeznamSouradnicTabulka");
                Orientation.Node.LoadValues(new CoordinatePointAdapter(table.Data as TableCoordinateListNode));
            }
            else if (args.Data.GetDataPresent("SeznamMereniTabulka"))
            {
                var data = (DragDropDataObjectBase)args.Data.GetData("SeznamMereniTabulka");
                var ddnode = (TableMeasureListNode)data.Data;
                if (ddnode.PointType == TableMeasureListNode.PointTypes.PointOfView)
                {
                    var table = (TableBase)data.MdiWindow;
                    var start = false;
                    foreach (var nodesBase in table.Nodes)
                    {
                        var node = (TableMeasureListNode)nodesBase;
                        if (node == ddnode)
                            start = true;
                        else if (start)
                        {
                            if (node.PointType == TableMeasureListNode.PointTypes.PointOfView)
                                break;
                            if (node.PointType == TableMeasureListNode.PointTypes.MeasuringPoint)
                                continue;
                            var souradnice = Owner.FindNodeFromTables(node.NumberWithPrefix, TableType.Souradnice);
                            if (souradnice != null) continue;
                            var newNode = new OrientationPoint();
                            newNode.LoadValues(new MeasurePointAdapter(node));
                            newNode.LoadValues(new CoordinatePointAdapter(souradnice as TableCoordinateListNode));
                            Orientation.TableNodes.Add(newNode);
                        }
                    }
                    Orientation.Node = new OrientationPoint();
                    OnPropertyChanged("CanRemoveTableItem");
                    OnPropertyChanged("CanAddTableItem");
                    OnPropertyChanged("Orientation");
                }
                else
                {
                    Orientation.Node.LoadValues(new MeasurePointAdapter(ddnode));
                }
            }
        }
    }
}