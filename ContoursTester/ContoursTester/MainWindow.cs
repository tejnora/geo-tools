using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using DxfLibrary;
using GeoCalculations.BasicTypes;
using GeoCalculations.Contours;
using GeoCalculations.Curves;
using GeoCalculations.Triangulation;
using System.Drawing.Drawing2D;
using Point = GeoCalculations.BasicTypes.Point;

namespace ContoursViewer
{
    public partial class MainWindow : Form
    {
        static class ViewOptions
        {
            static public readonly Pen PointColor = new Pen(Color.White);
            static public readonly Pen DelaunayTriangulation = new Pen(Color.Red);
            static public readonly Pen ContoursColor = new Pen(Color.SandyBrown);
            static public readonly Font TextFont = new Font("Arial", 10);
            static public readonly Brush TextBrush = new SolidBrush(Color.White);
            public const float PointRectSize = 2;
        }


        public MainWindow()
        {
            InitializeComponent();
            _yoffset = menuStrip1.Height;
            _showContours.Checked = true;
            _showDelaunayTriangulation.Checked = false;
            _showNumberOfPoints.Checked = false;
            _saveFileDialog.Filter = @"Drawing Interchange Format|*.dxf";
            _openFileDialog.Filter = @"Cordinates (Number,Y,X,Height)|*.txt";
#if DEBUG
            PrepareDate(@"e:\ss.txt");
#endif
        }

        IList<ContourNode> _vertices;
        double _minHeight, _maxHeight;
        double _minCordinateX, _maxCordinateX, _minCordinateY, _maxCordinateY;
        float _yoffset;

        VoronoiMesh<ContourNode, ContourTriangulationCell<ContourNode>, VoronoiEdge<ContourNode, ContourTriangulationCell<ContourNode>>> _voronoiMesh;
        ContoursCompouser _contoursPathCompouser;
        List<ContoursCompouser.ContourAtElevation> _contoursPaths;


        PointF _canvasOffset;
        float _canvasScale = 1;
        PointF _lastMousePos;
        bool _mouseDownPressed;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(Color.Black);
            if (_minCordinateX == _maxCordinateX || _minCordinateY == _maxCordinateY)
                return;
            var viewMatrix = GetViewMatrix();

            PaintPoints(viewMatrix, e.Graphics);

            if (_showDelaunayTriangulation.Checked)
                PaintDelaunayTriangulation(viewMatrix, e.Graphics);

            if (_showContours.Checked)
                PaintContours(viewMatrix, e.Graphics);
        }

        void PaintPoints(Matrix viewMatrix, Graphics graphics)
        {
            var points = new PointF[_vertices.Count];
            for (var i = 0; i < _vertices.Count; i++)
            {
                points[i] = new PointF(-(float)_vertices[i].CordinateY, (float)_vertices[i].CordinateX);
            }
            viewMatrix.TransformPoints(points);
            for (var i = 0; i < _vertices.Count; i++)
            {
                var point = points[i];
                graphics.DrawRectangle(ViewOptions.PointColor, point.X - ViewOptions.PointRectSize * 0.5f, point.Y - ViewOptions.PointRectSize * 0.5f, ViewOptions.PointRectSize, ViewOptions.PointRectSize);
                if (_showNumberOfPoints.Checked)
                    graphics.DrawString(_vertices[i].PointNumber, ViewOptions.TextFont, ViewOptions.TextBrush, point);
            }
        }

        void PaintDelaunayTriangulation(Matrix viewMatrix, Graphics graphics)
        {
            var trianglePoints = new PointF[3];
            foreach (var triangle in _voronoiMesh.Vertices)
            {
                trianglePoints[0] = new PointF((float)-triangle.Vertices[0].Position[1],
                                               (float)triangle.Vertices[0].Position[0]);
                trianglePoints[1] = new PointF((float)-triangle.Vertices[1].Position[1],
                                               (float)triangle.Vertices[1].Position[0]);
                trianglePoints[2] = new PointF((float)-triangle.Vertices[2].Position[1],
                                               (float)triangle.Vertices[2].Position[0]);
                viewMatrix.TransformPoints(trianglePoints);
                graphics.DrawLine(ViewOptions.DelaunayTriangulation, trianglePoints[0], trianglePoints[1]);
                graphics.DrawLine(ViewOptions.DelaunayTriangulation, trianglePoints[1], trianglePoints[2]);
                graphics.DrawLine(ViewOptions.DelaunayTriangulation, trianglePoints[2], trianglePoints[0]);
            }
        }

