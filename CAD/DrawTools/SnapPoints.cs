using System.Drawing;
using System.Windows;
using CAD.Utils;
using GeoBase.Utils;

namespace CAD.Canvas.DrawTools
{
    class SnapPointBase : ISnapPoint
    {
        public SnapPointBase(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
        {
            _owner = owner;
            _snappoint = snappoint;
            double size = canvas.ToUnit(14);
            _boundingRect.X = snappoint.X - size / 2;
            _boundingRect.Y = snappoint.Y - size / 2;
            _boundingRect.Width = size;
            _boundingRect.Height = size;
        }
        protected UnitPoint _snappoint;
        protected Rect _boundingRect;
        protected IDrawObject _owner;
        public IDrawObject Owner => _owner;
        public virtual UnitPoint SnapPoint => _snappoint;
        public virtual Rect BoundingRect => _boundingRect;

        public virtual void Draw(ICanvas canvas)
        {
        }
        protected void DrawPoint(ICanvas canvas, Pen pen, Brush fillBrush)
        {
            Rectangle screenrect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(canvas, _boundingRect));
            canvas.Graphics.DrawRectangle(pen, screenrect);
            screenrect.X++;
            screenrect.Y++;
            screenrect.Width--;
            screenrect.Height--;
            if (fillBrush != null)
                canvas.Graphics.FillRectangle(fillBrush, screenrect);
        }
    }
    class GridSnapPoint : SnapPointBase
    {
        public GridSnapPoint(ICanvas canvas, UnitPoint snappoint)
            : base(canvas, null, snappoint)
        {
        }
        public override void Draw(ICanvas canvas)
        {
            DrawPoint(canvas, Pens.Gray, null);
        }
    }
    class VertextSnapPoint : SnapPointBase
    {
        public VertextSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
            : base(canvas, owner, snappoint)
        {
        }
        public override void Draw(ICanvas canvas)
        {
            DrawPoint(canvas, Pens.Blue, Brushes.YellowGreen);
        }
    }
    class MidpointSnapPoint : SnapPointBase
    {
        public MidpointSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
            : base(canvas, owner, snappoint)
        {
        }
        public override void Draw(ICanvas canvas)
        {
            DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
        }
    }
    class IntersectSnapPoint : SnapPointBase
    {
        public IntersectSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
            : base(canvas, owner, snappoint)
        {
        }
        public override void Draw(ICanvas canvas)
        {
            DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
        }
    }
    class NearestSnapPoint : SnapPointBase
    {
        public NearestSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
            : base(canvas, owner, snappoint)
        {
        }
        public override void Draw(ICanvas canvas)
        {
            DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
        }
    }
    class QuadrantSnapPoint : SnapPointBase
    {
        public QuadrantSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
            : base(canvas, owner, snappoint)
        {
        }
        public override void Draw(ICanvas canvas)
        {
            DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
        }
    }
    class DivisionSnapPoint : SnapPointBase
    {
        public DivisionSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
            : base(canvas, owner, snappoint)
        {
        }
        public override void Draw(ICanvas canvas)
        {
            DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
        }
    }
    class CenterSnapPoint : SnapPointBase
    {
        public CenterSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
            : base(canvas, owner, snappoint)
        {
        }
        public override void Draw(ICanvas canvas)
        {
            DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
        }
    }
    class PerpendicularSnapPoint : SnapPointBase
    {
        public PerpendicularSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
            : base(canvas, owner, snappoint)
        {
        }
        public override void Draw(ICanvas canvas)
        {
            DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
        }
    }
    class TangentSnapPoint : SnapPointBase
    {
        public TangentSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
            : base(canvas, owner, snappoint)
        {
        }
        public override void Draw(ICanvas canvas)
        {
            DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
        }
    }
    class VfkSnapPoint : SnapPointBase
    {
        public VfkSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
            : base(canvas, owner, snappoint)
        {
        }
        public override void Draw(ICanvas canvas)
        {
            DrawPoint(canvas, Pens.Red, Brushes.YellowGreen);
        }
    }

    class PodrobnyBodZPS : SnapPointBase
    {
        public PodrobnyBodZPS(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
            : base(canvas, owner, snappoint)
        {
        }
        public override void Draw(ICanvas canvas)
        {
            DrawPoint(canvas, Pens.Red, Brushes.YellowGreen);
        }

    }

}
