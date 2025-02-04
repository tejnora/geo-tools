using System;
using System.Collections.Generic;
using System.Windows;
using CAD.Canvas;
using CAD.Export;
using CAD.Utils;
using System.Xml;
using CAD.Images;
using GeoBase.Utils;
using Rectangle = System.Drawing.Rectangle;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace CAD.BackgrounImages
{
    class BackgrounImage : IDrawObject, ISerialize
    {
                public BackgrounImage(string aLocation)
        {
            _imageLocation = aLocation;
        }

        public BackgrounImage()
        {
            _imageLocation = string.Empty;
        }
                        private string _imageLocation;
        private IImage _image = null;
        private Rectangle _imageRect = Rectangle.Empty;
                        public string Id 
        {
            get
            {
                return "BackgrounImage";
            }
        }
        public IDrawObject Clone()
        {
            BackgrounImage b = new BackgrounImage();
            return b;
        }
        public bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            Rect rect = GetBoundingRect(canvas);
            return true;
//            return rect.Contains(new Point(point.X, point.Y));
        }
        public bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint)
        {
            Rect boundingrect = GetBoundingRect(canvas);
            return rect.Contains(boundingrect);
        }
        public void Draw(ICanvas canvas, Rect unitrect)
        {
            initImage();
            if (_image == null) return;
         //   if(_image!=null)
                //canvas.DrawImage(canvas, _image);
        }
        public Rect GetBoundingRect(ICanvas canvas)
        {
            initImage();
            if (_image == null)
                return Rect.Empty;
            return new Rect(_imageRect.X,_imageRect.Y,_imageRect.Width,_imageRect.Height);
        }
        public void OnMouseMove(ICanvas canvas, UnitPoint point)
        { 
        }
        public eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            return eDrawObjectMouseDown.DoneRepeat;
        }
        public void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
        }
        public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }
        public UnitPoint RepeatStartingPoint
        {
            get 
            {
                return new UnitPoint();
            }
        }
        public INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            return null;
        }
        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes, Type usersnaptype)
        {
            return null;
        }
        public void Move(UnitPoint offset)
        {
        }
        public void GetHitObjects(List<IDrawObject> aHitObjects,ICanvas canvas, UnitPoint point)
        {
        }
        public bool getSelectDrawToolCreate()
        {
            return false;
        }
        public string GetInfoAsString()
        {
            return string.Empty;
        }
        private void initImage()
        {
            if (_imageLocation.Length == 0) return;
            if (_image == null)
            {
                _image = new CITReader.CitReader(_imageLocation);
                if (!_image.getTestImage())
                {
                    System.Diagnostics.Debug.Assert(false);
                    _image = null;
                }
            }
        }
        public void Export(IExport export)
        {
        }
                        public void GetObjectData(XmlWriter wr)
        {
        }
        public void AfterSerializedIn()
        {
        }
            }
}
