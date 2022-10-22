using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoCalculations.Points;
using NUnit.Framework;

namespace GeoCalculationsTest.Methods
{
    [TestFixture]
    class OrtogonalniMetodaTests
    {
        const double Delta = 0.0001;

        [Test]
        public void TestBasic()
        {
            var identickeBody = new OrtogonalContext();
            identickeBody.TableNodes.Add(new PointBaseEx { Number = "1", Y = 703081.6000, X = 1032229.6800, Stationing = 0, Vertical = 0 });
            identickeBody.TableNodes.Add(new PointBaseEx { Number = "2", Y = 703082.0300, X = 1032226.9700, Stationing = 2.7600, Vertical = 0 });
            var calculatedPoint = new CalculatedPointBase { Number = "3", Stationing = 1, Vertical = 0 };
            OrthogonalMethod.Calculate(identickeBody);
            OrthogonalMethod.CalculatePoint(identickeBody, calculatedPoint);
            Assert.AreEqual(703081.7558, calculatedPoint.Y, Delta);
            Assert.AreEqual(1032228.6981, calculatedPoint.X, Delta);
            Assert.AreEqual(0.994167511127, identickeBody.Scale, Delta);
            Assert.AreEqual(-0.0161, identickeBody.StationingCorrection, Delta);
            Assert.False(identickeBody.Deviations.HasError);
        }

        [Test]
        public void TestXlsVypocty()
        {
            var identickeBody = new OrtogonalContext();
            identickeBody.TableNodes.Add(new PointBaseEx { Number = "4001", Y = 850000.0000, X = 1040000.0000, Stationing = 0, Vertical = 0 });
            identickeBody.TableNodes.Add(new PointBaseEx { Number = "4003", Y = 849950.0000, X = 1039950.0000, Stationing = 70.7107, Vertical = 0 });
            identickeBody.TableNodes.Add(new PointBaseEx { Number = "4002", Y = 849900.0000, X = 1040000.0000, Stationing = 70.7110, Vertical = 70.7260 });
            var calculatedPoint = new CalculatedPointBase { Number = "0029", Stationing = 0, Vertical = -100 };
            OrthogonalMethod.Calculate(identickeBody);
            OrthogonalMethod.CalculatePoint(identickeBody, calculatedPoint);
            Assert.AreEqual(850070.7040, calculatedPoint.Y, Delta);
            Assert.AreEqual(1039929.2980, calculatedPoint.X, Delta);
            Assert.AreEqual(0.999890444426, identickeBody.Scale, Delta);
            Assert.AreEqual(0, identickeBody.StationingCorrection, Delta);
            Assert.False(identickeBody.Deviations.HasError);
            Assert.AreEqual(0.0027, identickeBody.TableNodes[0].dY, Delta);
            Assert.AreEqual(0.0028, identickeBody.TableNodes[0].dX, Delta);
            Assert.AreEqual(-0.0054, identickeBody.TableNodes[1].dY, Delta);
            Assert.AreEqual(-0.0001, identickeBody.TableNodes[1].dX, Delta);
            Assert.AreEqual(0.0028, identickeBody.TableNodes[2].dY, Delta);
            Assert.AreEqual(-0.0027, identickeBody.TableNodes[2].dX, Delta);
        }
    }
}
