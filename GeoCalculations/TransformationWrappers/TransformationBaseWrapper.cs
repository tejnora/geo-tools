using System;
using System.Runtime.InteropServices;

namespace GeoCalculations.TransformationWrappers
{
    public abstract class TransformationBaseWrapper : IDisposable
    {
        protected IntPtr Handle = IntPtr.Zero;
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern void Transformation_Release(IntPtr handle);
        public void Dispose()
        {
            if (Handle == IntPtr.Zero) return;
            Transformation_Release(Handle);
        }

        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern bool Transformation_Solved(IntPtr handle);
        public bool Solved()
        {
            return Transformation_Solved(Handle);
        }

        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern void Transformation_Solve(IntPtr handle);
        public void Solve()
        {
            Transformation_Solve(Handle);
        }

        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern IntPtr Transformation_TransformPoints(IntPtr handle, IntPtr localPoints);
        public MatrixWrapper TransformLocalPoints(MatrixWrapper localPoints)
        {
            return new MatrixWrapper(Transformation_TransformPoints(Handle, localPoints.GetHandle));
        }

        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern IntPtr Transformation_GetReport(IntPtr handle);
        public TransformationReportWrapper GetReport()
        {
            return new TransformationReportWrapper(Transformation_GetReport(Handle));
        }

    }
}
