using System;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.Methods
{
    public static class PolarMethod
    {
        public static void Calculate(PolarContex calculationContext, CalculatedPointBase calculatedPointBase)
        {
            if (double.IsNaN(calculatedPointBase.Hz))
                throw new PolarMethodCalculationException("E36");
            calculatedPointBase.X = calculationContext.PointOfView.X + calculatedPointBase.Distance * Math.Cos((calculatedPointBase.Hz + calculationContext.Orientation.OrientationMovement) * Math.PI / 200.0);
            calculatedPointBase.Y = calculationContext.PointOfView.Y + calculatedPointBase.Distance * Math.Sin((calculatedPointBase.Hz + calculationContext.Orientation.OrientationMovement) * Math.PI / 200.0);

            if (double.IsNaN(calculationContext.PointOfView.Z)) return;
            calculatedPointBase.Z = ElevationDifference.Calculate(calculatedPointBase.ZenitAngle, calculatedPointBase.Distance,
                                                                calculationContext.PointOfView.Z +
                                                                (double.IsNaN(calculationContext.PointOfView.SignalHeight)
                                                                    ? 0
                                                                    : calculationContext.PointOfView.SignalHeight),
                                                                calculatedPointBase.TargetHeight, calculatedPointBase.VerticalDistance);
        }
    }
}
