namespace GeoCalculations.Exceptions
{
    class DirectIntersectionCalculationException : CalculationException
    {
        public DirectIntersectionCalculationException(string exceptionId)
        {
            Description = ResolveException(exceptionId, null);
        }
    }
}
