using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols
{
    [ProtocolRootData("ConstructionDistanceMethod")]
    public class ConstructionDistanceProtocolContext : ProtocolContextBase<CalculatedPointBase>
    {
        [ProtocolPropertyDataAttribute("ControlPoints")]
        public ConstructionDistanceContex ControlPoints { get; set; }
    }
}
