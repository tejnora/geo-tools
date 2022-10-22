using System;
using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using NUnit.Framework;

namespace GeoCalculationsTest.Methods
{

    [TestFixture]
    public class ControlDistanceTests
    {
        [Test]
        public void Test1()
        {
            var body = new ControlDistanceContex { QualityCode = ControlDistanceContex.CodeQualities._3 };
            body.TableNodes.Add(new ControlDistancePoint { MeasuringLength = 0, Y = 800003.00, X = 1000000.00 });
            body.TableNodes.Add(new ControlDistancePoint { MeasuringLength = 5, Y = 800000.00, X = 1000004.00 });
            ControlDistanceMethod.Calculate(body);
            //            Assert.AreEqual(body.MezniOchylkyDodrzene, true);
            Assert.AreEqual(Math.Round(body.TableNodes[1].CoordinateLength, 2), 5.00);
            Assert.AreEqual(Math.Round(body.TableNodes[1].LengthDifference, 2), 0.00);
            Assert.AreEqual(Math.Round(body.TableNodes[1].LimitDeviation, 2), 0.27);
        }
        [Test]
        public void Test2()
        {
            var body = new ControlDistanceContex { QualityCode = ControlDistanceContex.CodeQualities._3 };
            body.TableNodes.Add(new ControlDistancePoint { MeasuringLength = 0, Y = 800003.00, X = 1000000.00 });
            body.TableNodes.Add(new ControlDistancePoint { MeasuringLength = 4.74, Y = 800000.00, X = 1000004.00 });
            ControlDistanceMethod.Calculate(body);
            //            Assert.AreEqual(body.MezniOchylkyDodrzene, true);
            Assert.AreEqual(Math.Round(body.TableNodes[1].CoordinateLength, 2), 5.00);
            Assert.AreEqual(Math.Round(body.TableNodes[1].LengthDifference, 2), 0.26);
            Assert.AreEqual(Math.Round(body.TableNodes[1].LimitDeviation, 2), 0.27);
        }
        [Test]
        public void Test3()
        {
            var body = new ControlDistanceContex { QualityCode = ControlDistanceContex.CodeQualities._3 };
            body.TableNodes.Add(new ControlDistancePoint { MeasuringLength = 0, Y = 800003.00, X = 1000000.00 });
            body.TableNodes.Add(new ControlDistancePoint { MeasuringLength = 4.73, Y = 800000.00, X = 1000004.00 });
            ControlDistanceMethod.Calculate(body);
            //            Assert.AreEqual(body.MezniOchylkyDodrzene, false);
            Assert.AreEqual(Math.Round(body.TableNodes[1].CoordinateLength, 2), 5.00);
            Assert.AreEqual(Math.Round(body.TableNodes[1].LengthDifference, 2), 0.27);
            Assert.AreEqual(Math.Round(body.TableNodes[1].LimitDeviation, 2), 0.27);
        }
        [Test]
        public void Test4()
        {
            var body = new ControlDistanceContex { QualityCode = ControlDistanceContex.CodeQualities._4 };
            body.TableNodes.Add(new ControlDistancePoint { MeasuringLength = 0, Y = 800003.00, X = 1000000.00 });
            body.TableNodes.Add(new ControlDistancePoint { MeasuringLength = 4.73, Y = 800000.00, X = 1000004.00 });
            body.TableNodes.Add(new ControlDistancePoint { MeasuringLength = 4.73, Y = 800003.00, X = 1000000.00 });
            ControlDistanceMethod.Calculate(body);
            //          Assert.AreEqual(body.MezniOchylkyDodrzene, true);
            Assert.AreEqual(Math.Round(body.TableNodes[1].CoordinateLength, 2), 5.00);
            Assert.AreEqual(Math.Round(body.TableNodes[1].LengthDifference, 2), 0.27);
            Assert.AreEqual(Math.Round(body.TableNodes[1].LimitDeviation, 2), 0.50);
            Assert.AreEqual(Math.Round(body.TableNodes[2].CoordinateLength, 2), 5.00);
            Assert.AreEqual(Math.Round(body.TableNodes[2].LengthDifference, 2), 0.27);
            Assert.AreEqual(Math.Round(body.TableNodes[2].LimitDeviation, 2), 0.50);

        }
    }
}
