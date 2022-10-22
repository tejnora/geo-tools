using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GeoCalculations.BasicTypes;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Deviations;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;
using GeoCalculations.Points;

namespace GeoCalculations.Methods
{
    public enum PolygonTraverseTypes
    {
        None,
        Free,
        Closure,
        Fixed,
        MutuallyConnectedAndOneSideFixed,
        MutuallyConnectedAndMutuallyFixed
    }

    public static class PolygonTraverseMethod
    {
        public static void PolygonTraverse(PolygonTraverseContext context , ref PolygonCalculatedPoints calculatedPoints)
        {
            calculatedPoints.Init();
            PolygonTraverseTypes type;
            context.Validate();
            if (context.HasErrors)
            {
                throw new PolygonCalculationException("E1");
            }
            if (string.IsNullOrEmpty(context.EndPoint.Number))
                type = PolygonTraverseTypes.Free;
            else
            {
                if (context.BeginPoint.Prefix == context.EndPoint.Prefix && context.BeginPoint.Number == context.EndPoint.Number)
                    type = PolygonTraverseTypes.Closure;
                else
                {
                    if (context.BeginOrientationContext.TableNodes.Count == 0 && context.EndOrientationContext.TableNodes.Count == 0)
                        type = PolygonTraverseTypes.Fixed;
                    else if (context.BeginOrientationContext.TableNodes.Count > 0 && context.EndOrientationContext.TableNodes.Count == 0)
                        type = PolygonTraverseTypes.MutuallyConnectedAndOneSideFixed;
                    else
                        type = PolygonTraverseTypes.MutuallyConnectedAndMutuallyFixed;
                }
            }
            switch (type)
            {
                case PolygonTraverseTypes.Free:
                    {
                        if (context.BeginOrientationContext.TableNodes.Count == 0)
                            throw new PolygonCalculationException("E2");
                        if (!ValidateOrientations(context.BeginOrientationContext))
                            throw new PolygonCalculationException("E3");
                        if (!ValidateMeasuringData(context.MeasuredContext.TableNodes))
                            throw new PolygonCalculationException("E4");
                        calculatedPoints = CalculateFree(context.BeginPoint, context.BeginOrientationContext, context.MeasuredContext.TableNodes);
                    } break;
                case PolygonTraverseTypes.Closure:
                    {
                        if (context.BeginOrientationContext.TableNodes.Count == 0)
                            throw new PolygonCalculationException("E2");
                        if (!ValidateOrientations(context.BeginOrientationContext))
                            throw new PolygonCalculationException("E3");
                        if (context.EndOrientationContext.TableNodes.Count == 0)
                            throw new PolygonCalculationException("E7");
                        if (!ValidateOrientations(context.EndOrientationContext))
                            throw new PolygonCalculationException("E8");
                        if (!ValidateMeasuringData(context.MeasuredContext.TableNodes))
                            throw new PolygonCalculationException("E4");
                        calculatedPoints = CalculateClosure(context.BeginPoint, context.BeginOrientationContext, context.EndPoint, context.EndOrientationContext, context.MeasuredContext.TableNodes);
                    } break;
                case PolygonTraverseTypes.Fixed:
                    {
                        if (!ValidateMeasuringData(context.MeasuredContext.TableNodes))
                            throw new PolygonCalculationException("E4");
                        calculatedPoints = CalculateFixed(context.BeginPoint, context.EndPoint, context.MeasuredContext.TableNodes);
                    } break;
                case PolygonTraverseTypes.MutuallyConnectedAndOneSideFixed:
                    {
                        if (context.BeginOrientationContext.TableNodes.Count == 0)
                            throw new PolygonCalculationException("E2");
                        if (!ValidateOrientations(context.BeginOrientationContext))
                            throw new PolygonCalculationException("E3");
                        if (!ValidateMeasuringData(context.MeasuredContext.TableNodes))
                            throw new PolygonCalculationException("E4");
                        calculatedPoints = CalculateMutuallyConnectedAndOneSideFixed(context.BeginPoint, context.BeginOrientationContext, context.EndPoint, context.MeasuredContext.TableNodes);
                    } break;
                case PolygonTraverseTypes.MutuallyConnectedAndMutuallyFixed:
                    {
                        if (context.BeginOrientationContext.TableNodes.Count == 0)
                            throw new PolygonCalculationException("E2");
                        if (!ValidateOrientations(context.BeginOrientationContext))
                            throw new PolygonCalculationException("E3");
                        if (context.EndOrientationContext.TableNodes.Count == 0)
                            throw new PolygonCalculationException("E7");
                        if (!ValidateOrientations(context.EndOrientationContext))
                            throw new PolygonCalculationException("E8");
                        if (!ValidateMeasuringData(context.MeasuredContext.TableNodes))
                            throw new PolygonCalculationException("E4");
                        calculatedPoints = CalculateMutuallyConnectedAndMutuallyFixed(context.BeginPoint, context.BeginOrientationContext, context.EndPoint, context.EndOrientationContext, context.MeasuredContext.TableNodes);
                    } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static PolygonCalculatedPoints RemoveRedundantPoints(PolygonCalculatedPoints polygonovyPoradVysledek)
        {
            if (polygonovyPoradVysledek == null) return null;
            switch (polygonovyPoradVysledek.PolygonTraverseType)
            {
                case PolygonTraverseTypes.None:
                    break;
                case PolygonTraverseTypes.Free:
                    polygonovyPoradVysledek.Nodes.Remove(polygonovyPoradVysledek.Nodes[0]);
                    goto case PolygonTraverseTypes.Closure;
                case PolygonTraverseTypes.Closure:
                case PolygonTraverseTypes.Fixed:
                case PolygonTraverseTypes.MutuallyConnectedAndOneSideFixed:
                case PolygonTraverseTypes.MutuallyConnectedAndMutuallyFixed:
                    polygonovyPoradVysledek.Nodes.Remove(polygonovyPoradVysledek.Nodes[polygonovyPoradVysledek.Nodes.Count - 1]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return polygonovyPoradVysledek;
        }

        private static bool ValidateOrientations(OrientationContext o)
        {
            foreach (var node in o.TableNodes)
            {
                node.Validate();
                if (node.HasErrors)
                    return false;
            }
            return true;
        }

        private static bool ValidateMeasuringData(IEnumerable<MeasuredPoint> merenaData)
        {
            foreach (var node in merenaData)
            {
                node.Validate();
                if (node.HasErrors)
                    return false;
            }
            return true;
        }

        private static PolygonCalculatedPoints CalculateFree(PointBaseEx beginPoint, OrientationContext beginOrientation, ObservableCollection<MeasuredPoint> measuringPoints)
        {
            if (measuringPoints.Count > 3)
                throw new PolygonCalculationException("E5");
            if (measuringPoints.Count == 0)
                throw new PolygonCalculationException("E9");
            var result = CreateResult(beginPoint, PolygonTraverseTypes.Free, measuringPoints);
            CalculateMeasuringAdditionalData(measuringPoints);
            CalculateBalancedAzimuts(beginPoint, beginOrientation, result.Deviations);
            measuringPoints[0].DirectionAzimut = SimpleCalculation.ValidateDirectionAzimut(beginOrientation.OrientationMovement + measuringPoints[0].Uhel);
            CaluculateAzimuts(measuringPoints);
            for (var i = 1; i < measuringPoints.Count; i++)
            {
                result.Nodes[i].X = result.Nodes[i - 1].X + measuringPoints[i - 1].ZprumerovanaDelka * Math.Cos(measuringPoints[i - 1].DirectionAzimut * Math.PI / 200.0);
                result.Nodes[i].Y = result.Nodes[i - 1].Y + measuringPoints[i - 1].ZprumerovanaDelka * Math.Sin(measuringPoints[i - 1].DirectionAzimut * Math.PI / 200.0);
            }
            CalculateStatisticData(measuringPoints, result);
            DeviationsChecking(result);
            return result;
        }

        private static PolygonCalculatedPoints CalculateFixed(PointBaseEx vychoziBod, PointBaseEx koncovyBod, ObservableCollection<MeasuredPoint> merenaData)
        {
            if (merenaData.Count < 3)
                throw new PolygonCalculationException("E9");
            var result = CreateResult(vychoziBod, PolygonTraverseTypes.Fixed, merenaData);
            result.Nodes[0].X = 0;
            result.Nodes[0].Y = 0;
            CalculateMeasuringAdditionalData(merenaData);
            merenaData[0].DirectionAzimut = 0.0;
            CaluculateAzimuts(merenaData);
            VypocetSouradnicovyRozdilu(merenaData);
            VypocetSouradnic(result, merenaData);
            //porovnani delek identicky bodu s pripustnou odchylkou
            var deltaX = Math.Abs(vychoziBod.X - koncovyBod.X);
            var deltaY = Math.Abs(vychoziBod.Y - koncovyBod.Y);
            var rozdilDelekPuvodni = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            var posBod = result.Nodes[result.Nodes.Count - 1];
            var rozdilDelekPocitany = Math.Sqrt(posBod.X * posBod.X + posBod.Y * posBod.Y);
            result.LocationDeviation = rozdilDelekPuvodni - rozdilDelekPocitany;
            //vypocet transformacnich parametru
            var a1 = ((koncovyBod.X - vychoziBod.X) * posBod.X + (koncovyBod.Y - vychoziBod.Y) * posBod.Y) /
                    (posBod.X * posBod.X + posBod.Y * posBod.Y);
            var a2 = ((koncovyBod.Y - vychoziBod.Y) * posBod.X - (koncovyBod.X - vychoziBod.X) * posBod.Y) /
                     (posBod.X * posBod.X + posBod.Y * posBod.Y);
            foreach (var node in result.Nodes)
            {
                var x = vychoziBod.X + node.X * a1 - node.Y * a2;
                var y = vychoziBod.Y + node.X * a2 + node.Y * a1;
                node.X = x;
                node.Y = y;
            }
            CalculateStatisticData(merenaData, result);
            DeviationsChecking(result);
            return result;
        }

        private static PolygonCalculatedPoints CalculateClosure(PointBaseEx vychoziBod, OrientationContext vychoziBodOrientace,
            PointBaseEx koncovyBod, OrientationContext koncovyBodOrientace, ObservableCollection<MeasuredPoint> merenaData)
        {
            if (merenaData.Count < 3)
                throw new PolygonCalculationException("E9");
            var result = CreateResult(vychoziBod, PolygonTraverseTypes.Closure, merenaData);
            CalculateMeasuringAdditionalData(merenaData);
            CalculateBalancedAzimuts(vychoziBod, vychoziBodOrientace, result.Deviations);
            CalculateBalancedAzimuts(koncovyBod, koncovyBodOrientace, result.Deviations);
            var pBod = merenaData[0];
            pBod.DirectionAzimut = SimpleCalculation.ValidateDirectionAzimut(vychoziBodOrientace.OrientationMovement + pBod.Uhel);
            var kBod = merenaData[merenaData.Count - 1];
            kBod.DirectionAzimut = SimpleCalculation.ValidateDirectionAzimut(koncovyBodOrientace.OrientationMovement + kBod.Uhel);
            kBod.Uhel = 400 - Math.Abs(kBod.DirectionAzimut - pBod.DirectionAzimut);
            //uhlove vyrovnani
            result.AgnleClosure = (merenaData.Count - 2 + 3) * 200;
            for (int i = 1; i < merenaData.Count; i++)
            {
                result.AgnleClosure -= merenaData[i].Uhel;
            }
            result.AngleDeviation = result.AgnleClosure / (result.Nodes.Count - 2 + 1);
            for (int i = 1; i < merenaData.Count; i++)
            {
                merenaData[i].Uhel += result.AngleDeviation;
                merenaData[i].UhlovaOdchylka = result.AngleDeviation;
            }
            //vypocet vyrovanych smerniku
            CaluculateAzimuts(merenaData);
            //vypocet souradnicovy rozdilu
            VypocetSouradnicovyRozdilu(merenaData);
            result.CoordinateDeviationX = 0;
            result.CoordinateDeviationY = 0;
            for (var i = 1; i < merenaData.Count; i++)
            {
                result.CoordinateDeviationX += merenaData[i].SouradnicovyRozdilX;
                result.CoordinateDeviationY += merenaData[i].SouradnicovyRozdilY;
            }
            result.CoordinateDeviationX = -result.CoordinateDeviationX;
            result.CoordinateDeviationY = -result.CoordinateDeviationY;
            result.LocationDeviation = Math.Sqrt(result.CoordinateDeviationY * result.CoordinateDeviationY + result.CoordinateDeviationX * result.CoordinateDeviationX);

            //Souradnicova vyrovnani
            VypocetSouradnicovaVyrovnani(result, merenaData);
            //vypocet souradnic
            VypocetSouradnic(result, merenaData);
            CalculateStatisticData(merenaData, result);
            DeviationsChecking(result);
            return result;
        }

        private static PolygonCalculatedPoints CalculateMutuallyConnectedAndMutuallyFixed(PointBaseEx vychoziBod,
            OrientationContext vychoziBodOrientace, PointBaseEx koncovyBod, OrientationContext koncovyBodOrientace,
            ObservableCollection<MeasuredPoint> merenaData)
        {
            if (merenaData.Count < 3)
                throw new PolygonCalculationException("E9");
            var result = CreateResult(vychoziBod, PolygonTraverseTypes.MutuallyConnectedAndMutuallyFixed, merenaData);
            CalculateMeasuringAdditionalData(merenaData);
            CalculateBalancedAzimuts(vychoziBod, vychoziBodOrientace, result.Deviations);
            CalculateBalancedAzimuts(koncovyBod, koncovyBodOrientace, result.Deviations);
            var pBod = merenaData[0];
            pBod.DirectionAzimut = SimpleCalculation.ValidateDirectionAzimut(vychoziBodOrientace.OrientationMovement + pBod.Uhel);
            var kBod = merenaData[result.Nodes.Count - 1];
            kBod.DirectionAzimut = SimpleCalculation.ValidateDirectionAzimut(koncovyBodOrientace.OrientationMovement + kBod.Uhel);
            kBod.Uhel = SimpleCalculation.ValidateDirectionAzimut(koncovyBodOrientace.OrientationMovement - kBod.DirectionAzimut);

            //uhlove vyrovnani
            var smernikKoncovyBod = pBod.DirectionAzimut;
            for (var i = 1; i < merenaData.Count; i++)
            {
                smernikKoncovyBod += merenaData[i].Uhel;
            }
            smernikKoncovyBod -= (result.Nodes.Count - 2 + 1) * 200;
            result.AgnleClosure = koncovyBodOrientace.OrientationMovement - smernikKoncovyBod;
            result.AngleDeviation = result.AgnleClosure / (result.Nodes.Count - 2 + 2);
            foreach (var node in merenaData)
            {
                node.Uhel += result.AngleDeviation;
                node.UhlovaOdchylka = result.AngleDeviation;
            }
            //vypocet vyrovnanych smerniku
            pBod.DirectionAzimut = SimpleCalculation.ValidateDirectionAzimut(vychoziBodOrientace.OrientationMovement + pBod.Uhel);
            for (var i = 1; i < merenaData.Count; i++)
            {
                merenaData[i].DirectionAzimut = merenaData[i - 1].DirectionAzimut - 200 + merenaData[i].Uhel;
                merenaData[i].DirectionAzimut = SimpleCalculation.ValidateDirectionAzimut(merenaData[i].DirectionAzimut);
            }
            //vypocet souradnicovych rozdilu
            VypocetSouradnicovyRozdilu(merenaData);
            var xSuma = vychoziBod.X;
            var ySuma = vychoziBod.Y;
            for (var i = 1; i < merenaData.Count; i++)
            {
                xSuma += merenaData[i].SouradnicovyRozdilX;
                ySuma += merenaData[i].SouradnicovyRozdilY;
            }
            result.CoordinateDeviationX = koncovyBod.X - xSuma;
            result.CoordinateDeviationY = koncovyBod.Y - ySuma;
            result.LocationDeviation =
                Math.Sqrt(result.CoordinateDeviationY * result.CoordinateDeviationY + result.CoordinateDeviationX * result.CoordinateDeviationX);
            //Souradnicova vyrovnani
            VypocetSouradnicovaVyrovnani(result, merenaData);
            //vypocet souradnic
            VypocetSouradnic(result, merenaData);
            CalculateStatisticData(merenaData, result);
            DeviationsChecking(result);
            return result;
        }

        private static PolygonCalculatedPoints CalculateMutuallyConnectedAndOneSideFixed(PointBaseEx vychoziBod,
            OrientationContext vychoziBodOrientace, PointBaseEx koncovyBod, ObservableCollection<MeasuredPoint> merenaData)
        {
            if (merenaData.Count < 3)
                throw new PolygonCalculationException("E9");
            var result = CreateResult(vychoziBod, PolygonTraverseTypes.MutuallyConnectedAndOneSideFixed, merenaData);
            CalculateMeasuringAdditionalData(merenaData);
            CalculateBalancedAzimuts(vychoziBod, vychoziBodOrientace, result.Deviations);
            var pBod = merenaData[0];
            pBod.DirectionAzimut = SimpleCalculation.ValidateDirectionAzimut(vychoziBodOrientace.OrientationMovement + pBod.Uhel);
            //vypocet vyrovanych smerniku
            CaluculateAzimuts(merenaData);
            //vypocet souradnicovy rozdilu
            VypocetSouradnicovyRozdilu(merenaData);
            //souradnicove vyrovnani
            var sumaX = 0.0;
            var sumaY = 0.0;
            for (int i = 1; i < merenaData.Count; i++)
            {
                sumaX += merenaData[i].SouradnicovyRozdilX;
                sumaY += merenaData[i].SouradnicovyRozdilY;
            }
            result.CoordinateDeviationX = koncovyBod.X - vychoziBod.X - sumaX;
            result.CoordinateDeviationY = koncovyBod.Y - vychoziBod.Y - sumaY;
            result.LocationDeviation = Math.Sqrt(result.CoordinateDeviationY * result.CoordinateDeviationY + result.CoordinateDeviationX * result.CoordinateDeviationX);
            VypocetSouradnicovaVyrovnani(result, merenaData);
            VypocetSouradnic(result, merenaData);
            CalculateStatisticData(merenaData, result);
            DeviationsChecking(result);
            return result;
        }

        private static void DeviationsChecking(PolygonCalculatedPoints porad)
        {
            //uhlova odchylka
            switch (porad.PolygonTraverseType)
            {
                case PolygonTraverseTypes.None:
                case PolygonTraverseTypes.Free:
                case PolygonTraverseTypes.Fixed:
                case PolygonTraverseTypes.MutuallyConnectedAndOneSideFixed:
                    break;
                case PolygonTraverseTypes.Closure:
                case PolygonTraverseTypes.MutuallyConnectedAndMutuallyFixed:
                    porad.Deviations.Deviations.Add(new PolygonAngleDeviation(porad));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //polohova odchylka
            switch (porad.PolygonTraverseType)
            {
                case PolygonTraverseTypes.None:
                case PolygonTraverseTypes.Free:
                    break;
                case PolygonTraverseTypes.Closure:
                case PolygonTraverseTypes.Fixed:
                case PolygonTraverseTypes.MutuallyConnectedAndOneSideFixed:
                case PolygonTraverseTypes.MutuallyConnectedAndMutuallyFixed:
                    porad.Deviations.Deviations.Add(new PolygonPositionDeviation(porad));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //delkova odchylka
            porad.Deviations.Deviations.Add(new PolygonLengthDeviation(porad));
        }


        private static void CalculateStatisticData(IEnumerable<MeasuredPoint> merenaData, PolygonCalculatedPoints porad)
        {
            double delkaPred = double.NaN;
            porad.LongestLength = double.MaxValue;
            porad.GreaterPeakAngle = double.MaxValue;
            foreach (var node in merenaData)
            {
                if (!double.IsNaN(node.ZprumerovanaDelka))
                {
                    porad.LenghtOfTraverse += node.ZprumerovanaDelka;
                    porad.LongestLength = Math.Min(porad.LongestLength, node.ZprumerovanaDelka);
                    porad.LeastLength = Math.Max(porad.LeastLength, node.ZprumerovanaDelka);
                    if (!double.IsNaN(delkaPred))
                    {
                        var pomer = delkaPred / node.ZprumerovanaDelka;
                        porad.MaximumRadioNeighboringLengths = Math.Max(porad.MaximumRadioNeighboringLengths, pomer);
                    }
                    delkaPred = node.ZprumerovanaDelka;
                }
                if (!double.IsNaN(node.RozdilDelek))
                {
                    porad.MaximumDifferenceNeighboringMeasuringLengths = Math.Max(porad.MaximumDifferenceNeighboringMeasuringLengths, node.RozdilDelek);
                }
                if (!double.IsNaN(node.Uhel))
                {
                    porad.GreaterPeakAngle = Math.Min(porad.GreaterPeakAngle, node.Uhel);
                }
            }
        }

        private static void VypocetSouradnic(PolygonCalculatedPoints porad, ObservableCollection<MeasuredPoint> merenaData)
        {
            var deltaX = porad.Nodes[0].X;
            var deltaY = porad.Nodes[0].Y;
            for (var i = 1; i < merenaData.Count; i++)
            {
                deltaX += merenaData[i].SouradnicovyRozdilX;
                deltaY += merenaData[i].SouradnicovyRozdilY;
                porad.Nodes[i].X=deltaX;
                porad.Nodes[i].Y=deltaY;
            }
        }
        private static void CaluculateAzimuts(IList<MeasuredPoint> measurePoints)
        {
            for (var i = 1; i < measurePoints.Count - 1; i++)
            {
                measurePoints[i].DirectionAzimut = measurePoints[i - 1].DirectionAzimut - 200 + measurePoints[i].Uhel;
                measurePoints[i].DirectionAzimut = SimpleCalculation.ValidateDirectionAzimut(measurePoints[i].DirectionAzimut);
            }
        }
        private static void VypocetSouradnicovyRozdilu(IList<MeasuredPoint> measurePoints)
        {
            for (var i = 1; i < measurePoints.Count; i++)
            {
                measurePoints[i].SouradnicovyRozdilX = measurePoints[i - 1].ZprumerovanaDelka * Math.Cos(measurePoints[i - 1].DirectionAzimut * Math.PI / 200.0);
                measurePoints[i].SouradnicovyRozdilY = measurePoints[i - 1].ZprumerovanaDelka * Math.Sin(measurePoints[i - 1].DirectionAzimut * Math.PI / 200.0);
            }
        }
        private static void VypocetSouradnicovaVyrovnani(PolygonCalculatedPoints porad, ObservableCollection<MeasuredPoint> merenaData)
        {
            double sumaSouradnicovychRozdiluX = 0, sumaSouradnicovychRozdiluY = 0;
            for (var i = 1; i < merenaData.Count; i++)
            {
                sumaSouradnicovychRozdiluX += Math.Abs(merenaData[i].SouradnicovyRozdilX);
                sumaSouradnicovychRozdiluY += Math.Abs(merenaData[i].SouradnicovyRozdilY);
            }
            sumaSouradnicovychRozdiluX = Math.Abs(sumaSouradnicovychRozdiluX);
            sumaSouradnicovychRozdiluY = Math.Abs(sumaSouradnicovychRozdiluY);
            for (int i = 1; i < merenaData.Count; i++)
            {
                merenaData[i].OpravaVyrovnaniX = porad.CoordinateDeviationX / sumaSouradnicovychRozdiluX * Math.Abs(merenaData[i].SouradnicovyRozdilX);
                merenaData[i].OpravaVyrovnaniY = porad.CoordinateDeviationY / sumaSouradnicovychRozdiluY * Math.Abs(merenaData[i].SouradnicovyRozdilY);
                merenaData[i].SouradnicovyRozdilX += merenaData[i].OpravaVyrovnaniX;
                merenaData[i].SouradnicovyRozdilY += merenaData[i].OpravaVyrovnaniY;
            }

        }
        //smernik od J, smer z totalky
        private static void CalculateBalancedAzimuts(PointBase pointOfView, OrientationContext orientation, CalculationDeviation deviations)
        {
            var orientaceBody = orientation.TableNodes;
            if (orientaceBody.Count == 0)
                throw new PolygonCalculationException("E6");
            double citatel = 0, jmenovatel = 0;
            foreach (var bod in orientaceBody)
            {
                if (!bod.IsEnabled) continue;
                bod.DirectionAzimut = SimpleCalculation.CalculateDirectionAzimut(new Point(pointOfView.X, pointOfView.Y), new Point(bod.X, bod.Y));
                var rozdilMeziSkutecnymAMerenym = SimpleCalculation.CalculateDifferenceBetweenDirectionAzimutAndMeasurePoint(bod.DirectionAzimut, bod.Hz);
                var delka = SimpleCalculation.CalculateDistance(new Point(pointOfView.X, pointOfView.Y), new Point(bod.X, bod.Y));
                bod.Scale = SimpleCalculation.ApplyVerticalDistanceScale(delka);
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
                double vOrientace = double.MaxValue;
                while (Math.Abs(bod.VerticalOrientation) < Math.Abs(vOrientace))
                {
                    vOrientace = bod.VerticalOrientation;
                    bod.VerticalOrientation -= 400;
                }
                bod.VerticalOrientation = vOrientace;
                deviations.Deviations.Add(new BalancedDirectionAzimutDeviation(bod));
            }
            double vv = 0;
            int nodeCount = 0;
            var maxVOrientace = 0.0;
            foreach (var node in orientaceBody)
            {
                if (!node.IsEnabled) continue;
                vv += Math.Pow(node.VerticalOrientation, 2);
                nodeCount++;
                maxVOrientace = Math.Max(Math.Abs(node.VerticalOrientation), maxVOrientace);
            }
            deviations.Deviations.Add(new PolarMethodDeviation(orientation, PolarMethodDeviation.Types.MaxVOrientace, maxVOrientace));
            //m0 = SQRT([vv]/(n-1))
            orientation.m0 = Math.Sqrt(vv / (nodeCount - 1));
            //SQRT( [vv]/(n*(n-1)) )
            orientation.m1 = Math.Sqrt(vv / (nodeCount * (nodeCount - 1)));
        }

        static public void calcPolarniMetodataVysky(PointBaseEx stanovisko, OrientationContext orientace)
        {
            return;
            //todo
            /*if (!double.IsNaN(pointOfView.Height) && !double.IsNaN(pointOfView.SignalHeight))
            {
                var orientaceBody = orientation.TableNodes;
                var citatel = 0.0;
                var jmenovatel = 0.0;
                foreach (var node in orientaceBody)
                {
                    if (double.IsNaN(node.Height)) continue;
                    node.ElevationDifferenceScale = GetVahaPrevyseni(node.Distance + node.VDistance);
                    citatel += node.ElevationDifferenceScale * (node.Height - pointOfView.SignalHeight);
                    jmenovatel += node.ElevationDifferenceScale;
                }
                var result = citatel / jmenovatel;
                foreach (var node in orientation.TableNodes)
                {
                    if (double.IsNaN(node.Height)) continue;
                    node.vZ = pointOfView.Height - node.Zp;
                }
            }*/
        }


        private static PolygonCalculatedPoints CreateResult(PointBase beginPoint, PolygonTraverseTypes type, IList<MeasuredPoint> measuringData)
        {
            var result = new PolygonCalculatedPoints();
            result.Nodes = new ObservableCollection<PolygonCalculatedPoint>();
            foreach (var measurePoint in measuringData)
            {
                result.Nodes.Add(new PolygonCalculatedPoint());
            }
            result.Nodes[0] = new PolygonCalculatedPoint { X = beginPoint.X, Y = beginPoint.Y };
            result.PolygonTraverseType = type;
            return result;
        }

        private static void CalculateMeasuringAdditionalData(IList<MeasuredPoint> measuringData)
        {
            for (var i = 0; i < measuringData.Count; i++)
            {
                measuringData[i].SmerVpred = measuringData[i].MeasuringForward.Hz;
                measuringData[i].SmerZpet = measuringData[i].MeasuringBack.Hz;
                measuringData[i].DelkaVpred = measuringData[i].MeasuringForward.Distance;
                measuringData[i].DelkaZpet = measuringData[i].MeasuringBack.Distance;
                if (i != measuringData.Count - 1) continue;
                if (double.IsNaN(measuringData[i].DelkaZpet))
                {
                    measuringData[i].DelkaZpet = measuringData[i - 1].DelkaVpred;
                }
                if (double.IsNaN(measuringData[i].SmerZpet))
                {
                    measuringData[i].SmerZpet = SimpleCalculation.InverseAngle(measuringData[i - 1].SmerVpred);
                }
            }
            for (var i = 0; i < measuringData.Count; i++)
            {
                var node = measuringData[i];
                if (i == 0)
                    node.Uhel = node.SmerVpred;
                else if (i == measuringData.Count - 1)
                {
                    node.RozdilDelek = 0;
                    node.ZprumerovanaDelka = node.DelkaZpet;
                    node.Uhel = node.SmerZpet;
                    continue;
                }
                else
                    node.Uhel = SimpleCalculation.CalculateAngle(node.SmerZpet, node.SmerVpred);
                node.RozdilDelek = node.DelkaVpred - measuringData[i + 1].DelkaZpet;
                node.ZprumerovanaDelka = (node.DelkaVpred + measuringData[i + 1].DelkaZpet) / 2;
            }
        }

        public static PolygonCalculatedPoints PolygonTraversHeight(PolygonCalculatedPoints porad, PointBaseEx vychoziBod, PointBaseEx koncovyBod, ObservableCollection<MeasuredPoint> merenaData)
        {
            var pocatecniVyska = vychoziBod.Z;
            var koncovaVyska = koncovyBod.Z;
            if (double.IsNaN(pocatecniVyska))
            {
                throw new PolygonCalculationException("E10");
            }
            if (porad.PolygonTraverseType == PolygonTraverseTypes.Closure)
            {
                if (pocatecniVyska != koncovaVyska)
                    throw new PolygonCalculationException("E11");
            }
            else if (porad.PolygonTraverseType != PolygonTraverseTypes.Free)
            {
                if (double.IsNaN(koncovaVyska))
                    throw new PolygonCalculationException("E12");
            }
            //init
            for (int i = 0; i < porad.Nodes.Count - 1; i++)
            {
                var curNode = porad.Nodes[i];
                curNode.Cislo2 = merenaData[i + 1].PointOfView.Number;
                curNode.Predcisli2 = merenaData[i + 1].PointOfView.Prefix;
                curNode.ZTam = merenaData[i].MeasuringForward.ZenitAngle;
                curNode.ZZpet = merenaData[i + 1].MeasuringBack.ZenitAngle;
                curNode.dhTam = merenaData[i].MeasuringForward.ElevationDifference;
                curNode.dhZpet = merenaData[i + 1].MeasuringBack.ElevationDifference;
                if ((i > 0 && double.IsNaN(curNode.ZTam) && double.IsNaN(curNode.dhTam)) || (double.IsNaN(curNode.ZZpet) && double.IsNaN(curNode.dhZpet)))
                    throw new PolygonCalculationException("E13");
                if (double.IsNaN(curNode.dhTam))
                    curNode.dhTam = CalckVyska(curNode.ZTam, merenaData[i].MeasuringForward.Distance);
                if (double.IsNaN(curNode.dhZpet))
                    curNode.dhZpet = CalckVyska(curNode.ZZpet, merenaData[i + 1].MeasuringBack.Distance);
                curNode.dhTam += CalcDelta(merenaData[i].PointOfView.SignalHeight, merenaData[i].MeasuringForward.SignalHeight);
                curNode.dhZpet += CalcDelta(merenaData[i + 1].PointOfView.SignalHeight, merenaData[i + 1].MeasuringBack.SignalHeight);
                curNode.dH = (Math.Abs(curNode.dhTam) + Math.Abs(curNode.dhZpet)) * 0.5;
                curNode.VdH = Math.Abs(Math.Abs(curNode.dhTam) - Math.Abs(curNode.dhZpet));
                if (curNode.dhTam < 0)
                    curNode.dH = -curNode.dH;
            }
            //vyskovy uzaver
            if (porad.PolygonTraverseType != PolygonTraverseTypes.Free)
            {
                porad.HeightClosure = pocatecniVyska;
                for (int i = 0; i < porad.Nodes.Count - 1; i++)
                {
                    porad.HeightClosure += porad.Nodes[i].dH;
                }
                porad.HeightClosure = -(porad.HeightClosure - koncovaVyska);

                //vyskove vyrovnani
                var deltaVyskaVyrovani = porad.HeightClosure / porad.LenghtOfTraverse;
                for (int i = 0; i < porad.Nodes.Count - 1; i++)
                {
                    var curNode = porad.Nodes[i];
                    curNode.VdHVyrovnani = deltaVyskaVyrovani * merenaData[i].ZprumerovanaDelka;
                    curNode.dhVyrovanini = curNode.dH + curNode.VdHVyrovnani;
                }
                //vypocet vysky
                var vypoctenaVyska = pocatecniVyska;
                for (int i = 0; i < porad.Nodes.Count; i++)
                {
                    var curNode = porad.Nodes[i];
                    curNode.Z = vypoctenaVyska;
                    vypoctenaVyska += curNode.dhVyrovanini;
                }
                porad.Deviations.Deviations.Add(new PolygonHeightClosureDeviation(porad));
            }
            else
            {
                //vypocet vysky
                var vypoctenaVyska = pocatecniVyska;
                for (int i = 0; i < porad.Nodes.Count; i++)
                {
                    var curNode = porad.Nodes[i];
                    curNode.Z = vypoctenaVyska;
                    vypoctenaVyska += curNode.dH;
                }
            }
            return porad;
        }

        private static double CalckVyska(double angle, double length)
        {
            if (double.IsNaN(angle) || double.IsNaN(length))
                throw new PolygonCalculationException("E14");
            angle = SimpleCalculation.ValidateHeight(angle);
            double calcAngle = 100 - angle;
            if (calcAngle == 0 || calcAngle == 180)
                throw new PolygonCalculationException("E15");
            double vyska = Math.Tan(calcAngle * Math.PI / 200.0) * length;
            return vyska;
        }

        private static double CalcDelta(double vyskaStroje, double vyskaSignalu)
        {
            if (double.IsNaN(vyskaStroje) || double.IsNaN(vyskaSignalu))
                throw new PolygonCalculationException("E16");
            return vyskaStroje - vyskaSignalu;
        }
    }
}
