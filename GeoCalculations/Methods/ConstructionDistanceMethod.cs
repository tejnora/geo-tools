using System;
using GeoBase.Utils;
using GeoCalculations.BasicTypes;
using GeoCalculations.CalculationContexts;
using GeoCalculations.CalculationResultContexts;
using GeoCalculations.Deviations;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.Methods
{
    public static class ConstructionDistanceMethod
    {
        static public void Calculate(ConstructionDistanceContex contex, ConstructionDistanceResultContext resultContext)
        {
            var nodes = contex.TableNodes;
            if (nodes.Count < 2 || double.IsNaN(nodes[0].X) || double.IsNaN(nodes[0].Y) || double.IsNaN(nodes[nodes.Count - 1].X) || double.IsNaN(nodes[nodes.Count - 1].X))
                throw new ConstructionDistanceCalculationException("E26");
            var startPointIdx = 0;
            double offsetX = 0, offsetY = 0, direction = 0;
            var privniDelka = true;
            for (var i = 0; i < nodes.Count; i++)
            {
                var bod = nodes[i];
                bool calcPoints = false;
                if (!double.IsNaN(bod.X) || !double.IsNaN(bod.Y))
                {
                    if (double.IsNaN(bod.X) || double.IsNaN(bod.Y))
                    {
                        var par = new ResourceParams();
                        par.Add("point", bod.NumberWithPrefix);
                        throw new ConstructionDistanceCalculationException("E27", par);
                    }
                    if (i == 0)
                    {
                        bod.XLocal = 0;
                        bod.YLocal = 0;
                        bod.Direction = 0;
                        bod.dX = 0;
                        bod.dY = 0;
                        continue;
                    }
                    calcPoints = true;
                }
                if (double.IsNaN(bod.Distance))
                {
                    var par = new ResourceParams();
                    par.Add("point", bod.NumberWithPrefix);
                    throw new ConstructionDistanceCalculationException("E30", par);
                }
                if (privniDelka)
                {
                    if (bod.Distance < 0)
                        throw new ConstructionDistanceCalculationException("E29");
                    offsetX = Math.Abs(bod.Distance);
                    bod.XLocal = offsetX;
                    bod.YLocal = offsetY;
                    bod.Direction = 0;
                    bod.dX = 0;
                    bod.dY = 0;
                    privniDelka = false;
                }
                else
                {
                    if (bod.Distance > 0) //zalomeni doprava
                    {
                        direction += 100;
                        if (direction >= 400) direction -= 400;
                    }
                    else //zalomeni doleva
                    {
                        direction -= 100;
                        if (direction < 0) direction += 400;
                    }
                    bod.dX = 0;
                    bod.dY = 0;
                    if (direction == 0)
                    {
                        bod.dX = Math.Abs(bod.Distance);
                        offsetX += bod.dX;
                    }
                    else if (direction == 100)
                    {
                        bod.dY = Math.Abs(bod.Distance);
                        offsetY += bod.dY;
                    }
                    else if (direction == 200)
                    {
                        bod.dX = -Math.Abs(bod.Distance);
                        offsetX += bod.dX;
                    }
                    else if (direction == 300)
                    {
                        bod.dY = -Math.Abs(bod.Distance);
                        offsetY += bod.dY;
                    }
                    bod.XLocal = offsetX;
                    bod.YLocal = offsetY;
                    bod.Direction = direction;
                }
                if (calcPoints)
                {
                    var startPoint = nodes[startPointIdx];
                    var endPoint = nodes[i];
                    var delkaVypoctena = Math.Sqrt(Math.Pow(endPoint.YLocal - startPoint.YLocal, 2) + Math.Pow(endPoint.XLocal - startPoint.XLocal, 2));
                    var delkaZeSouradnic = Math.Sqrt(Math.Pow(endPoint.Y - startPoint.Y, 2) + Math.Pow(endPoint.X - startPoint.X, 2));
                    var odchylka = delkaZeSouradnic - delkaVypoctena;
                    contex.Deviations.Deviations.Add(new ConstructionDistanceDeviation(endPoint, odchylka, delkaZeSouradnic));
                    var smerniZeSouradnic = SimpleCalculation.CalculateDirectionAzimut(new Point(startPoint.X, startPoint.Y), new Point(endPoint.X, endPoint.Y));
                    var smerniZeIdentickychBodu = SimpleCalculation.CalculateDirectionAzimut(new Point(startPoint.XLocal, startPoint.YLocal), new Point(endPoint.XLocal, endPoint.YLocal));
                    var otoceni = smerniZeSouradnic - smerniZeIdentickychBodu;
                    var lastPointX = startPoint.X + Math.Cos(otoceni * Math.PI / 200.0) * endPoint.XLocal - Math.Sin(otoceni * Math.PI / 200.0) * endPoint.YLocal;
                    var lastPointY = startPoint.Y + Math.Sin(otoceni * Math.PI / 200.0) * endPoint.XLocal + Math.Cos(otoceni * Math.PI / 200.0) * endPoint.YLocal;
                    var odchylkaX = ((endPoint.X - startPoint.X) - (lastPointX - startPoint.X)) / (i - startPointIdx);
                    var odchylkaY = ((endPoint.Y - startPoint.Y) - (lastPointY - startPoint.Y)) / (i - startPointIdx);
                    var deltaOdchylkaX = 0.0;
                    var deltaOdchylkaY = 0.0;
                    for (var j = resultContext.Nodes.Count > 0 ? startPointIdx + 1 : startPointIdx; j <= i; j++)
                    {
                        var srcPoint = nodes[j];
                        var xLocal = srcPoint.XLocal;
                        var yLocal = srcPoint.YLocal;
                        var newPoint = new CalculatedPointBase { Prefix = srcPoint.Prefix, Number = srcPoint.Number };
                        newPoint.X = startPoint.X + Math.Cos(otoceni * Math.PI / 200.0) * xLocal - Math.Sin(otoceni * Math.PI / 200.0) * yLocal;
                        newPoint.Y = startPoint.Y + Math.Sin(otoceni * Math.PI / 200.0) * xLocal + Math.Cos(otoceni * Math.PI / 200.0) * yLocal;
                        if (j != startPointIdx)
                        {
                            deltaOdchylkaX += odchylkaX;
                            deltaOdchylkaY += odchylkaY;
                            newPoint.X += deltaOdchylkaX;
                            newPoint.Y += deltaOdchylkaY;
                        }
                        newPoint.Distance = srcPoint.Distance;
                        resultContext.Nodes.Add(newPoint);
                    }
                    startPointIdx = i;
                }
            }
        }
    }
}
