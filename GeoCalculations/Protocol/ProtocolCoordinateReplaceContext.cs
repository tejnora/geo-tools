using GeoCalculations.MethodPoints;

namespace GeoCalculations.Protocol
{
    public class ProtocolCoordinateReplaceContext
    {
        public ProtocolCoordinateReplaceContext()
        {
            OldCoordinates = new PointBase();
            NewCoordinates = new PointBase();
            SavedCoordinates = new PointBase();
        }

        [ProtocolPropertyData("OldCoordinates")]
        public PointBase OldCoordinates { get; set; }

        [ProtocolPropertyData("NewCoordinates")]
        public PointBase NewCoordinates { get; set; }

        [ProtocolPropertyData("CoordinateDifferenceY"), ProtocolPropertyValueTypeAttribute(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double CoordinateDifferenceY
        {
            get { return OldCoordinates.Y - NewCoordinates.Y; }
        }

        [ProtocolPropertyData("CoordinateDifferenceX"), ProtocolPropertyValueTypeAttribute(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double CoordinateDifferenceX
        {
            get { return OldCoordinates.X - NewCoordinates.X; }
        }

        [ProtocolPropertyData("CoordinateDifferenceZ"), ProtocolPropertyValueTypeAttribute(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double CoordinateDifferenceZ
        {
            get { return OldCoordinates.Z - NewCoordinates.Z; }
        }

        [ProtocolPropertyData("SavedCoordinates")]
        public PointBase SavedCoordinates { get; set; }

        [ProtocolPropertyData("WasPointRenamed")]
        public bool WasPointRenamed { get; set; }
    }
}
