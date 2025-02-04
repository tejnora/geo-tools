using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using CAD.Canvas;
using CAD.Canvas.DrawTools;
using CAD.Export;
using CAD.UITools;
using CAD.Utils;
using GeoBase.Utils;
using VFK;
using VFK.Tables;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;
using Size = System.Windows.Size;
using SolidBrush = System.Drawing.SolidBrush;
using StringFormat = System.Drawing.StringFormat;

namespace CAD.VFK.DrawTools
{
    public class VfkActivePointCollection : ObservableCollection<VfkActivePoint>
    {
        public VfkActivePointCollection(VFKMain aVfkMain)
        {
            VfkMain = aVfkMain;
            UseModelForModification = true;
        }

        private VFKMain VfkMain { get; }

        public bool UseModelForModification { get; set; }

        protected override void InsertItem(int aIndex, VfkActivePoint aItem)
        {
            if (UseModelForModification)
                VfkMain.Model.AddVFKObject(aItem);
            else
                base.InsertItem(aIndex, aItem);
        }

        protected override void RemoveItem(int aIndex)
        {
            if (UseModelForModification)
                VfkMain.Model.DeleteVFKObjects(new IDrawObject[] { this[aIndex] }, false);
            else
                base.RemoveItem(aIndex);
        }
    }

    public class VfkActivePoint : ActivePoint, IVFKTool, INotifyPropertyChanged
    {
        private readonly SolidBrush _brush = new SolidBrush(Color.Yellow);
        private readonly Font _font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Pixel, 0);

        public VfkActivePoint()
        {
            Color = Color.Yellow;
            DrawCircle = false;
            Width = 0.001f;
        }

        public VfkActivePoint(VfkProxyActivePoint aVfkOwner)
        {
            VfkItem = aVfkOwner;
            Color = Color.Yellow;
            DrawCircle = false;
            Width = 0.001f;
        }

        public override UnitPoint P1
        {
            get => new UnitPoint(-VfkItem.VfkSobrItem.SOURADNICE_Y, -VfkItem.VfkSobrItem.SOURADNICE_X);
            set => throw new UnExpectException();
        }

        public VfkProxyActivePoint VfkItem { get; set; }

        public string PointNumber
        {
            get
            {
                if (VfkItem == null)
                    return string.Empty;
                return VfkItem.VfkSobrItem.CISLO_ZPMZ + "-" + VfkItem.VfkSobrItem.CISLO_BODU;
            }
        }

        public string VfkPointName
        {
            get
            {
                if (VfkItem == null)
                    return string.Empty;
                return VfkItem.PointFullName;
            }
            set
            {
                if (VfkItem.PointFullName != value)
                {
                    VfkItem.PointFullName = value;
                    OnPropertyChanged("VfkPointName");
                    OnPropertyChanged("PointNumber");
                }
            }
        }

        public string VfkSouradniceObrazuY
        {
            get
            {
                if (VfkItem == null)
                    return string.Empty;
                return VfkItem.SouradniceObrazuY;
            }
            set
            {
                if (VfkItem.SouradniceObrazuY != value)
                {
                    VfkItem.SouradniceObrazuY = value;
                    OnPropertyChanged("VfkSouradniceObrazuY");
                }
            }
        }

        public string VfkSouradniceObrazuX
        {
            get
            {
                if (VfkItem == null)
                    return string.Empty;
                return VfkItem.SouradniceObrazuX;
            }
            set
            {
                if (VfkItem.SouradniceObrazuX != value)
                {
                    VfkItem.SouradniceObrazuX = value;
                    OnPropertyChanged("VfkSouradniceObrazuX");
                }
            }
        }

        public string VfkSouradnicePolohyX
        {
            get
            {
                if (VfkItem == null)
                    return string.Empty;
                return VfkItem.VfkSouradnicePolohyX;
            }
            set
            {
                if (VfkItem.VfkSouradnicePolohyX != value)
                {
                    VfkItem.VfkSouradnicePolohyX = value;
                    OnPropertyChanged("VfkSouradnicePolohyX");
                }
            }
        }

        public string VfkSouradnicePolohyY
        {
            get
            {
                if (VfkItem == null)
                    return string.Empty;
                return VfkItem.VfkSouradnicePolohyY;
            }
            set
            {
                if (VfkItem.VfkSouradnicePolohyY != value)
                {
                    VfkItem.VfkSouradnicePolohyY = value;
                    OnPropertyChanged("VfkSouradnicePolohyY");
                }
            }
        }

        public string VfkKkb
        {
            get
            {
                if (VfkItem == null)
                    return string.Empty;
                return VfkItem.VfkKkb;
            }
            set
            {
                if (VfkItem.VfkKkb != value)
                {
                    VfkItem.VfkKkb = value;
                    OnPropertyChanged("VfkKkb");
                }
            }
        }

