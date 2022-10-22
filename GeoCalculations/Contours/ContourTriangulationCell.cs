using GeoCalculations.ConvexHull;
using GeoCalculations.Triangulation;

namespace GeoCalculations.Contours
{
    public class ContourTriangulationCell<TVertex> : TriangulationCell<TVertex, ContourTriangulationCell<TVertex>>
        where TVertex : IVertex
    {
    }
}
