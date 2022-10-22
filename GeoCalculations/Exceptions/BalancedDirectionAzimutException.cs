namespace GeoCalculations.Exceptions
{
    class BalancedDirectionAzimutException : CalculationException
    {
        public BalancedDirectionAzimutException(string exceptionId)
        {
            Description = ResolveException(exceptionId, null);
        }
    }
}
