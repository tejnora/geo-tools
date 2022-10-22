using DotNetMatrix;
using GeoCalculations.TransformationWrappers;
using NUnit.Framework;

namespace GeoCalculationsTest.Transformation
{
    [TestFixture]
    class Affine2DTransformation
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

            var tr = new Affine2DTransformationWrapper(globalPoints, localPoints);
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

            Assert.AreEqual(760133.71685, transformedPoints.GetElement(0, 0), 3);
            Assert.AreEqual(1033284.49878, transformedPoints.GetElement(0, 1), 3);
            Assert.AreEqual(762162.18322, transformedPoints.GetElement(1, 0), 3);
            Assert.AreEqual(1032943.51372, transformedPoints.GetElement(1, 1), 3);
            Assert.AreEqual(762589.49001, transformedPoints.GetElement(2, 0), 3);
            Assert.AreEqual(1031896.66112, transformedPoints.GetElement(2, 1), 3);
            Assert.AreEqual(759684.80680, transformedPoints.GetElement(3, 0), 3);
            Assert.AreEqual(1031422.32821, transformedPoints.GetElement(3, 1), 3);
            Assert.AreEqual(761893.05312, transformedPoints.GetElement(4, 0), 3);
            Assert.AreEqual(1031047.14818, transformedPoints.GetElement(4, 1), 3);
            Assert.AreEqual(760616.39451, transformedPoints.GetElement(5, 0), 3);
            Assert.AreEqual(1031254.36854, transformedPoints.GetElement(5, 1), 3);
            Assert.AreEqual(761465.60983, transformedPoints.GetElement(6, 0), 3);
            Assert.AreEqual(1031977.38589, transformedPoints.GetElement(6, 1), 3);
            Assert.AreEqual(760394.61477, transformedPoints.GetElement(7, 0), 3);
            Assert.AreEqual(1032109.08580, transformedPoints.GetElement(7, 1), 3);
            Assert.AreEqual(761171.08058, transformedPoints.GetElement(8, 0), 3);
            Assert.AreEqual(1032880.98509, transformedPoints.GetElement(8, 1), 3);

            TestReport(tr);

            tr.Dispose();
            localTransformPoints.Dispose();
            transformedPoints.Dispose();
            globalPoints.Dispose();
            localPoints.Dispose();
        }

        void TestReport(Affine2DTransformationWrapper tr)
        {
            var report = tr.GetReport();
            Assert.AreEqual(747909.85999398597, report.Keys[0]);
            Assert.AreEqual(982828.70149961428, report.Keys[1]);
            Assert.AreEqual(0.99993426079300773, report.Keys[2]);
            Assert.AreEqual(0.99987080065901413, report.Keys[3]);
            Assert.AreEqual(0.99743006519478117, report.Keys[4]);
            Assert.AreEqual(0.071646807647580579, report.Keys[5]);
            Assert.False(report.Nullity);
            Assert.AreEqual(5, report.NumberOfIdenticalPoints);
            Assert.AreEqual(4, report.NumberOfIterations);
            Assert.AreEqual(0.15739981655754914, report.StandardDeviation);
            Assert.AreEqual(0.11129847764536162, report.StandardDeviationInCoordinates);
            Assert.True(report.Success);
            Assert.AreEqual(0.000000000022796234742723403, report.SumOfNormalEquationsAbsoluteResiduals);
            Assert.AreEqual(UsedSolutionAlgorithms.GaussJordanElimination, report.UsedSolutionAlgorithm);
        }
    }
}
