using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols
{
    [ProtocolRootData("DirectionAzimutAndLengthMethod")]
    class DirectionAzimutAndLengthProtocolContext : ProtocolContextBase<DirectionAzimutAndLengthCalculatedPoint>
    {
        readonly DirectionAzimutAndLengthContext _calculationContext;

        public DirectionAzimutAndLengthProtocolContext(DirectionAzimutAndLengthContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        [ProtocolPropertyDataAttribute("PointOfView")]
        public PointBase PointOfView
        {
            get { return _calculationContext.PointOfView; }
        }

        [ProtocolPropertyDataAttribute("Orientation")]
        public PointBase Orientation
        {
            get { return _calculationContext.Orientation; }
        }

        public override void AddCalculatedPoint(CalculatedPointBase point)
        {
            _replacedNodes.Clear();
            _calculatedPoints.Clear();
            base.AddCalculatedPoint(point);
        }
    }
}
