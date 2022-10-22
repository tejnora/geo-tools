using GeoBase.Utils;

namespace GeoCalculations.Exceptions
{
    class ConstructionDistanceCalculationException : CalculationException
    {
        public ConstructionDistanceCalculationException(string exceptionId)
        {
            Description = ResolveException(exceptionId, null);
        }
        public ConstructionDistanceCalculationException(string exceptionId, ResourceParams @params)
        {
            Description = ResolveException(exceptionId, @params);
        }

    }
}
