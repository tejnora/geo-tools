using GeoCalculations.CalculationContexts;
using GeoCalculations.Deviations;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols
{
    [ProtocolRootData("LengthIntersectionMethod")]
    public class LengthIntersectionProtocolContext : ProtocolContextBase<LengthIntersectionCalculatedPoint>
    {
        readonly LengthIntersectionContext _calculationContext;

        public LengthIntersectionProtocolContext(LengthIntersectionContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        [ProtocolPropertyDataAttribute("LeftPointOfView")]
        public PointBaseEx LeftPointOfView { get { return _calculationContext.LeftPointOfView; } }
        [ProtocolPropertyDataAttribute("RightPointOfView")]
        public PointBaseEx RightPointOfView { get { return _calculationContext.RightPointOfView; } }
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