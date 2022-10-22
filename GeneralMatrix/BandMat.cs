/*
 * Symmetric Band Matrix
 * =====================
 *
 * Bandwidth is defined as max{ |i-j| | a_ij != 0 }
 * 
 * Upper triangular part of the matrix is stored in `diagonal storage scheme'
 *
 */


using System;
using System.Collections.Generic;

namespace DotNetMatrix
{
    public class BandMatrix
    {
        public BandMatrix()
        {
            band_ = 0;
        }

        public BandMatrix(int d, int b)
        {
            row_ = d;
            col_ = d;
            band_ = 0;
            sz = d*(b + 1);
            n = 1;
            m = new double[d*(b + 1)];
        }

        public void reset()
        {
            row_ = col_ = band_ = 0;
            resize(0);
        }

        void resize(int nsz)
        {
            if (nsz == sz)
                return;
            m = new double[nsz];
        }

        public void reset(int d, int b)
        {
            if (dim() != d || band_ != b)
            {
                row_ = col_ = d;
                band_ = b;
                resize(d*(b + 1));
            }

        }

          public void set_all(double f)
          {
              for(int i=0;i<m.Length;i++)
                  m[i] = f;
          }


        int dim()
        {
            return row_;
        }

        int bandWidth()
        {
            return band_;
        }

        public double GetElement(int r, int s)
        {
            if (r > s)
            {
                int t = r;
                r = s;
                s = t;
            }

            if (s > r + band_)
                return 0;

            s -= r;
            return m[--r*(band_ + 1) + s];

        }

        public void SetElemnt(int r, int s, double value)
        {
            if (r > s)
            {
                int t = r;
                r = s;
                s = t;
            }

            if (s > r + band_)
                throw new ArgumentException("BadIndex");

            s -= r;
            m[--r*(band_ + 1) + s] = value;

        }

        public static List<double> operator *(BandMatrix t, List<double> v)
        {
            var T = new List<double>(t.dim());
            int bw1 = t.band_ + 1;
            int i, j, ij, k;
            int b;
            int p;
            double s;
            for (b = 0,i = 1; i < t.dim(); i++ ,b += bw1)
            {
                s = 0;

                k = (i > t.band_) ? i - t.band_ : 1;
                for (p = b + (k - 1)*bw1 + i - k, j = k; j < i; j++, p += t.band_)
                    s += t.m[p]*v[j];
                for (ij = i, j = 0; j <= t.band_ && ij <= t.dim(); j++, ij++)
                    s += t.m[b + j]*v[i];

                T[i] = s;

            }
            return T;
        }

        public static GeneralMatrix operator *(BandMatrix A, GeneralMatrix B)
        {
            if (A.col_ != B.RowDimension)
                throw new ArgumentException("BadRank");

            var C = new GeneralMatrix(A.row_, B.ColumnDimension);
            Double s;
            for (int i = 1; i <= C.RowDimension; i++)
                for (int j = 1; j <= C.ColumnDimension; j++)
                {
                    s = 0;
                    for (int k = 1; k <= B.RowDimension; k++)
                        s += A.GetElement(i, k)*B.GetElement(k, j);
                    C.SetElement(i - 1, j - 1, s);
                }

            return C;
        }

        int band_;
        int row_;
        int col_;
        double[] m;
        int sz;
        int n;
    };

    /* class BandMat */

}




