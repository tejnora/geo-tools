using System.Diagnostics;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Deviations;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols
{
    [ProtocolRootData("FreePointOfViewMethod")]
    public class FreePointOfViewProtocolContext : ProtocolContextBase<CalculatedPointBase>
    {
        [ProtocolPropertyDataAttribute("Orientation")]
        public OrientationContext Orientation { get; set; }
        [ProtocolPropertyDataAttribute("PointOfView")]
        public PointBaseEx PointOfView { get; set; }
        [ProtocolPropertyDataAttribute("Deviations")]
        public CalculationDeviation Deviations { get { return Orientation.Deviations; } }
        [ProtocolPropertyDataAttribute("IsNotFromBatch")]
        public bool IsNotFromBatch { get; set; }

        ProtocolCoordinateReplaceContext _conversionContext;
        public override void AddCalculatedPoint(CalculatedPointBase point)
        {
            _conversionContext = null;
        }

        public override void AddPointConversion(string numberWithPrefix, ProtocolCoordinateReplaceContext coordinateReplaceContext)
        {
            Debug.Assert(PointOfView.NumberWithPrefix == numberWithPrefix);
            _conversionContext = coordinateReplaceContext;
        }

        public override string GetConversionString(string array, int arrayIndex)
        {
            if (_conversionContext == null) return "";
            var converstionString = EvaluateProtocolTemplate("CoordinateCorrection", _conversionContext);
            return converstionString;
        }
    }
}