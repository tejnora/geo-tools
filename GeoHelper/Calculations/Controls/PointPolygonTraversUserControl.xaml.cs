using System.Windows;
using GeoCalculations.MethodPoints;

namespace GeoHelper.Calculations.Controls
{
    public partial class PointPolygonTraversUserControl
    {
        public static readonly DependencyProperty GroupBoxHeaderProperty = DependencyProperty.Register("GroupBoxHeader", typeof(string),
                                        typeof(PointPolygonTraversUserControl), new FrameworkPropertyMetadata(string.Empty));

        PointBaseEx _point;

        public PointPolygonTraversUserControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public PointBaseEx Point
        {
            get { return _point; }
            set
            {
                if (_point == value) return;
                _point = value;
                OnPropertyChanged("Point");
            }
        }

        public string GroupBoxHeader
        {
            get { return (string)GetValue(GroupBoxHeaderProperty); }
            set { SetValue(GroupBoxHeaderProperty, value); }
        }
    }
}