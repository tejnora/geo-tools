using System;
using GeoBase.Utils;
using GeoCalculations.BasicTypes;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Deviations;
using GeoCalculations.Exceptions;

namespace GeoCalculations.Methods
{
    public static class ControlDistanceMethod
    {
        static public bool Calculate(ControlDistanceContex points)
        {
            //            points.ResetValues();
            for (var i = 1; i < points.TableNodes.Count; i++)
            {
                var point1 = points.TableNodes[i - 1];
                var point2 = points.TableNodes[i];
                points.TableNodes[i].CoordinateLength = SimpleCalculation.CalculateDistance(new Point(point1.X, point1.Y), new Point(point2.X, point2.Y));
            }
            for (var i = 1; i < points.TableNodes.Count; i++)
            {
                var point = points.TableNodes[i];
                double limitDeviation;
                switch (points.QualityCode)
                {
                    case ControlDistanceContex.CodeQualities._3:
                        limitDeviation = 0.14;
                        break;
                    case ControlDistanceContex.CodeQualities._4:
                        limitDeviation = 0.26;
                        break;
                    case ControlDistanceContex.CodeQualities._5:
                        limitDeviation = 0.50;
                        break;
                    case ControlDistanceContex.CodeQualities._6:
                        limitDeviation = 0.21;
                        break;
                    case ControlDistanceContex.CodeQualities._7:
                        limitDeviation = 0.50;
                        break;
                    case ControlDistanceContex.CodeQualities._8:
                        limitDeviation = 1.0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (double.IsNaN(point.MeasuringLength) || double.IsNaN(point.CoordinateLength))
                {
                    var par = new ResourceParams();
                    par.Add("number", point.Number);
                    throw new ControlDistanceException("E34", par);
                }
                point.LengthDifference = Math.Abs(point.CoordinateLength - point.MeasuringLength);
                point.LimitDeviation = (point.CoordinateLength + 12) / (point.CoordinateLength + 20) * Math.Sqrt(2) * 2 * limitDeviation;
                points.Deviations.Deviations.Add(new ControlDistanceDeviation(point, point.LengthDifference, point.LimitDeviation));
            }
            return true;
        }

    }
}
