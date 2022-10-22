using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols
{
    [ProtocolRootData("TransformCoordinatesMethod")]
    class TransformCoordinatesProtocolContex : ProtocolContextBase<CordinateTransformationCalculatedPoint>
    {
        readonly CoordinatesTransformationContex _calculationContext;

        public TransformCoordinatesProtocolContex(CoordinatesTransformationContex calculationContext)
        {
            _calculationContext = calculationContext;
        }

        [ProtocolPropertyDataAttribute("IdenticallyPoints")]
        public CoordinatesTransformationContex IdenticallyPoints
        {
            get { return _calculationContext; }
        }
    }
}