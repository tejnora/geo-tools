using System;
using System.Runtime.InteropServices;

namespace GeoCalculations.TransformationWrappers
{
    public enum UsedSolutionAlgorithms
    {
        GaussJordanElimination = 0,
        Svd = 1
    }
    public class TransformationReportWrapper
    {
        public double[] Keys { get; private set; }
        public int NumberOfIdenticalPoints { get; private set; }
        public double StandardDeviation { get; private set; }//"Standard deviation a posteriori is (sqrt([vv]/number_of_redundant_points)): "
        public double StandardDeviationInCoordinates { get; private set; }//"Standard deviation a posteriori in coordinates is (sqrt([vv]/number_of_redundant_coordinates)): "
        public int NumberOfIterations { get; private set; }
        public bool Success { get; private set; }
        public UsedSolutionAlgorithms UsedSolutionAlgorithm { get; private set; }
        public bool Nullity { get; private set; }//"Coefficient matrix of project equations is ill-conditioned or singular, the number of matrix defect is:nullity "
        public double SumOfNormalEquationsAbsoluteResiduals { get; private set; }//"The sum of normal equations absolute residuals: "
        public int ConditionNumber { get; private set; }
        public double[][] Residuals { get; private set; }


        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern int Report_NumberOfIdenticalPoints(IntPtr handle);
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern double Report_StandardDeviation(IntPtr handle);
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern double Report_StandardDeviationInCoordinates(IntPtr handle);
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern int Report_NumberOfIterations(IntPtr handle);
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern bool Report_Success(IntPtr handle);
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern int Report_UsedSolutionAlgorithm(IntPtr handle);
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern bool Report_Nullity(IntPtr handle);
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern double Report_SumOfNormalEquationsAbsoluteResiduals(IntPtr handle);
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern int Report_ConditionNumber(IntPtr handle);
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern int Report_Release(IntPtr handle);

        public TransformationReportWrapper(IntPtr handle)
        {
            Keys = GetKey(handle);
            Residuals = GetResiduals(handle);
            NumberOfIdenticalPoints = Report_NumberOfIdenticalPoints(handle);
            StandardDeviation = Report_StandardDeviation(handle);
            StandardDeviationInCoordinates = Report_StandardDeviationInCoordinates(handle);
            NumberOfIterations = Report_NumberOfIterations(handle);
            Success = Report_Success(handle);
            UsedSolutionAlgorithm = (UsedSolutionAlgorithms)Report_UsedSolutionAlgorithm(handle);
            Nullity = Report_Nullity(handle);
            SumOfNormalEquationsAbsoluteResiduals = Report_SumOfNormalEquationsAbsoluteResiduals(handle);
            ConditionNumber = Report_ConditionNumber(handle);
            Report_Release(handle);
        }


        [DllImport(Settings.DllPath)]
        public static extern IntPtr Report_Keys(IntPtr handle);
        static double[] GetKey(IntPtr handle)
        {
            var srcKey = Report_Keys(handle);
            var destKey = new double[8];
            Marshal.Copy(srcKey, destKey, 0, 8);
            return destKey;
        }

        [DllImport(Settings.DllPath)]
        public static extern int Report_Residuals_Size(IntPtr handle);
        [DllImport(Settings.DllPath)]
        public static extern IntPtr Report_Residuals_Data(IntPtr handle);

        static double[][] GetResiduals(IntPtr handle)
        {
            var size = Report_Residuals_Size(handle);
            var data = Report_Residuals_Data(handle);
            var dataInArray = new double[size];
            Marshal.Copy(data, dataInArray, 0, size);
            var result = new double[size / 3][];
            for (var i = 0; i < result.Length; i++)
            {
                var deltaIdx = i * 3;
                result[i] = new[] { dataInArray[deltaIdx], dataInArray[deltaIdx + 1], dataInArray[deltaIdx + 2] };
            }
            return result;
        }

    }
}
