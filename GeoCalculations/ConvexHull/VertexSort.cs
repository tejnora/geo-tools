using System.Collections.Generic;
using System.Linq;

namespace GeoCalculations.ConvexHull
{
    internal class VertexSort : IComparer<VertexWrap>
    {
        readonly int _dimension;

        public VertexSort(int dimension)
        {
            _dimension = dimension;
            Duplicates = new List<VertexWrap>();
        }

        public List<VertexWrap> Duplicates { get; private set; }

        public int Compare(VertexWrap x, VertexWrap y)
        {
            if (x == y) return 0;
            for (int i = 0; i < _dimension; i++)
            {
                if (x.PositionData[i] < y.PositionData[i]) return -1;
                if (x.PositionData[i] > y.PositionData[i]) return 1;
            }
            if (IsANewDuplicate(x)) Duplicates.Add(x);
            return 0;
        }


        bool IsANewDuplicate(VertexWrap x)
        {
            return Duplicates.All(t => !Constants.SamePosition(x.PositionData, t.PositionData, _dimension));
        }
    }
}