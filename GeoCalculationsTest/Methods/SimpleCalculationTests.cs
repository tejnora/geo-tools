using GeoCalculations.BasicTypes;
using GeoCalculations.Methods;
using NUnit.Framework;

namespace GeoCalculationsTest.Methods
{
    [TestFixture]
    class SimpleCalculationTests
    {
        [Test]
        public void CalculatePointOnLineAtSpecificDistance()
        {
            var point3 = SimpleCalculation.CalculatePointOnLineAtSpecificDistance(new Point(0, 0), new Point(5, 5), 2);
            Assert.AreEqual(1.4142135623730949, point3.X);
            Assert.AreEqual(1.4142135623730949, point3.Y);
        }

        [Test]
        public void CalculatePointOnLineAtSpecificDistance1()
        {
            var point3 = SimpleCalculation.CalculatePointOnLineAtSpecificDistance(new Point(5, 5), new Point(0, 0), 2);
            Assert.AreEqual(3.5857864376269051, point3.X);
            Assert.AreEqual(3.5857864376269051, point3.Y);
        }

        [Test]
        public void CalculateHeightByDistanceAndAngle1()
        {
            var height = SimpleCalculation.CalculateHeightByDistanceAndAngle(30, 10);
            Assert.AreEqual(19.626105055051504, height);
        }

        [Test]
        public void CalculateHeightByDistanceAndAngle2()
        {
            var height = SimpleCalculation.CalculateHeightByDistanceAndAngle(170, 10);
            Assert.AreEqual(-19.626105055051504, height);
        }

        [Test]
        public void CalculateHeightByDistanceAndAngle3()
        {
            var height = SimpleCalculation.CalculateHeightByDistanceAndAngle(570, 10);
            Assert.AreEqual(-19.626105055051504, height);
        }

    }
}
