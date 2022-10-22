using GeoCalculations.CalculationContexts;
using GeoCalculations.CalculationResultContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using NUnit.Framework;

namespace GeoCalculationsTest.Methods
{
    [TestFixture]
    class ConstractionDistanceTests
    {
        const double Delta = 0.01;

        [Test]
        public void Test1()
        {
            var ib = new ConstructionDistanceContex();
            ib.TableNodes.Add(new ConstructionDistancePoint { Number = "1", Distance = 0, Y = 800003.0000, X = 1000000.0000 });
            ib.TableNodes.Add(new ConstructionDistancePoint { Number = "2", Distance = 1.0100 });
            ib.TableNodes.Add(new ConstructionDistancePoint { Number = "3", Distance = 0.9900 });
            ib.TableNodes.Add(new ConstructionDistancePoint { Number = "4", Distance = -2.0100 });
            ib.TableNodes.Add(new ConstructionDistancePoint { Number = "5", Distance = 2.9800, Y = 800000.0000, X = 1000004.0000 });
            var ub = new ConstructionDistanceResultContext();
            ConstructionDistanceMethod.Calculate(ib, ub);
            Assert.AreEqual(ib.TableNodes[0].XLocal, 0, Delta);
            Assert.AreEqual(ib.TableNodes[0].YLocal, 0, Delta);
            Assert.AreEqual(ib.TableNodes[1].XLocal, 1.01, Delta);
            Assert.AreEqual(ib.TableNodes[1].YLocal, 0, Delta);
            Assert.AreEqual(ib.TableNodes[2].XLocal, 1.01, Delta);
            Assert.AreEqual(ib.TableNodes[2].YLocal, 0.99, Delta);
            Assert.AreEqual(ib.TableNodes[3].XLocal, 3.02, Delta);
            Assert.AreEqual(ib.TableNodes[3].YLocal, 0.99, Delta);
            Assert.AreEqual(ib.TableNodes[4].XLocal, 3.02, Delta);
            Assert.AreEqual(ib.TableNodes[4].YLocal, 3.97, Delta);
            Assert.False(ib.Deviations.HasError);

            Assert.AreEqual(ib.Deviations.Deviations[0].CalculationDeviation, 0.01188, Delta);

            Assert.AreEqual(ub.Nodes[0].X, 1000000.0000, Delta);
            Assert.AreEqual(ub.Nodes[0].Y, 800003.0000, Delta);
            Assert.AreEqual(ub.Nodes[1].X, 1000000.0069, Delta);
            Assert.AreEqual(ub.Nodes[1].Y, 800001.9875, Delta);
            Assert.AreEqual(ub.Nodes[2].X, 1000000.9992, Delta);
            Assert.AreEqual(ub.Nodes[2].Y, 800001.9944, Delta);
            Assert.AreEqual(ub.Nodes[3].X, 1000001.0130, Delta);
            Assert.AreEqual(ub.Nodes[3].Y, 799999.9796, Delta);
            Assert.AreEqual(ub.Nodes[4].X, 1000004.0000, Delta);
            Assert.AreEqual(ub.Nodes[4].Y, 800000.0000, Delta);
        }
    }
}

