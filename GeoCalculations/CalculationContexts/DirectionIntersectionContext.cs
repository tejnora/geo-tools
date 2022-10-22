using GeoBase.Utils;

namespace GeoCalculations.CalculationContexts
{
    public class DirectionIntersectionContext : CalculationContextBase
    {
        readonly PropertyData _pointOfViewAProperty = RegisterProperty("PointOfViewA", typeof(PolarContex), new PolarContex());
        public PolarContex PointOfViewA
        {
            get { return GetValue<PolarContex>(_pointOfViewAProperty); }
            set { SetValue(_pointOfViewAProperty, value); }
        }

        readonly PropertyData _pointOfViewBProperty = RegisterProperty("PointOfViewB", typeof(PolarContex), new PolarContex());
        public PolarContex PointOfViewB
        {
            get { return GetValue<PolarContex>(_pointOfViewBProperty); }
            set { SetValue(_pointOfViewBProperty, value); }
        }
    }
}
