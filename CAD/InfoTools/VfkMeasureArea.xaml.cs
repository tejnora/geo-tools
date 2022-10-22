using System;
using System.Collections.Generic;
using System.Windows;
using CAD.Canvas;
using CAD.Utils;
using CAD.VFK.DrawTools;
using GeoBase.Utils;
using Draw = System.Drawing;

namespace CAD.InfoTools
{
    public partial class VfkMeasureArea : InfoPropPage
    {
        #region Constructor
        public VfkMeasureArea(string registryname)
            : base(registryname)
        {
            InitializeComponent();
            _points = new List<VfkActivePoint>();
            DataContext = this;
        }
        #endregion
        #region Property

        private List<VfkActivePoint> _points;
        private UnitPoint _mousePoint;
        private double _width = 0.005;
        private double _measureArea = 0.0;
        public double MeasureArea
        {
            get { return _measureArea; }
            set { _measureArea = value; OnPropertyChanged("MeasureArea"); }
        }

        private double _curcuitOfArea = 0.0;
        public double CurcuitOfArea
        {
            get { return _curcuitOfArea; }
            set { _curcuitOfArea = value; OnPropertyChanged("CurcuitOfArea"); }
        }
        #endregion
        #region InfoPropPage
        public override void Draw(ICanvas canvas)
        {
            var color = System.Drawing.Color.Violet;
            Draw.Pen pen = canvas.CreatePen(color, (float)_width);
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            if (_points.Count == 0) return;
            if (_points.Count == 1)
                canvas.DrawLine(canvas, pen, _points[0].P1, _mousePoint);
            else
            {
                for (int i = 0; i < _points.Count - 1; i++)
                {
                    canvas.DrawLine(canvas, pen, _points[i].P1, _points[i + 1].P1);
                }
                canvas.DrawLine(canvas, pen, _points[_points.Count - 1].P1, _mousePoint);
                canvas.DrawLine(canvas, pen, _mousePoint, _points[0].P1);
            }
        }
        public override void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            _mousePoint = point;
        }
        public override Rect GetBoundingRect(ICanvas canvas)
        {
            double thWidth = ThresholdWidth(canvas, _width);
            Rect rect = Rect.Empty;
            if (_points.Count == 0) return rect;
            if (_points.Count == 1)
                rect = ScreenUtils.GetRect(_points[0].P1, _mousePoint, thWidth);
            else
            {
                for (int i = 0; i < _points.Count - 1; i++)
                {
                    rect = WPFToFormConverter.unionRect(rect, ScreenUtils.GetRect(_points[i].P1, _points[i + 1].P1, thWidth));
                }
                rect = WPFToFormConverter.unionRect(rect, ScreenUtils.GetRect(_points[_points.Count - 1].P1, _mousePoint, thWidth));
                rect = WPFToFormConverter.unionRect(rect, ScreenUtils.GetRect(_mousePoint, _points[0].P1, thWidth));
            }
            return rect;

        }
        #endregion
        #region Methods
        public void AddPoint(VfkActivePoint point)
        {
            _points.Add(point);
        }
        static double GetDeterminant(double x1, double y1, double x2, double y2)
        {
            return x1 * y2 - x2 * y1;
        }
        double GetArea()
        {
            if (_points.Count < 3)
                return 0;
            double area = GetDeterminant(_points[_points.Count - 1].P1.X, _points[_points.Count - 1].P1.Y, _points[0].P1.X, _points[0].P1.Y);
            for (int i = 1; i < _points.Count; i++)
            {
                area += GetDeterminant(_points[i - 1].P1.X, _points[i - 1].P1.Y, _points[i].P1.X, _points[i].P1.Y);
            }
            return Math.Abs(area / 2);
        }
        private double GetCurcuit()
        {
            if (_points.Count < 2)
                return 0;
            double res = 0;
            for(int i=1;i<_points.Count;i++)
            {
                res +=
                    Math.Sqrt(Math.Pow(_points[i].P1.X - _points[i - 1].P1.X, 2) +
                              Math.Pow(_points[i].P1.Y - _points[i - 1].P1.Y, 2));
            }
            res += Math.Sqrt(Math.Pow(_points[_points.Count - 1].P1.X - _points[0].P1.X, 2) +
                             Math.Pow(_points[_points.Count - 1].P1.Y - _points[0].P1.Y, 2));
            return res;

        }
        public void CalcMeasure()
        {
            MeasureArea = GetArea();
            CurcuitOfArea = GetCurcuit();
            _points.Clear();
        }
        static double ThresholdPixel = 6;
        static double ThresholdWidth(ICanvas canvas, double objectwidth)
        {
            return ThresholdWidth(canvas, objectwidth, ThresholdPixel);
        }
        public static double ThresholdWidth(ICanvas canvas, double objectwidth, double pixelwidth)
        {
            double minWidth = canvas.ToUnit(pixelwidth);
            double width = Math.Max(objectwidth / 2, minWidth);
            return (float)width;
        }
        #endregion
    }
}
