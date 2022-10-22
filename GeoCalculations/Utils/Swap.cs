using System.Collections.Generic;

namespace GeoCalculations.Utils
{
    static class Swap
    {
        public static void Do<T>(ref T x, ref T y)
        {
            T t = y;
            y = x;
            x = t;
        }

        public static void Do<T>(List<T> list, int i1, int i2)
        {
            T t = list[i1];
            list[i1] = list[i2];
            list[i2] = t;
        }
    }
}
