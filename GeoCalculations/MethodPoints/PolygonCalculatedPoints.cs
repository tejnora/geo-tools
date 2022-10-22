using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.Deviations;
using GeoCalculations.Methods;
using GeoCalculations.Protocol;

namespace GeoCalculations.Points
{
    public class PolygonCalculatedPoints : DataObjectBase<PolygonCalculatedPoints>
    {
        public PolygonCalculatedPoints()
            : base(null, new StreamingContext())
        {
            Init();
        }
        public PolygonCalculatedPoints(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Init();
        }

        public CalculationDeviation Deviations
        { get; set; }

        [ProtocolPropertyData("Items")]
        public ObservableCollection<PolygonCalculatedPoint> Nodes
        {
            get;
            set;
        }
        readonly PropertyData _polygonTraverseTypeProperty = RegisterProperty("PolygonTraverseType", typeof(PolygonTraverseTypes), PolygonTraverseTypes.None);
        public PolygonTraverseTypes PolygonTraverseType
        {
            get { return GetValue<PolygonTraverseTypes>(_polygonTraverseTypeProperty); }
            set { SetValue(_polygonTraverseTypeProperty, value); OnPropertyChanged("TypPoradu"); }
        }

        readonly PropertyData _angleClosureProperty = RegisterProperty("AgnleClosure", typeof(double), double.NaN);
        public double AgnleClosure
        {
            get { return GetValue<double>(_angleClosureProperty); }
            set { SetValue(_angleClosureProperty, value); }
        }

        readonly PropertyData _angleDeviationProperty = RegisterProperty("AngleDeviation", typeof(double), double.NaN);
        public double AngleDeviation
        {
            get { return GetValue<double>(_angleDeviationProperty); }
            set { SetValue(_angleDeviationProperty, value); }
        }

        readonly PropertyData _coordinateDeviationXProperty = RegisterProperty("CoordinateDeviationX", typeof(double), double.NaN);
        public double CoordinateDeviationX
        {
            get { return GetValue<double>(_coordinateDeviationXProperty); }
            set { SetValue(_coordinateDeviationXProperty, value); OnPropertyChanged("CoordinateDeviationXY"); }
        }

        readonly PropertyData _coordinateDeviationYProperty = RegisterProperty("CoordinateDeviationY", typeof(double), double.NaN);
        public double CoordinateDeviationY
        {
            get { return GetValue<double>(_coordinateDeviationYProperty); }
            set { SetValue(_coordinateDeviationYProperty, value); OnPropertyChanged("CoordinateDeviationXY"); }
        }

        public string CoordinateDeviationXY
        {
            get
            {
                if (double.IsNaN(CoordinateDeviationX) || double.IsNaN(CoordinateDeviationY))
                    return "";
                return CoordinateDeviationX + "/" + CoordinateDeviationY;
            }
        }

        readonly PropertyData _locationDeviationProperty = RegisterProperty("LocationDeviation", typeof(double), double.NaN);
        public double LocationDeviation
        {
            get { return GetValue<double>(_locationDeviationProperty); }
            set { SetValue(_locationDeviationProperty, value); }
        }

        readonly PropertyData _heightClosureProperty = RegisterProperty("HeightClosure", typeof(double), double.NaN);
        public double HeightClosure
        {
            get { return GetValue<double>(_heightClosureProperty); }
            set { SetValue(_heightClosureProperty, value); }
        }

        readonly PropertyData _lengthOfTraverseProperty = RegisterProperty("LenghtOfTraverse", typeof(double), 0);
        public double LenghtOfTraverse
        {
            get { return GetValue<double>(_lengthOfTraverseProperty); }
            set { SetValue(_lengthOfTraverseProperty, value); }
        }

        readonly PropertyData _longestLengthProperty = RegisterProperty("LongestLength", typeof(double), 0);
        public double LongestLength
        {
            get { return GetValue<double>(_longestLengthProperty); }
            set { SetValue(_longestLengthProperty, value); }
        }

        readonly PropertyData _leastLengthProperty = RegisterProperty("LeastLength", typeof(double), 0);
        public double LeastLength
        {
            get { return GetValue<double>(_leastLengthProperty); }
            set { SetValue(_leastLengthProperty, value); }
        }

        readonly PropertyData _maximumRadioNeighboringLengthsProperty = RegisterProperty("MaximumRadioNeighboringLengths", typeof(double), 0);
        public double MaximumRadioNeighboringLengths
        {
            get { return GetValue<double>(_maximumRadioNeighboringLengthsProperty); }
            set { SetValue(_maximumRadioNeighboringLengthsProperty, value); }
        }

        readonly PropertyData _maximumDifferenceNeighboringMeasuringLengthsProperty = RegisterProperty("MaximumDifferenceNeighboringMeasuringLengths", typeof(double), 0);
        public double MaximumDifferenceNeighboringMeasuringLengths
        {
            get { return GetValue<double>(_maximumDifferenceNeighboringMeasuringLengthsProperty); }
            set { SetValue(_maximumDifferenceNeighboringMeasuringLengthsProperty, value); }
        }

        PropertyData _greatestPeakAngleProperty = RegisterProperty("GreaterPeakAngle", typeof(double), 0);
        public double GreaterPeakAngle
        {
            get { return GetValue<double>(_greatestPeakAngleProperty); }
            set { SetValue(_greatestPeakAngleProperty, value); }
        }

        public void Init()
        {
            Nodes = new ObservableCollection<PolygonCalculatedPoint>();
            Deviations = new CalculationDeviation();
            PolygonTraverseType = PolygonTraverseTypes.None;
            AgnleClosure = double.NaN;
            AngleDeviation = double.NaN;
            CoordinateDeviationX = double.NaN;
            CoordinateDeviationY = double.NaN;
            LocationDeviation = double.NaN;
            HeightClosure = double.NaN;
            LenghtOfTraverse = double.NaN;
            LongestLength = double.NaN;
            LeastLength = double.NaN;
            MaximumRadioNeighboringLengths = double.NaN;
            MaximumDifferenceNeighboringMeasuringLengths = double.NaN;
            GreaterPeakAngle = double.NaN;
        }
    }
}
