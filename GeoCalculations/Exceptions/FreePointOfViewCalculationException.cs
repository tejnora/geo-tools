namespace GeoCalculations.Exceptions
{
    public class FreePointOfViewCalculationException : CalculationException
    {
        public FreePointOfViewCalculationException(string exceptionId)
        {
            Description = ResolveException(exceptionId, null);
        }
    }
}
