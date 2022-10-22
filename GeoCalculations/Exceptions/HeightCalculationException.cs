namespace GeoCalculations.Exceptions
{
    class HeightCalculationException : CalculationException
    {
        public HeightCalculationException(string exceptionId)
        {
            Description = ResolveException(exceptionId, null);
        }
    }
}
