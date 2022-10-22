using GeoCalculations.CalculationContexts;
using GeoCalculations.Deviations;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols
{
    [ProtocolRootData("ControlDistanceMethod")]
    public class ControlDistanceProtocolContext : ProtocolContextBase<CalculatedPointBase>
    {
        readonly ControlDistanceContex _calculationContext;

        public ControlDistanceProtocolContext(ControlDistanceContex calculationContext)
        {
            _calculationContext = calculationContext;
        }

        [ProtocolPropertyDataAttribute("ControlDistance")]
        public ControlDistanceContex ControlPoints { get { return _calculationContext; } }
        [ProtocolPropertyDataAttribute("Deviations")]
        public CalculationDeviation Deviations { get { return _calculationContext.Deviations; } }
    }
}