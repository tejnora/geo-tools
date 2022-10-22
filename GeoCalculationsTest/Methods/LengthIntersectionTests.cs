using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using NUnit.Framework;

namespace GeoCalculationsTest.Methods
{
    [TestFixture]
    class LengthIntersectionTests
    {
        private const double Delta = 0.0001;

        PointBaseEx CreatePointOfView(string number, double distance, double y, double x)
        {
            return new PointBaseEx()
                       {
                           Number = number,
                           Distance = distance,
                           X = x,
                           Y = y
                       };
        }

        [Test]
        public void Test1()
        {
            var calculationContext = new LengthIntersectionContext
                                         {
                                             LeftPointOfView = CreatePointOfView("4003", 140.7519, 702535.9400, 1031704.7800),
                                             RightPointOfView = CreatePointOfView("4004", 300, 702848.6500, 1031908.9900),
                                         };
            var calculatedPoint = new LengthIntersectionCalculatedPoint { Number = "1" };
            LengthIntersectionMethod.CalculatePoint(calculationContext, calculatedPoint);
            Assert.AreEqual(calculatedPoint.Y, 702555.7449, Delta);
            Assert.AreEqual(calculatedPoint.X, 1031844.1316, Delta);
            Assert.AreEqual(calculatedPoint.DistanceFromLeft, 140.7519, Delta);
            Assert.AreEqual(calculatedPoint.DistanceFromRight, 300.0000, Delta);
            Assert.AreEqual(calculatedPoint.MinimumIntersectionAngle, 122.86050, Delta);
            Assert.AreEqual(calculatedPoint.ShorterLength, 140.7519, Delta);
        }

        [Test]
        public void Test2()
        {
            var calculationContext = new LengthIntersectionContext
            {
                LeftPointOfView = CreatePointOfView("0250", 300, 703047.5300, 1032239.5500),
                RightPointOfView = CreatePointOfView("4004", 350, 702848.6500, 1031908.9900),
            };

            var calculatedPoint = new LengthIntersectionCalculatedPoint { Number = "1" };
            LengthIntersectionMethod.CalculatePoint(calculationContext, calculatedPoint);
            Assert.AreEqual(calculatedPoint.Y, 703192.0473, Delta);
            Assert.AreEqual(calculatedPoint.X, 1031976.6532, Delta);
            Assert.AreEqual(calculatedPoint.DistanceFromLeft, 300.0000, Delta);
            Assert.AreEqual(calculatedPoint.DistanceFromRight, 350.0000, Delta);
            Assert.AreEqual(calculatedPoint.MinimumIntersectionAngle, 80.38746, Delta);
            Assert.AreEqual(calculatedPoint.ShorterLength, 300.0000, Delta);
        }

    }
}
