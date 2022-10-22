using DotNetMatrix;
using GeoCalculations.TransformationWrappers;
using NUnit.Framework;

namespace GeoCalculationsTest.Transformation
{
    [TestFixture]
    class Similar2DTransformation
    {
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

            var tr = new Similarity2DTransformationWrapper(globalPoints, localPoints);
            Assert.False(tr.Solved());
            tr.Solve();
            Assert.True(tr.Solved());

            var localTransformPointsArray = new double[9][];
            localTransformPointsArray[0] = new[] { 8577.78, 51208.49, 8 };
            localTransformPointsArray[1] = new[] { 10625.60, 51013.68, 12 };
            localTransformPointsArray[2] = new[] { 11126.85, 50000.00, 13 };
            localTransformPointsArray[3] = new[] { 8263.43, 49318.70, 24 };
            localTransformPointsArray[4] = new[] { 10493.03, 49102.66, 43 };
            localTransformPointsArray[5] = new[] { 9204.72, 49217.90, 501 };
            localTransformPointsArray[6] = new[] { 10000.00, 50000.00, 502 };
            localTransformPointsArray[7] = new[] { 8922.25, 50054.64, 503 };
            localTransformPointsArray[8] = new[] { 9641.46, 50880.29, 504 };
            var localTransformPoints = new MatrixWrapper(new GeneralMatrix(localTransformPointsArray));
            var transformedPoints = tr.TransformLocalPoints(localTransformPoints).ConvertToGeneralMatrix();

            Assert.AreEqual(760133.73990, transformedPoints.GetElement(0, 0), 3);
            Assert.AreEqual(1033284.54326, transformedPoints.GetElement(0, 1), 3);
            Assert.AreEqual(762162.16132, transformedPoints.GetElement(1, 0), 3);
            Assert.AreEqual(1032943.54977, transformedPoints.GetElement(1, 1), 3);
            Assert.AreEqual(762589.46129, transformedPoints.GetElement(2, 0), 3);
            Assert.AreEqual(1031896.65555, transformedPoints.GetElement(2, 1), 3);
            Assert.AreEqual(759684.84504, transformedPoints.GetElement(3, 0), 3);
            Assert.AreEqual(1031422.29540, transformedPoints.GetElement(3, 1), 3);
            Assert.AreEqual(761893.04245, transformedPoints.GetElement(4, 0), 3);
            Assert.AreEqual(1031047.10601, transformedPoints.GetElement(4, 1), 3);
            Assert.AreEqual(760616.41214, transformedPoints.GetElement(5, 0), 3);
            Assert.AreEqual(1031254.33139, transformedPoints.GetElement(5, 1), 3);
            Assert.AreEqual(761465.60630, transformedPoints.GetElement(6, 0), 3);
            Assert.AreEqual(1031977.38058, transformedPoints.GetElement(6, 1), 3);
            Assert.AreEqual(760394.63509, transformedPoints.GetElement(7, 0), 3);
            Assert.AreEqual(1032109.08297, transformedPoints.GetElement(7, 1), 3);
            Assert.AreEqual(761171.08127, transformedPoints.GetElement(8, 0), 3);
            Assert.AreEqual(1032881.01590, transformedPoints.GetElement(8, 1), 3);

            localTransformPoints.Dispose();
            transformedPoints.Dispose();
            globalPoints.Dispose();
            localPoints.Dispose();
        }

    }
}
