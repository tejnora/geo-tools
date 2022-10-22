using System.Globalization;
using GeoBase.Utils;

namespace GeoCalculations.Protocol
{
    public class ProtocolUnitsPlugin : IProtocolPlugin
    {
        readonly Registry _registry;
        public ProtocolUnitsPlugin(Registry registry)
        {
            _registry = registry;
        }

        public string CoordinateToString(double value)
        {
            var precision = _registry.getEntry(Registry.SubKey.kCurrentUser, "VstypVystupSouradnicePocetDesMist").getInt(2);
            return DoubleToString(precision, value);
        }

        public string HightToString(double value)
        {
            var precision = _registry.getEntry(Registry.SubKey.kCurrentUser, "VstypVystupVyskyPocetDesMist").getInt(2);
            return DoubleToString(precision, value);
        }

        public string DistanceToString(double value)
        {
            var precision = _registry.getEntry(Registry.SubKey.kCurrentUser, "VstypVystupVyskyPocetDesMist").getInt(2);
            return DoubleToString(precision, value);
        }

        public string ScaleToString(double value)
        {
            var precision = _registry.getEntry(Registry.SubKey.kCurrentUser, "VstypVystupSouradnicePocetDesMist").getInt(2);
            return DoubleToString(precision, value);
        }

        public string AngleToString(double value)
        {
            var precision = _registry.getEntry(Registry.SubKey.kCurrentUser, "VstypVystupVyskyPocetDesMist").getInt(2);
            return DoubleToString(precision, value);
        }

        public string AreaToString(double value)
        {
            var precision = _registry.getEntry(Registry.SubKey.kCurrentUser, "VstypVystupPlochaPocetDesMist").getInt(2);
            return DoubleToString(precision, value);
        }

        public static string DoubleToString(int precision, double value)
        {
            var format = "".PadRight(precision, '0');
            return string.Format(CultureInfo.InvariantCulture, "{0:0." + format + "}", value);
        }
    }
}
