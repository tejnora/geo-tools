using GeoCalculations.CalculationContexts;
using GeoCalculations.CalculationResultContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using NUnit.Framework;

namespace GeoCalculationsTest.Methods
{
    [TestFixture]
    class CordinateTransformationTests
    {
        private const double Delta = 0.001;
        CoordinatesTransformationContex _points;
        CoordinateTransformationResultContext _resultPoints;

        [SetUp]
        public void Init()
        {
            _points = new CoordinatesTransformationContex();
            _points.TableNodes.Add(new CordinateTransformationPoint { Number = "8", YLocal = 8577.7800, XLocal = 51208.4900, Y = 760133.7900, X = 1033284.4700 });
            _points.TableNodes.Add(new CordinateTransformationPoint { Number = "12", YLocal = 10625.6000, XLocal = 51013.6800, Y = 762162.2900, X = 1032943.5900 });
            _points.TableNodes.Add(new CordinateTransformationPoint { Number = "13", YLocal = 11126.8500, XLocal = 50000.0000, Y = 762589.3900, X = 1031896.6100 });
            _points.TableNodes.Add(new CordinateTransformationPoint { Number = "24", YLocal = 8263.4300, XLocal = 49318.7000, Y = 759684.7300, X = 1031422.2300 });
            _points.TableNodes.Add(new CordinateTransformationPoint { Number = "43", YLocal = 10493.0300, XLocal = 49102.6600, Y = 761893.0500, X = 1031047.2500 });
            _resultPoints = new CoordinateTransformationResultContext();
            _resultPoints.Nodes.Add(new CordinateTransformationCalculatedPoint { Number = "501", YLocal = 9204.72, XLocal = 49217.90 });
            _resultPoints.Nodes.Add(new CordinateTransformationCalculatedPoint { Number = "502", YLocal = 10000.00, XLocal = 50000.00 });
            _resultPoints.Nodes.Add(new CordinateTransformationCalculatedPoint { Number = "503", YLocal = 8922.25, XLocal = 50054.64 });
            _resultPoints.Nodes.Add(new CordinateTransformationCalculatedPoint { Number = "504", YLocal = 9641.46, XLocal = 50880.29 });
        }

        [Test]
        public void HelmertSimilarTransformation()
        {
            _points.TransformationType = TransformationTypes.HelmertSimilar;
            CordinateTransformationMethod.CalculatePoint(_points, _resultPoints);
            Assert.AreEqual(-4.56492, _points.Rotation, 5);
            Assert.AreEqual(0.999911653802, _points.Scale, 12);
            //residues
            Assert.AreEqual(0.092, _points.ResiduesMiddleDeviationX, Delta);
            Assert.AreEqual(0.096, _points.ResiduesMiddleDeviationY, Delta);
            //Assert.AreEqual(0.11, _resultPoints.Residues.GetMiddleDeviaction(), Delta);todo
            Assert.AreEqual(-0.073, _points.TableNodes[0].vX, Delta);
            Assert.AreEqual(0.050, _points.TableNodes[0].vY, Delta);
            Assert.AreEqual(0.040, _points.TableNodes[1].vX, Delta);
            Assert.AreEqual(0.129, _points.TableNodes[1].vY, Delta);
            Assert.AreEqual(-0.046, _points.TableNodes[2].vX, Delta);
            Assert.AreEqual(-0.071, _points.TableNodes[2].vY, Delta);
            Assert.AreEqual(-0.065, _points.TableNodes[3].vX, Delta);
            Assert.AreEqual(-0.115, _points.TableNodes[3].vY, Delta);
            Assert.AreEqual(0.144, _points.TableNodes[4].vX, Delta);
            Assert.AreEqual(0.008, _points.TableNodes[4].vY, Delta);
            //calculated points
            Assert.AreEqual(760616.4121, _resultPoints.Nodes[0].Y, Delta);
            Assert.AreEqual(1031254.3314, _resultPoints.Nodes[0].X, Delta);
            Assert.AreEqual(761465.6063, _resultPoints.Nodes[1].Y, Delta);
            Assert.AreEqual(1031977.3806, _resultPoints.Nodes[1].X, Delta);
            Assert.AreEqual(760394.6351, _resultPoints.Nodes[2].Y, Delta);
            Assert.AreEqual(1032109.0830, _resultPoints.Nodes[2].X, Delta);
            Assert.AreEqual(761171.0813, _resultPoints.Nodes[3].Y, Delta);
            Assert.AreEqual(1032881.0159, _resultPoints.Nodes[3].X, Delta);
        }

