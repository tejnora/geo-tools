using System.Collections.Generic;
using System.Linq;
using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols
{
    [ProtocolRootData("PolarMethodBatch")]
    class PolarMethodBatchProtocolContext : ProtocolContextBase<CalculatedPointBase>
    {
        readonly PolarMethodBatchContext _calculationContext;

        public PolarMethodBatchProtocolContext(PolarMethodBatchContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        public List<string> PointOfView { get { return _calculationContext.PolarMethodResults; } }

        [ProtocolPropertyDataAttribute("ConcatenatedResults")]
        public string ConcatenatedResults
        {
            get
            {
                return _calculationContext.PolarMethodResults.Aggregate(string.Empty, (current, result) => current + result);
            }
        }
    }
}
