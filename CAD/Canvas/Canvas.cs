using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CAD.InfoTools;
using CAD.UITools;
using CAD.Utils;
using CAD.Tools;
using CAD.Resources;
using CAD.GUI;
using CAD.Canvas.DrawTools;
using CAD.Canvas.Layers;
using System.Windows;
using System.Windows.Input;
using CAD.VFK.DrawTools;
using GeoBase.Utils;
using VFK.GUI;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;
using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;
using Rectangle = System.Drawing.Rectangle;
using Pen = System.Drawing.Pen;
using Graphics = System.Drawing.Graphics;
using Color = System.Drawing.Color;
using Brush = System.Drawing.Brush;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;
using StringFormat = System.Drawing.StringFormat;
using Matrix = System.Drawing.Drawing2D.Matrix;
using UserControl = System.Windows.Forms.UserControl;
using Bitmap = System.Drawing.Bitmap;
using Cursor = System.Windows.Forms.Cursor;
using Cursors = System.Windows.Forms.Cursors;
using GraphicsUnit = System.Drawing.GraphicsUnit;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using LineSegment = GeoBase.Utils.LineSegment;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Point = System.Windows.Point;
using RectangleF = System.Drawing.RectangleF;
using Size = System.Windows.Size;

namespace CAD.Canvas
{
    public struct CanvasWrapper : ICanvas
    {
        readonly CanvasCtrl _canvas;
        Graphics _graphics;
        Rectangle _rect;
        public CanvasWrapper(CanvasCtrl canvas)
        {
            _canvas = canvas;
            _graphics = null;
            _rect = new Rectangle();
        }
        public CanvasWrapper(CanvasCtrl canvas, Graphics graphics, Rectangle clientrect)
        {
            _canvas = canvas;
            _graphics = graphics;
            _rect = clientrect;
        }
        public IModel Model
        {
            get { return _canvas.Model; }
        }
        public CanvasCtrl CanvasCtrl
        {
            get { return _canvas; }
        }
        public void Dispose()
        {
            _graphics = null;
        }
        #region ICanvas Members
        public IModel DataModel
        {
            get { return _canvas.Model; }
        }
        public UnitPoint ScreenTopLeftToUnitPoint()
        {
            return _canvas.ScreenTopLeftToUnitPoint();
        }
        public UnitPoint ScreenBottomRightToUnitPoint()
        {
            return _canvas.ScreenBottomRightToUnitPoint();
        }
        public Point ToScreen(UnitPoint unitpoint)
        {
            return _canvas.ToScreen(unitpoint);
        }
        public GraphicsPath ToScreen(GraphicsPath aPath)
        {
            return _canvas.ToScreen(aPath);
        }
        public double ToScreen(double unitvalue)
        {
            return _canvas.ToScreen(unitvalue);
        }
        public double ToUnit(double screenvalue)
        {
            return _canvas.ToUnit(screenvalue);
        }
        public UnitPoint ToUnit(Point screenpoint)
        {
            return _canvas.ToUnit(screenpoint);
        }
        public double getZoom()
        {
            return _canvas.GetZoom();
        }
        public Graphics Graphics
        {
            get { return _graphics; }
        }
        public Rectangle ClientRectangle
        {
            get { return _rect; }
            set { _rect = value; }
        }
        public Pen CreatePen(Color color, float unitWidth)
        {
            return _canvas.CreatePen(color, unitWidth);
        }
        public Font CreateFont(string fontName, float fontSize, FontStyle fontStyle)
        {
            return _canvas.CreateFont(fontName, fontSize, fontStyle);
        }
        public void DrawLine(ICanvas canvas, Pen pen, UnitPoint p1, UnitPoint p2)
        {
            _canvas.DrawLine(canvas, pen, p1, p2);
        }
        public void DrawLine(ICanvas canvas, Pen pen, Point p1, Point p2)
        {
            _canvas.DrawLine(canvas, pen, p1, p2);
        }
        public void DrawArc(ICanvas canvas, Pen pen, UnitPoint center, float radius, float beginangle, float angle)
        {
            _canvas.DrawArc(canvas, pen, center, radius, beginangle, angle);
        }
        public void DrawPath(ICanvas aCanvas, Pen aPen, GraphicsPath aPath)
        {
            _canvas.DrawPath(aCanvas, aPen, aPath);
        }
        public void DrawPath(ICanvas aCanvas, Pen aPen, PathImpl aPath, UnitPoint offset)
        {
            _canvas.DrawPath(aCanvas, aPen, aPath, offset);
        }
        public void FillPath(ICanvas aCanvas, Brush aBrush, GraphicsPath aPath)
        {
            _canvas.FillPath(aCanvas, aBrush, aPath);
        }
        public void Invalidate()
        {
            _canvas.DoInvalidate(false);
        }
        public IDrawObject CurrentObject
        {
            get { return _canvas.NewObject; }
        }
        public Size MeasureString(ICanvas aCanvas, String aText, Font aFont, Size aSize, StringFormat aFormat, out Int32 aNumberOfCharacter, out Int32 aNumberOfLines)
        {
            return _canvas.MeasureString(aCanvas, aText, aFont, aSize, aFormat, out aNumberOfCharacter, out aNumberOfLines);
        }
        #endregion
    }
    public partial class CanvasCtrl : UserControl, ICanvasCommand
    {
        #region Enums
        enum ECommandType
        {
            Select,
            Pan,
            Move,
            Draw,
            Edit,
            EditNode,
            Info
        }
        #endregion
        #region Fields
        ICanvasOwner _owner;
        readonly CursorCollection _cursors = new CursorCollection();
        IModel _model;
        MoveHelper _moveHelper;
        NodeMoveHelper _nodeMoveHelper;
        CanvasWrapper _canvaswrapper;
        ECommandType _commandType = ECommandType.Select;
        bool _runningSnaps = true;
        Type[] _runningSnapTypes;
        Point _mousedownPoint;
        IDrawObject _newObject;
        IEditTool _editTool;
        SelectionRectangle _selection;
        GeoCadRoutedCommand _drawObjectId;
        GeoCadRoutedCommand _editToolId;
        Bitmap _staticImage;
        bool _staticDirty = true;
        ISnapPoint _snappoint;
        #endregion
        #region Property
        public Type[] RunningSnaps
        {
            get { return _runningSnapTypes; }
            set { _runningSnapTypes = value; }
        }
        public bool RunningSnapsEnabled
        {
            get { return _runningSnaps; }
            set { _runningSnaps = value; }
        }