        public bool IsEnabled
        {
            get
            {
                if (VfkItem == null)
                    return false;
                return !(VfkItem.IsSobrReadOnly || VfkItem.IsSpolReadOnly);
            }
            set { }
        }

        public bool IsSobrEnabled
        {
            get
            {
                if (VfkItem == null)
                    return false;
                return !VfkItem.IsSobrReadOnly;
            }
            set { }
        }

        public bool IsSpolEnabled
        {
            get
            {
                if (VfkItem == null)
                    return false;
                return !VfkItem.IsSpolReadOnly;
            }
            set { }
        }

        public Brush SobrColor
        {
            get
            {
                if (VfkItem.VfkSobrItem.STAV_DAT != 0 && VfkItem.VfkSobrItem.ItemFromImport())
                    return Brushes.Red;
                return Brushes.Black;
            }
            set => throw new UnExpectException();
        }

        public Brush SpolColor
        {
            get
            {
                if (VfkItem.VfkSpolItem.STAV_DAT != 0 && VfkItem.VfkSpolItem.ItemFromImport())
                    return Brushes.Red;
                return Brushes.Black;
            }
            set => throw new UnExpectException();
        }

        public override string Id => VfkToolBar.VfkActivePoint.Name;

        public event PropertyChangedEventHandler PropertyChanged;

        public void RegisterObject(IVFKMain aOwner)
        {
            if (VfkItem == null)
                VfkItem = new VfkProxyActivePoint(null, null, aOwner);
            VfkItem.RegisterSegment(aOwner);
        }

        public void DeleteObject(IVFKMain aOwner)
        {
            VfkItem.DeleteSegment(aOwner);
        }

        public VFKSOBRTableItem getActivePoint(uint aIdx)
        {
            return VfkItem.VfkSobrItem;
        }

        public uint TYPPPD_KOD
        {
            get => throw new UnExpectException();
            set => throw new UnExpectException();
        }

        public void SetVfkElement(VfkElement vfkElement)
        {
        }

        public bool GetMustBeConnectedWithSnap()
        {
            return false;
        }

        public void OnPropertyChangedAll()
        {
            OnPropertyChanged("VfkPointName");
            OnPropertyChanged("PointNumber");
            OnPropertyChanged("VfkSouradniceObrazuY");
            OnPropertyChanged("VfkSouradniceObrazuX");
            OnPropertyChanged("VfkSouradnicePolohyX");
            OnPropertyChanged("VfkSouradnicePolohyY");
            OnPropertyChanged("VfkKkb");
        }

        public override void Draw(ICanvas canvas, Rect unitrect)
        {
            if (Selected)
                if (P1.IsEmpty == false)
                    DrawUtils.DrawNode(canvas, P1, new Size(5.5f, 5.5f));
            var fontBrush = _brush;
            var color = Color;
            if ((VfkItem.VfkSobrItem.STAV_DAT != 0 && VfkItem.VfkSobrItem.ItemFromImport()) ||
                (VfkItem.VfkSpolItem.STAV_DAT != 0 && VfkItem.VfkSpolItem.ItemFromImport()))
            {
                color = Color.Red;
                fontBrush = new SolidBrush(color);
            }

            var pen = canvas.CreatePen(color, (float)Width);
            pen.EndCap = LineCap.Flat;
            pen.StartCap = LineCap.Flat;
            var p1 = canvas.ToScreen(P1);
            var p2 = p1;
            p1.X -= 5;
            p2.X += 5;
            canvas.DrawLine(canvas, pen, p1, p2);
            p1.X += 5;
            p2.X -= 5;
            p1.Y -= 5;
            p2.Y += 5;
            canvas.DrawLine(canvas, pen, p1, p2);
            var f = new StringFormat();
            f.Alignment = StringAlignment.Center;
            p1.X += 20;
            p1.Y -= 15;
            canvas.Graphics.DrawString(PointNumber, _font, fontBrush, p1.FromWpfPoint(), f);
        }

        public override ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobjs,
            Type[] runningsnaptypes, Type usersnaptype)
        {
            var thWidth = ThresholdWidth(canvas, Width);
            if (usersnaptype == typeof(VfkSnapPoint))
                if (HitUtil.CircleHitPoint(P1, thWidth, point))
                    return new VfkSnapPoint(canvas, this, P1);
            return null;
        }

        public override void Export(IExport export)
        {
            var color = Color;
            if ((VfkItem.VfkSobrItem.STAV_DAT != 0 && VfkItem.VfkSobrItem.ItemFromImport()) ||
                (VfkItem.VfkSpolItem.STAV_DAT != 0 && VfkItem.VfkSpolItem.ItemFromImport()))
                color = Color.Red;

            export.AddPoint(P1.X, P1.Y, color);
            export.AddText(PointNumber, P1.X, P1.Y, 0.5, string.Empty, color, 0);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}