        void PaintContours(Matrix viewMatrix, Graphics graphics)
        {
            if (_contoursPaths == null) return;
            foreach (var coutour in _contoursPaths)
            {
                if (coutour == null || coutour.Path == null || coutour.Path.Count == 0) continue;
                var contourLine = new PointF[2];
                var contourBezierSegment = new PointF[4];
                for (var i = 0; i < coutour.Path.Count; i++)
                {
                    var currentPoint = new PointF((float)-coutour.Path[i].Y, (float)coutour.Path[i].X);
                    switch (coutour.Path.GetPointType(i))
                    {
                        case Path<Point>.PathTypes.Move:
                            contourLine[0] = currentPoint;
                            break;
                        case Path<Point>.PathTypes.Line:
                            contourLine[1] = currentPoint;
                            viewMatrix.TransformPoints(contourLine);
                            graphics.DrawLine(ViewOptions.ContoursColor, contourLine[0], contourLine[1]);
                            contourLine[0] = currentPoint;
                            break;
                        case Path<Point>.PathTypes.Bezier1:
                            {
                                contourBezierSegment[0] = contourLine[0];
                                contourBezierSegment[1] = currentPoint;
                                contourBezierSegment[2] = new PointF((float)-coutour.Path[i + 1].Y, (float)coutour.Path[i + 1].X);
                                contourBezierSegment[3] = new PointF((float)-coutour.Path[i + 2].Y, (float)coutour.Path[i + 2].X);
                                viewMatrix.TransformPoints(contourBezierSegment);
                                graphics.DrawBezier(ViewOptions.ContoursColor, contourBezierSegment[0], contourBezierSegment[1], contourBezierSegment[2], contourBezierSegment[3]);
                                contourLine[0] = new PointF((float)-coutour.Path[i + 2].Y, (float)coutour.Path[i + 2].X);
                                i += 2;
                            } break;
                        default:
                            Debug.Assert(true);
                            break;
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _lastMousePos = e.Location;
            _mouseDownPressed = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _mouseDownPressed = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_mouseDownPressed || e.Button != MouseButtons.Left) return;
            _canvasOffset = new PointF(_canvasOffset.X + e.Location.X - _lastMousePos.X, _canvasOffset.Y + e.Location.Y - _lastMousePos.Y);
            _lastMousePos = e.Location;
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            _canvasScale += e.Delta / 240f;
            if (_canvasScale <= 0)
                _canvasScale = 0.1f;
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        Matrix GetViewMatrix()
        {
            var viewMatrix = new Matrix();
            var cordinateRangeX = (float)(_maxCordinateX - _minCordinateX);
            var cordinateRangeY = (float)(_maxCordinateY - _minCordinateY);
            var scaleY = (Width - (_yoffset + ViewOptions.PointRectSize)) / cordinateRangeY;
            var scaleX = (Height - ViewOptions.PointRectSize) / cordinateRangeX;
            viewMatrix.Translate(ViewOptions.PointRectSize * 0.5f, _yoffset + ViewOptions.PointRectSize * 0.5f);
            viewMatrix.Translate(_canvasOffset.X, _canvasOffset.Y);
            var scale = Math.Min(scaleX, scaleY) * _canvasScale;
            viewMatrix.Scale(scale, scale);
            viewMatrix.Translate((float)(_minCordinateY + cordinateRangeY), -(float)(_minCordinateX));
            return viewMatrix;
        }

        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = _openFileDialog.ShowDialog();
            if (result == DialogResult.Cancel) return;
            PrepareDate(_openFileDialog.FileName);
            Invalidate();
        }

        void PrepareDate(string path)
        {
            try
            {
                _vertices = ContoursFileParser.Parse(path);
                FindMinMaxValues();
                _voronoiMesh = VoronoiMesh.Create<ContourNode, ContourTriangulationCell<ContourNode>>(_vertices);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), @"Any error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _minCordinateX = _minCordinateY = _maxCordinateX = _maxCordinateY = 0;
            }
            CreateContours();
        }

        void CreateContours()
        {
            try
            {
                double interval;
                if (!GetContoursInterval(_minHeight, _maxHeight, out interval))
                    return;
                var coutourCreater = new CotoursCreater(interval, _minHeight, _maxHeight);
                coutourCreater.CreateContours(_voronoiMesh.Vertices);
                _contoursPathCompouser = coutourCreater.ContoursCompouser;
                _contoursPaths = _contoursPathCompouser.GetPaths();
                if (_showBezierSegments.Checked)
                {
                    ContoursPathToBezierSegments(_contoursPaths);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), @"Contours creator failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _minCordinateX = _minCordinateY = _maxCordinateX = _maxCordinateY = 0;
            }
        }

        bool GetContoursInterval(double minValue, double maxValue, out double interval)
        {
            var dialog = new ContoursIntervalDialog(minValue, maxValue);
            interval = default(double);
            if (dialog.ShowDialog(this) == DialogResult.Cancel)
                return false;
            interval = dialog.Interval;
            return true;
        }

        void ContoursPathToBezierSegments(List<ContoursCompouser.ContourAtElevation> contours)
        {
            for (var i = 0; i < contours.Count; i++)
            {
                contours[i].Path = PathTo.ToBezierSegments(contours[i].Path);
            }
        }

        void FindMinMaxValues()
        {
            if (_vertices.Count == 0)
            {
                _minCordinateX = _maxCordinateX = _minCordinateY = _maxCordinateY = 0;
                _minHeight = _maxHeight = 0;
                return;
            }
            _minCordinateX = _maxCordinateX = _vertices[0].CordinateX;
            _minCordinateY = _maxCordinateY = _vertices[0].CordinateY;
            _minHeight = _maxHeight = _vertices[0].Height;
            for (var i = 0; i < _vertices.Count; i++)
            {
                var vertex = _vertices[i];
                _minCordinateX = Math.Min(_minCordinateX, vertex.CordinateX);
                _maxCordinateX = Math.Max(_maxCordinateX, vertex.CordinateX);
                _minCordinateY = Math.Min(_minCordinateY, vertex.CordinateY);
                _maxCordinateY = Math.Max(_maxCordinateY, vertex.CordinateY);
                _minHeight = Math.Min(_minHeight, vertex.Height);
                _maxHeight = Math.Max(_maxHeight, vertex.Height);
            }
        }

        void showNumberOfPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _showNumberOfPoints.Checked = !_showNumberOfPoints.Checked;
            Invalidate();
        }

