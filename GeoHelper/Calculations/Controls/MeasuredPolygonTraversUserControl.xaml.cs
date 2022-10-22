using System.Windows;
using System.Windows.Controls;
using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;

namespace GeoHelper.Calculations.Controls
{
    public partial class MeasuredPolygonTraversUserControl
    {
        MeasuredPointsContext _measuredContext;

        public MeasuredPolygonTraversUserControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public MeasuredPointsContext CalculationContext
        {
            get { return _measuredContext; }
            set
            {
                if (value == _measuredContext) return;
                _measuredContext = value;
                OnPropertyChanged("Context");
            }
        }

        void OnAddIntoTable(object sender, RoutedEventArgs e)
        {
            CalculationContext.AddNode();
        }

        void OnRemoveFromTable(object sender, RoutedEventArgs e)
        {
            CalculationContext.RemoveNode((MeasuredPoint)_dataGrid.SelectedItem);
        }

        void OnTableSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculationContext.SelectionChanged(_dataGrid.SelectedItem);
        }

        void OnPointOfViewDragEnter(object sender, DragEventArgs args)
        {
            if (args.IsMeasureListTable())
                args.Accept();
        }

        void OnPointOfViewDrop(object sender, DragEventArgs args)
        {
            if (!args.IsMeasureListTable()) return;
            args.Accept();
            CalculationContext.Node.PointOfView.LoadValues(new MeasurePointAdapter(args.TableNode<TableMeasureListNode>()));
        }

        void OnMeasuringBackDrop(object sender, DragEventArgs args)
        {
            if (!args.IsMeasureListTable()) return;
            args.Accept();
            CalculationContext.Node.MeasuringBack.LoadValues(new MeasurePointAdapter(args.TableNode<TableMeasureListNode>()));
        }

        void OnMeasuringBackDragEnter(object sender, DragEventArgs args)
        {
            if (args.IsMeasureListTable())
                args.Accept();
        }

        void OnMeasuringForwardDrop(object sender, DragEventArgs args)
        {
            if (!args.IsMeasureListTable()) return;
            args.Accept();
            CalculationContext.Node.MeasuringForward.LoadValues(new MeasurePointAdapter(args.TableNode<TableMeasureListNode>()));
        }

        void OnMeasuringForwardDragEnter(object sender, DragEventArgs args)
        {
            if (args.IsMeasureListTable())
                args.Accept();
        }
    }
}