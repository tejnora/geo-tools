using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows;
using CAD.Export;
using CAD.Utils;
using CAD.Canvas.DrawTools;
using CAD.VFK.DrawTools;
using CAD.VFK.GUI;
using GeoBase.Localization;
using GeoBase.Utils;
using VFK;

namespace CAD.Canvas.Layers
{
    public interface IVFKDravingLayerMain
    {
        bool ShowVfkLayerManager();
    }

    [Serializable]
    class VFKDrawingLayerMain : ICanvasLayer, IVFKDravingLayerMain, ISerializable, IDeserializationCallback
    {
        
        public bool ViewPointEnable { get; set; }
        public VFKMain VFKMain { get; set; }
        Dictionary<UInt32, ICanvasLayer> _layers = new Dictionary<UInt32, ICanvasLayer>();

        
        
        public VFKDrawingLayerMain(VFKMain aVFKMain)
        {
            Name = "VFK";
            _mActivePoints = new VfkActivePointCollection(aVFKMain);
            Color = Color.Black;
            Width = 0.001f;
            Visible = true;
            Enabled = true;
            VFKMain = aVFKMain;
            AddVFKObjects();
            ViewPointEnable = true;
        }

        
        
        protected VfkActivePointCollection _mActivePoints;

        public VfkActivePointCollection VFKActivePoints
        {
            get { return _mActivePoints; }
        }

        public void addPoint(VfkActivePoint aPoint)
        {
            _mActivePoints.UseModelForModification = false;
            _mActivePoints.Add(aPoint);
            _mActivePoints.UseModelForModification = true;
        }

        
        
        public string Id => "WFK";

        public void Draw(ICanvas canvas, Rect unitrect)
        {
            if (ViewPointEnable)
            {
                foreach (IDrawObject drawobject in _mActivePoints)
                {
                    DrawObjectBase obj = drawobject as DrawObjectBase;
                    if (obj is IDrawObject && ((IDrawObject)obj).ObjectInRectangle(canvas, unitrect, true) == false)
                        continue;
                    bool sel = obj.Selected;
                    bool high = obj.Highlighted;
                    obj.Selected = false;
                    drawobject.Draw(canvas, unitrect);
                    obj.Selected = sel;
                    obj.Highlighted = high;
                }
            }

            foreach (var layer in _layers)
            {
                if (layer.Value.Visible)
                    layer.Value.Draw(canvas, unitrect);
            }
        }

        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj)
        {
            foreach (IDrawObject obj in _mActivePoints)
            {
                ISnapPoint sp = obj.SnapPoint(canvas, point, otherobj, null, null);
                if (sp != null)
                    return sp;
            }

            foreach (var layer in _layers)
            {
                if (!layer.Value.Visible) continue;
                ISnapPoint snap = layer.Value.SnapPoint(canvas, point, otherobj);
                if (snap != null)
                    return snap;
            }

            return null;
        }

        public IEnumerable<IDrawObject> Objects => null;

        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public Color Color { get; set; }
        public double Width { get; set; }
        public string Name { get; set; }

        public Rect GetBoundingRect(ICanvas canvas)
        {
            Rect bb = Rect.Empty;
            foreach (IDrawObject drawobject in _mActivePoints)
                bb = WPFToFormConverter.unionRect(bb, drawobject.GetBoundingRect(canvas));
            foreach (var layer in _layers)
            {
                if (!layer.Value.Visible) continue;
                bb = WPFToFormConverter.unionRect(bb, layer.Value.GetBoundingRect(canvas));
            }

            return bb;
        }

        public void GetHitObjects(List<IDrawObject> selected, ICanvas canvas, Rect selection, bool anyPoint)
        {
            foreach (IDrawObject drawobject in _mActivePoints)
            {
                if (drawobject.ObjectInRectangle(canvas, selection, anyPoint))
                    selected.Add(drawobject);
            }

            foreach (var layer in _layers)
            {
                if (!layer.Value.Visible) continue;
                layer.Value.GetHitObjects(selected, canvas, selection, anyPoint);
            }
        }

        public void GetHitObjects(List<IDrawObject> aHitObjects, ICanvas canvas, UnitPoint point)
        {
            foreach (IDrawObject drawobject in _mActivePoints)
            {
                if (drawobject.PointInObject(canvas, point))
                    aHitObjects.Add(drawobject);
            }

            foreach (var layer in _layers)
            {
                if (!layer.Value.Visible) continue;
                layer.Value.GetHitObjects(aHitObjects, canvas, point);
            }
        }

        public void AddObject(IDrawObject drawobject)
        {
            if (drawobject is VfkActivePoint)
            {
                _mActivePoints.UseModelForModification = false;
                _mActivePoints.Add(drawobject as VfkActivePoint);
                _mActivePoints.UseModelForModification = true;
            }
            else
            {
                UInt32 typ = ((IVFKTool)drawobject).TYPPPD_KOD;
                Singletons.VFKElements.GetElement(typ);
                if (!_layers.ContainsKey(typ))
                {
                    _layers[typ] = new VfkDrawinLayer(Singletons.VFKElements.GetElement(typ));
                }

                ((VfkDrawinLayer)_layers[typ]).AddVfkObjectFast(drawobject);
            }
        }

