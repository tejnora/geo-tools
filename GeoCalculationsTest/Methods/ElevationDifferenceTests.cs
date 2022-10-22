using GeoCalculations.Methods;
using NUnit.Framework;

namespace GeoCalculationsTest.Methods
{
    [TestFixture]
    class ElevationDifferenceTests
    {

        [TestCase(30, 10, 0, 0, double.NaN, Result = 19.626105055051504)]
        [TestCase(30, 10, 10, 0, double.NaN, Result = 29.626105055051504)]
        [TestCase(30, 10, 0, 10, double.NaN, Result = 9.626105055051504)]
        [TestCase(30, 10, 20, 10, double.NaN, Result = 29.626105055051504)]
        [TestCase(30, 10, 20, 10, 0, Result = 10)]
        public double ElevationDifference1(double zenitAngle, double distance, double firstPointHeight, double secondPointHeight, double forceElevationDifferenceByThisValue)
        {
            var result = ElevationDifference.Calculate(zenitAngle, distance, firstPointHeight, secondPointHeight,
                                                       forceElevationDifferenceByThisValue);
            return result;
        }
    }
}
