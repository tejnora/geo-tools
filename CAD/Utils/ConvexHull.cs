using System.Collections.Generic;
using System.Linq;

namespace CAD.Utils
{
    interface IPointConvexHull
    {
        double X { get; }
        double Y { get; }
    }
    class ConvexHull<T> where T : IPointConvexHull
    {
        public static double Cross(T O, T A, T B)
        {
            return (A.X - O.X) * (B.Y - O.Y) - (A.Y - O.Y) * (B.X - O.X);
        }

        public static List<T> GetConvexHull(List<T> points)
        {
            if (points == null)
                return null;

            if (points.Count() <= 1)
                return points;

            int n = points.Count(), k = 0;
            var H = new List<T>(new T[2 * n]);

            points.Sort((a, b) => a.X == b.X ? a.Y.CompareTo(b.Y) : a.X.CompareTo(b.X));

            // Build lower hull
            for (var i = 0; i < n; ++i)
            {
                while (k >= 2 && Cross(H[k - 2], H[k - 1], points[i]) <= 0)
                    k--;
                H[k++] = points[i];
            }

            // Build upper hull
            for (int i = n - 2, t = k + 1; i >= 0; i--)
            {
                while (k >= t && Cross(H[k - 2], H[k - 1], points[i]) <= 0)
                    k--;
                H[k++] = points[i];
            }
            return H.Take(k - 1).ToList();
        }
    }
}
