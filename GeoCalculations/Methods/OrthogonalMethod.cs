using System;
using DotNetMatrix;
using GeoCalculations.BasicTypes;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Deviations;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.Methods
{
    public static class OrthogonalMethod
    {
        static public void Calculate(OrtogonalContext ortogonalContext)
        {
            if (ortogonalContext.TableNodes.Count < 2)
                throw new OrtogonalMethodCalculationException("E23");
            var pointCount = ortogonalContext.TableNodes.Count;
            var a = new GeneralMatrix(pointCount * 2, 4);
            var b = new GeneralMatrix(pointCount * 2, 1);
            var i = 0;
            foreach (var node in ortogonalContext.TableNodes)
            {
                a.SetElement(i, 0, 1);
                a.SetElement(i, 1, 0);
                a.SetElement(i, 2, node.Stationing);
                a.SetElement(i, 3, -node.Vertical);

                a.SetElement(i + 1, 0, 0);
                a.SetElement(i + 1, 1, 1);
                a.SetElement(i + 1, 2, node.Vertical);
                a.SetElement(i + 1, 3, node.Stationing);

                b.SetElement(i, 0, node.X);
                b.SetElement(i + 1, 0, node.Y);
                i += 2;
            }
            try
            {
                var at = a.Transpose();
                ortogonalContext.Matrix = (at * a).Inverse() * at * b;
            }
            catch
            {
                throw new OrtogonalMethodCalculationException("E24");
            }
            //w = arctg(d / c) -> rotation
            ortogonalContext.Rotation = Math.Atan(ortogonalContext.Matrix.GetElement(3, 0) / ortogonalContext.Matrix.GetElement(2, 0)) * 200 / Math.PI + 200;
            //m = c / cos w -> scale  
            ortogonalContext.Scale = ortogonalContext.Matrix.GetElement(2, 0) / Math.Cos(ortogonalContext.Rotation * Math.PI / 200);
            if (ortogonalContext.TableNodes.Count > 2)
            {
                var rezidua = b - a * ortogonalContext.Matrix;
                i = 0;
                foreach (var node in ortogonalContext.TableNodes)
                {
                    node.dX = rezidua.GetElement(i, 0);
                    node.dY = rezidua.GetElement(i + 1, 0);
                    i += 2;
                }
            }
            ortogonalContext.MeasuringLength = 0;
            ortogonalContext.LengthFromCoordinates = 0;
            for (i = 1; i < ortogonalContext.TableNodes.Count; i++)
            {
                var distance = SimpleCalculation.CalculateDistance(new Point { X = ortogonalContext.TableNodes[i - 1].X, Y = ortogonalContext.TableNodes[i - 1].Y },
                                   new Point { X = ortogonalContext.TableNodes[i].X, Y = ortogonalContext.TableNodes[i].Y });
                if (i == 1)
                {
                    ortogonalContext.StationingCorrection = distance - ortogonalContext.TableNodes[i].Stationing;
                }
                ortogonalContext.MeasuringLength += ortogonalContext.TableNodes[i].Stationing;
                ortogonalContext.LengthFromCoordinates += distance;
            }
            ortogonalContext.Deviations.Deviations.Add(new OrthogonalMethodDeviation(ortogonalContext, OrthogonalMethodDeviation.Types.Length));
            ortogonalContext.Deviations.Deviations.Add(new OrthogonalMethodDeviation(ortogonalContext, OrthogonalMethodDeviation.Types.LengthDiffirence));
        }

        static public void CalculatePoint(OrtogonalContext ortogonalContext, CalculatedPointBase calculatedPoint)
        {
            //X = b + d . x + c . y
            //Y = a + c . x - d . y
            if (calculatedPoint.Number.Length == 0) return;
            if (double.IsNaN(calculatedPoint.Stationing) || double.IsNaN(calculatedPoint.Vertical))
                throw new OrtogonalMethodCalculationException("E25");
            calculatedPoint.X = ortogonalContext.Matrix.GetElement(0, 0) + ortogonalContext.Matrix.GetElement(2, 0) * calculatedPoint.Stationing -
                                  ortogonalContext.Matrix.GetElement(3, 0) * calculatedPoint.Vertical;
            calculatedPoint.Y = ortogonalContext.Matrix.GetElement(1, 0) + ortogonalContext.Matrix.GetElement(3, 0) * calculatedPoint.Stationing +
                                  ortogonalContext.Matrix.GetElement(2, 0) * calculatedPoint.Vertical;
        }
    }
}
