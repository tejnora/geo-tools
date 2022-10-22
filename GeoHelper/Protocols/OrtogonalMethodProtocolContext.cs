using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols
{
    [ProtocolRootData("OrtogonalMethod")]
    public class OrtogonalMethodProtocolContext : ProtocolContextBase<CalculatedPointBase>
    {
        private readonly OrtogonalContext _calculationContext;

        public OrtogonalMethodProtocolContext(OrtogonalContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        [ProtocolPropertyDataAttribute("IdenticallyPoints")]
        public OrtogonalContext IdenticallyPoints { get { return _calculationContext; } }
    }
}