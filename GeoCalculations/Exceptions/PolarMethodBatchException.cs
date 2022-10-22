namespace GeoCalculations.Exceptions
{
    public class PolarMethodBatchException : CalculationException
    {
        public PolarMethodBatchException(string exceptionId)
        {
            Description = ResolveException(exceptionId, null);
        }
    }
}
