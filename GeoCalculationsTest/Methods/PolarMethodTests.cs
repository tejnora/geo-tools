using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using NUnit.Framework;

namespace GeoCalculationsTest.Methods
{
    [TestFixture]
    class PolarMethodTests
    {
        const double Delta = 0.0001;
        [Test]
        public void Test1()
        {
            var polarContex = new PolarContex();
            polarContex.PointOfView = new PointBaseEx { Prefix = "04900522", Number = "4001", Y = 681882.51, X = 1055998.34 };
            polarContex.Orientation = new OrientationContext();
            polarContex.Orientation.TableNodes.Add(new OrientationPoint { Prefix = "04900409", Number = "0016", Hz = 139.62040, Distance = 83.4200, Y = 681833.15, X = 1056065.62 });
            polarContex.Orientation.TableNodes.Add(new OrientationPoint { Prefix = "04900423", Number = "0010", Hz = 205.06740, Distance = 45.8000, Y = 681900.18, X = 1056040.65 });
            polarContex.Orientation.TableNodes.Add(new OrientationPoint { Prefix = "04900520", Number = "0006", Hz = 299.19480, Distance = 28.2000, Y = 681909.42, X = 1055989.92 });
            SimpleCalculation.CalculateOrientationMovement(polarContex.PointOfView, polarContex.Orientation);
            Assert.AreEqual(polarContex.Orientation.OrientationMovement, 220.0988, Delta);
            var node = polarContex.Orientation.TableNodes[0];
            Assert.AreEqual(node.Scale, 0.0834, Delta);
            Assert.AreEqual(node.DirectionAzimut, 359.7048, Delta);
            Assert.AreEqual(node.VerticalOrientation, 0.01445, Delta);
            Assert.AreEqual(node.VerticalDistance, 0.0246, Delta);
            node = polarContex.Orientation.TableNodes[1];
            Assert.AreEqual(node.Scale, 0.0458, Delta);
            Assert.AreEqual(node.DirectionAzimut, 25.18552, Delta);
            Assert.AreEqual(node.VerticalOrientation, -0.01924, Delta);
            Assert.AreEqual(node.VerticalDistance, 0.0516, Delta);
            node = polarContex.Orientation.TableNodes[2];
            Assert.AreEqual(node.Scale, 0.0282, Delta);
            Assert.AreEqual(node.DirectionAzimut, 119.30515, Delta);
            Assert.AreEqual(node.VerticalOrientation, -0.01147, Delta);
            Assert.AreEqual(node.VerticalDistance, -0.0035, Delta);

            var urcovanyBod = new CalculatedPointBase { Hz = 387.40200, Distance = 46.6100 };
            PolarMethod.Calculate(polarContex, urcovanyBod);
            Assert.AreEqual(urcovanyBod.Y, 681877.03094, Delta);
            Assert.AreEqual(urcovanyBod.X, 1055952.0531, Delta);

            urcovanyBod = new CalculatedPointBase { Hz = 326.10580, Distance = 54.9200 };
            PolarMethod.Calculate(polarContex, urcovanyBod);
            Assert.AreEqual(urcovanyBod.Y, 681923.5891, Delta);
            Assert.AreEqual(urcovanyBod.X, 1055961.8884, Delta);
        }
        [Test]
        public void Test2()
        {
            var polarContext = new PolarContex();
            polarContext.PointOfView = new PointBaseEx { Prefix = "04900522", Number = "4002", Y = 681877.03, X = 1055952.05 };
            polarContext.Orientation = new OrientationContext();
            polarContext.Orientation.TableNodes.Add(new OrientationPoint { Prefix = "04900522", Number = "4001", Hz = 360.65460, Distance = 46.6189, Y = 681882.51, X = 1055998.34 });
            polarContext.Orientation.TableNodes.Add(new OrientationPoint { Prefix = "04900522", Number = "0006", Hz = 358.20860, Distance = 20.6300, Y = 681878.68, X = 1055972.63 });
            SimpleCalculation.CalculateOrientationMovement(polarContext.PointOfView, polarContext.Orientation);
            Assert.AreEqual(polarContext.Orientation.OrientationMovement, 46.85858, Delta);
            var node = polarContext.Orientation.TableNodes[0];
            Assert.AreEqual(node.Scale, 0.0466, Delta);
            Assert.AreEqual(node.DirectionAzimut, 7.50165, Delta);
            Assert.AreEqual(node.VerticalOrientation, 0.01153, Delta);
            Assert.AreEqual(node.VerticalDistance, -0.0057, Delta);
            node = polarContext.Orientation.TableNodes[1];
            Assert.AreEqual(node.Scale, 0.0206, Delta);
            Assert.AreEqual(node.DirectionAzimut, 5.09320, Delta);
            Assert.AreEqual(node.VerticalOrientation, -0.02602, Delta);
            Assert.AreEqual(node.VerticalDistance, 0.0160, Delta);

            var urcovanyBod = new CalculatedPointBase { Hz = 62.41340, Distance = 4.8303 };
            PolarMethod.Calculate(polarContext, urcovanyBod);
            Assert.AreEqual(urcovanyBod.Y, 681881.8092, Delta);
            Assert.AreEqual(urcovanyBod.X, 1055951.3490, Delta);

            urcovanyBod = new CalculatedPointBase { Hz = 291.10040, Distance = 20.8173 };
            PolarMethod.Calculate(polarContext, urcovanyBod);
            Assert.AreEqual(urcovanyBod.Y, 681859.8049, Delta);
            Assert.AreEqual(urcovanyBod.X, 1055963.7400, Delta);

        }

