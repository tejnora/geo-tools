using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols
{
    [ProtocolRootData("LinesIntersectionMethod")]
    public class LinesIntersectionProtocolContext : ProtocolContextBase<CalculatedPointBase>
    {
        readonly LinesIntersectionContext _calculationContext;
        public LinesIntersectionProtocolContext(LinesIntersectionContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        [ProtocolPropertyData("FirstLine")]
        public Line FirstLine { get { return _calculationContext.FirstLine; } }
        [ProtocolPropertyData("SecondLine")]
        public Line SecondLine { get { return _calculationContext.SecondLine; } }

        public override void AddCalculatedPoint(CalculatedPointBase point)
        {
            _replacedNodes.Clear();
            _calculatedPoints.Clear();
            base.AddCalculatedPoint(point);
        }

    }
}
