using System;

namespace GeoCalculations.MethodPoints
{
    [Flags]
    public enum LoadAdapterProperties : uint
    {
        CordinateData = 0x01,
        MeasureData = 0x02
    }

    public interface IPointLoadAdapter
    {
        LoadAdapterProperties Properties { get; }
        string Prefix { get; }
        string Number { get; }
        double X { get; }
        double Y { get; }
        double Z { get; }
        double Hz { get; }
        double HorizontalDistance { get; }
        double ElevationDifference { get; }
        double Signal { get; }
        double ZenitAgnel { get; }
        string Description { get; }
    }
}
