using System;
using System.Runtime.InteropServices;

namespace GeoCalculations.TransformationWrappers
{
    public class Affine2DTransformationWrapper : TransformationBaseWrapper
    {
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern IntPtr Transformation_Affine2D_Create(IntPtr globalPoints, IntPtr localPoints);
        public Affine2DTransformationWrapper(MatrixWrapper globalPoints, MatrixWrapper localPoints)
        {
            Handle = Transformation_Affine2D_Create(globalPoints.GetHandle, localPoints.GetHandle);
        }

    }
}
