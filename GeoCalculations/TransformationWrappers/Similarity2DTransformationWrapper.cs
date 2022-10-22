using System;
using System.Runtime.InteropServices;

namespace GeoCalculations.TransformationWrappers
{
    public class Similarity2DTransformationWrapper : TransformationBaseWrapper
    {
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern IntPtr Transformation_Similarity2D_Create(IntPtr globalPoints, IntPtr localPoints);
        public Similarity2DTransformationWrapper(MatrixWrapper globalPoints, MatrixWrapper localPoints)
        {
            Handle = Transformation_Similarity2D_Create(globalPoints.GetHandle, localPoints.GetHandle);
        }

    }
}
