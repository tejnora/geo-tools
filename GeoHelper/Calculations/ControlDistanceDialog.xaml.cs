using System.Windows;
using System.Windows.Controls;
using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoHelper.Protocols;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;
using Point = GeoCalculations.BasicTypes.Point;

namespace GeoHelper.Calculations
{
    public partial class ControlDistanceDialog
    {
        public ControlDistanceDialog(string dialogName, IMainWindow mainWindow)
            : base(dialogName, mainWindow)
        {
            InitializeComponent();
            CalculationContext = new ControlDistanceContex();
            ProtocolContext = new ControlDistanceProtocolContext(CalculationContext);
            DataContext = this;
        }

        public ControlDistanceContex CalculationContext { get; set; }
        protected override CalculationContextBase CalculationContextBase { get { return CalculationContext; } }

        protected override void AddCalculatedPointIntoTable(CalculatedPointBase point)
        {

        }

        protected override void Calculate()
        {
            ControlDistanceMethod.Calculate(CalculationContext);
        }

        protected override void CalculatePoint()
        {
        }

        void OnIdenticiallyPointDragEnter(object sender, DragEventArgs e)
        {
            if (e.IsCoordinateListTable()) e.Accept();
        }

        void OnIdenticiallyPointDrop(object sender, DragEventArgs args)
        {
            args.Accept();
            CalculationContext.Node.LoadValues(new CoordinatePointAdapter(args.TableNode<TableCoordinateListNode>()));
            if (CalculationContext.TableNodes.Count <= 0) return;
            var point1 = new Point(CalculationContext.TableNodes[CalculationContext.TableNodes.Count - 1].X, CalculationContext.TableNodes[CalculationContext.TableNodes.Count - 1].Y);
            var point2 = new Point(CalculationContext.Node.X, CalculationContext.Node.Y);
            CalculationContext.Node.MeasuringLength = SimpleCalculation.CalculateDistance(point1, point2);
        }

        void OnIdenticllyPointSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculationContext.SelectionChanged(_dataGrid.SelectedItem);
        }

        void OnRemove(object sender, RoutedEventArgs e)
        {
            CalculationContext.RemoveNode((ControlDistancePoint)_dataGrid.SelectedItem);
        }

        void OnAdd(object sender, RoutedEventArgs e)
        {
            CalculationContext.AddNode();
            TryToCalculate(true);
        }
    }
}