        System.Drawing.Drawing2D.SmoothingMode _mSmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        public System.Drawing.Drawing2D.SmoothingMode SmoothingMode
        {
            get { return _mSmoothingMode; }
            set { _mSmoothingMode = value; }
        }

        public IModel Model
        {
            get { return _model; }
            set { _model = value; }
        }
        #endregion
        #region Constructors
        public void Construct(ICanvasOwner owner, IModel datamodel)
        {
            _canvaswrapper = new CanvasWrapper(this);
            _owner = owner;
            _model = datamodel;

            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            _commandType = ECommandType.Select;
            _cursors.AddCursor(ECommandType.Select, Cursors.Arrow);
            _cursors.AddCursor(ECommandType.Draw, Cursors.Cross);
            _cursors.AddCursor(ECommandType.Pan, Cursors.Hand);
            // m_cursors.AddCursor(eCommandType.pan, "hmove.cur");
            _cursors.AddCursor(ECommandType.Move, Cursors.SizeAll);
            _cursors.AddCursor(ECommandType.Edit, Cursors.Cross);
            UpdateCursor();

            _moveHelper = new MoveHelper(this);
            _nodeMoveHelper = new NodeMoveHelper(_canvaswrapper);
        }
        #endregion
        public UnitPoint GetMousePoint()
        {
            Point point = PointToClient(MousePosition).ToWpfPoint();
            return ToUnit(point);
        }
        public void SetCenter(UnitPoint unitPoint)
        {
            Point point = ToScreen(unitPoint);
            _lastCenterPoint = unitPoint;
            SetCenterScreen(point, false);
        }
        public void SetCenter()
        {
            Point point = PointToClient(MousePosition).ToWpfPoint();
            SetCenterScreen(point, true);
        }
        public UnitPoint GetCenter()
        {
            return ToUnit(new Point(ClientRectangle.Width / 2.0, ClientRectangle.Height / 2.0));
        }
        protected void SetCenterScreen(Point screenPoint, bool setCursor)
        {
            double centerX = ClientRectangle.Width / 2.0;
            _panOffset.X += centerX - screenPoint.X;

            double centerY = (ClientRectangle.Height / 2.0);
            _panOffset.Y += centerY - screenPoint.Y;

            if (setCursor)
            {
                Point pp = new Point(centerX, centerY);
                Cursor.Position = PointToScreen(pp.FromWpfPoint());
            }
            DoInvalidate(true);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Tracing.StartTrack(App.TracePaint);
            ClearPens();
            e.Graphics.SmoothingMode = _mSmoothingMode;
            CanvasWrapper dc = new CanvasWrapper(this, e.Graphics, ClientRectangle);
            Rectangle cliprectangle = e.ClipRectangle;
            if (_staticImage == null)
            {
                cliprectangle = ClientRectangle;
                _staticImage = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                _staticDirty = true;
            }
            Rect r = ScreenUtils.ToUnitNormalized(dc, cliprectangle);
            if (double.IsNaN(r.Width) || double.IsInfinity(r.Width))
            {
                r = ScreenUtils.ToUnitNormalized(dc, cliprectangle);
            }
            if (_staticDirty)
            {
                _staticDirty = false;
                CanvasWrapper dcStatic = new CanvasWrapper(this, Graphics.FromImage(_staticImage), ClientRectangle);
                dcStatic.Graphics.SmoothingMode = _mSmoothingMode;
                _model.BackgroundLayer.Draw(dcStatic, r);
                if (_model.GridLayer.Enabled)
                    _model.GridLayer.Draw(dcStatic, r);

                /* Point nullPoint = ToScreen(new UnitPoint(0, 0));
                 dcStatic.Graphics.DrawLine(Pens.Blue, (float)(nullPoint.X - 10), (float)nullPoint.Y, (float)(nullPoint.X + 10), (float)nullPoint.Y);
                 dcStatic.Graphics.DrawLine(Pens.Blue, (float)nullPoint.X, (float)(nullPoint.Y - 10), (float)nullPoint.X, (float)(nullPoint.Y + 10));
                 */
                ICanvasLayer[] layers = _model.Layers;
                for (int layerindex = layers.Length - 1; layerindex >= 0; layerindex--)
                {
                    if (layers[layerindex] != _model.ActiveLayer && layers[layerindex].Visible)
                        layers[layerindex].Draw(dcStatic, r);
                }
                if (_model.ActiveLayer != null)
                    _model.ActiveLayer.Draw(dcStatic, r);

                dcStatic.Dispose();
            }
            e.Graphics.DrawImage(_staticImage, cliprectangle, cliprectangle, GraphicsUnit.Pixel);

            foreach (IDrawObject drawobject in _model.SelectedObjects)
                drawobject.Draw(dc, r);

            if (_newObject != null)
                _newObject.Draw(dc, r);

            if (_snappoint != null)
                _snappoint.Draw(dc);

            if (_selection != null)
            {
                _selection.Reset();
                _selection.SetMousePoint(e.Graphics, PointToClient(MousePosition).ToWpfPoint());
            }
            if (_moveHelper.IsEmpty == false)
                _moveHelper.DrawObjects(dc, r);

            if (_nodeMoveHelper.IsEmpty == false)
                _nodeMoveHelper.DrawObjects(dc, r);
            if (_commandType == ECommandType.Info)
            {
                ((InfoPropPage)_propPageWindow).Draw(dc);
            }
            dc.Dispose();
            ClearPens();
            Tracing.EndTrack(App.TracePaint, "OnPaint complete");
        }
        void RepaintStatic(Rectangle r)
        {
            if (_staticImage == null)
                return;
            Graphics dc = Graphics.FromHwnd(Handle);
            if (r.X < 0) r.X = 0;
            if (r.X > _staticImage.Width) r.X = 0;
            if (r.Y < 0) r.Y = 0;
            if (r.Y > _staticImage.Height) r.Y = 0;

            if (r.Width > _staticImage.Width || r.Width < 0)
                r.Width = _staticImage.Width;
            if (r.Height > _staticImage.Height || r.Height < 0)
                r.Height = _staticImage.Height;
            dc.DrawImage(_staticImage, r, r, GraphicsUnit.Pixel);
            dc.Dispose();
        }
        void RepaintSnappoint(ISnapPoint snappoint)
        {
            if (snappoint == null)
                return;
            CanvasWrapper dc = new CanvasWrapper(this, Graphics.FromHwnd(Handle), ClientRectangle);
            snappoint.Draw(dc);
            dc.Graphics.Dispose();
            dc.Dispose();
        }
        void RepaintObject(IDrawObject obj)
        {
            if (obj == null)
                return;
            CanvasWrapper dc = new CanvasWrapper(this, Graphics.FromHwnd(Handle), ClientRectangle);
            //Rect invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(dc, obj.GetBoundingRect(dc))); ?? bylo tohle a zmenil jsem na:
            Rect invalidaterect = ScreenUtils.ToScreenNormalized(dc, obj.GetBoundingRect(dc));
            obj.Draw(dc, invalidaterect);
            dc.Graphics.Dispose();
            dc.Dispose();
        }
        public void DoInvalidate(bool dostatic, Rect rect)
        {
            if (dostatic)
                _staticDirty = true;
            Invalidate(ScreenUtils.ConvertRect(rect));
        }
        public void DoInvalidate(bool dostatic)
        {
            if (dostatic)
                _staticDirty = true;
            Invalidate();
        }
        public IDrawObject NewObject
        {
            get { return _newObject; }
        }
        protected void HandleSelection(List<IDrawObject> selected, bool selectOnlyOne)
        {
            if (selected == null)
                return;
            /*
            //it is not possilbe selected active point
            for (Int32 i = 0; i < selected.Count; )
            {
                if (selected[i] is VfkActivePoint)
                {
                    selected.Remove(selected[i]);
                    continue;
                }
                i++;
            }
             */
            bool add = ModifierKeys == Keys.Shift;
            bool toggle = ModifierKeys == Keys.Control;
            bool invalidate = false;
            bool anyoldsel = false;
            int selcount = 0;
            if (selected != null)
                selcount = selected.Count;
            foreach (IDrawObject obj in _model.SelectedObjects)
            {
                anyoldsel = true;
                break;
            }
            if (toggle || add)
            {
                if (toggle && selcount > 0)
                {
                    invalidate = true;
                    foreach (IDrawObject obj in selected)
                    {
                        if (_model.IsSelected(obj))
                            _model.RemoveSelectedObject(obj);
                        else
                            _model.AddSelectedObject(obj);
                    }
                }
                if (add && selcount > 0)
                {
                    invalidate = true;
                    foreach (IDrawObject obj in selected)
                        _model.AddSelectedObject(obj);
                }
            }
            else if (selcount == 0 && anyoldsel)
            {
                invalidate = true;
                _model.ClearSelectedObjects();
            }
            else if (selcount > 0)
            {
                invalidate = true;
                if (selectOnlyOne)
                {
                    if (selcount > 1)
                    {
                        int i = selected.Count;
                        while (i-- > 0)
                        {
                            if (_model.IsSelected(selected[i]))
                                break;
                        }
                        i--;
                        if (i < 0)
                            i = selected.Count - 1;
                        _model.ClearSelectedObjects();
                        _model.AddSelectedObject(selected[i]);
                    }
                    else
                    {
                        _model.ClearSelectedObjects();
                        _model.AddSelectedObject(selected[0]);
                    }
                }
                else
                {
                    _model.ClearSelectedObjects();
                    foreach (IDrawObject obj in selected)
                        _model.AddSelectedObject(obj);
                }
            }
            if (invalidate)
                DoInvalidate(false);
        }
        void FinishNodeEdit()
        {
            _commandType = ECommandType.Select;
            _snappoint = null;
        }
        protected virtual void HandleMouseDownWhenDrawing(UnitPoint mouseunitpoint, ISnapPoint snappoint)
        {
            if (_commandType == ECommandType.Draw)
            {
                if (_newObject == null)
                {
                    if (_model.ActiveLayer is IVFKDravingLayerMain)
                    {
                        _newObject = _model.CreateVFKObject(_drawObjectId.Name, mouseunitpoint, snappoint);
                        if (_newObject != null && _newObject is IVFKTool)
                        {
                            if (((IVFKTool)_newObject).GetMustBeConnectedWithSnap())
                            {
                                snappoint = _model.SnapPoint(_canvaswrapper, ToUnit(_mousedownPoint), null, null);
                                if (snappoint != null && snappoint.Owner.Id == VfkToolBar.VfkActivePoint.Name)
                                {
                                    _newObject = _model.CreateVFKObject(_drawObjectId.Name, mouseunitpoint, snappoint);
                                }
                                else
                                    _newObject = null;
                            }
                        }
                        else
                            _newObject = null;
                        if (_newObject == null)
                        {
                            MessageBox.Show(@"Objekt nemuze byt vytvoren mimo body...", @"Vytvoreni objektu", MessageBoxButtons.OK);
                            return;
                        }
                    }
                    else
                        _newObject = _model.CreateObject(_drawObjectId.Name, mouseunitpoint, snappoint);
                    DoInvalidate(false, _newObject.GetBoundingRect(_canvaswrapper));
                }
                else
                {
                    if (_newObject is IVFKTool)
                    {
                        if (((IVFKTool)_newObject).GetMustBeConnectedWithSnap())
                        {
                            snappoint = _model.SnapPoint(_canvaswrapper, ToUnit(_mousedownPoint), null, null);
                            if (snappoint == null || snappoint.Owner.Id != VfkToolBar.VfkActivePoint.Name)
                            {
                                MessageBox.Show(@"Objekt nemuze byt vytvoren mimo body...", @"Vytvoreni objektu", MessageBoxButtons.OK);
                                return;
                            }
                        }
                        if (_newObject is IObjectEditInstance)
                        {
                            IObjectEditInstance edit = _newObject as IObjectEditInstance;
                            if (!edit.ValidateObjectContent())
                                return;
                        }
                        eDrawObjectMouseDown result = _newObject.OnMouseDown(_canvaswrapper, mouseunitpoint, snappoint);
                        switch (result)
                        {
                            case eDrawObjectMouseDown.Done:
                                _model.AddVFKObject(_newObject);
                                _newObject = null;
                                DoInvalidate(true);
                                break;
                            case eDrawObjectMouseDown.DoneRepeat:
                                _model.AddVFKObject(_newObject);
                                _newObject = _model.CreateVFKObject(_newObject.Id, _newObject.RepeatStartingPoint, snappoint);
                                if (_propPageWindow != null)
                                {
                                    _propPageWindow.SetOwner(_newObject);
                                }
                                DoInvalidate(true);
                                break;
                            case eDrawObjectMouseDown.Continue:
                                break;
                        }
                    }
                    else
                    {
                        eDrawObjectMouseDown result = _newObject.OnMouseDown(_canvaswrapper, mouseunitpoint, snappoint);
                        switch (result)
                        {
                            case eDrawObjectMouseDown.Done:
                                _model.AddObject(_model.ActiveLayer, _newObject);
                                _newObject = null;
                                DoInvalidate(true);
                                break;
                            case eDrawObjectMouseDown.DoneRepeat:
                                _model.AddObject(_model.ActiveLayer, _newObject);
                                _newObject = _model.CreateObject(_newObject.Id, _newObject.RepeatStartingPoint, null);
                                DoInvalidate(true);
                                if (_propPageWindow != null)
                                {
                                    _propPageWindow.SetOwner(_newObject);
                                }
                                break;
                            case eDrawObjectMouseDown.Continue:
                                break;
                        }
                    }
                }
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                base.OnMouseDown(e);
                return;
            }
            if (e.Button == MouseButtons.Right)
            {
                if (_commandType == ECommandType.Info)
                {
                    switch (_infoTool)
                    {
                        case InfoTools.VfkMeasureArea:
                            ((VfkMeasureArea)_propPageWindow).CalcMeasure();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    if (_newObject is IObjectEditInstance)
                        return;
                    if (_newObject != null || _snappoint != null)
                    {
                        _newObject = null;
                        _snappoint = null;
                        if (_editTool != null)
                            _editTool.Finished();
                    }
                }
                DoInvalidate(false);
                return;
            }
            _mousedownPoint = new Point(e.X, e.Y); // used when panning
            _dragOffset = new Point(0, 0);

            UnitPoint mousepoint = ToUnit(_mousedownPoint);
            if (_snappoint != null)
            {
                mousepoint = _snappoint.SnapPoint;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    var hitObjects = _model.GetHitObjects(_canvaswrapper, mousepoint);
                    if (hitObjects.Count > 1)
                    {
                        var dialog = new MultiSelectPointDialog(hitObjects);
                        if (!(bool)dialog.ShowDialog()) return;
                        var snapPoint = dialog.SelectedPoint.DrawObject.SnapPoint(_canvaswrapper, mousepoint,
                            hitObjects, null, typeof(VfkSnapPoint));
                        if (snapPoint == null) return;
                        mousepoint = snapPoint.SnapPoint;
                    }
                }
            }

            if (_commandType == ECommandType.EditNode)
            {
                bool handled = false;
                if (_nodeMoveHelper.HandleMouseDown(mousepoint, ref handled))
                {
                    FinishNodeEdit();
                    base.OnMouseDown(e);
                    return;
                }
            }
            if (_commandType == ECommandType.Select)
            {
                bool handled = false;
                if (_nodeMoveHelper.HandleMouseDown(mousepoint, ref handled))
                {
                    _commandType = ECommandType.EditNode;
                    _snappoint = null;
                    base.OnMouseDown(e);
                    return;
                }
                _selection = new SelectionRectangle(_mousedownPoint);
            }
            if (_commandType == ECommandType.Move)
            {
                _moveHelper.HandleMouseDownForMove(mousepoint, _snappoint);
            }
            if (_commandType == ECommandType.Draw)
            {
                HandleMouseDownWhenDrawing(mousepoint, null);
                DoInvalidate(true);
            }
            if (_commandType == ECommandType.Edit)
            {
                if (_editTool == null)
                    _editTool = _model.GetEditTool(_editToolId.Name);
                if (_editTool != null)
                {
                    if (_editTool.SupportSelection)
                        _selection = new SelectionRectangle(_mousedownPoint);

                    eDrawObjectMouseDown mouseresult = _editTool.OnMouseDown(_canvaswrapper, mousepoint, _snappoint);
                    /*
                    if (mouseresult == eDrawObjectMouseDown.Continue)
                    {
                        if (m_editTool.SupportSelection)
                            m_selection = new SelectionRectangle(m_mousedownPoint);
                    }
                     * */
                    if (mouseresult == eDrawObjectMouseDown.Done)
                    {
                        _editTool.Finished();
                        _editTool = _model.GetEditTool(_editToolId.Name); // continue with new tool
                        //m_editTool = null;

                        if (_editTool.SupportSelection)
                            _selection = new SelectionRectangle(_mousedownPoint);
                    }
                }
                DoInvalidate(true);
                UpdateCursor();
            }
            if (_commandType == ECommandType.Info)
            {
                switch (_infoTool)
                {
                    case InfoTools.None:
                        break;
                    case InfoTools.VfkMeasureArea:
                        {
                            var pp = _propPageWindow as VfkMeasureArea;
                            if (pp != null)
                            {
                                var snappoint = _model.SnapPoint(_canvaswrapper, ToUnit(_mousedownPoint), null, null);
                                if (snappoint != null && snappoint.Owner.Id == VfkToolBar.VfkActivePoint.Name)
                                {
                                    var activePoint = snappoint.Owner as VfkActivePoint;
                                    pp.AddPoint(activePoint);
                                }
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_commandType == ECommandType.Pan)
            {
                _panOffset.X += _dragOffset.X;
                _panOffset.Y += _dragOffset.Y;
                _dragOffset = new Point(0, 0);
            }

            List<IDrawObject> hitlist = null;
            Rectangle screenSelRect = Rectangle.Empty;
            Rect selectionRect = Rect.Empty;
            if (_selection != null)
            {
                screenSelRect = _selection.ScreenRect();
                selectionRect = _selection.Selection(_canvaswrapper);
                if (selectionRect != Rect.Empty)
                {
                    // is any selection rectangle. use it for selection
                    hitlist = _model.GetHitObjects(_canvaswrapper, selectionRect, _selection.AnyPoint());
                    DoInvalidate(true);
                }
                else
                {
                    // else use mouse point
                    UnitPoint mousepoint = ToUnit(new Point(e.X, e.Y));
                    hitlist = _model.GetHitObjects(_canvaswrapper, mousepoint);
                }
                _selection = null;
            }
            if (_commandType == ECommandType.Select)
            {
                HandleSelection(hitlist, selectionRect == Rect.Empty);
            }
            if (_commandType == ECommandType.Edit && _editTool != null)
            {
                UnitPoint mousepoint = ToUnit(_mousedownPoint);
                if (_snappoint != null)
                    mousepoint = _snappoint.SnapPoint;
                if (screenSelRect != Rectangle.Empty)
                    _editTool.SetHitObjects(mousepoint, hitlist);
                _editTool.OnMouseUp(_canvaswrapper, mousepoint, _snappoint);
            }
            if (_commandType == ECommandType.Draw && _newObject != null)
            {
                UnitPoint mousepoint = ToUnit(_mousedownPoint);
                if (_snappoint != null)
                    mousepoint = _snappoint.SnapPoint;
                _newObject.OnMouseUp(_canvaswrapper, mousepoint, _snappoint);
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_selection != null)
            {
                Graphics dc = Graphics.FromHwnd(Handle);
                _selection.SetMousePoint(dc, new Point(e.X, e.Y));
                dc.Dispose();
                return;
            }

            if (_commandType == ECommandType.Pan && e.Button == MouseButtons.Left)
            {
                _dragOffset.X = -(_mousedownPoint.X - e.X);
                _dragOffset.Y = -(_mousedownPoint.Y - e.Y);
                _lastCenterPoint = CenterPointUnit();
                DoInvalidate(true);
            }
            UnitPoint mousepoint;
            UnitPoint unitpoint = ToUnit(new Point(e.X, e.Y));
            if (_commandType == ECommandType.Draw || _commandType == ECommandType.Move || _commandType == ECommandType.Info || _nodeMoveHelper.IsEmpty == false)
            {
                Rectangle invalidaterect = Rectangle.Empty;
                ISnapPoint newsnap = null;
                mousepoint = GetMousePoint();
                if (RunningSnapsEnabled)
                    newsnap = _model.SnapPoint(_canvaswrapper, mousepoint, _runningSnapTypes, null);
                if (newsnap == null)
                    newsnap = _model.GridLayer.SnapPoint(_canvaswrapper, mousepoint, null);
                if ((_snappoint != null) && ((newsnap == null) || (newsnap.SnapPoint != _snappoint.SnapPoint) || _snappoint.GetType() != newsnap.GetType()))
                {
                    invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(_canvaswrapper, _snappoint.BoundingRect));
                    invalidaterect.Inflate(2, 2);
                    RepaintStatic(invalidaterect); // remove old snappoint
                    _snappoint = newsnap;
                }
                if (_commandType == ECommandType.Move)
                    Invalidate(invalidaterect);

                if (_snappoint == null)
                    _snappoint = newsnap;
            }
            _owner.SetPositionInfo(unitpoint);
            _owner.SetSnapInfo(_snappoint);

            //UnitPoint mousepoint;
            if (_snappoint != null)
                mousepoint = _snappoint.SnapPoint;
            else
                mousepoint = GetMousePoint();
            if (_commandType == ECommandType.Info)
            {
                InfoPropPage ipp = (InfoPropPage)_propPageWindow;
                Rectangle invalidaterect =
                    ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(_canvaswrapper,
                                                           ipp.GetBoundingRect(_canvaswrapper)));
                invalidaterect.Inflate(2, 2);
                RepaintStatic(invalidaterect);
                ipp.OnMouseMove(_canvaswrapper, mousepoint);
                CanvasWrapper dc = new CanvasWrapper(this, Graphics.FromHwnd(Handle), ClientRectangle);
                ipp.Draw(dc);
                dc.Graphics.Dispose();
                dc.Dispose();
            }
            else
            {

                if (_newObject != null)
                {
                    Rectangle invalidaterect =
                        ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(_canvaswrapper,
                                                                               _newObject.GetBoundingRect(_canvaswrapper)));
                    invalidaterect.Inflate(2, 2);
                    RepaintStatic(invalidaterect);

                    _newObject.OnMouseMove(_canvaswrapper, mousepoint);
                    RepaintObject(_newObject);
                }
            }
            if (_snappoint != null)
                RepaintSnappoint(_snappoint);

            if (_moveHelper.HandleMouseMoveForMove(mousepoint))
                Refresh(); //Invalidate();

            Rect rNoderect = _nodeMoveHelper.HandleMouseMoveForNode(mousepoint);
            if (rNoderect != Rect.Empty)
            {
                Rectangle invalidaterect =
                    ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(_canvaswrapper, rNoderect));
                RepaintStatic(invalidaterect);

                CanvasWrapper dc = new CanvasWrapper(this, Graphics.FromHwnd(Handle), ClientRectangle);
                dc.Graphics.Clip = new System.Drawing.Region(ClientRectangle);
                //m_nodeMoveHelper.DrawOriginalObjects(dc, rNoderect);
                _nodeMoveHelper.DrawObjects(dc, rNoderect);
                if (_snappoint != null)
                    RepaintSnappoint(_snappoint);

                dc.Graphics.Dispose();
                dc.Dispose();
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            UnitPoint p = GetMousePoint();
            float wheeldeltatick = 120;
            float zoomdelta = (1.25f * (Math.Abs(e.Delta) / wheeldeltatick));
            if (e.Delta < 0)
                _model.Zoom = _model.Zoom / zoomdelta;
            else
                _model.Zoom = _model.Zoom * zoomdelta;
            SetCenterScreen(ToScreen(p), true);
            DoInvalidate(true);
            base.OnMouseWheel(e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //	if (m_lastCenterPoint != UnitPoint.Empty && Width != 0)
            //		SetCenterScreen(ToScreen(m_lastCenterPoint), false);
            //	m_lastCenterPoint = CenterPointUnit();
            _staticImage = null;
            DoInvalidate(true);
        }

        UnitPoint _lastCenterPoint;
        Point _panOffset = new Point(25, -25);
        Point _dragOffset = new Point(0, 0);
        float m_screenResolution = 96;
        float _inche = 0.0254f;

        Point Translate(UnitPoint point)
        {
            return point.Point;
        }
        double ScreenHeight()
        {
            return ToUnit(ClientRectangle.Height) / _model.Zoom;
        }

        #region ICanvas
        public UnitPoint CenterPointUnit()
        {
            UnitPoint p1 = ScreenTopLeftToUnitPoint();
            UnitPoint p2 = ScreenBottomRightToUnitPoint();
            UnitPoint center = new UnitPoint();
            center.X = (p1.X + p2.X) / 2;
            center.Y = (p1.Y + p2.Y) / 2;
            return center;
        }
        public UnitPoint ScreenTopLeftToUnitPoint()
        {
            return ToUnit(new Point(0, 0));
        }
        public UnitPoint ScreenBottomRightToUnitPoint()
        {
            return ToUnit(new Point(ClientRectangle.Width, ClientRectangle.Height));
        }
        public Point ToScreen(UnitPoint point)
        {
            Point transformedPoint = Translate(point);
            transformedPoint.X /= _inche;
            transformedPoint.Y /= _inche;
            transformedPoint.Y = ScreenHeight() - transformedPoint.Y;
            transformedPoint.Y *= m_screenResolution * _model.Zoom;
            transformedPoint.X *= m_screenResolution * _model.Zoom;

            transformedPoint.X += _panOffset.X + _dragOffset.X;
            transformedPoint.Y += _panOffset.Y + _dragOffset.Y;
            return transformedPoint;
        }
        public GraphicsPath ToScreen(GraphicsPath aPath)
        {
            Matrix matrix = new Matrix();
            matrix.Scale((1 / _inche), -(1 / _inche));
            aPath.Transform(matrix);
            matrix = new Matrix();
            matrix.Translate(0, (float)ScreenHeight());
            aPath.Transform(matrix);
            matrix = new Matrix();
            matrix.Scale((float)((m_screenResolution * _model.Zoom)), (float)((m_screenResolution * _model.Zoom)));
            aPath.Transform(matrix);
            matrix = new Matrix();
            matrix.Translate((float)(_panOffset.X + _dragOffset.X), (float)(_panOffset.Y + _dragOffset.Y));
            aPath.Transform(matrix);
            return aPath;
        }
        public double ToScreen(double value)
        {
            return value / _inche * m_screenResolution * _model.Zoom;
        }
        public double ToUnit(double screenvalue)
        {
            return (screenvalue / (m_screenResolution * _model.Zoom)) * _inche;
        }
        public UnitPoint ToUnit(Point screenpoint)
        {
            double panoffsetX = _panOffset.X + _dragOffset.X;
            double panoffsetY = _panOffset.Y + _dragOffset.Y;
            double xpos = ((screenpoint.X - panoffsetX) / (m_screenResolution * _model.Zoom)) * _inche;
            double ypos = (ScreenHeight() - ((screenpoint.Y - panoffsetY)) / (m_screenResolution * _model.Zoom)) * _inche;
            return new UnitPoint(xpos, ypos);
        }
        public double GetZoom()
        {
            return _model.Zoom;
        }
        public Pen CreatePen(Color color, float unitWidth)
        {
            return GetPen(color, (float)ToScreen(unitWidth));
        }
        struct FontCache
        {
            public string FontName;
            public float FontSize;
            public FontStyle FontStyle;
        }
        private FontCache _fontItem;
        private Dictionary<FontCache, Font> _fontCache = new Dictionary<FontCache, Font>();
        public Font CreateFont(string fontName, float fontSize, FontStyle fontStyle)
        {
            _fontItem.FontName = fontName;
            _fontItem.FontSize = fontSize;
            _fontItem.FontStyle = fontStyle;
            if (!_fontCache.ContainsKey(_fontItem))
            {
                try
                {
                    _fontCache[_fontItem] = new Font(fontName, fontSize, fontStyle);
                }
                catch
                {
                    _fontCache[_fontItem] = new Font("Arial", fontSize, fontStyle);
                }
            }
            return _fontCache[_fontItem];
        }
        public void DrawLine(ICanvas canvas, Pen pen, UnitPoint p1, UnitPoint p2)
        {
            System.Drawing.Point tmpp1 = ToScreen(p1).FromWpfPoint();
            System.Drawing.Point tmpp2 = ToScreen(p2).FromWpfPoint();
            canvas.Graphics.DrawLine(pen, tmpp1, tmpp2);
        }
        public void DrawLine(ICanvas canvas, Pen pen, Point p1, Point p2)
        {
            canvas.Graphics.DrawLine(pen, p1.FromWpfPoint(), p2.FromWpfPoint());
        }
        public void DrawArc(ICanvas canvas, Pen pen, UnitPoint center, float radius, float startAngle, float sweepAngle)
        {
            Point p1 = ToScreen(center);
            radius = (float)Math.Round(ToScreen(radius));
            RectangleF r = new RectangleF(p1.FromWpfPoint(), new System.Drawing.Size());
            r.Inflate(radius, radius);
            if (radius > 0 && radius < 1e8f)
                canvas.Graphics.DrawArc(pen, r, -startAngle, -sweepAngle);
        }
        public void DrawPath(ICanvas aCanvas, Pen aPen, GraphicsPath aPath)
        {
            aCanvas.Graphics.DrawPath(aPen, aPath);
        }
        public void DrawPath(ICanvas aCanvas, Pen aPen, PathImpl aPath, UnitPoint offset)
        {
            foreach (PathSegment item in aPath)
            {
                switch (item.SegmentType)
                {
                    case PathSegment.SegmentTypes.Line:
                        {
                            var l = (LineSegment)item;
                            DrawLine(aCanvas, aPen, l.P1 + offset, l.P2 + offset);
                        }
                        break;
                    case PathSegment.SegmentTypes.Arc:
                        {
                            var a = (ArcSegment)item;
                            DrawArc(aCanvas, aPen, a.Center + offset, a.Radius, a.StartAngle, a.Angle);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public void FillPath(ICanvas aCanvas, Brush aBursh, GraphicsPath aPath)
        {
            aCanvas.Graphics.FillPath(aBursh, aPath);
        }
        public Size MeasureString(ICanvas aCanvas, String aText, Font aFont, Size aSize, StringFormat aFormat, out Int32 aNumberOfCharacter, out Int32 aNumberOfLines)
        {
            Graphics gr = Graphics.FromHwnd(Handle);
            SizeF size = gr.MeasureString(aText, aFont,
                                                         new SizeF((float)aSize.Width, (float)aSize.Height), aFormat,
                                                         out aNumberOfCharacter, out aNumberOfLines);
            return new Size(size.Width, size.Height);
        }
        #endregion

        Dictionary<float, Dictionary<Color, Pen>> m_penCache = new Dictionary<float, Dictionary<Color, Pen>>();
        Pen GetPen(Color color, float width)
        {
            if (m_penCache.ContainsKey(width) == false)
                m_penCache[width] = new Dictionary<Color, Pen>();
            if (m_penCache[width].ContainsKey(color) == false)
            {
                if (color.R == 0 && color.G == 0 && color.B == 0)
                    m_penCache[width][color] = new Pen(Color.White, width);
                else
                    m_penCache[width][color] = new Pen(color, width);
            }
            return m_penCache[width][color];
        }
        void ClearPens()
        {
            m_penCache.Clear();
        }

        void UpdateCursor()
        {
            Cursor = _cursors.GetCursor(_commandType);
        }

        Dictionary<Keys, Type> m_QuickSnap = new Dictionary<Keys, Type>();
        public void AddQuickSnapType(Keys key, Type snaptype)
        {
            m_QuickSnap.Add(key, snaptype);
        }
        #region ICommandIface
        Dictionary<GeoCadRoutedCommand, IDrawObject> _commandSelectDrawTool = new Dictionary<GeoCadRoutedCommand, IDrawObject>();
        public void CommandSelectDrawTool(GeoCadRoutedCommand command)
        {
            CommandEscape(false);
            _model.ClearSelectedObjects();
            _commandType = ECommandType.Draw;
            _drawObjectId = command;
            if (_commandSelectDrawTool.ContainsKey(_drawObjectId))
            {
                _newObject = _commandSelectDrawTool[_drawObjectId];
            }
            else
            {
                if (_model.ActiveLayer is IVFKDravingLayerMain)
                    _newObject = _model.CreateVFKObject(_drawObjectId.Name, new UnitPoint(), null);
                else
                    _newObject = _model.CreateObject(_drawObjectId.Name, new UnitPoint(), null);
            }
            if (_newObject.getSelectDrawToolCreate())
            {
                IObjectEditInstance edit = _newObject as IObjectEditInstance;
                if (edit != null && edit.HasPropPage())
                {
                    if (!(_model.ActiveLayer is IVFKDravingLayerMain))
                        _commandSelectDrawTool[_drawObjectId] = _newObject;
                    ClosePropPage();
                    _propPageWindow = edit.GetPropPage(_model);
                    _propPageWindow.Closed += PropPageClosed;
                    _propPageWindow.Topmost = true;
                    _propPageWindow.ShowInTaskbar = false;
                    _propPageWindow.Owner = Application.Current.MainWindow;
                    _propPageWindow.Show();
                }
            }
            else
                _newObject = null;
            UpdateCursor();
        }
        public void CommandEdit(GeoCadRoutedCommand command)
        {
            CommandEscape(false);
            _model.ClearSelectedObjects();
            _commandType = ECommandType.Edit;
            _editToolId = command;
            _editTool = _model.GetEditTool(_editToolId.Name);
            UpdateCursor();
            _owner.UpdateToolBars(_editToolId);
        }

        public void CommandEscape(bool updateToolBar)
        {
            bool dirty = (_newObject != null) || (_snappoint != null);
            _newObject = null;
            _snappoint = null;
            if (_editTool != null)
                _editTool.Finished();
            _editTool = null;
            if (!dirty || updateToolBar)
            {
                _commandType = ECommandType.Select;
                _moveHelper.HandleCancelMove();
                _nodeMoveHelper.HandleCancelMove();
            }
            UpdateCursor();
            ClosePropPage();
            if (updateToolBar)
                _owner.UpdateToolBars(EditToolBar.Select);
            DoInvalidate(dirty);
        }
        public void CommandPan()
        {
            CommandEscape(false);
            if (_commandType == ECommandType.Select || _commandType == ECommandType.Move)
                _commandType = ECommandType.Pan;
            UpdateCursor();
            _owner.UpdateToolBars(EditToolBar.Pan);
        }
        public void CommandMove(bool handleImmediately)
        {
            CommandEscape(false);
            if (_model.SelectedCount > 0)
            {
                if (handleImmediately && _commandType == ECommandType.Move)
                    _moveHelper.HandleMouseDownForMove(GetMousePoint(), _snappoint);
                _commandType = ECommandType.Move;
                UpdateCursor();
            }
            _owner.UpdateToolBars(EditToolBar.Move);
        }

        private enum InfoTools
        {
            None,
            VfkMeasureArea
        };
        private InfoTools _infoTool = InfoTools.None;
        public void CommandInfoTool(GeoCadRoutedCommand command)
        {
            CommandEscape(false);
            _model.ClearSelectedObjects();
            _commandType = ECommandType.Info;
            _drawObjectId = command;
            switch (_drawObjectId.Name)
            {
                case "VfkMeasureArea":
                    _infoTool = InfoTools.VfkMeasureArea;
                    ClosePropPage();
                    _propPageWindow = new VfkMeasureArea("InfoTool/VfkMeasureArea");
                    _propPageWindow.Closed += PropPageClosed;
                    _propPageWindow.Topmost = true;
                    _propPageWindow.ShowInTaskbar = false;
                    _propPageWindow.Owner = Application.Current.MainWindow;
                    _propPageWindow.Show();
                    break;
                default:
                    throw new ArgumentException("_drawObjectId.Name");
            }
        }
        public void CommandDeleteSelected()
        {
            _model.DeleteVFKObjects(_model.SelectedObjects, false);
            _model.ClearSelectedObjects();
            DoInvalidate(true);
            UpdateCursor();
        }
        public void CommandFitView()
        {
            ICanvasLayer[] layers = _model.Layers;
            Rect bb = Rect.Empty;
            foreach (var layer in layers)
            {
                bb = WPFToFormConverter.unionRect(bb, layer.GetBoundingRect(_canvaswrapper));
            }
            bb = WPFToFormConverter.unionRect(bb, _model.ImageBackgrounLayer.GetBoundingRect(_canvaswrapper));
            SetRectView(bb);
        }
        public void CommandSelectVFKActiveObject(VfkActivePoint activePoint)
        {
            _model.ClearSelectedObjects();
            _model.AddSelectedObject(activePoint);
            SetCenter(activePoint.P1);
        }
        public void InvalidateAll()
        {
            DoInvalidate(true);
        }
        #endregion
        private void SetRectView(Rect aBoundingBox)
        {
            if (aBoundingBox.IsEmpty) return;

            double w = aBoundingBox.Width / _inche * m_screenResolution;
            double h = aBoundingBox.Height / _inche * m_screenResolution;
            w = ClientRectangle.Width / w;
            h = ClientRectangle.Height / h;
            _model.Zoom = Math.Min(w, h);
            double x = Math.Min(aBoundingBox.Left, aBoundingBox.Right);
            double y = Math.Max(aBoundingBox.Top, aBoundingBox.Bottom);

            _panOffset.X = ((1 - (x / _inche * m_screenResolution)) * _model.Zoom);
            _panOffset.Y = y / _inche;
            _panOffset.Y = ScreenHeight() - _panOffset.Y;
            _panOffset.Y *= (m_screenResolution * _model.Zoom);
            _panOffset.Y = 1 - _panOffset.Y;
            //m_panOffset.Y = (y/ _inche * m_screenResolution);
            DoInvalidate(true);
        }

        void HandleQuickSnap(KeyEventArgs e)
        {
            if (_commandType == ECommandType.Select || _commandType == ECommandType.Pan)
                return;
            ISnapPoint p = null;
            UnitPoint mousepoint = GetMousePoint();
            if (m_QuickSnap.ContainsKey(e.KeyCode))
                p = _model.SnapPoint(_canvaswrapper, mousepoint, null, m_QuickSnap[e.KeyCode]);
            if (p != null)
            {
                if (_commandType == ECommandType.Draw)
                {
                    HandleMouseDownWhenDrawing(p.SnapPoint, p);
                    if (_newObject != null)
                        _newObject.OnMouseMove(_canvaswrapper, GetMousePoint());
                    DoInvalidate(true);
                    e.Handled = true;
                }
                if (_commandType == ECommandType.Move)
                {
                    _moveHelper.HandleMouseDownForMove(p.SnapPoint, p);
                    e.Handled = true;
                }
                if (_nodeMoveHelper.IsEmpty == false)
                {
                    bool handled = false;
                    _nodeMoveHelper.HandleMouseDown(p.SnapPoint, ref handled);
                    FinishNodeEdit();
                    e.Handled = true;
                }
                if (_commandType == ECommandType.Edit)
                {
                }
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            HandleQuickSnap(e);

            if (_nodeMoveHelper.IsEmpty == false)
            {
                _nodeMoveHelper.OnKeyDown(_canvaswrapper, e);
                if (e.Handled)
                    return;
            }
            base.OnKeyDown(e);
            if (e.Handled)
            {
                UpdateCursor();
                return;
            }
            if (_editTool != null)
            {
                _editTool.OnKeyDown(_canvaswrapper, e);
                if (e.Handled)
                    return;
            }
            if (_newObject != null)
            {
                _newObject.OnKeyDown(_canvaswrapper, e);
                if (e.Handled)
                    return;
            }
            foreach (IDrawObject obj in _model.SelectedObjects)
            {
                obj.OnKeyDown(_canvaswrapper, e);
                if (e.Handled)
                    return;
            }

            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (e.KeyCode == Keys.G)
                {
                    _model.GridLayer.Enabled = !_model.GridLayer.Enabled;
                    DoInvalidate(true);
                }
                if (e.KeyCode == Keys.S)
                {
                    RunningSnapsEnabled = !RunningSnapsEnabled;
                    if (!RunningSnapsEnabled)
                        _snappoint = null;
                    DoInvalidate(false);
                }
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                CommandEscape(true);
            }
            if (e.KeyCode == Keys.P)
            {
                CommandPan();
            }
            if (e.KeyCode == Keys.S)
            {
                RunningSnapsEnabled = !RunningSnapsEnabled;
                if (!RunningSnapsEnabled)
                    _snappoint = null;
                DoInvalidate(false);
            }
            if (e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D9)
            {
                int layerindex = (int)e.KeyCode - (int)Keys.D1;
                if (layerindex >= 0 && layerindex < _model.Layers.Length)
                {
                    _model.ActiveLayer = _model.Layers[layerindex];
                    DoInvalidate(true);
                }
            }
            if (e.KeyCode == Keys.Delete)
            {
                CommandDeleteSelected();
            }
            /* if (e.KeyCode == Keys.O)
             {
                 CommandEdit("linesmeet");
             }*/
            UpdateCursor();
        }

        PpWindow _propPageWindow;
        private bool _closeFromOutside = false;
        private void ClosePropPage()
        {
            if (_propPageWindow != null)
            {
                _closeFromOutside = true;
                _propPageWindow.Close();
                _propPageWindow = null;
                _closeFromOutside = false;
            }
        }

        private void PropPageClosed(object sender, EventArgs args)
        {
            if (!_closeFromOutside)
            {
                _propPageWindow = null;
                CommandEscape(true);
            }
        }
    }
}
