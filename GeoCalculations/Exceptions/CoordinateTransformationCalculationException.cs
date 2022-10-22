using GeoBase.Utils;

namespace GeoCalculations.Exceptions
{
    public class CoordinateTransformationCalculationException : CalculationException
    {
        public CoordinateTransformationCalculationException(string exceptionId)
        {
            Description = ResolveException(exceptionId, null);
        }
        public CoordinateTransformationCalculationException(string exceptionId, ResourceParams @params)
        {
            Description = ResolveException(exceptionId, @params);
        }

    }
}
