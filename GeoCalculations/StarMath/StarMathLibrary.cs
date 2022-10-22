using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoCalculations.StarMath
{
    public static class StarMath
    {
        private const int CellWidth = 10;
        private const int NumDecimals = 3;

        public static string MakePrintString(double[,] a)
        {
            if (a == null) return "<null>";
            var format = "{0:F" + NumDecimals + "}";
            var p = "";
            for (var i = 0; i < a.GetLength(0); i++)
            {
                p += "| ";
                for (var j = 0; j < a.GetLength(1); j++)
                    p += FormatCell(format, a[i, j]) + ",";
                p = p.Remove(p.Length - 1);
                p += " |\n";
            }
            p = p.Remove(p.Length - 1);
            return p;
        }

        private static object FormatCell(string format, double p)
        {
            var numStr = string.Format(format, p);
            numStr = numStr.TrimEnd('0');
            numStr = numStr.TrimEnd('.');
            var padAmt = ((double)(CellWidth - numStr.Length)) / 2;
            numStr = numStr.PadLeft((int)Math.Floor(padAmt + numStr.Length));
            numStr = numStr.PadRight(CellWidth);
            return numStr;
        }

        public static double[] Solve(double[,] a, IList<double> b)
        {
            var aLength = a.GetLength(0);
            if (aLength != a.GetLength(1))
                throw new Exception("Matrix, A, must be square.");
            if (aLength != b.Count)
                throw new Exception("Matrix, A, must be have the same number of rows as the vector, b.");

            /****** need code to determine when to switch between *****
             ****** this analytical approach and the SOR approach *****/
            return multiply(Inverse(a, aLength), b, aLength, aLength);
        }

        public static void SolveDestructiveInto(double[,] a, double[] b, double[] target)
        {
            var aLength = a.GetLength(0);
            if (aLength != a.GetLength(1))
                throw new Exception("Matrix, A, must be square.");
            if (aLength != b.Length)
                throw new Exception("Matrix, A, must be have the same number of rows as the vector, b.");

            /****** need code to determine when to switch between *****
             ****** this analytical approach and the SOR approach *****/
            InverseInPlace(a, aLength);
            MultiplyInto(a, b, aLength, aLength, target);
        }

        public static void GaussElimination(int nDim, double[][] pfMatr, double[] pfVect, double[] pfSolution)
        {
            double fMaxElem;
            double fAcc;

            int i, j, k, m;

            for (k = 0; k < (nDim - 1); k++) // base row of matrix
            {
                var rowK = pfMatr[k];

                // search of line with max element
                fMaxElem = Math.Abs(rowK[k]);
                m = k;
                for (i = k + 1; i < nDim; i++)
                {
                    if (fMaxElem < Math.Abs(pfMatr[i][k]))
                    {
                        fMaxElem = pfMatr[i][k];
                        m = i;
                    }
                }

                // permutation of base line (index k) and max element line(index m)                
                if (m != k)
                {
                    var rowM = pfMatr[m];
                    for (i = k; i < nDim; i++)
                    {
                        fAcc = rowK[i];
                        rowK[i] = rowM[i];
                        rowM[i] = fAcc;
                    }
                    fAcc = pfVect[k];
                    pfVect[k] = pfVect[m];
                    pfVect[m] = fAcc;
                }

                //if( pfMatr[k*nDim + k] == 0.0) return 1; // needs improvement !!!

                // triangulation of matrix with coefficients
                for (j = (k + 1); j < nDim; j++) // current row of matrix
                {
                    var rowJ = pfMatr[j];
                    fAcc = -rowJ[k] / rowK[k];
                    for (i = k; i < nDim; i++)
                    {
                        rowJ[i] = rowJ[i] + fAcc * rowK[i];
                    }
                    pfVect[j] = pfVect[j] + fAcc * pfVect[k]; // free member recalculation
                }
            }

            for (k = (nDim - 1); k >= 0; k--)
            {
                var rowK = pfMatr[k];
                pfSolution[k] = pfVect[k];
                for (i = (k + 1); i < nDim; i++)
                {
                    pfSolution[k] -= (rowK[i] * pfSolution[i]);
                }
                pfSolution[k] = pfSolution[k] / rowK[k];
            }
        }

        public static double[,] Inverse(double[,] a, int length)
        {
            if (length == 1) return new[,] { { 1 / a[0, 0] } };
            return InverseWithLUResult(LUDecomposition(a, length), length);
        }

        public static void InverseInPlace(double[,] a, int length)
        {
            if (length == 1)
            {
                a[0, 0] = 1 / a[0, 0];
                return;
            }
            LUDecompositionInPlace(a, length);
            InverseWithLUResult(a, length);
        }


        public static double[,] LUDecomposition(double[,] a, int length)
        {
            var b = (double[,])a.Clone();
            // normalize row 0
            for (var i = 1; i < length; i++) b[0, i] /= b[0, 0];

            for (var i = 1; i < length; i++)
            {
                for (var j = i; j < length; j++)
                {
                    // do a column of L
                    var sum = 0.0;
                    for (var k = 0; k < i; k++)
                        sum += b[j, k] * b[k, i];
                    b[j, i] -= sum;
                }
                if (i == length - 1) continue;
                for (var j = i + 1; j < length; j++)
                {
                    // do a row of U
                    var sum = 0.0;
                    for (var k = 0; k < i; k++)
                        sum += b[i, k] * b[k, j];
                    b[i, j] =
                        (b[i, j] - sum) / b[i, i];
                }
            }
            return b;
        }

        public static void LUDecompositionInPlace(double[,] a, int length)
        {
            var b = a;
            // normalize row 0
            for (var i = 1; i < length; i++) b[0, i] /= b[0, 0];

            for (var i = 1; i < length; i++)
            {
                for (var j = i; j < length; j++)
                {
                    // do a column of L
                    var sum = 0.0;
                    for (var k = 0; k < i; k++)
                        sum += b[j, k] * b[k, i];
                    b[j, i] -= sum;
                }
                if (i == length - 1) continue;
                for (var j = i + 1; j < length; j++)
                {
                    // do a row of U
                    var sum = 0.0;
                    for (var k = 0; k < i; k++)
                        sum += b[i, k] * b[k, j];
                    b[i, j] =
                        (b[i, j] - sum) / b[i, i];
                }
            }
        }

        private static double[,] InverseWithLUResult(double[,] b, int length)
        {
            // this code is adapted from http://users.erols.com/mdinolfo/matrix.htm
            // one constraint/caveat in this function is that the diagonal elts. cannot
            // be zero.
            // if the matrix is not square or is less than B 2x2, 
            // then this function won't work
            for (var i = 0; i < length; i++)
                for (var j = i; j < length; j++)
                {
                    var x = 1.0;
                    if (i != j)
                    {
                        x = 0.0;
                        for (var k = i; k < j; k++)
                            x -= b[j, k] * b[k, i];
                    }
                    b[j, i] = x / b[j, j];
                }

            for (var i = 0; i < length; i++)
                for (var j = i; j < length; j++)
                {
                    if (i == j) continue;
                    var sum = 0.0;
                    for (var k = i; k < j; k++)
                        sum += b[k, j] * ((i == k) ? 1.0 : b[i, k]);
                    b[i, j] = -sum;
                }

            for (var i = 0; i < length; i++)
                for (var j = 0; j < length; j++)
                {
                    var sum = 0.0;
                    for (var k = ((i > j) ? i : j); k < length; k++)
                        sum += ((j == k) ? 1.0 : b[j, k]) * b[k, i];
                    b[j, i] = sum;
                }
            return b;
        }

        public static double Determinant(double[,] a)
        {
            if (a == null) throw new Exception("The matrix, A, is null.");
            var length = a.GetLength(0);
            if (length != a.GetLength(1))
                throw new Exception("The determinant is only possible for square matrices.");
            if (length == 0) return 0.0;
            if (length == 1) return a[0, 0];
            if (length == 2) return (a[0, 0] * a[1, 1]) - (a[0, 1] * a[1, 0]);
            if (length == 3)
                return (a[0, 0] * a[1, 1] * a[2, 2])
                       + (a[0, 1] * a[1, 2] * a[2, 0])
                       + (a[0, 2] * a[1, 0] * a[2, 1])
                       - (a[0, 0] * a[1, 2] * a[2, 1])
                       - (a[0, 1] * a[1, 0] * a[2, 2])
                       - (a[0, 2] * a[1, 1] * a[2, 0]);
            return DeterminantBig(a, length);
        }

        public static double DeterminantDestructive(double[,] a, int dimension)
        {
            if (a == null) throw new Exception("The matrix, A, is null.");
            var length = dimension;
            //if (length != A.GetLength(1))
            //    throw new Exception("The determinant is only possible for square matrices.");
            if (length == 0) return 0.0;
            if (length == 1) return a[0, 0];
            if (length == 2) return (a[0, 0] * a[1, 1]) - (a[0, 1] * a[1, 0]);
            if (length == 3)
                return (a[0, 0] * a[1, 1] * a[2, 2])
                       + (a[0, 1] * a[1, 2] * a[2, 0])
                       + (a[0, 2] * a[1, 0] * a[2, 1])
                       - (a[0, 0] * a[1, 2] * a[2, 1])
                       - (a[0, 1] * a[1, 0] * a[2, 2])
                       - (a[0, 2] * a[1, 1] * a[2, 0]);
            return DeterminantBigDestructive(a, length);
        }

        static double DeterminantBigDestructive(double[,] a, int length)
        {
            LUDecompositionInPlace(a, length);
            var result = 1.0;
            for (var i = 0; i < length; i++)
                if (double.IsNaN(a[i, i]))
                    return 0;
                else result *= a[i, i];
            return result;
        }

        public static double DeterminantBig(double[,] a, int length)
        {
            double[,] l, u;
            LUDecomposition(a, out l, out u, length);
            var result = 1.0;
            for (var i = 0; i < length; i++)
                if (double.IsNaN(l[i, i]))
                    return 0;
                else result *= l[i, i];
            return result;
        }

        public static void LUDecomposition(double[,] a, out double[,] l, out double[,] u, int length)
        {
            l = LUDecomposition(a, length);
            u = new double[length, length];
            for (var i = 0; i < length; i++)
            {
                u[i, i] = 1.0;
                for (var j = i + 1; j < length; j++)
                {
                    u[i, j] = l[i, j];
                    l[i, j] = 0.0;
                }
            }
        }

        public static double Norm2(double[] x, int dim, Boolean dontDoSqrt = false)
        {
            double norm = 0;
            for (int i = 0; i < dim; i++)
            {
                var t = x[i];
                norm += t * t;
            }
            return dontDoSqrt ? norm : Math.Sqrt(norm);
        }

        public static double[] Normalize(double[] x, int length)
        {
            return Divide(x, Norm2(x, length), length);
        }

        public static void NormalizeInPlace(double[] x, int length)
        {
            double f = 1.0 / Norm2(x, length);
            for (int i = 0; i < length; i++) x[i] *= f;
        }

        public static double[] GetRow(int rowIndex, double[,] a)
        {
            var numRows = a.GetLength(0);
            var numCols = a.GetLength(1);
            if ((rowIndex < 0) || (rowIndex >= numRows))
                throw new Exception("MatrixMath Size Error: An index value of "
                                    + rowIndex
                                    + " for getRow is not in required range from 0 up to (but not including) "
                                    + numRows + ".");
            var v = new double[numCols];
            for (var i = 0; i < numCols; i++)
                v[i] = a[rowIndex, i];
            return v;
        }

        public static void SetColumn(int colIndex, double[,] a, IList<double> v)
        {
            var numRows = a.GetLength(0);
            var numCols = a.GetLength(1);
            if ((colIndex < 0) || (colIndex >= numCols))
                throw new Exception("MatrixMath Size Error: An index value of "
                                    + colIndex
                                    + " for getColumn is not in required range from 0 up to (but not including) "
                                    + numCols + ".");
            for (var i = 0; i < numRows; i++)
                a[i, colIndex] = v[i];
        }

        public static void SetRow(int rowIndex, double[,] a, IList<double> v)
        {
            var numRows = a.GetLength(0);
            var numCols = a.GetLength(1);
            if ((rowIndex < 0) || (rowIndex >= numRows))
                throw new Exception("MatrixMath Size Error: An index value of "
                                    + rowIndex
                                    + " for getRow is not in required range from 0 up to (but not including) "
                                    + numRows + ".");
            for (var i = 0; i < numCols; i++)
                a[rowIndex, i] = v[i];
        }

        public static double[] MakeZeroVector(int p)
        {
            if (p <= 0) throw new Exception("The size, p, must be a positive integer.");
            return new double[p];
        }

        public static double CrossProduct2(IList<double> a, IList<double> b)
        {
            if (((a.Count() == 2) && (b.Count() == 2))
                || ((a.Count() == 3) && (b.Count() == 3) && a[2] == 0.0 && b[2] == 0.0))
                return a[0] * b[1] - b[0] * a[1];
            throw new Exception("This cross product \"shortcut\" is only used with 2D vectors to get the single value in the,"
                                + "would be, Z-direction.");
        }


        public static double DotProduct(IList<int> a, IList<double> b, int length)
        {
            var c = 0.0;
            for (var i = 0; i != length; i++)
                c += a[i] * b[i];
            return c;
        }

        public static double[] Subtract(IList<double> a, IList<double> b, int length)
        {
            var c = new double[length];
            for (var i = 0; i != length; i++)
                c[i] = a[i] - b[i];
            return c;
        }

        public static double[] Add(IList<double> a, IList<double> b, int length)
        {
            var c = new double[length];
            for (var i = 0; i != length; i++)
                c[i] = a[i] + b[i];
            return c;
        }

        public static double[] Divide(IList<double> b, double a, int length)
        {
            return multiply((1 / a), b, length);
        }

        public static double[] multiply(double a, IList<double> b, int length)
        {
            // scale vector B by the amount of scalar a
            var c = new double[length];
            for (var i = 0; i != length; i++)
                c[i] = a * b[i];
            return c;
        }

        public static double[] multiply(double[,] a, IList<double> b, int numRows, int numCols)
        {
            var C = new double[numRows];

            for (var i = 0; i != numRows; i++)
            {
                C[i] = 0.0;
                for (var j = 0; j != numCols; j++)
                    C[i] += a[i, j] * b[j];
            }
            return C;
        }

        public static void MultiplyInto(double[,] a, double[] b, int numRows, int numCols, double[] target)
        {
            var c = target;

            for (var i = 0; i != numRows; i++)
            {
                c[i] = 0.0;
                for (var j = 0; j != numCols; j++)
                    c[i] += a[i, j] * b[j];
            }
        }

        public static double[] CrossProduct3(IList<double> a, IList<double> b)
        {
            return new[]
                       {
                           a[1]*b[2] - b[1]*a[2],
                           a[2]*b[0] - b[2]*a[0],
                           a[0]*b[1] - b[0]*a[1]
                       };
        }

        public static double SubtractAndDot(double[] n, double[] l, double[] r, int dim)
        {
            double acc = 0;
            for (int i = 0; i < dim; i++)
            {
                double t = l[i] - r[i];
                acc += n[i] * t;
            }

            return acc;
        }
    }
}