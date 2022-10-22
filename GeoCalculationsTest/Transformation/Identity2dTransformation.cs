using DotNetMatrix;
using GeoCalculations.TransformationWrappers;
using NUnit.Framework;

namespace GeoCalculationsTest.Transformation
{
    [TestFixture]
    class Identity2DTransformation
    {
        [Test]
        [Ignore]//todo
        public void Test1()
        {
            var globalPointsArray = new double[3][];
            globalPointsArray[0] = new[] { 10d, 10d, 8 };
            globalPointsArray[1] = new[] { 100d, 100d, 12 };
            globalPointsArray[2] = new[] { 10d, 10d, 13 };
            var globalPoints = new MatrixWrapper(new GeneralMatrix(globalPointsArray));

            var localPointsArray = new double[3][];
            localPointsArray[0] = new[] { 10d, 10d, 8 };
            localPointsArray[1] = new[] { 100d, 100d, 12 };
            localPointsArray[2] = new[] { 10d, 10d, 13 };
            var localPoints = new MatrixWrapper(new GeneralMatrix(localPointsArray));
            var tr = new Identity2DTransformationWrapper(globalPoints, localPoints);
            Assert.False(tr.Solved());
            tr.Solve();
            Assert.True(tr.Solved());
        }

        [Test]
        public void BasicTest()
        {
            var globalPointsArray = new double[5][];
            globalPointsArray[0] = new[] { 760133.79, 1033284.47, 8 };
            globalPointsArray[1] = new[] { 762162.29, 1032943.59, 12 };
            globalPointsArray[2] = new[] { 762589.39, 1031896.61, 13 };
            globalPointsArray[3] = new[] { 759684.73, 1031422.23, 24 };
            globalPointsArray[4] = new[] { 761893.05, 1031047.25, 43 };
            var globalPoints = new MatrixWrapper(new GeneralMatrix(globalPointsArray));

            var localPointsArray = new double[5][];
            localPointsArray[0] = new[] { 8577.78, 51208.49, 8 };
            localPointsArray[1] = new[] { 10625.60, 51013.68, 12 };
            localPointsArray[2] = new[] { 11126.85, 50000.00, 13 };
            localPointsArray[3] = new[] { 8263.43, 49318.70, 24 };
            localPointsArray[4] = new[] { 10493.03, 49102.66, 43 };
            var localPoints = new MatrixWrapper(new GeneralMatrix(localPointsArray));

            var tr = new Identity2DTransformationWrapper(globalPoints, localPoints);
            Assert.False(tr.Solved());
            tr.Solve();
            Assert.True(tr.Solved());

            var localTransformPointsArray = new double[4][];
            localTransformPointsArray[0] = new[] { 9204.72, 49217.90, 501 };
            localTransformPointsArray[1] = new[] { 10000.00, 50000.00, 502 };
            localTransformPointsArray[2] = new[] { 8922.25, 50054.64, 503 };
            localTransformPointsArray[3] = new[] { 9641.46, 50880.29, 504 };
            var localTransformPoints = new MatrixWrapper(new GeneralMatrix(localTransformPointsArray));
            var transformedPoints = tr.TransformLocalPoints(localTransformPoints).ConvertToGeneralMatrix();

            Assert.AreEqual(760616.412, transformedPoints.GetElement(0, 0), 3);
            Assert.AreEqual(1031254.331, transformedPoints.GetElement(0, 1), 3);
            Assert.AreEqual(761465.606, transformedPoints.GetElement(1, 0), 3);
            Assert.AreEqual(1031977.381, transformedPoints.GetElement(1, 1), 3);
            Assert.AreEqual(760394.635, transformedPoints.GetElement(2, 0), 3);
            Assert.AreEqual(1032109.083, transformedPoints.GetElement(2, 1), 3);
            Assert.AreEqual(761171.081, transformedPoints.GetElement(3, 0), 3);
            Assert.AreEqual(1032881.016, transformedPoints.GetElement(3, 1), 3);

            tr.Dispose();
            localTransformPoints.Dispose();
            transformedPoints.Dispose();
            globalPoints.Dispose();
            localPoints.Dispose();
        }
    }
}
