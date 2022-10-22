namespace GeoCalculations.Exceptions
{
    class PolarMethodCalculationException : CalculationException
    {
        public PolarMethodCalculationException(string exceptionId)
        {
            Description = ResolveException(exceptionId, null);
        }
    }
}
