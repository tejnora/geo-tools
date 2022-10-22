using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using CAD.Canvas;
using DxfLibrary;
using GeoBase.Utils;

namespace CAD.Export
{
    class DxfExport : IExport
    {
        #region Constructor
        public DxfExport(string fileName)
        {
            _fileName = fileName;
        }
        #endregion
        #region Fields
        private string _fileName;
        private DxfLibrary.Document _document;
        private Table _layers;
        private Table _styles;
        private Stack<ICanvasLayer> _layersStack = new Stack<ICanvasLayer>();
        #endregion
        #region IExport
        public void Init()
        {
            _document = new DxfLibrary.Document();
            Tables tables = new Tables();
            _document.SetTables(tables);
            _layers = new Table("LAYER");
            tables.AddTable(_layers);
            _styles = new Table("STYLE");
            tables.AddTable(_styles);
        }
        public void Finish()
        {
            FileStream f1 = new FileStream(_fileName, FileMode.Create);
            Writer.Write(_document, f1);
            f1.Close();
        }

        public void SinkLayer(ICanvasLayer layer)
        {
            var color = layer.Color;
            var dxfLayer = new Layer(layer.Name, Colors.GetNearesColorIndex(color.R, color.G, color.B), "CONTINUOUS");
            _layers.AddTableEntry(dxfLayer);
            _layersStack.Push(layer);
        }
        public void RiseLayer()
        {
            _layersStack.Pop();
        }
        
        public void SetTopLayerName(string name)
        {
            _layersStack.Peek().Name = name;
        }

        public string GetTopLayerName()
        {
            return _layersStack.Peek().Name;
        }

        public void AddLine(UnitPoint p1, UnitPoint p2, Color color, double width)
        {
            Line line = new Line(_layersStack.Peek().Name, p1.X, p1.Y, p2.X, p2.Y);
            line.Color = Colors.GetNearesColorIndex(color.R, color.G, color.B);
            _document.Add(line);
        }
        public void AddPolyline(ref UnitPoint[] points, System.Drawing.Color color, double width)
        {
            PolyLine polyLine = new PolyLine(_layersStack.Peek().Name,PolyLine.CurvesSmoothSurfaceType.NoSmooth);
            polyLine.Color = Colors.GetNearesColorIndex(color.R, color.G, color.B);
            foreach (var point in points)
                polyLine.AddVertex(point.X, point.Y);
            _document.Add(polyLine);
        }

        public void AddArc(UnitPoint centerPoint, double radius, double startAngle, double endAngle, Color color, double width)
        {
            Arc arc = new Arc(centerPoint.X, centerPoint.Y, radius, startAngle, endAngle, _layersStack.Peek().Name);
            arc.Color = Colors.GetNearesColorIndex(color.R, color.G, color.B);
            _document.Add(arc);
        }

        public void AddPath(PathImpl path, Color color, double width)
        {
            foreach (PathSegment item in path)
            {
                switch (item.SegmentType)
                {
                    case PathSegment.SegmentTypes.Line:
                        {
                            var ls = (LineSegment)item;
                            AddLine(ls.P1, ls.P2, color, width);
                        } break;
                    case PathSegment.SegmentTypes.Arc:
                        {
                            var a = (ArcSegment)item;
                            AddArc(a.Center, a.Radius, a.StartAngle, a.Angle, color, width);
                        } break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void AddText(string text, double x, double y, double height, string styleName, Color color, double rotation)
        {
            var textBlock = new Text(text, x, y, height, _layersStack.Peek().Name);
            textBlock.Color = Colors.GetNearesColorIndex(color.R, color.G, color.B);
            textBlock.SetStyleName(styleName);
            textBlock.SetAngleRotation(rotation);
            _document.Add(textBlock);
        }

        public struct Style
        {
            public bool Shape;
            public double Height;
            public double Width;
            public double ObliqueAngle;
            public bool Backward;
            public bool Upsidedown;
            public double LastHeightUsed;
            public string PrimaryFontFile;
        }

        private Dictionary<Style, string> _usedStyle = new Dictionary<Style, string>();
        public string AddStyle(bool shape, double height, double width, double obliqueAngle, bool backward, bool upsidedown,
                double lastHeightUsed, string primaryFontFile)
        {
            Style style = new Style
                              {
                                  Shape = shape,
                                  Backward = backward,
                                  Height = height,
                                  LastHeightUsed = lastHeightUsed,
                                  ObliqueAngle = obliqueAngle,
                                  PrimaryFontFile = primaryFontFile,
                                  Upsidedown = upsidedown,
                                  Width = width
                              };
            if(_usedStyle.ContainsKey(style))
                return _usedStyle[style];
            string name = primaryFontFile;
            int counter = 0;
            while(_usedStyle.ContainsValue(name))
            {
                int index = name.IndexOf('_');
                if(index>0)
                    name = name.Substring(index) + "_" + counter;
                else
                    name = name + "_" + counter;
                counter++;
            }
            _usedStyle[style] = name;
            var dxfStyle = new DxfLibrary.Style(name, shape, height, width, obliqueAngle, backward,
                                                             upsidedown, lastHeightUsed, primaryFontFile);
            dxfStyle.AddMicrostationExtendetData(primaryFontFile);
            _styles.AddTableEntry(dxfStyle);

            return name;
        }

        public void AddPoint(double x, double y, Color color)
        {
            var line = new DxfLibrary.Point(_layersStack.Peek().Name, x, y);
            line.Color = Colors.GetNearesColorIndex(color.R, color.G, color.B);
            _document.Add(line);
        }
        #endregion
    }
}
