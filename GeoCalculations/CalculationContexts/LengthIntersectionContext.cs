using GeoBase.Utils;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.CalculationContexts
{
    public class LengthIntersectionContext : CalculationContextBase
    {
        readonly PropertyData _leftPointOfViewProperty = RegisterProperty("LeftPointOfView", typeof(PointBaseEx), new PointBaseEx());
        public PointBaseEx LeftPointOfView
        {
            get { return GetValue<PointBaseEx>(_leftPointOfViewProperty); }
            set { SetValue(_leftPointOfViewProperty, value); }
        }

        readonly PropertyData _rightPointOfViewProperty = RegisterProperty("RightPointOfView", typeof(PointBaseEx), new PointBaseEx());
        public PointBaseEx RightPointOfView
        {
            get { return GetValue<PointBaseEx>(_rightPointOfViewProperty); }
            set { SetValue(_rightPointOfViewProperty, value); }
        }

    }
}
