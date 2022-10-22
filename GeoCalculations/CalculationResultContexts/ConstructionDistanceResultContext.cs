using GeoCalculations.MethodPoints;
using GeoBase.Utils;

namespace GeoCalculations.CalculationResultContexts
{
    public class ConstructionDistanceResultContext
        : CalculationResultContextBase<CalculatedPointBase, ConstructionDistanceResultContext>
    {

        readonly PropertyData _distanceDifferenceProperty = RegisterProperty("DistanceDifference", typeof(double), 0);
        public double DistanceDifference
        {
            get { return GetValue<double>(_distanceDifferenceProperty); }
            set { SetValue(_distanceDifferenceProperty, value); }
        }
    }
}
