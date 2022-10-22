using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using CAD.Export;
using CAD.UITools;
using CAD.VFK.DrawTools;
using GeoBase.Utils;
using VFK;
using VFK.Tables;

using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;
using Rectangle = System.Drawing.Rectangle;
using Pen = System.Drawing.Pen;
using Graphics = System.Drawing.Graphics;
using Color = System.Drawing.Color;
using Brush = System.Drawing.Brush;
using Font = System.Drawing.Font;
using StringFormat = System.Drawing.StringFormat;
using FontStyle = System.Drawing.FontStyle;


namespace CAD.Canvas
{
    public interface ICanvasOwner
    {
        void SetPositionInfo(UnitPoint unitpos);
        void SetSnapInfo(ISnapPoint snap);
        void UpdateToolBars(GeoCadRoutedCommand command);
    }
    public interface ICanvas
    {
        IModel DataModel { get; }
        UnitPoint ScreenTopLeftToUnitPoint();
        UnitPoint ScreenBottomRightToUnitPoint();
        Point ToScreen(UnitPoint unitpoint);
        GraphicsPath ToScreen(GraphicsPath aPath);
        double ToScreen(double unitvalue);
        double ToUnit(double screenvalue);
        UnitPoint ToUnit(Point screenpoint);
        double getZoom();

        void Invalidate();
        IDrawObject CurrentObject { get; }

        Rectangle ClientRectangle { get; }
        Graphics Graphics { get; }
        Pen CreatePen(Color color, float unitWidth);
        Font CreateFont(string fontName, float fontSize, FontStyle fontStyle);
        void DrawLine(ICanvas canvas, Pen pen, UnitPoint p1, UnitPoint p2);
        void DrawLine(ICanvas canvas, Pen pen, Point p1, Point p2);
        void DrawArc(ICanvas canvas, Pen pen, UnitPoint center, float radius, float beginangle, float angle);
        void DrawPath(ICanvas aCanvas, Pen aPen, GraphicsPath aPath);
        void FillPath(ICanvas aCanvas, Brush aBrush, GraphicsPath aPath);
        Size MeasureString(ICanvas aCanvas, string aText, Font aFont, Size aSize, StringFormat aFormat, out int aNumberOfCharacter, out int aNumberOfLines);
        void DrawPath(ICanvas aCanvas, Pen aPen, PathImpl aPath, UnitPoint offset);
    }
    public enum ExportType
    {
        Dxf
    } ;
    public interface IModel
    {
        double Zoom { get; set; }
        ICanvasLayer BackgroundLayer { get; }
        ICanvasLayer ImageBackgrounLayer { get; }
        ICanvasLayer GridLayer { get; }
        ICanvasLayer[] Layers { get; }
        ICanvasLayer ActiveLayer { get; set; }
        ICanvasLayer GetLayer(string id);
        IDrawObject CreateObject(string type, UnitPoint point, ISnapPoint snappoint);
        IDrawObject CreateVFKObject(string type, UnitPoint point, ISnapPoint snappoint);
        void AddObject(ICanvasLayer layer, IDrawObject drawobject);
        void AddVFKObject(IDrawObject drawobject);
        void DeleteObjects(IEnumerable<IDrawObject> objects);
        void DeleteVFKObjects(IEnumerable<IDrawObject> objects, bool silentDeleteObjects);
        void MoveObjects(UnitPoint offset, IEnumerable<IDrawObject> objects);
        void CopyObjects(UnitPoint offset, IEnumerable<IDrawObject> objects);
        void MoveNodes(UnitPoint position, IEnumerable<INodePoint> nodes);

        IEditTool GetEditTool(string id);
        void AfterEditObjects(IEditTool edittool);

        List<IDrawObject> GetHitObjects(ICanvas canvas, Rect selection, bool anyPoint);
        List<IDrawObject> GetHitObjects(ICanvas canvas, UnitPoint point);
        bool IsSelected(IDrawObject drawobject);
        void AddSelectedObject(IDrawObject drawobject);
        void RemoveSelectedObject(IDrawObject drawobject);
        IEnumerable<IDrawObject> SelectedObjects { get; }
        int SelectedCount { get; }
        void ClearSelectedObjects();

        ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, Type[] runningsnaptypes, Type usersnaptype);

