using GeoCalculations.CalculationContexts;
using GeoCalculations.Deviations;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols
{
    [ProtocolRootData("PolarMethod")]
    public class PolarMethodProtocolContext : ProtocolContextBase<CalculatedPointBase>
    {
        private readonly PolarContex _calculationContext;

        public PolarMethodProtocolContext(PolarContex calculationContext, bool isFromBatch)
        {
            _calculationContext = calculationContext;
            IsNotFromBatch = !isFromBatch;
        }

        [ProtocolPropertyDataAttribute("PointOfView")]
        public PointBaseEx PointOfView { get { return _calculationContext.PointOfView; } }
        [ProtocolPropertyDataAttribute("Orientation")]
        public OrientationContext Orientation { get { return _calculationContext.Orientation; } }
        [ProtocolPropertyDataAttribute("Deviations")]
        public CalculationDeviation Deviations { get { return _calculationContext.Deviations; } }
        [ProtocolPropertyDataAttribute("IsNotFromBatch")]
        public bool IsNotFromBatch { get; private set; }

    }
}