using System;
using System.Runtime.InteropServices;

namespace GeoCalculations.TransformationWrappers
{
    public class Identity2DTransformationWrapper : TransformationBaseWrapper
    {
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern IntPtr Transformation_Identity2D_Create(IntPtr globalPoints, IntPtr localPoints);
        public Identity2DTransformationWrapper(MatrixWrapper globalPoints, MatrixWrapper localPoints)
        {
            Handle = Transformation_Identity2D_Create(globalPoints.GetHandle, localPoints.GetHandle);
        }
    }
}
