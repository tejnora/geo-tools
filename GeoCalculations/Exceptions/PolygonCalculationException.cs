namespace GeoCalculations.Exceptions
{
    public class PolygonCalculationException : CalculationException
    {
        public PolygonCalculationException(string exceptionId)
        {
            Description = ResolveException(exceptionId, null);
        }

    }
}
