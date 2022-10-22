
namespace GeoCalculations
{
    internal static class Constants
    {
        internal const double Epsilon = 1e-8;
        internal const double EpsilonSquared = 1e-16;

        public static bool SamePosition(double[] pt1, double[] pt2, int dimension)
        {
            return (StarMath.StarMath.Norm2(StarMath.StarMath.Subtract(pt1, pt2, dimension), dimension, true) < EpsilonSquared);
        }
    }
}