        [Test]
        public void Test3()
        {
            var polarContext = new PolarContex();
            polarContext.PointOfView = new PointBaseEx { Prefix = "00000001", Number = "4001", Y = 702962.3700, X = 1032038.3700, SignalHeight = 1.380, Z = 192.580 };
            polarContext.Orientation = new OrientationContext();
            polarContext.Orientation.TableNodes.Add(new OrientationPoint { Prefix = "00091404", Number = "0251", Hz = 340.78040, Distance = 206.5200, Y = 703071.5500, X = 1032213.6700 });
            polarContext.Orientation.TableNodes.Add(new OrientationPoint { Prefix = "00091404", Number = "0252", Hz = 335.31900, Distance = 185.4900, Y = 703046.6000, X = 1032203.7300 });
            polarContext.Orientation.TableNodes.Add(new OrientationPoint { Prefix = "00091404", Number = "0250", Hz = 330.79660, Y = 703047.5300, X = 1032239.5500 });
            polarContext.Orientation.TableNodes.Add(new OrientationPoint { Prefix = "00000001", Number = "4002", Hz = 123.07720, Distance = 137.1433, Y = 702924.6100, X = 1031906.5400 });
            SimpleCalculation.CalculateOrientationMovement(polarContext.PointOfView, polarContext.Orientation);
            Assert.AreEqual(polarContext.Orientation.OrientationMovement, 94.68361, Delta);
            var node = polarContext.Orientation.TableNodes[0];
            Assert.AreEqual(node.Scale, 0.2065, Delta);
            Assert.AreEqual(node.DirectionAzimut, 35.46155, Delta);
            Assert.AreEqual(node.VerticalOrientation, 0.00246, Delta);
            Assert.AreEqual(node.VerticalDistance, -0.0004, Delta);
            node = polarContext.Orientation.TableNodes[1];
            Assert.AreEqual(node.Scale, 0.1856, Delta);
            Assert.AreEqual(node.DirectionAzimut, 29.99232, Delta);
            Assert.AreEqual(node.VerticalOrientation, 0.01029, Delta);
            Assert.AreEqual(node.VerticalDistance, 0.0865, Delta);
            node = polarContext.Orientation.TableNodes[2];
            Assert.AreEqual(node.Scale, 0.2185, Delta);
            Assert.AreEqual(node.DirectionAzimut, 25.49229, Delta);
            Assert.AreEqual(node.VerticalOrientation, -0.01208, Delta);
            Assert.AreEqual(node.VerticalDistance, double.NaN, Delta);
            node = polarContext.Orientation.TableNodes[3];
            Assert.AreEqual(node.Scale, 0.1371, Delta);
            Assert.AreEqual(node.DirectionAzimut, 217.75919, Delta);
            Assert.AreEqual(node.VerticalOrientation, 0.00162, Delta);
            Assert.AreEqual(node.VerticalDistance, -0.0121, Delta);

            var urcovanyBod = new CalculatedPointBase { Hz = 157.66800, Distance = 283.9100, ZenitAngle = 100.05200, TargetHeight = 1.520 };
            PolarMethod.Calculate(polarContext, urcovanyBod);
            Assert.AreEqual(urcovanyBod.Y, 702754.3383, Delta);
            Assert.AreEqual(urcovanyBod.X, 1031845.1663, Delta);
            Assert.AreEqual(urcovanyBod.Z, 192.208, Delta);

            urcovanyBod = new CalculatedPointBase { Hz = 368.93120, Distance = 65.2000, ZenitAngle = 100.34500, TargetHeight = 1.520 };
            PolarMethod.Calculate(polarContext, urcovanyBod);
            Assert.AreEqual(urcovanyBod.Y, 703017.2078, Delta);
            Assert.AreEqual(urcovanyBod.X, 1032073.6383, Delta);
            Assert.AreEqual(urcovanyBod.Z, 192.0866, Delta);
        }
    }
}
