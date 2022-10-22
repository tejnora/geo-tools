namespace GeoCalculations.Exceptions
{
    class LengthIntersectionCalculationException : CalculationException
    {
        public LengthIntersectionCalculationException(string exceptionId)
        {
            Description = ResolveException(exceptionId, null);
        }
    }
}
