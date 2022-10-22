using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoCalculations.Points;
using NUnit.Framework;

namespace GeoCalculationsTest.Methods
{
    [TestFixture]
    class FreePointOfViewTests
    {
        private const double Delta = 0.0001;
        [Test]
        public void Test1()
        {
            var orientations = new OrientationContext();
            orientations.TableNodes.Add(new OrientationPoint
            {
                Number = "0251",
                Y = 703071.5500,
                X = 1032213.6700,
                Z = 195.610,
                Hz = 340.78040,
                Distance = 206.5200,
                ZenitAngle = 99.02760,
                SignalHeight = 1.520
            });
            orientations.TableNodes.Add(new OrientationPoint
            {
                Number = "0252",
                Y = 703046.6000,
                X = 1032203.7300,
                Z = 195.540,
                Hz = 335.31900,
                Distance = 185.4900,
                ZenitAngle = 98.87740,
                SignalHeight = 1.520
            });
            orientations.TableNodes.Add(new OrientationPoint
            {
                Number = "4002",
                Y = 702924.6100,
                X = 1031906.5400,
                Z = 194.300,
                Hz = 123.07720,
                Distance = 137.1433,
                ZenitAngle = 99.13240,
                SignalHeight = 1.520
            });
            var pointOfView = new CalculatedPointBase { SignalHeight = 3.0 };
            FreePointOfViewMethod.Calculate(orientations);
            FreePointOfViewMethod.CalculatePoint(orientations, pointOfView);
            Assert.AreEqual(pointOfView.Y, 702962.3725, Delta);
            Assert.AreEqual(pointOfView.X, 1032038.3969, Delta);
            Assert.AreEqual(pointOfView.Z, 190.9114, Delta);

            var node = orientations.TableNodes[0];
            Assert.AreEqual(node.vY, -0.0116, Delta);
            Assert.AreEqual(node.vX, -0.0427, Delta);
            Assert.AreEqual(node.dhCalculated, 1.6343, Delta);
            Assert.AreEqual(node.ElevationDifferenceScale, 0.0000, Delta);
            Assert.AreEqual(node.Zp, 190.9757, Delta);
            Assert.AreEqual(node.vZ, -0.0642, Delta);

            node = orientations.TableNodes[1];
            Assert.AreEqual(node.vY, 0.0097, Delta);
            Assert.AreEqual(node.vX, 0.0459, Delta);
            Assert.AreEqual(node.dhCalculated, 1.7523, Delta);
            Assert.AreEqual(node.ElevationDifferenceScale, 0.0000, Delta);
            Assert.AreEqual(node.Zp, 190.7877, Delta);
            Assert.AreEqual(node.vZ, 0.1237, Delta);

            node = orientations.TableNodes[2];
            Assert.AreEqual(node.vY, 0.0019, Delta);
            Assert.AreEqual(node.vX, -0.0032, Delta);
            Assert.AreEqual(node.dhCalculated, 0.3493, Delta);
            Assert.AreEqual(node.ElevationDifferenceScale, 0.0001, Delta);
            Assert.AreEqual(node.Zp, 190.9507, Delta);
            Assert.AreEqual(node.vZ, -0.0393, Delta);

            Assert.AreEqual(orientations.OrientationMovement, 94.68067, 0.00001);
            Assert.AreEqual(orientations.m0, 0.00413, 0.00001);
            Assert.AreEqual(orientations.m1, 0.00238, 0.00001);
            Assert.AreEqual(orientations.Scale, 1.000086622892, 9);
            Assert.AreEqual(orientations.Y, 0.0107, Delta);
            Assert.AreEqual(orientations.X, 0.0444, Delta);
            Assert.True(orientations.CalculateHeightsAlso);

        }

        [Test]
        public void Test2()
        {
            var orientations = new OrientationContext();
            orientations.TableNodes.Add(new OrientationPoint
                                         {
                                             Number = "4002",
                                             Y = 702924.6100,
                                             X = 1031906.5400,
                                             Z = 194.300,
                                             Hz = 99.83660,
                                             Distance = 140.7519,
                                             ZenitAngle = 100.47860,
                                             SignalHeight = 1.520
                                         });
            orientations.TableNodes.Add(new OrientationPoint
                                         {
                                             Number = "4004",
                                             Y = 703016.4500,
                                             X = 1031630.4600,
                                             Z = 195.430,
                                             Hz = 275.45160,
                                             Distance = 155.6144,
                                             ZenitAngle = 99.95600,
                                             SignalHeight = 1.520
                                         });
            orientations.TableNodes.Add(new OrientationPoint
                                         {
                                             Number = "0066",
                                             Y = 703035.0700,
                                             X = 1031886.6100,
                                             Z = 195.430,
                                             Hz = 149.69900,
                                             Distance = 152.0538,
                                             ZenitAngle = 100.24080,
                                             SignalHeight = 1.520,
                                             IsEnabled = false
                                         });
            var pointOfView = new CalculatedPointBase();
            FreePointOfViewMethod.Calculate(orientations);
            FreePointOfViewMethod.CalculatePoint(orientations, pointOfView);
            Assert.AreEqual(orientations.Scale, 0.999977854220, 9);
            Assert.AreEqual(orientations.Y, 0.0000, Delta);
            Assert.AreEqual(orientations.X, 0.0000, Delta);

            var node = orientations.TableNodes[0];
            Assert.AreEqual(node.vY, -0.0000, Delta);
            Assert.AreEqual(node.vX, 0.0000, Delta);
            node = orientations.TableNodes[1];
            Assert.AreEqual(node.vY, 0.0000, Delta);
            Assert.AreEqual(node.vX, 0.0000, Delta);

            Assert.AreEqual(pointOfView.Y, 702941.4453, Delta);
            Assert.AreEqual(pointOfView.X, 1031766.8017, Delta);

            Assert.AreEqual(orientations.OrientationMovement, 292.53038, 0.00001);
            Assert.AreEqual(orientations.m0, 0.0, 0.00001);
            Assert.AreEqual(orientations.m1, 0.0, 0.00001);
        }

    }
}
