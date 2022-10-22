using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using NUnit.Framework;

namespace GeoCalculationsTest.Methods
{
    [TestFixture]
    class DirectionIntersectionTests
    {
        private const double Delta = 0.0001;
        [Test]
        public void Test1()
        {
            var pointOfViewA = new PolarContex
                                  {
                                      PointOfView = new PointBaseEx { Number = "4001", Y = 850000.0000, X = 1040000.0000 },
                                      Orientation = new OrientationContext()
                                  };
            pointOfViewA.Orientation.TableNodes.Add(new OrientationPoint { Number = "4002", Hz = 0.00000, Y = 850004.0000, X = 1040000.0000 });
            var pointOfViewB = new PolarContex
                                  {
                                      PointOfView = new PointBaseEx { Number = "4002", Y = 850004.0000, X = 1040000.0000 },
                                      Orientation = new OrientationContext()
                                  };
            pointOfViewB.Orientation.TableNodes.Add(new OrientationPoint { Number = "4001", Hz = 0.00000, Y = 850000.0000, X = 1040000.0000 });
            var calculatedPoint = new DirectionIntersectionCalculatedPoint { DirectionFromA = 359.03340, DirectionFromB = 350.00000 };
            DirectionIntersectionMethod.Calculate(new DirectionIntersectionContext { PointOfViewA = pointOfViewA, PointOfViewB = pointOfViewB }, calculatedPoint);
            Assert.AreEqual(calculatedPoint.Y, 850016.0001, Delta);
            Assert.AreEqual(calculatedPoint.X, 1040012.0001, Delta);
        }

        [Test]
        public void Test2()
        {
            var pointOfViewA = new PolarContex
                                  {
                                      PointOfView = new PointBaseEx { Number = "4001", Y = 850000.0000, X = 1040000.0000 },
                                      Orientation = new OrientationContext()
                                  };
            pointOfViewA.Orientation.TableNodes.Add(new OrientationPoint { Number = "4002", Hz = 0.00000, Y = 850004.0000, X = 1040000.0000 });
            var pointOfViewB = new PolarContex
                                  {
                                      PointOfView = new PointBaseEx { Number = "4002", Y = 850004.0000, X = 1040000.0000 },
                                      Orientation = new OrientationContext()
                                  };
            pointOfViewB.Orientation.TableNodes.Add(new OrientationPoint { Number = "4001", Hz = 0.00000, Y = 850000.0000, X = 1040000.0000 });
            var calculatedPoint = new DirectionIntersectionCalculatedPoint { DirectionFromA = 100.00000, DirectionFromB = 50.00000 };
            DirectionIntersectionMethod.Calculate(new DirectionIntersectionContext { PointOfViewA = pointOfViewA, PointOfViewB = pointOfViewB }, calculatedPoint);
            Assert.AreEqual(calculatedPoint.Y, 850000.0000, Delta);
            Assert.AreEqual(calculatedPoint.X, 1040004.0000, Delta);
        }

    }
}
