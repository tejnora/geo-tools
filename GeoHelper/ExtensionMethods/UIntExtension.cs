namespace GeoHelper.ExtensionMethods
{
    internal static class UIntExtension
    {
        public static string ToPredcisliBodu(this uint value)
        {
            return string.Format("{0:00000000}", value);
        }

        public static string ToCisloBodu(this uint value)
        {
            return string.Format("{0:0000}", value);
        }
    }
}