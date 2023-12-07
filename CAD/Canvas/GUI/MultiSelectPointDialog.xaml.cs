using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using CAD.Canvas.DrawTools;
using CAD.VFK.DrawTools;
using GeoBase.Gui;

namespace CAD.Canvas
{
    public class PointContext
    {
        public IDrawObject DrawObject { get; }
        public PointContext(IDrawObject drawObject)
        {
            DrawObject = drawObject;
        }
        public override string ToString()
        {
            return PointNumber;
        }
        public string PointNumber => ((VfkActivePoint)DrawObject).PointNumber;
    }
    public partial class MultiSelectPointDialog : DialogBase
    {
        PointContext _selectedPoint;

        public MultiSelectPointDialog(List<IDrawObject> drawObjects)
            : base("MultiSelectPointDialog")
        {
            InitializeComponent();
            _elementsGroup.DataContext = this;
            var list = drawObjects.Where(n => n is ActivePoint).Select(n => new PointContext(n)).ToArray();
            Points = new CollectionView(list);
            _selectedPoint = list.First();
        }
        public CollectionView Points { get; }

        public PointContext SelectedPoint
        {
            get => _selectedPoint;
            set
            {
                _selectedPoint = value;
                OnPropertyChanged("SelectedPoint");
            }
        }
    }
}
