using System;
using GeoCalculations.BasicTypes;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Deviations;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.Methods
{
    class BalancedDirectionAzimut
    {
        //smernik od J, smer z totalky
        public static void Calculate(PointBaseEx pointOfView, OrientationContext orientation, CalculationDeviation deviations)
        {
            var orientaceBody = orientation.TableNodes;
            if (orientaceBody.Count == 0)
                throw new BalancedDirectionAzimutException("E6");
            double citatel = 0, jmenovatel = 0;
            foreach (var bod in orientaceBody)
            {
                if (!bod.IsEnabled) continue;
                bod.DirectionAzimut = SimpleCalculation.CalculateDirectionAzimut(new Point(pointOfView.X, pointOfView.Y), new Point(bod.X, bod.Y));
                var rozdilMeziSkutecnymAMerenym = SimpleCalculation.CalculateDifferenceBetweenDirectionAzimutAndMeasurePoint(bod.DirectionAzimut, bod.Hz);
                var delka = SimpleCalculation.CalculateDistance(new Point(pointOfView.X, pointOfView.Y), new Point(bod.X, bod.Y));
                bod.Scale = SimpleCalculation.ApplayDistanceScale(delka);
                citatel += bod.Scale * rozdilMeziSkutecnymAMerenym;
                jmenovatel += bod.Scale;
                if (!double.IsNaN(bod.Distance))
                    bod.VerticalDistance = delka - bod.Distance;
            }
            orientation.OrientationMovement = citatel / jmenovatel;//vazeny prumer
            foreach (var bod in orientaceBody)
            {
                if (!bod.IsEnabled) continue;
                bod.VerticalOrientation = bod.Hz + orientation.OrientationMovement - bod.DirectionAzimut;
                var vOrientace = double.MaxValue;
                while (Math.Abs(bod.VerticalOrientation) < Math.Abs(vOrientace))
                {
                    vOrientace = bod.VerticalOrientation;
                    bod.VerticalOrientation -= 400;
                }
                bod.VerticalOrientation = vOrientace;
                deviations.Deviations.Add(new BalancedDirectionAzimutDeviation(bod));
            }
            double vv = 0;
            var nodeCount = 0;
            var maxVOrientace = 0.0;
            foreach (var node in orientaceBody)
            {
                if (!node.IsEnabled) continue;
                vv += Math.Pow(node.VerticalOrientation, 2);
                nodeCount++;
                maxVOrientace = Math.Max(Math.Abs(node.VerticalOrientation), maxVOrientace);
            }
            deviations.Deviations.Add(new PolarMethodDeviation(orientation, PolarMethodDeviation.Types.MaxVOrientace, maxVOrientace));
            if (nodeCount > 1)
            {
                //m0 = SQRT([vv]/(n-1))
                orientation.m0 = Math.Sqrt(vv / (nodeCount - 1));
                //SQRT( [vv]/(n*(n-1)) )
                orientation.m1 = Math.Sqrt(vv / (nodeCount * (nodeCount - 1)));
            }
        }
    }
}
