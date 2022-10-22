namespace GeoCalculations.Methods
{
    public static class ElevationDifference
    {
        static public double Calculate(double zenitAngle, double distance, double firstPointHeight, double secondPointHeight, double forceElevationDifferenceByThisValue)
        {
            double elevationDifference;
            if (double.IsNaN(forceElevationDifferenceByThisValue))
            {
                if (double.IsNaN(zenitAngle) || double.IsNaN(distance))
                    return double.NaN;
                elevationDifference = SimpleCalculation.CalculateHeightByDistanceAndAngle(zenitAngle, distance);
            }
            else
            {
                elevationDifference = forceElevationDifferenceByThisValue;
            }
            if (!double.IsNaN(firstPointHeight))
            {
                elevationDifference += firstPointHeight;
            }
            if (!double.IsNaN(secondPointHeight))
            {
                elevationDifference -= secondPointHeight;
            }
            return elevationDifference;
        }
    }
}
