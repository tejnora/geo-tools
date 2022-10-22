using System.Runtime.InteropServices;

namespace GeoCalculations.TransformationWrappers
{
    class Settings
    {
#if DEBUG
        public const string DllPath = @"c:\GeoTools\GeoCalculations\bin\Debug\Transformation.dll";
#else
        public const string DllPath = @"Transformation.dll";
#endif
        public const CallingConvention CallingConventionValue = CallingConvention.StdCall;
    }
}
