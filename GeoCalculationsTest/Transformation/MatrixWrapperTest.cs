using DotNetMatrix;
using GeoCalculations.TransformationWrappers;
using NUnit.Framework;

namespace GeoCalculationsTest.Transformation
{
    [TestFixture]
    class MatrixWrapperTest
    {
        [Test]
        public void CreateMatrix()
        {
            using (var m = new MatrixWrapper(10, 10))
            {
                for (var i = 0; i < 10; i++)
                {
                    for (var j = 0; j < 10; j++)
                    {
                        m.SetElement(i, j, i * j);
                    }
                }
                for (var i = 0; i < 10; i++)
                {
                    for (var j = 0; j < 10; j++)
                    {
                        Assert.AreEqual(i * j, m.GetElement(i, j));
                    }
                }

            }
        }

        [Test]
        public void CreateMatrixFromGeneralMatrix()
        {
            var matrix = new GeneralMatrix(10, 10);
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    matrix.SetElement(i, j, i * j);
                }
            }

            using (var wrapper = new MatrixWrapper(matrix))
            {
                var newMatrix = wrapper.ConvertToGeneralMatrix();
                for (var i = 0; i < 10; i++)
                {
                    for (var j = 0; j < 10; j++)
                    {
                        Assert.AreEqual(i * j, newMatrix.GetElement(i, j));
                    }
                }
            }
        }
    }
}
