using GeoCalculations.MethodPoints;
using GeoCalculations.Points;
using GeoCalculations.Protocol;
using GeoHelper.Calculations.Controls;

namespace GeoHelper.Protocols
{
    [ProtocolRootData("ConstructionDistanceMethod")]
    public class PolygonTraverseProtocolContext : ProtocolContextBase<CalculatedPointBase>
    {
        public PointPolygonTraversUserControl BeginPoint { get; set; }
        public PointPolygonTraversUserControl EndPoint { get; set; }
        [ProtocolPropertyDataAttribute("MeasuringData")]
        public MeasuredPolygonTraversUserControl MeasuringData { get; set; }
        [ProtocolPropertyDataAttribute("CalculatedPointBase")]
        public PolygonCalculatedPoints PolygonCalculatedPoint { get; set; }
    }
}