        public List<IDrawObject> DeleteVFKObjects(IEnumerable<IDrawObject> objects, bool silentRemoveObject)
        {
            List<IDrawObject> deletedObjects = new List<IDrawObject>();
            foreach (IDrawObject vfkObject in objects)
            {
                IVFKTool vfkTool = vfkObject as IVFKTool;
                if (vfkObject is VfkActivePoint)
                {
                    VfkActivePoint activePoint = (VfkActivePoint)vfkObject;
                    LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                    ResourceParams par = new ResourceParams();
                    par.Add("PointNumber", activePoint.VfkPointName);
                    if (!silentRemoveObject &&
                        dictionary.ShowMessageBox("90", par, MessageBoxButton.YesNo, MessageBoxImage.Warning) !=
                        MessageBoxResult.Yes)
                        continue;
                    _mActivePoints.UseModelForModification = false;
                    if (activePoint.VfkItem.CanRemove())
                    {
                        _mActivePoints.Remove(activePoint);
                        deletedObjects.Add(vfkObject);
                    }
                    else
                        dictionary.ShowMessageBox("91", par, MessageBoxButton.OK, MessageBoxImage.Warning);

                    _mActivePoints.UseModelForModification = true;
                }
                else if (vfkTool != null)
                {
                    ((VfkDrawinLayer)_layers[((IVFKTool)vfkObject).TYPPPD_KOD]).DeleteObjects(vfkObject);
                    deletedObjects.Add(vfkObject);
                }
            }

            return deletedObjects;
        }

        public void DeleteObjects(IEnumerable<IDrawObject> objects)
        {
            foreach (var vfkObject in objects)
            {
                ((VfkDrawinLayer)_layers[((IVFKTool)vfkObject).TYPPPD_KOD]).DeleteObjects(vfkObject);
            }
        }

        public void Export(IExport export)
        {
            export.SinkLayer(this);
            {
                string layerNameBackup = export.GetTopLayerName();
                export.SetTopLayerName("Active Points");
                foreach (IDrawObject drawobject in _mActivePoints)
                    drawobject.Export(export);
                export.SetTopLayerName(layerNameBackup);
            }
            foreach (var layer in _layers)
                layer.Value.Export(export);
            export.RiseLayer();
        }

        
        
        private void AddVFKObjects()
        {
            addActivePoints();
            addSimpleLines();
            addSimpleMarks();
            addSimpleNumbers();
        }

        private void addActivePoints()
        {
            List<VfkProxyActivePoint> points = VFKMain.GetProxyActivePoins();
            foreach (var point in points)
            {
                addPoint(new VfkActivePoint(point));
            }
        }

        private void addSimpleLines()
        {
            List<VfkMultiLine> lines = VFKMain.GetDravingLineObjects();
            foreach (var line in lines)
            {
                if (!_layers.ContainsKey(line.TYPPPD_KOD))
                {
                    _layers[line.TYPPPD_KOD] = new VfkDrawinLayer(line.VfkElement);
                }

                ((VfkDrawinLayer)_layers[line.TYPPPD_KOD]).AddVfkObjectFast(line);
            }
        }

        private void addSimpleMarks()
        {
            List<VfkMark> marks = VFKMain.GetDravingMarkObjects();
            foreach (var mark in marks)
            {
                if (!_layers.ContainsKey(mark.VfkElement.TYPPPD_KOD))
                {
                    _layers[mark.VfkElement.TYPPPD_KOD] = new VfkDrawinLayer(mark.VfkElement);
                }

                ((VfkDrawinLayer)_layers[mark.TYPPPD_KOD]).AddVfkObjectFast(mark);
            }
        }

        private void addSimpleNumbers()
        {
            List<VfkText> texts = VFKMain.GetDrawingTextObjects();
            foreach (var text in texts)
            {
                if (!_layers.ContainsKey(text.VfkElement.TYPPPD_KOD))
                {
                    _layers[text.VfkElement.TYPPPD_KOD] = new VfkDrawinLayer(text.VfkElement);
                }

                ((VfkDrawinLayer)_layers[text.VfkElement.TYPPPD_KOD]).AddVfkObjectFast(text);
            }
        }

        
        
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Name", Name);
            info.AddValue("Visible", Visible);
            info.AddValue("Enalbed", Enabled);
            info.AddValue("ViewPointEnable", ViewPointEnable);
            info.AddValue("Color", Color);
            info.AddValue("Width", Width);
            info.AddValue("VFKCore", VFKMain);
        }

        public VFKDrawingLayerMain(SerializationInfo info, StreamingContext ctxt)
        {
            Name = info.GetString("Name");
            Visible = info.GetBoolean("Visible");
            Enabled = info.GetBoolean("Enalbed");
            ViewPointEnable = info.GetBoolean("ViewPointEnable");
            Color = (Color)info.GetValue("Color", typeof(Color));
            Width = (float)info.GetValue("Width", typeof(float));
            VFKMain = info.GetValue("VFKCore", typeof(VFKMain)) as VFKMain;
        }

        
        
        public void OnDeserialization(object sender)
        {
            _mActivePoints = new VfkActivePointCollection(VFKMain);
            AddVFKObjects();
        }

        
        
        public bool ShowVfkLayerManager()
        {
            var dialog = new VfkLayerManager();
            var vfkElements = Singletons.VFKElements;
            foreach (var layer in _layers)
            {
                dialog.AddElement(vfkElements.GetElement(layer.Key), layer.Value.Visible);
            }

            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                var elements = dialog.GetElements();
                foreach (var element in elements)
                {
                    ICanvasLayer layer;
                    if (_layers.TryGetValue(element.Object.TYPPPD_KOD, out layer))
                        layer.Visible = element.Object2;
                }

                return true;
            }

            return false;
        }

            }
}