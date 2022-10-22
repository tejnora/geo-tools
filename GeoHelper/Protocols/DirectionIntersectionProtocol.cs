using GeoCalculations.CalculationContexts;
using GeoCalculations.Deviations;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols
{
    [ProtocolRootData("DirectionIntersectionMethod")]
    public class DirectionIntersectionProtocolContext : ProtocolContextBase<DirectionIntersectionCalculatedPoint>
    {
        readonly DirectionIntersectionContext _calculationContext;

        public DirectionIntersectionProtocolContext(DirectionIntersectionContext calculationContext)
        {
            _calculationContext = calculationContext;
        }
        [ProtocolPropertyDataAttribute("A")]
        public PolarContex PointOfViewA { get { return _calculationContext.PointOfViewA; } }
        [ProtocolPropertyDataAttribute("B")]
        public PolarContex PointOfViewB { get { return _calculationContext.PointOfViewB; } }
        [ProtocolPropertyDataAttribute("Deviations")]
        public CalculationDeviation Deviations { get { return _calculationContext.Deviations; } }

        public override void AddCalculatedPoint(CalculatedPointBase point)
        {
            _replacedNodes.Clear();
            _calculatedPoints.Clear();
            base.AddCalculatedPoint(point);
        }
    }
}
