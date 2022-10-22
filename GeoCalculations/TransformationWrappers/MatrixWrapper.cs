using System;
using System.Runtime.InteropServices;
using DotNetMatrix;

namespace GeoCalculations.TransformationWrappers
{
    public class MatrixWrapper : IDisposable
    {
        readonly IntPtr _handle = IntPtr.Zero;

        public IntPtr GetHandle { get { return _handle; } }
        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern IntPtr Matrix_Create(int columns, int rows);

        readonly int _columns, _rows;

        public MatrixWrapper(IntPtr handle)
        {
            _handle = handle;
            _columns = GetColumns();
            _rows = GetRows();
        }

        public MatrixWrapper(int columns, int rows)
        {
            _columns = columns;
            _rows = rows;
            _handle = Matrix_Create(columns, rows);
        }

        public MatrixWrapper(GeneralMatrix matrix)
            : this(matrix.ColumnDimension, matrix.RowDimension)
        {
            for (var r = 0; r < matrix.RowDimension; r++)
            {
                for (var c = 0; c < matrix.ColumnDimension; c++)
                {
                    SetElement(r, c, matrix.GetElement(r, c));
                }
            }
        }

        [DllImport(Settings.DllPath, CharSet = CharSet.Auto, CallingConvention = Settings.CallingConventionValue)]
        public static extern void Matrix_Release(IntPtr handle);
        public void Dispose()
        {
            if (_handle == IntPtr.Zero) return;
            Matrix_Release(_handle);
        }

        [DllImport(Settings.DllPath, CharSet = CharSet.Auto, CallingConvention = Settings.CallingConventionValue)]
        public static extern void Matrix_Set(IntPtr handle, int rows, int columns, double value);
        public void SetElement(int rows, int columns, double value)
        {
            if (rows < 0 || rows > _rows || columns < 0 || columns > _columns)
                throw new ArgumentException("Invalid argument values.");
            Matrix_Set(_handle, rows, columns, value);
        }

        [DllImport(Settings.DllPath, CharSet = CharSet.Auto, CallingConvention = Settings.CallingConventionValue)]
        public static extern double Matrix_Get(IntPtr handle, int rows, int columns);
        public double GetElement(int rows, int columns)
        {
            if (rows > _rows || columns > _columns)
                throw new ArgumentException("Invalid argument values.");
            return Matrix_Get(_handle, rows, columns);
        }

        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern int Matrix_Columns(IntPtr handle);
        public int GetColumns()
        {
            return Matrix_Columns(_handle);
        }

        [DllImport(Settings.DllPath, CallingConvention = Settings.CallingConventionValue)]
        public static extern int Matrix_Rows(IntPtr handle);
        public int GetRows()
        {
            return Matrix_Rows(_handle);
        }

        public GeneralMatrix ConvertToGeneralMatrix()
        {
            var gm = new GeneralMatrix(_rows, _columns);
            for (var i = 0; i < _rows; i++)
            {
                for (var j = 0; j < _columns; j++)
                {
                    gm.SetElement(i, j, GetElement(i, j));
                }
            }
            return gm;
        }
    }
}
