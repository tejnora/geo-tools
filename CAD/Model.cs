using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Windows;
using CAD.Export;
using CAD.UITools;
using CAD.Utils;
using CAD.Canvas.Layers;
using CAD.Canvas.DrawTools;
using CAD.VFK;
using CAD.VFK.DrawTools;
using CAD.VFK.GUI;
using GeoBase.Localization;
using GeoBase.Utils;
using VFK;

namespace CAD.Canvas
{
    public class DataModel : IModel
    {
        #region Constructor
        public DataModel()
        {
            _toolTypes.Clear();
            _toolTypes[DrawToolBar.Line.Name] = typeof(Line);
            _toolTypes[Circle.ObjectType] = typeof(Circle);
            _toolTypes[DrawToolBar.Arc.Name] = typeof(Arc);
            _toolTypes[DrawToolBar.Arc3Points.Name] = typeof(Arc3Point);
            _toolTypes[DrawToolBar.TextBox.Name] = typeof(TextBox);

            _toolTypes[VfkToolBar.VfkMultiLine.Name] = typeof(VfkMultiLineType);
            _toolTypes[VfkToolBar.VfkActivePoint.Name] = typeof(VfkActivePoint);
            _toolTypes[VfkToolBar.VfkText.Name] = typeof(VfkText);
            _toolTypes[VfkToolBar.VfkMark.Name] = typeof(VfkMark);
            DefaultLayer();
            _centerPoint = new UnitPoint(0, 0);
            VfkMain = null;
        }
        #endregion
        #region Property & Fields
        static Dictionary<string, Type> _toolTypes = new Dictionary<string, Type>();
        private Dictionary<string, IDrawObject> _drawObjectTypes = new Dictionary<string, IDrawObject>();
        private Dictionary<string, IEditTool> m_editTools = new Dictionary<string, IEditTool>();
        private UndoRedoBuffer _undoBuffer = new UndoRedoBuffer();
        private UnitPoint _centerPoint = UnitPoint.Empty;
        private double _zoom = 0.01f;
        private GridLayer _gridLayer = new GridLayer();
        private BackgroundLayer _backgroundLayer = new BackgroundLayer();
        private List<ICanvasLayer> _layers = new List<ICanvasLayer>();
        private ImageBackgroundLayer _imageBackgroundLayer = new ImageBackgroundLayer();
        private ICanvasLayer _activeLayer;
        private readonly Dictionary<IDrawObject, bool> _selection = new Dictionary<IDrawObject, bool>();
        private UInt32 _slaveCounter;
        private readonly List<Color> _colors = new List<Color>{ Color.White, Color.Black, Color.Red, Color.Yellow };
        private readonly List<double> _widths = new List<double>{ 0.001, 0.002, 0.003, 0.004, 0.005, 0.006, 0.007, 0.008 };
        public bool IsDirty
        {
            get { return _undoBuffer.Dirty; }
        }
        [XmlSerializable]
        public UnitPoint CenterPoint
        {
            get { return _centerPoint; }
            set { _centerPoint = value; }
        }
        public VFKMain VfkMain
        {
            get;
            set;
        }
        private VFKDrawingLayerMain _vfkDrawingLayerMain;
        #endregion
        #region IModel
        [XmlSerializable]
        public double Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }
        public ICanvasLayer BackgroundLayer
        {
            get { return _backgroundLayer; }
        }
        public ICanvasLayer ImageBackgrounLayer
        {
            get { return _imageBackgroundLayer; }
        }
        public ICanvasLayer GridLayer
        {
            get { return _gridLayer; }
        }
        public ICanvasLayer[] Layers
        {
            get { return _layers.ToArray(); }
        }
        public ICanvasLayer ActiveLayer
        {
            get
            {
                if (_activeLayer == null)
                    _activeLayer = _layers[0];
                return _activeLayer;
            }
            set
            {
                _activeLayer = value;
            }
        }
        public ICanvasLayer GetLayer(string id)
        {
            foreach (ICanvasLayer layer in _layers)
            {
                if (layer.Id == id)
                    return layer;
            }
            return null;
        }
        public IDrawObject CreateObject(string type, UnitPoint point, ISnapPoint snappoint)
        {
            var layer = ActiveLayer;
            if (layer.Enabled == false)
                return null;
            DrawObjectBase newobj = CreateObject(type);
            if (newobj != null)
            {
                newobj.Layer = layer;
                newobj.InitializeFromModel(point, layer, snappoint);
            }
            return newobj as IDrawObject;
        }
        public IDrawObject CreateVFKObject(string type, UnitPoint point, ISnapPoint snappoint)
        {
            var layer = ActiveLayer;
            if (layer.Enabled == false)
                return null;
            DrawObjectBase newobj = CreateObject(type);
            if (newobj != null && newobj is IVFKTool)
            {
                newobj.Layer = layer;
                newobj.InitializeFromModel(point, layer, snappoint);
            }
            return newobj as IDrawObject;
        }
        public void AddObject(ICanvasLayer layer, IDrawObject drawobject)
        {
            if (drawobject is IObjectEditInstance)
                drawobject = ((IObjectEditInstance)drawobject).GetDrawObject();
            if (_undoBuffer.CanCapture)
                _undoBuffer.AddCommand(new EditCommandAdd(layer, drawobject));
            layer.AddObject(drawobject);
        }
        public void AddVFKObject(IDrawObject drawobject)
        {
            if (drawobject is IObjectEditInstance)
                drawobject = ((IObjectEditInstance)drawobject).GetDrawObject();
            _vfkDrawingLayerMain.AddObject(drawobject);
            if (_undoBuffer.CanCapture)
                _undoBuffer.AddCommand(new EditVFKCommandAdd(_vfkDrawingLayerMain, drawobject));
            ((IVFKTool)drawobject).RegisterObject(VfkMain);
        }
        public void DeleteObjects(IEnumerable<IDrawObject> objects)
        {
            EditCommandRemove undocommand = null;
            if (_undoBuffer.CanCapture)
                undocommand = new EditCommandRemove();
            foreach (ICanvasLayer layer in _layers)
            {
                if (layer is VFKDrawingLayerMain) continue;
                List<IDrawObject> removedobjects = ((DrawingLayer)layer).DeleteObjects(objects);
                if (removedobjects != null && undocommand != null)
                    undocommand.AddLayerObjects(layer, removedobjects);
            }
            if (undocommand != null)
                _undoBuffer.AddCommand(undocommand);
        }
        public void DeleteVFKObjects(IEnumerable<IDrawObject> objects, bool silentDeleteObjects)
        {
            EditVFKCommandRemove undocommand = null;
            if (_undoBuffer.CanCapture)
                undocommand = new EditVFKCommandRemove();
            foreach (ICanvasLayer layer in _layers)
            {
                if (layer is VFKDrawingLayerMain)
                {
                    List<IDrawObject> removedobjects = ((VFKDrawingLayerMain)layer).DeleteVFKObjects(objects, silentDeleteObjects);
                    if (removedobjects != null && undocommand != null)
                        undocommand.AddLayerObjects(layer, removedobjects);
                    if (removedobjects != null)
                        foreach (var obj in removedobjects)
                        {
                            ((IVFKTool)obj).DeleteObject(VfkMain);
                        }
                }
                else
                {
                    List<IDrawObject> removedobjects = ((DrawingLayer)layer).DeleteObjects(objects);
                    if (removedobjects != null && undocommand != null)
                        undocommand.AddLayerObjects(layer, removedobjects);
                }
            }
            if (undocommand != null)
                _undoBuffer.AddCommand(undocommand);
        }
        public void MoveObjects(UnitPoint offset, IEnumerable<IDrawObject> objects)
        {
            if (_undoBuffer.CanCapture)
                _undoBuffer.AddCommand(new EditCommandMove(offset, objects));
            foreach (IDrawObject obj in objects)
                obj.Move(offset);
        }
        public void CopyObjects(UnitPoint offset, IEnumerable<IDrawObject> objects)
        {
            ClearSelectedObjects();
            List<IDrawObject> newobjects = new List<IDrawObject>();
            foreach (IDrawObject obj in objects)
            {
                IDrawObject newobj = obj.Clone();
                newobjects.Add(newobj);
                newobj.Move(offset);
                ((DrawingLayer)ActiveLayer).AddObject(newobj);
                AddSelectedObject(newobj);
            }
            if (_undoBuffer.CanCapture)
                _undoBuffer.AddCommand(new EditCommandAdd(ActiveLayer, newobjects));
        }
        public void MoveNodes(UnitPoint position, IEnumerable<INodePoint> nodes)
        {
            if (_activeLayer is IVFKDravingLayerMain)
                return;
            if (_undoBuffer.CanCapture)
                _undoBuffer.AddCommand(new EditCommandNodeMove(nodes));
            foreach (INodePoint node in nodes)
            {
                node.SetPosition(position);
                node.Finish();
            }
        }
        public IEditTool GetEditTool(string edittoolid)
        {
            if (m_editTools.ContainsKey(edittoolid))
                return m_editTools[edittoolid].Clone();
            return null;
        }
        public void AfterEditObjects(IEditTool edittool)
        {
            edittool.Finished();
            if (_undoBuffer.CanCapture)
                _undoBuffer.AddCommand(new EditCommandEditTool(edittool));
        }
        public List<IDrawObject> GetHitObjects(ICanvas canvas, Rect selection, bool anyPoint)
        {
            List<IDrawObject> selected = new List<IDrawObject>();
            foreach (ICanvasLayer layer in _layers)
            {
                if (layer.Visible == false)
                    continue;
                layer.GetHitObjects(selected, canvas, selection, anyPoint);
            }
            return selected;
        }
        public List<IDrawObject> GetHitObjects(ICanvas canvas, UnitPoint point)
        {
            List<IDrawObject> selected = new List<IDrawObject>();
            foreach (ICanvasLayer layer in _layers)
            {
                if (layer.Visible == false)
                    continue;
                layer.GetHitObjects(selected, canvas, point);
            }
            return selected;
        }
        public bool IsSelected(IDrawObject drawobject)
        {
            return _selection.ContainsKey(drawobject);
        }
        public void AddSelectedObject(IDrawObject drawobject)
        {
            DrawObjectBase obj = drawobject as DrawObjectBase;
            RemoveSelectedObject(drawobject);
            _selection[drawobject] = true;
            if (obj != null) obj.Selected = true;
        }
        public void RemoveSelectedObject(IDrawObject drawobject)
        {
            if (_selection.ContainsKey(drawobject))
            {
                DrawObjectBase obj = drawobject as DrawObjectBase;
                if (obj != null) obj.Selected = false;
                _selection.Remove(drawobject);
            }
        }
        public IEnumerable<IDrawObject> SelectedObjects
        {
            get
            {
                return _selection.Keys;
            }
        }
        public int SelectedCount
        {
            get { return _selection.Count; }
        }
        public void ClearSelectedObjects()
        {
            IEnumerable<IDrawObject> x = SelectedObjects;
            foreach (IDrawObject drawobject in x)
            {
                DrawObjectBase obj = drawobject as DrawObjectBase;
                if (obj != null) obj.Selected = false;
            }
            _selection.Clear();
        }
        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, Type[] runningsnaptypes, Type usersnaptype)
        {
            List<IDrawObject> objects = GetHitObjects(canvas, point);
            if (objects.Count == 0)
                return null;

            foreach (IDrawObject obj in objects)
            {
                ISnapPoint snap;
                if (_activeLayer is IVFKDravingLayerMain)
                    snap = obj.SnapPoint(canvas, point, objects, null, typeof(VfkSnapPoint));
                else
                    snap = obj.SnapPoint(canvas, point, objects, runningsnaptypes, usersnaptype);
                if (snap != null)
                    return snap;
            }
            return null;
        }
        public List<Color> getColors()
        {
            return _colors;
        }
        public List<double> getWidths()
        {
            return _widths;
        }
        public bool CanUndo()
        {
            return _undoBuffer.CanUndo;
        }
        public bool DoUndo()
        {
            return _undoBuffer.DoUndo(this);
        }
        public bool CanRedo()
        {
            return _undoBuffer.CanRedo;

        }
        public bool DoRedo()
        {
            return _undoBuffer.DoRedo(this);
        }
        public void StopUndoRedo()
        {
            _undoBuffer.StopUndoRedo = true;
        }
        public void StarUndoRedo()
        {
            _undoBuffer.StopUndoRedo = false;
        }
        public void Export(string fileName, ExportType type)
        {
            IExport export;
            switch (type)
            {
                case ExportType.Dxf:
                    export = new DxfExport(fileName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
            try
            {
                export.Init();
                foreach (var layer in _layers)
                    layer.Export(export);
                export.Finish();
            }
            catch (Exception e)
            {
                ResourceParams resourceParams = new ResourceParams();
                resourceParams.Add("msg",e.Message);
                LanguageDictionary.Current.ShowMessageBox("107", resourceParams, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
        #region Methods
        static public IDrawObject NewDrawObject(string objecttype)
        {
            if (_toolTypes.ContainsKey(objecttype))
            {
                string type = _toolTypes[objecttype].ToString();
                return Assembly.GetExecutingAssembly().CreateInstance(type) as IDrawObject;
            }
            return null;
        }
        DrawObjectBase CreateObject(string objecttype)
        {
            if (_drawObjectTypes.ContainsKey(objecttype))
            {
                return _drawObjectTypes[objecttype].Clone() as DrawObjectBase;
            }
            return null;
        }
        public void AddEditTool(string key, IEditTool tool)
        {
            m_editTools.Add(key, tool);
        }
        public void RemoveLayer(ICanvasLayer aLayer)
        {
            _layers.Remove(aLayer);
        }
        public void AddLayer(ICanvasLayer aLayer)
        {
            _layers.Add(aLayer);
        }
        public void AddDrawTool(string key, IDrawObject drawtool)
        {
            _drawObjectTypes[key] = drawtool;
        }
        void DefaultLayer()
        {
            _layers.Clear();
            _layers.Add(new DrawingLayer("Main", "Main Layer", Color.White, 0.001f));
        }
        public IDrawObject GetFirstSelected()
        {
            if (_selection.Count > 0)
            {
                Dictionary<IDrawObject, bool>.KeyCollection.Enumerator e = _selection.Keys.GetEnumerator();
                e.MoveNext();
                return e.Current;
            }
            return null;
        }
        public string FindUniqueId()
        {
            return string.Format("SlaveId{0}", _slaveCounter++);
        }

        #endregion
        #region Serializace & Deserialize
        public void Save(string filename)
        {
            try
            {
                using (Stream stream = File.Open(filename, FileMode.Create))
                {
                    BinaryWriter bw = new BinaryWriter(stream);
                    int layerCount = 0;
                    foreach (ICanvasLayer layer in _layers)
                    {
                        if (layer is ISerializable)
                            layerCount++;
                    }
                    bw.Write(layerCount);
                    BinaryFormatter bin = new BinaryFormatter();
                    foreach (ICanvasLayer layer in _layers)
                    {
                        if (layer is ISerializable)
                            bin.Serialize(stream, layer);
                    }
                }
                _undoBuffer.Dirty = false;
            }
            catch (IOException)
            {
                throw new UnExpectException();
            }

        }
        public bool Load(string filename)
        {
            try
            {
                using (Stream stream = File.Open(filename, FileMode.Open))
                {
                    BinaryReader bw = new BinaryReader(stream);
                    int layerCount = bw.ReadInt32();
                    BinaryFormatter bin = new BinaryFormatter();
                    _layers.Clear();
                    for (int i = 0; i < layerCount; i++)
                    {
                        var canvasLayer = (ICanvasLayer)bin.Deserialize(stream);
                        if (canvasLayer is VFKDrawingLayerMain)
                        {
                            _vfkDrawingLayerMain = (VFKDrawingLayerMain)canvasLayer;
                            VfkMain = _vfkDrawingLayerMain.VFKMain;
                        }
                        _layers.Add(canvasLayer);
                    }
                }
                return true;
            }
            catch (IOException)
            {
                DefaultLayer();
            }
            return false;
        }
        #endregion
        #region IVFKModel
        public void OnImportVfk(VFKDataContext aDataContext)
        {
            if (_layers[0] is VFKDrawingLayerMain)
                _layers.RemoveAt(0);
            VfkMain = new VFKMain(aDataContext,this);
            VfkMain.VFKOpenFile();
            _vfkDrawingLayerMain = new VFKDrawingLayerMain(VfkMain);
            _layers.Insert(0, _vfkDrawingLayerMain);
            _activeLayer = _layers[0];
        }
        public void OnRemoveVfkData()
        {
            if (VfkMain != null)
            {
                VfkMain = null;
                _vfkDrawingLayerMain = null;
                _layers.RemoveAt(0);
            }
        }
        public bool IsImportedVfkFile()
        {
            return VfkMain != null;
        }
        public bool ViewPointEnable
        {
            get
            {
                if (_vfkDrawingLayerMain == null)
                    return false;
                return _vfkDrawingLayerMain.ViewPointEnable;
            }
            set
            {
                _vfkDrawingLayerMain.ViewPointEnable = value;
                
            }
        }
        public VfkActivePointCollection VfkActivePoints
        {
            get { return _vfkDrawingLayerMain.VFKActivePoints; }
        }

        public bool ShowVfkLayerManager()
        {
            return _vfkDrawingLayerMain.ShowVfkLayerManager();
        }
        #endregion
    }
}
