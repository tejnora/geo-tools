using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using CAD.Canvas;
using CAD.Canvas.DrawTools;
using CAD.Export;
using CAD.GUI;
using CAD.UITools;
using CAD.Utils;
using GeoBase.Localization;
using GeoBase.Utils;
using VFK;
using VFK.Tables;
using FontStyle = System.Drawing.FontStyle;

using Font = System.Drawing.Font;
using Matrix = System.Drawing.Drawing2D.Matrix;
using Rectangle = System.Drawing.Rectangle;
using StringFormat = System.Drawing.StringFormat;

namespace CAD.VFK.DrawTools
{
    #region Enums
    public enum VFKTextEnum
    {
        Unset,
        Dpm,
        Op,
        Obbp,
        Obpej
    }
    #endregion
    public class VfkText : VfkDrawObjectBase, IDrawObject
    {
        #region Constructors
        public VfkText()
        {
            Type = VFKTextEnum.Unset;
            UseLayerColor = false;
        }
        public VfkText(VFKMain owner, VFKDPMTableItem dpmItem)
        {
            Owner = owner;
            Item = dpmItem;
            SetVfkElement(Singletons.VFKElements.GetElement(dpmItem.TYPPPD_KOD));
            Text = dpmItem.TEXT;
            P1=new UnitPoint(-dpmItem.SOURADNICE_Y, -dpmItem.SOURADNICE_X);
            UseLayerColor = false;
        }
        public VfkText(VFKMain owner, VFKOPTableItem opItem)
        {
            Owner = owner;
            Item = opItem;
            SetVfkElement(Singletons.VFKElements.GetElement(opItem.TYPPPD_KOD));
            Text = opItem.TEXT;
            P1 = new UnitPoint(-opItem.SOURADNICE_Y, -opItem.SOURADNICE_X);
            UseLayerColor = false;
        }
        public VfkText(VFKMain owner, VFKOBBPTableItem obbpItem)
        {
            Owner = owner;
            Item = obbpItem;
            SetVfkElement(Singletons.VFKElements.GetElement(obbpItem.TYPPPD_KOD));
            Text = obbpItem.TEXT;
            P1 = new UnitPoint(-obbpItem.SOURADNICE_Y, -obbpItem.SOURADNICE_X);
            UseLayerColor = false;
        }
        public VfkText(VFKMain owner, VFKOBPEJTableItem obpej)
        {
            Owner = owner;
            Item = obpej;
            SetVfkElement(Singletons.VFKElements.GetElement(obpej.TYPPPD_KOD));
            Text = obpej.TEXT;
            P1 = new UnitPoint(-obpej.SOURADNICE_Y, -obpej.SOURADNICE_X);
            UseLayerColor = false;         
        }
        #endregion
        #region Property
        private VFKMain Owner
        {
            get; set;
        }
        private VFKDataTableBaseItemWithProp Item
        {
            get; set;
        }
        public VFKTextEnum Type
        {
            get; private set;
        }
        protected UnitPoint P1
        {
            get;
            set;
        }
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                ReloadBbValues = true;
            }
        }
        private double FontSize
        {
            get;
            set;
        }

        private double _vyskaText;
        public double VyskaTextu
        {
            get
            {
                return _vyskaText;
            }
            set
            {
                _vyskaText = value;
                UpdateFontSize();
                ReloadBbValues = true;
            }
        }
        private double _uhelNatoceni;
        protected double UhelNatoceni
        {
            get
            {
                return _uhelNatoceni;
            }
            set
            {
                _uhelNatoceni = value;
                ReloadBbValues = true;
            }
        }

        private string _fontName;
        #endregion
        #region IDrawObject
        public virtual string Id
        {
            get { return VfkToolBar.VfkText.Name; }
        }
        public virtual IDrawObject Clone()
        {
            var l = new VfkText();
            l.Copy(this);
            return l;
        }
        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            P1 = point;
            SetVfkElement(Singletons.VFKElements.SelectedSubGroup.SelectedElement);
            Selected = true;
            ReloadBbValues = true;
        }
        public virtual void Draw(ICanvas canvas, Rect unitrect)
        {
            Rect bb = GetBoundingRect(canvas);
            if(bb.IsEmpty)return;
            float fontSize = (float)(FontSize * canvas.getZoom());
            if (fontSize < 1) return;
            try
            {
                Font font = canvas.CreateFont(_fontName,fontSize,FontStyle.Regular);
                UnitPoint finalPoint = P1;
                Matrix backupMatrix=canvas.Graphics.Transform;
                System.Drawing.Point pt = canvas.ToScreen(finalPoint).FromWpfPoint();
                canvas.Graphics.TranslateTransform(pt.X, pt.Y);
                canvas.Graphics.RotateTransform((float)-UhelNatoceni);
                canvas.Graphics.TranslateTransform(0,-(float)canvas.ToScreen(TextBoxSize.Height));
                var color = Color;
                var pen = canvas.CreatePen(color, (float) Width);
                canvas.Graphics.DrawString(Text, font, new System.Drawing.SolidBrush(pen.Color), 0, 0);
                canvas.Graphics.Transform = backupMatrix;
            }
            catch (Exception)
            {
                throw new UnExpectException();
            }
            if (Selected)
            {
                Rectangle screenRect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(canvas, bb));
                canvas.Graphics.DrawRectangle(DrawUtils.SelectedPen, screenRect);
            }
        }
        private Size TextBoxSize{get;set;}
        private bool ReloadBbValues { get; set; }
        private TransformGroup _rotateTr;
        public virtual Rect GetBoundingRect(ICanvas canvas)
        {
            if (FontSize == 0)
                return Rect.Empty;
            if (ReloadBbValues)
            {
                Font font = canvas.CreateFont(_fontName, (float)(FontSize), FontStyle.Regular);
                Size layoutSize = new Size(float.MaxValue, float.MaxValue);
                int charactersFitted;
                int linesFilled;
                TextBoxSize = canvas.MeasureString(canvas, Text, font, layoutSize, new StringFormat(), out charactersFitted, out linesFilled);
                TextBoxSize = new Size(TextBoxSize.Width / 96 * 0.0254f , TextBoxSize.Height / 96 * 0.0254f );
                ReloadBbValues = false;
                _rotateTr = new TransformGroup();
                _rotateTr.Children.Add(new TranslateTransform(0,-TextBoxSize.Height));
                _rotateTr.Children.Add(new RotateTransform(-UhelNatoceni));
            }
            PathGeometry path = new PathGeometry();
            path.AddGeometry(new RectangleGeometry(new Rect(0, 0, TextBoxSize.Width, TextBoxSize.Height)));
            path.Transform = _rotateTr;
            return new Rect(P1.Point.X + path.Bounds.X, P1.Point.Y - (path.Bounds.Height+path.Bounds.Y), path.Bounds.Width,
                            path.Bounds.Height);
        }
        public virtual bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            Rect rect = GetBoundingRect(canvas);
            return rect.Contains(new Point((float)point.X, (float)point.Y));
        }
        public virtual bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint)
        {
            Rect boundingrect = GetBoundingRect(canvas);
            return rect.Contains(boundingrect) || boundingrect.Contains(rect);
        }
        public virtual void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            P1 = point;
        }
        public virtual eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            return eDrawObjectMouseDown.DoneRepeat;
        }
        public virtual void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
        }
        public virtual void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }
        public virtual UnitPoint RepeatStartingPoint
        {
            get { return P1; }
        }
        public virtual INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            return null;
        }
        public virtual ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes, Type usersnaptype)
        {
            return null;
        }
        public virtual void Move(UnitPoint offset)
        {
        }
        public bool getSelectDrawToolCreate()
        {
            return true;
        }
        public string GetInfoAsString()
        {
            return VfkElement.Description;
        }
        public virtual void Copy(VfkText acopy)
        {
            base.Copy(acopy);
            Owner = acopy.Owner;
            Item = acopy.Item;
            Type = acopy.Type;
            P1 = acopy.P1;
            Text = acopy.Text;
            FontSize = acopy.FontSize;
            VyskaTextu = acopy.VyskaTextu;
            UhelNatoceni = acopy.UhelNatoceni;
            _fontName = acopy._fontName;
        }
        public void Export(IExport export)
        {
            var height= FontSize/96.0*0.0254f;
            string styleName = export.AddStyle(false, 0.0, 1, 0.0, false, false, 0.0, _fontName);
            export.AddText(Text, P1.X, P1.Y, height, styleName, Color, UhelNatoceni);
        }
        #endregion
        #region IVFKTool
        public override void RegisterObject(IVFKMain aOwner)
        {
            bool newItem = Item == null;
            switch (Type)
            {
                case VFKTextEnum.Op:
                    {
                        VFKOPTableItem op = (VFKOPTableItem) Item;
                        Item = op = aOwner.UpdateOP(op);
                        if (newItem)
                        {
                            op.TYPPPD_KOD = TYPPPD_KOD;
                            op.OPAR_TYPE = VfkElement.OparType;
                        }
                        op.SOURADNICE_X = -P1.Y;
                        op.SOURADNICE_Y = -P1.X;
                        op.VELIKOST = VyskaTextu;
                        op.UHEL = UhelNatoceni/0.9;
                        op.TEXT = Text;
                        op.VZTAZNY_BOD = 1;
                    }break;
                case VFKTextEnum.Obbp:
                    {
                        VFKOBBPTableItem obbp = (VFKOBBPTableItem) Item;
                        Item = obbp = aOwner.UpdateOBBP(obbp);
                        if(newItem)
                        {
                            obbp.TYPPPD_KOD = TYPPPD_KOD;
                            obbp.OBBP_TYPE = VfkElement.ObbpType.ToString();
                        }
                        obbp.SOURADNICE_X = -P1.Y;
                        obbp.SOURADNICE_Y = -P1.X;
                        obbp.VELIKOST = VyskaTextu;
                        obbp.UHEL = UhelNatoceni / 0.9;
                        obbp.TEXT = Text;
                        obbp.VZTAZNY_BOD = 1;
                        
                    }break;
                case VFKTextEnum.Dpm:
                    {
                        VFKDPMTableItem dpm = (VFKDPMTableItem) Item;
                        Item = dpm = aOwner.UpdateDPM(dpm);
                        if(newItem)
                        {
                            dpm.TYPPPD_KOD = TYPPPD_KOD;
                            dpm.DPM_TYPE = VfkElement.DpmType.ToString();
                        }
                        dpm.SOURADNICE_X = -P1.Y;
                        dpm.SOURADNICE_Y = -P1.X;
                        dpm.VELIKOST = VyskaTextu;
                        dpm.UHEL = UhelNatoceni / 0.9;
                        dpm.TEXT = Text;
                        dpm.VZTAZNY_BOD = 1;
                    }break;
                default:
                    throw new NotImplemented();
            }
        }
        public override void DeleteObject(IVFKMain aOwner)
        {
            switch (Type)
            {
                case VFKTextEnum.Op:
                    aOwner.DeleteOP((VFKOPTableItem) Item);
                    break;
                case VFKTextEnum.Obbp:
                    aOwner.DeleteOBBP((VFKOBBPTableItem) Item);
                    break;
                case VFKTextEnum.Dpm:
                    aOwner.DeleteDPM((VFKDPMTableItem) Item);
                    break;
                default:
                    throw new NotImplemented();
            }
            if (!Item.ItemFromImport())
                Item = null;
        }
        public override bool GetMustBeConnectedWithSnap()
        {
            return false;
        }
        public override void SetVfkElement(VfkElement vfkElement)
        {
            base.SetVfkElement(vfkElement);
            VfkElement.ElementDrawInfo drawInfo;
            if(Item!=null)
            {
                if(Item.STAV_DAT==VFKMain.STAV_DAT_PRITOMNOST)
                    drawInfo = vfkElement.PritomnyStav;
                else if (Item.STAV_DAT==VFKMain.STAV_DAT_BUDOUCNOST)
                    drawInfo = vfkElement.BudouciStav;
                else
                    throw new UnExpectException();
            }
            else
                drawInfo = vfkElement.BudouciStav;
            VyskaTextu = drawInfo.VyskaTextu;
            _fontName = "Arial";
            UhelNatoceni = 0;
            switch(vfkElement.BlockNvf)
            {
                case BlockNvf.DPM:
                    Type = VFKTextEnum.Dpm;
                    if (Item != null)
                    {
                        var dpm= (VFKDPMTableItem)Item;
                        VyskaTextu = dpm.VELIKOST;
                        UhelNatoceni = dpm.UHEL*0.9;
                    }
                    break;
                case BlockNvf.OP:
                    Type = VFKTextEnum.Op;
                    if (Item != null)
                    {
                        var op = (VFKOPTableItem)Item;
                        VyskaTextu = op.VELIKOST;
                        UhelNatoceni = op.UHEL * 0.9;
                    }
                    break;
                case BlockNvf.OBBP:
                    Type = VFKTextEnum.Obbp;
                    if (Item != null)
                    {
                        var obbp = (VFKOBBPTableItem)Item;
                        VyskaTextu = obbp.VELIKOST;
                        UhelNatoceni = obbp.UHEL * 0.9;
                    }
                    break;
                case BlockNvf.OBPEJ:
                    Type = VFKTextEnum.Obpej;
                    if (Item != null)
                    {
                        var obpej = (VFKOBPEJTableItem)Item;
                        VyskaTextu = obpej.VELIKOST;
                        UhelNatoceni = obpej.UHEL * 0.9;
                    }
                    break;
                default:
                    throw new UnExpectException();
            }
            UpdateFontSize();
        }

        #endregion
        #region Methods
        private void UpdateFontSize()
        {
            FontSize = (VyskaTextu * 100 / 2.54) * 96;
        }
        #endregion
    }
    internal class VfkTextsEdit : VfkText, IObjectEditInstance
    {
        #region Methods
        public void Copy(ActivePointEdit acopy)
        {
            base.Copy(acopy);
        }

        public override IDrawObject Clone()
        {
            var l = new VfkTextsEdit();
            l.Copy(this);
            return l;
        }
        #endregion
        #region Property
        public bool SetAngle
        { get; set; }
        #endregion
        #region IObjectEditInstance
        public IDrawObject GetDrawObject()
        {
            VfkText text = new VfkText();
            text.Copy(this);
            return text;
        }

        public PpWindow GetPropPage(IModel aDataMode)
        {
            return new VfkNumberPropPage(this);
        }

        public bool HasPropPage()
        {
            return true;
        }
        public bool ValidateObjectContent()
        {
            if (Text==null || Text.Length == 0)
            {
                LanguageDictionary.Current.ShowMessageBox("108", null, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        #endregion
        #region IDrawObject
        private bool _modifyText;
        public override eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            if(!SetAngle)
                return eDrawObjectMouseDown.DoneRepeat;
            if (!_modifyText)
            {
                _modifyText = true;
                return eDrawObjectMouseDown.Continue;
            }
            _modifyText = false;
            return eDrawObjectMouseDown.DoneRepeat;
        }
        public override void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            if (!_modifyText)
            {
                P1 = point;
                return;
            }
            double x = point.X - P1.X;
            double y = point.Y - P1.Y;
            if(y==0)
            {
                if (x >= 0)
                    UhelNatoceni = 0;
                else
                    UhelNatoceni = 180;
            }
            else
            {
                double atan = Math.Atan(y/ x)*180/Math.PI;
                if (x >= 0)
                {
                    if (y >= 0)
                        UhelNatoceni = atan;
                    else
                        UhelNatoceni = atan + 360;
                }
                else
                {
                    if (y >= 0)
                        UhelNatoceni = atan + 180;
                    else
                        UhelNatoceni = atan + 180;
                }
            }
        }
        #endregion
    }

}
