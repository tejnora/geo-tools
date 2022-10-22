using System;
using DotNetMatrix;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.Methods
{
    public static class FreePointOfViewMethod
    {
        static public void Calculate(OrientationContext orientations)
        {
            var pointCount = 0;
            foreach (var node in orientations.TableNodes)
            {
                if (!node.IsEnabled)
                    continue;
                if (double.IsNaN(node.Hz) || double.IsNaN(node.Distance))
                    throw new FreePointOfViewCalculationException("E19");
                node.Temp1 = node.Distance * Math.Sin(node.Hz * Math.PI / 200);
                node.Temp2 = node.Distance * Math.Cos(node.Hz * Math.PI / 200);
                pointCount++;
            }
            if (pointCount < 2)
                throw new FreePointOfViewCalculationException("E17");

            var a = new GeneralMatrix(pointCount * 2, 4);
            var b = new GeneralMatrix(pointCount * 2, 1);
            var i = 0;
            foreach (var node in orientations.TableNodes)
            {
                if (!node.IsEnabled)
                    continue;

                a.SetElement(i, 0, 1);
                a.SetElement(i, 1, 0);
                a.SetElement(i, 2, node.Temp2);
                a.SetElement(i, 3, -node.Temp1);

                a.SetElement(i + 1, 0, 0);
                a.SetElement(i + 1, 1, 1);
                a.SetElement(i + 1, 2, node.Temp1);
                a.SetElement(i + 1, 3, node.Temp2);

                b.SetElement(i, 0, node.X);
                b.SetElement(i + 1, 0, node.Y);
                i += 2;
            }
            try
            {
                var at = a.Transpose();
                orientations.Matrix = (at * a).Inverse() * at * b;
            }
            catch (Exception)
            {
                throw new FreePointOfViewCalculationException("E18");
            }
            //w = arctg(d / c) -> rotation
            orientations.Rotation = Math.Atan(orientations.Matrix.GetElement(3, 0) / orientations.Matrix.GetElement(2, 0)) * 200 / Math.PI + 200;
            //m = c / cos w -> scale 
            orientations.Scale = orientations.Matrix.GetElement(2, 0) / Math.Cos(orientations.Rotation * Math.PI / 200);
            var rezidua = b - a * orientations.Matrix;
            i = 0;
            foreach (var node in orientations.TableNodes)
            {
                if (!node.IsEnabled) continue;
                node.vX = rezidua.GetElement(i, 0);
                node.vY = rezidua.GetElement(i + 1, 0);
                i += 2;
            }

            orientations.Y = 0;
            orientations.X = 0;
            foreach (var node in orientations.TableNodes)
            {
                if (!node.IsEnabled) continue;
                orientations.Y += Math.Pow(node.vY, 2);
                orientations.X += Math.Pow(node.vX, 2);
            }
            orientations.Y /= pointCount - 1;
            orientations.X /= pointCount - 1;
            orientations.Y = Math.Sqrt(orientations.Y);
            orientations.X = Math.Sqrt(orientations.X);
        }

        public static void CalculatePoint(OrientationContext orientations, CalculatedPointBase pointOfView)
        {
            pointOfView.X = orientations.Matrix.GetElement(0, 0);
            pointOfView.Y = orientations.Matrix.GetElement(1, 0);
            SimpleCalculation.CalculateOrientationMovement(pointOfView, orientations);
            orientations.CalculateHeightsAlso = !double.IsNaN(pointOfView.SignalHeight);
            if (!orientations.CalculateHeightsAlso) return;
            var numerator = 0.0;
            var denominator = 0.0;
            foreach (var node in orientations.TableNodes)
            {
                if (double.IsNaN(node.SignalHeight) || double.IsNaN(node.Distance)) continue;
                if (!double.IsNaN(node.ZenitAngle))
                    node.dhCalculated = SimpleCalculation.CalculateHeightByDistanceAndAngle(node.ZenitAngle, node.Distance + node.VerticalDistance);
                else if (!double.IsNaN(node.ElevationDifference))
                    node.dhCalculated = node.ElevationDifference;
                else
                    continue;
                node.dhCalculated -= node.SignalHeight;
                node.ElevationDifferenceScale = SimpleCalculation.ApplyVerticalDistanceScale(node.Distance + node.VerticalDistance);
                node.Zp = node.Z - node.dhCalculated - pointOfView.SignalHeight;
                numerator += node.ElevationDifferenceScale * node.Zp;
                denominator += node.ElevationDifferenceScale;
            }
            pointOfView.Z = numerator / denominator;
            foreach (var node in orientations.TableNodes)
            {
                node.vZ = pointOfView.Z - node.Zp;
            }
        }
    }
}
