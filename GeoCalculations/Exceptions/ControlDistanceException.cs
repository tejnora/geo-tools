using GeoBase.Utils;

namespace GeoCalculations.Exceptions
{
    class ControlDistanceException : CalculationException
    {
        public ControlDistanceException(string exceptionId,ResourceParams resourceParams)
        {
            Description = ResolveException(exceptionId, null);
        }
    }
}