        void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _showDelaunayTriangulation.Checked = !_showDelaunayTriangulation.Checked;
            Invalidate();
        }

        void showToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _showContours.Checked = !_showContours.Checked;
            Invalidate();
        }

        void showBezierSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _showBezierSegments.Checked = !_showBezierSegments.Checked;
            if (_contoursPathCompouser != null)
            {
                _contoursPaths = _contoursPathCompouser.GetPaths();
                if (_showBezierSegments.Checked)
                {
                    ContoursPathToBezierSegments(_contoursPaths);
                }
            }
            else
            {
                _contoursPaths = null;
            }
            Invalidate();
        }

        void changeContoursIntervalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateContours();
            Invalidate();
        }

        void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_saveFileDialog.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                using (var fileStream = new FileStream(_saveFileDialog.FileName, FileMode.Create))
                {
                    var document = CreateDxfDocument();
                    Writer.Write(document, fileStream);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(@"File could not be saved.", @"Save error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        ﻿Document CreateDxfDocument()
        ﻿{
        ﻿    var doc = new Document();
        ﻿    var tables = new Tables();
        ﻿    doc.SetTables(tables);
             var layers = new Table("LAYER");
             tables.AddTable(layers);
             DxfExportPoints(doc, layers);
             if (_showDelaunayTriangulation.Checked)
            ﻿    DxfExportTriangles(doc, layers);
             if (_showContours.Checked)
            ﻿    DxfExportContourLines(doc, layers);
        ﻿    return doc;
        ﻿}

         void DxfExportTriangles(Document doc, Table layers)
         {
             if (_voronoiMesh == null) return;
             var color = ViewOptions.DelaunayTriangulation.Color;
             const string delaunayTranglesName = "Delaunay Trangles";
             layers.AddTableEntry(new Layer(delaunayTranglesName, Colors.GetNearesColorIndex(color.R, color.G, color.B), "CONTINUOUS"));
             foreach (var triangle in _voronoiMesh.Vertices)
             {
                 var polyLine = new PolyLine(delaunayTranglesName, PolyLine.CurvesSmoothSurfaceType.NoSmooth);
                 polyLine.AddVertex(-triangle.Vertices[0].CordinateY, -triangle.Vertices[0].CordinateX);
                 polyLine.AddVertex(-triangle.Vertices[1].CordinateY, -triangle.Vertices[1].CordinateX);
                 polyLine.AddVertex(-triangle.Vertices[2].CordinateY, -triangle.Vertices[2].CordinateX);
                 polyLine.AddVertex(-triangle.Vertices[0].CordinateY, -triangle.Vertices[0].CordinateX);
                 doc.Add(polyLine);
             }
         }

         void DxfExportPoints(Document document, Table layers)
         {
             if (_vertices == null) return;
             var color = ViewOptions.PointColor.Color;
             const string pointsLayerName = "Points";
             const string pointsNameLayerName = "Name of Points";
             layers.AddTableEntry(new Layer(pointsLayerName, Colors.GetNearesColorIndex(color.R, color.G, color.B), "CONTINUOUS"));
             layers.AddTableEntry(new Layer(pointsNameLayerName, Colors.GetNearesColorIndex(color.R, color.G, color.B), "CONTINUOUS"));
             const double delta = ViewOptions.PointRectSize * 0.5 * 0.2;
             foreach (var vertex in _vertices)
             {
                 document.Add(new DxfLibrary.Line(pointsLayerName, -vertex.CordinateY - delta, -vertex.CordinateX, -vertex.CordinateY + delta, -vertex.CordinateX));
                 document.Add(new DxfLibrary.Line(pointsLayerName, -vertex.CordinateY, -vertex.CordinateX - delta, -vertex.CordinateY, -vertex.CordinateX + delta));
                 if (_showContours.Checked)
                 {
                     document.Add(new Text(vertex.PointNumber, -vertex.CordinateY, -vertex.CordinateX, 0.7, pointsNameLayerName));
                 }
             }
         }

         void DxfExportContourLines(Document document, Table layers)
         {
        ﻿    var color = ViewOptions.ContoursColor.Color;
             if (_contoursPaths == null) return;
             foreach (var cotour in _contoursPaths)
             {
                 if (cotour == null || cotour.Path == null || cotour.Path.Count == 0) continue;
                 var contourLinesLabel = string.Format(CultureInfo.InvariantCulture, "Contour Lines at: {0:0.###}", cotour.Elevation);
                 layers.AddTableEntry(new Layer(contourLinesLabel, Colors.GetNearesColorIndex(color.R, color.G, color.B), "CONTINUOUS"));
                 var contourLine = new PointF[2];
                 for (var i = 0; i < cotour.Path.Count; i++)
                 {
                     var currentPoint = new PointF((float)-cotour.Path[i].Y, (float)-cotour.Path[i].X);
                     switch (cotour.Path.GetPointType(i))
                     {
                         case Path<Point>.PathTypes.Move:
                             contourLine[0] = currentPoint;
                             break;
                         case Path<Point>.PathTypes.Line:
                             contourLine[1] = currentPoint;
                             document.Add(new DxfLibrary.Line(contourLinesLabel, contourLine[0].X, contourLine[0].Y, contourLine[1].X, contourLine[1].Y));
                             contourLine[0] = currentPoint;
                             break;
                         case Path<Point>.PathTypes.Bezier1:
                             {
                                 var path = new Path2D();
                                 path.MoveTo(new Point(contourLine[0].X, contourLine[0].Y));
                                 path.BezierTo(new Point(currentPoint.X, currentPoint.Y),
                                               new Point(-cotour.Path[i + 1].Y, -cotour.Path[i + 1].X),
                                               new Point(-cotour.Path[i + 2].Y, -cotour.Path[i + 2].X));
                                 var bezier = new Bezier(contourLinesLabel);
                                 bezier.AddBezier(path);
                                 document.Add(bezier);
                                 contourLine[0] = new PointF((float)-cotour.Path[i + 2].Y, (float)-cotour.Path[i + 2].X);
                                 i += 2;
                             } break;
                         default:
                             Debug.Assert(true);
                             break;
                     }
                 }
             }

         }

    }
}
