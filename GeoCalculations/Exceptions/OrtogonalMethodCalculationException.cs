namespace GeoCalculations.Exceptions
{
    class OrtogonalMethodCalculationException : CalculationException
    {
        public OrtogonalMethodCalculationException(string exceptionId)
        {
            Description = ResolveException(exceptionId, null);
        }

    }
}