        List<Color> getColors();
        List<double> getWidths();

        bool CanUndo();
        bool DoUndo();
        bool CanRedo();
        bool DoRedo();
        void StopUndoRedo();
        void StarUndoRedo();

        void Export(string fileName, ExportType type);
    }
    public interface ICanvasLayer
    {
        string Id { get; }
        void Draw(ICanvas canvas, Rect unitrect);
        ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj);
        IEnumerable<IDrawObject> Objects { get; }
        bool Enabled { get; set; }
        bool Visible { get; set; }
        Color Color { get; set; }
        double Width { get; set; }
        string Name { get; set; }
        Rect GetBoundingRect(ICanvas canvas);
        void GetHitObjects(List<IDrawObject> selected, ICanvas canvas, Rect selection, bool anyPoint);
        void GetHitObjects(List<IDrawObject> aHitObjects, ICanvas canvas, UnitPoint point);
        void AddObject(IDrawObject drawobject);
        void Export(IExport export);
    }
    public interface ISnapPoint
    {
        IDrawObject Owner { get; }
        UnitPoint SnapPoint { get; }
        Rect BoundingRect { get; }
        void Draw(ICanvas canvas);
    }
    public enum eDrawObjectMouseDown
    {
        Done,		// this draw object is complete
        DoneRepeat,	// this object is complete, but create new object of same type
        Continue,	// this object requires additional mouse inputs
    }
    public interface INodePoint
    {
        IDrawObject GetClone();
        IDrawObject GetOriginal();
        void Cancel();
        void Finish();
        void SetPosition(UnitPoint pos);
        void Undo();
        void Redo();
        void OnKeyDown(ICanvas canvas, KeyEventArgs e);
    }
    public interface IDrawObject
    {
        string Id { get; }
        IDrawObject Clone();
        bool PointInObject(ICanvas canvas, UnitPoint point);
        bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint);
        void Draw(ICanvas canvas, Rect unitrect);
        Rect GetBoundingRect(ICanvas canvas);
        void OnMouseMove(ICanvas canvas, UnitPoint point);
        eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint);
        void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint);
        void OnKeyDown(ICanvas canvas, KeyEventArgs e);
        UnitPoint RepeatStartingPoint { get; }
        INodePoint NodePoint(ICanvas canvas, UnitPoint point);
        ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes, Type usersnaptype);
        void Move(UnitPoint offset);
        bool getSelectDrawToolCreate();
        string GetInfoAsString();
        void Export(IExport export);
    }
    public interface IEditTool
    {
        IEditTool Clone();

        bool SupportSelection { get; }
        void SetHitObjects(UnitPoint mousepoint, List<IDrawObject> list);

        void OnMouseMove(ICanvas canvas, UnitPoint point);
        eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint);
        void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint);
        void OnKeyDown(ICanvas canvas, KeyEventArgs e);
        void Finished();
        void Undo();
        void Redo();
    }
    public interface IEditToolOwner
    {
        void SetHint(string text);
    }
    public interface ICanvasCommand
    {
        void CommandSelectDrawTool(GeoCadRoutedCommand command);
        void CommandEdit(GeoCadRoutedCommand command);
        void CommandEscape(bool updateToolBar);
        void CommandPan();
        void CommandMove(bool aFromKeyboard);
        void CommandFitView();
        void CommandSelectVFKActiveObject(VfkActivePoint activePoint);
        void CommandInfoTool(GeoCadRoutedCommand command);

        void InvalidateAll();
    }

    public interface IVFKTool
    {
        void RegisterObject(IVFKMain aOwner);
        void DeleteObject(IVFKMain aOwner);
        VFKSOBRTableItem getActivePoint(UInt32 aIdx);
        UInt32 TYPPPD_KOD { get; set; }
        void SetVfkElement(VfkElement vfkElement);
        bool GetMustBeConnectedWithSnap();
    }
}
