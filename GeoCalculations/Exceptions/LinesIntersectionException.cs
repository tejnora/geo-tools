namespace GeoCalculations.Exceptions
{
    public class LinesIntersectionException : CalculationException
    {
        public LinesIntersectionException(string exceptionId)
        {
            Description = ResolveException(exceptionId, null);
        }
    }
}
