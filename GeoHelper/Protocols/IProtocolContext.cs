using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols
{
    public interface IProtocolContext
    {
        void Clear();
        void AddCalculatedPoint(CalculatedPointBase point);
        void AddPointConversion(string numberWithPrefix, ProtocolCoordinateReplaceContext coordinateReplaceContext);
    }
}
