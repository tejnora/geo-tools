using System.Windows;
using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoHelper.Protocols;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;

namespace GeoHelper.Calculations
{
    public partial class LengthIntersectionDialog
    {
        public LengthIntersectionDialog(string dialogName, IMainWindow aMainWindow)
            : base(dialogName, aMainWindow)
        {
            InitializeComponent();
            DataContext = this;
            CalculationContext = new LengthIntersectionContext();
            CalculatedPoint = new LengthIntersectionCalculatedPoint();
            ProtocolContext = new LengthIntersectionProtocolContext(CalculationContext);
        }


        public LengthIntersectionContext CalculationContext { get; set; }
        protected override CalculationContextBase CalculationContextBase { get { return CalculationContext; } }

        protected override void ResetCalculation(object sender, RoutedEventArgs e)
        {
            CalculationContext.SetDefaultValues();
            CalculatedPoint.SetDefaultValues();
        }

        void OnDragEnter(object sender, DragEventArgs args)
        {
            if (args.IsCoordinateListTable() || args.IsMeasureListTable())
                args.Accept();
        }

        void OnLeftPointOfViewDrop(object sender, DragEventArgs args)
        {
            ProcessDrop(CalculationContext.LeftPointOfView, args);
        }

        void OnRightPointOfViewDrop(object sender, DragEventArgs args)
        {
            ProcessDrop(CalculationContext.RightPointOfView, args);
        }

        void ProcessDrop(PointBase pointOfView, DragEventArgs args)
        {
            if (args.IsCoordinateListTable())
            {
                pointOfView.LoadValues(new CoordinatePointAdapter(args.TableNode<TableCoordinateListNode>()));
                return;
            }
            if (!args.IsMeasureListTable()) return;
            pointOfView.LoadValues(new MeasurePointAdapter(args.TableNode<TableMeasureListNode>()));
            var coordinates = MainWindow.FindNodeFromTables(pointOfView.NumberWithPrefix, TableType.Souradnice) as TableCoordinateListNode;
            if (coordinates != null)
                pointOfView.LoadValues(new CoordinatePointAdapter(coordinates));
        }

        protected override void Calculate()
        {
            LengthIntersectionMethod.CalculatePoint(CalculationContext, (LengthIntersectionCalculatedPoint)CalculatedPoint);
        }

        protected override void CalculatePoint()
        {
            LengthIntersectionMethod.CalculatePoint(CalculationContext, (LengthIntersectionCalculatedPoint)CalculatedPoint);
        }

    }
}