        [Test]
        public void SimilarTransformation()
        {
            _points.TransformationType = TransformationTypes.Similar;
            CordinateTransformationMethod.CalculatePoint(_points, _resultPoints);
            Assert.AreEqual(4.56492, _points.Rotation, 5);
            Assert.AreEqual(0.999911653802, _points.Scale, 12);
            //residues
            Assert.AreEqual(0.092, _points.ResiduesMiddleDeviationX, Delta);
            Assert.AreEqual(0.096, _points.ResiduesMiddleDeviationY, Delta);
            Assert.AreEqual(0.073, _points.TableNodes[0].vX, Delta);
            Assert.AreEqual(-0.050, _points.TableNodes[0].vY, Delta);
            Assert.AreEqual(-0.040, _points.TableNodes[1].vX, Delta);
            Assert.AreEqual(-0.129, _points.TableNodes[1].vY, Delta);
            Assert.AreEqual(0.046, _points.TableNodes[2].vX, Delta);
            Assert.AreEqual(0.071, _points.TableNodes[2].vY, Delta);
            Assert.AreEqual(0.065, _points.TableNodes[3].vX, Delta);
            Assert.AreEqual(0.115, _points.TableNodes[3].vY, Delta);
            Assert.AreEqual(-0.144, _points.TableNodes[4].vX, Delta);
            Assert.AreEqual(-0.008, _points.TableNodes[4].vY, Delta);
            //calculated points
            Assert.AreEqual(760616.4121, _resultPoints.Nodes[0].Y, Delta);
            Assert.AreEqual(1031254.3314, _resultPoints.Nodes[0].X, Delta);
            Assert.AreEqual(761465.6063, _resultPoints.Nodes[1].Y, Delta);
            Assert.AreEqual(1031977.3806, _resultPoints.Nodes[1].X, Delta);
            Assert.AreEqual(760394.6351, _resultPoints.Nodes[2].Y, Delta);
            Assert.AreEqual(1032109.0830, _resultPoints.Nodes[2].X, Delta);
            Assert.AreEqual(761171.0813, _resultPoints.Nodes[3].Y, Delta);
            Assert.AreEqual(1032881.0159, _resultPoints.Nodes[3].X, Delta);
        }

        [Test]
        public void IdentityTransformation()
        {
            _points.TransformationType = TransformationTypes.Identity;
            CordinateTransformationMethod.CalculatePoint(_points, _resultPoints);
            Assert.AreEqual(4.5649, _points.Rotation, 5);
            Assert.AreEqual(1, _points.Scale, 12);
            //rezidues
            Assert.AreEqual(0.15, _points.ResiduesMiddleDeviationX, Delta);
            Assert.AreEqual(0.1257, _points.ResiduesMiddleDeviationY, Delta);
            Assert.AreEqual(-0.15250, _points.TableNodes[0].vY, Delta);
            Assert.AreEqual(0.17626, _points.TableNodes[0].vX, Delta);
            Assert.AreEqual(-0.05185, _points.TableNodes[1].vY, Delta);
            Assert.AreEqual(0.03264, _points.TableNodes[1].vX, Delta);
            Assert.AreEqual(0.18587, _points.TableNodes[2].vY, Delta);
            Assert.AreEqual(0.02592, _points.TableNodes[2].vX, Delta);
            Assert.AreEqual(-0.02702, _points.TableNodes[3].vY, Delta);
            Assert.AreEqual(0.00386, _points.TableNodes[3].vX, Delta);
            Assert.AreEqual(0.04550, _points.TableNodes[4].vY, Delta);
            Assert.AreEqual(-0.23868, _points.TableNodes[4].vX, Delta);
            //calculated points
            Assert.AreEqual(760616.352, _resultPoints.Nodes[0].Y, Delta);
            Assert.AreEqual(1031254.255, _resultPoints.Nodes[0].X, Delta);
            Assert.AreEqual(761465.621, _resultPoints.Nodes[1].Y, Delta);
            Assert.AreEqual(1031977.368, _resultPoints.Nodes[1].X, Delta);
            Assert.AreEqual(760394.555, _resultPoints.Nodes[2].Y, Delta);
            Assert.AreEqual(1032109.082, _resultPoints.Nodes[2].X, Delta);
            Assert.AreEqual(761171.070, _resultPoints.Nodes[3].Y, Delta);
            Assert.AreEqual(1032881.083, _resultPoints.Nodes[3].X, Delta);
        }

        [Test]
        public void AffineTransformation()
        {
            _points.TransformationType = TransformationTypes.Affine;
            CordinateTransformationMethod.CalculatePoint(_points, _resultPoints);
            Assert.AreEqual(4.5651, _points.Rotation, 5);
            Assert.AreEqual(1, _points.Scale, 12);
            //residues
            Assert.AreEqual(0.0855, _points.ResiduesMiddleDeviationX, Delta);
            Assert.AreEqual(0.0900, _points.ResiduesMiddleDeviationY, Delta);
            Assert.AreEqual(-0.0731, _points.TableNodes[0].vY, Delta);
            Assert.AreEqual(0.02877, _points.TableNodes[0].vX, Delta);
            Assert.AreEqual(-0.1067, _points.TableNodes[1].vY, Delta);
            Assert.AreEqual(-0.07627, _points.TableNodes[1].vX, Delta);
            Assert.AreEqual(0.1000, _points.TableNodes[2].vY, Delta);
            Assert.AreEqual(0.0511, _points.TableNodes[2].vX, Delta);
            Assert.AreEqual(0.07680, _points.TableNodes[3].vY, Delta);
            Assert.AreEqual(0.0982, _points.TableNodes[3].vX, Delta);
            Assert.AreEqual(0.0031, _points.TableNodes[4].vY, Delta);
            Assert.AreEqual(-0.1018, _points.TableNodes[4].vX, Delta);
            //calculated points
            Assert.AreEqual(760616.395, _resultPoints.Nodes[0].Y, Delta);
            Assert.AreEqual(1031254.369, _resultPoints.Nodes[0].X, Delta);
            Assert.AreEqual(761465.610, _resultPoints.Nodes[1].Y, Delta);
            Assert.AreEqual(1031977.386, _resultPoints.Nodes[1].X, Delta);
            Assert.AreEqual(760394.615, _resultPoints.Nodes[2].Y, Delta);
            Assert.AreEqual(1032109.086, _resultPoints.Nodes[2].X, Delta);
            Assert.AreEqual(761171.081, _resultPoints.Nodes[3].Y, Delta);
            Assert.AreEqual(1032880.985, _resultPoints.Nodes[3].X, Delta);
        }
    }
}
