using System;

namespace GeoHelper.FileParses
{
    public interface IRecord
    {
    }

    public interface ISouradnice
    {
        string PointNumber { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
    }

    public interface IStationRecord
    {
        string StationPointNumber { get; set; }

        Double HeightOfInstrument { get; set; }
    }

    public interface INodeSideshot
    {
        string PointNumber { get; set; }
        double Hz { get; set; }
        double Z { get; set; }
        double VerticalAngle { get; set; }
        double SlopeDistance { get; set; }
        double HeightOfTarget { get; set; }
    }
}