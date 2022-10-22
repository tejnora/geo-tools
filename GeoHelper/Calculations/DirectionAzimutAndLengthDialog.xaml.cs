using System.Windows;
using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoHelper.Protocols;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;

namespace GeoHelper.Calculations
{
    public partial class DirectionAzimutAndLengthDialog
    {
        public DirectionAzimutAndLengthDialog(string dialogName, IMainWindow mainWindow)
            : base(dialogName, mainWindow)
        {
            InitializeComponent();
            CalculationContext = new DirectionAzimutAndLengthContext();
            CalculatedPoint = new DirectionAzimutAndLengthCalculatedPoint();
            ProtocolContext = new DirectionAzimutAndLengthProtocolContext(CalculationContext);
            DataContext = this;
        }


        public DirectionAzimutAndLengthContext CalculationContext { get; set; }
        protected override CalculationContextBase CalculationContextBase { get { return CalculationContext; } }

        protected override void Calculate()
        {
            DirectionAzimutAndLengthMethod.Calculate(CalculationContext.PointOfView, CalculationContext.Orientation, (DirectionAzimutAndLengthCalculatedPoint)CalculatedPoint);
        }

        protected override void CalculatePoint()
        {
            DirectionAzimutAndLengthMethod.Calculate(CalculationContext.PointOfView, CalculationContext.Orientation, (DirectionAzimutAndLengthCalculatedPoint)CalculatedPoint);
        }

        protected override void AddCalculatedPointIntoTable(CalculatedPointBase point)
        {

        }

        void OnPointOfViewDragEnter(object sender, DragEventArgs e)
        {
            if (e.IsCoordinateListTable()) e.Accept();
        }

        void OnPointOfViewDrop(object sender, DragEventArgs e)
        {
            if (!e.IsCoordinateListTable()) return;
            CalculationContext.PointOfView.LoadValues(new CoordinatePointAdapter(e.TableNode<TableCoordinateListNode>()));
            TryToCalculate(true);
        }

        void OnOrientationDragEnter(object sender, DragEventArgs e)
        {
            if (e.IsCoordinateListTable()) e.Accept();
        }

        void OnOrientationDrop(object sender, DragEventArgs e)
        {
            if (!e.IsCoordinateListTable()) return;
            CalculationContext.Orientation.LoadValues(new CoordinatePointAdapter(e.TableNode<TableCoordinateListNode>()));
            TryToCalculate(true);
        }
    }
}