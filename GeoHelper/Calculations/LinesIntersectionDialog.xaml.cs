using System.Windows;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoHelper.Protocols;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;

namespace GeoHelper.Calculations
{
    public partial class LinesIntersectionDialog
    {
        public LinesIntersectionDialog(string dialogName, IMainWindow mainWindow)
            : base(dialogName, mainWindow)
        {
            InitializeComponent();
            CalculationContext = new LinesIntersectionContext();
            CalculatedPoint = new CalculatedPointBase();
            ProtocolContext = new LinesIntersectionProtocolContext(CalculationContext);
            DataContext = this;
        }

        public LinesIntersectionContext CalculationContext { get; set; }
        protected override CalculationContextBase CalculationContextBase { get { return CalculationContext; } }

        protected override void Calculate()
        {
            LinesIntersectionMethod.Calculated(CalculationContext,CalculatedPoint);
        }

        protected override void CalculatePoint()
        {
            if(string.IsNullOrEmpty(CalculatedPoint.Number))
                throw new LinesIntersectionException("E39");
            LinesIntersectionMethod.Calculated(CalculationContext, CalculatedPoint);
        }

        void OnDragEnter(object sender, DragEventArgs args)
        {
            if (args.IsCoordinateListTable())
                args.Accept();
        }

        void OnDropFirstLinePointA(object sender, DragEventArgs args)
        {
            if (!args.IsCoordinateListTable()) return;
            CalculationContext.FirstLine.StartPoint.LoadValues(new CoordinatePointAdapter(args.TableNode<TableCoordinateListNode>()));
            args.Handled = true;
            TryToCalculate(true);
        }

        void OnDropFirstLinePointB(object sender, DragEventArgs args)
        {
            if (!args.IsCoordinateListTable()) return;
            CalculationContext.FirstLine.EndPoint.LoadValues(new CoordinatePointAdapter(args.TableNode<TableCoordinateListNode>()));
            args.Handled = true;
            TryToCalculate(true);
        }

        void OnDropSecondLinePointA(object sender, DragEventArgs args)
        {
            if (!args.IsCoordinateListTable()) return;
            CalculationContext.SecondLine.StartPoint.LoadValues(new CoordinatePointAdapter(args.TableNode<TableCoordinateListNode>()));
            args.Handled = true;
            TryToCalculate(true);
        }

        void OnDropSecondLinePoint2(object sender, DragEventArgs args)
        {
            if (!args.IsCoordinateListTable()) return;
            CalculationContext.SecondLine.EndPoint.LoadValues(new CoordinatePointAdapter(args.TableNode<TableCoordinateListNode>()));
            args.Handled = true;
            TryToCalculate(true);
        }
    }
}
