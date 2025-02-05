using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using CAD.Canvas;
using CAD.Canvas.DrawTools;
using CAD.Export;
using CAD.GUI;
using CAD.UITools;
using CAD.Utils;
using GeoBase.Utils;
using VFK;
using VFK.Tables;

using Pen = System.Drawing.Pen;

namespace CAD.VFK.DrawTools
{
        public enum VfkMarkType
    {
        Dpm,
        Op,
        Obbp,
        Ob
    }
        public class VfkMark : VfkDrawObjectBase, IDrawObject
    {
                public VfkMark()
        {

        }
        public VfkMark(VFKMain owner, VFKDPMTableItem dpmItem)
        {
            Owner = owner;
            Item = dpmItem;
            VfkMarkType = VfkMarkType.Dpm;
            SetVfkElement(Singletons.VFKElements.GetElement(dpmItem.TYPPPD_KOD));
            if (VfkElement.DpmType != DpmType.BPP)
            {
                Sobr = Owner.getSouradniceBodu(dpmItem.BP_ID);
            }
            else
            {
                P1 = new UnitPoint(-dpmItem.SOURADNICE_Y, -dpmItem.SOURADNICE_X);
            }
        }
        public VfkMark(VFKMain owner, VFKOPTableItem opItem)
        {
            Owner = owner;
            Item = opItem;
            VfkMarkType = VfkMarkType.Op;
            SetVfkElement(Singletons.VFKElements.GetElement(opItem.TYPPPD_KOD));
            P1 = new UnitPoint(-opItem.SOURADNICE_Y, -opItem.SOURADNICE_X);

        }
        public VfkMark(VFKMain owner, VFKOBBPTableItem obbpItem)
        {
            Owner = owner;
            Item = obbpItem;
            VfkMarkType = VfkMarkType.Obbp;
            SetVfkElement(Singletons.VFKElements.GetElement(obbpItem.TYPPPD_KOD));
            if (GetMustBeConnectedWithSnap())
            {
                Sobr = Owner.getSouradniceBodu(obbpItem.BP_ID);
            }
            else
            {
                P1 = new UnitPoint(-obbpItem.SOURADNICE_Y, -obbpItem.SOURADNICE_X);
            }

        }
        public VfkMark(VFKMain owner, VFKOBTableItem obItem)
        {
            Owner = owner;
            Item = obItem;
            VfkMarkType = VfkMarkType.Ob;
            SetVfkElement(Singletons.VFKElements.GetElement(obItem.TYPPPD_KOD));
            P1 = new UnitPoint(-obItem.SOURADNICE_Y, -obItem.SOURADNICE_X);
        }
                        public VfkMarkType VfkMarkType
        { get; set; }

        private UnitPoint P1
        {
            get;
            set;
        }

        private VFKSOBRTableItem _sobr;
        private VFKSOBRTableItem Sobr
        {
            get
            {
                return _sobr;
            }
            set
            {
                _sobr = value;
                if (_sobr != null)
                    P1 = new UnitPoint(-_sobr.SOURADNICE_Y, -_sobr.SOURADNICE_X);
            }
        }

        private VFKMain Owner
        { get; set; }
        private VFKDataTableBaseItemWithProp Item
        { get; set; }
                        public virtual string Id
        {
            get { return VfkToolBar.VfkMark.Name; }
        }
        public virtual IDrawObject Clone()
        {
            VfkMark l = new VfkMark();
            l.Copy(this);
            return l;
        }
        public bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            Rect rect = GetBoundingRect(canvas);
            return rect.Contains(point.Point);
        }
        public bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint)
        {
            Rect rectt = GetBoundingRect(canvas);
            return rect.Contains(rectt);
        }
        private static Dictionary<uint, PathImpl> _markCache = new Dictionary<uint, PathImpl>();
        public void Draw(ICanvas canvas, Rect unitrect)
        {
            Pen pen = canvas.CreatePen(Color, (float)Width);
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            canvas.DrawPath(canvas, pen, GetMarkPath(), P1);
            if (Selected)
            {
                if (P1.IsEmpty == false)
                    DrawUtils.DrawNode(canvas, P1);
            }
        }
        public Rect GetBoundingRect(ICanvas canvas)
        {
            return GetBoundigBox(P1, VfkElement);
        }
        public void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            P1 = point;
        }
        public eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            if (GetMustBeConnectedWithSnap() && snappoint != null)
                Sobr = ((IVFKTool)snappoint.Owner).getActivePoint(0);
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
            get { return new UnitPoint(); }
        }
        public INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            return null;
        }
        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes,
                                    Type usersnaptype)
        {
            float thWidth = ThresholdWidth(canvas, (float)Width);
            if (runningsnaptypes != null)
            {
                foreach (Type snaptype in runningsnaptypes)
                {
                    if (snaptype == typeof(VertextSnapPoint))
                    {
                        if (HitUtil.CircleHitPoint(P1, thWidth, point))
                            return new VertextSnapPoint(canvas, this, P1);
                    }
                }
                return null;
            }
            if (usersnaptype == typeof(VertextSnapPoint))
            {
                return new VertextSnapPoint(canvas, this, P1);
            }
            return null;
        }
        public void Move(UnitPoint offset)
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
        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            P1 = point;
            SetVfkElement(Singletons.VFKElements.SelectedSubGroup.SelectedElement);
            if (GetMustBeConnectedWithSnap() && snap != null)
                Sobr = ((IVFKTool)snap.Owner).getActivePoint(0);
            Selected = true;
        }
        public virtual void Copy(VfkMark acopy)
        {
            base.Copy(acopy);
            Item = acopy.Item;
            P1 = acopy.P1;
            Sobr = acopy.Sobr;
            VfkMarkType = acopy.VfkMarkType;
            Owner = acopy.Owner;
        }
        public void Export(IExport export)
        {
            export.AddPath(GetMarkPath().GetTransformPath(P1),Color,Width);
        }
                        private const int ThresholdPixel = 6;
        public static float ThresholdWidth(ICanvas canvas, float objectwidth)
        {
            return ThresholdWidth(canvas, objectwidth, ThresholdPixel);
        }
        public static float ThresholdWidth(ICanvas canvas, float objectwidth, float pixelwidth)
        {
            double minWidth = canvas.ToUnit(pixelwidth);
            double width = Math.Max(objectwidth / 2, minWidth);
            return (float)width;
        }
        static public Rect GetBoundigBox(UnitPoint nodePoint, VfkElement element)
        {
            switch (element.TYPPPD_KOD)
            {
                case 301://OrnaPuda:
                    return new Rect(new Point(nodePoint.X - 3.0f, nodePoint.Y - 3.0f), new Size(6.0f, 6.0f));
                case 302://Chmelnice:
                    return new Rect(new Point(nodePoint.X - 0.5f, nodePoint.Y - 1.5f), new Size(1, 3));
                case 304://Zaharada:
                case 305://OvocnySad:
                    return new Rect(new Point(nodePoint.X - 0.75f, nodePoint.Y - 0.75f), new Size(3.0f, 3.0f));
                case 306://TrvalyTravnatyPorost:
                    return new Rect(new Point(nodePoint.X - 0.5f, nodePoint.Y - 0.75f), new Size(1.0f, 1.5f));
                case 308://tLesniPuda:
                    return new Rect(new Point(nodePoint.X - 0.75f, nodePoint.Y - 0.75f), new Size(1.5f, 4.0f));
                case 314://tPark:
                    return new Rect(new Point(nodePoint.X - 1f, nodePoint.Y - 0.75f), new Size(2f, 1.5f));
                case 315://Hrbitov:
                    return new Rect(new Point(nodePoint.X - 1f, nodePoint.Y - 1.0f), new Size(2f, 2f));
                case 316://NeplodnaPuda:
                    return new Rect(new Point(nodePoint.X - 0.5f, nodePoint.Y - 0.5f), new Size(1f, 1f));
                case 403://BudovaDrevena:
                    return new Rect(new Point(nodePoint.X - 0.5f, nodePoint.Y - 0.5f), new Size(1f, 1f));
                case 404:
                case 402://BudovaZdena:
                    return new Rect(new Point(nodePoint.X - 0.5f, nodePoint.Y - 0.5f), new Size(1f, 1f));
                case 105://tPBPPHranicniZnak:
                    return new Rect(new Point(nodePoint.X - 0.5f, nodePoint.Y - 0.5f), new Size(1f, 1f));
                /*case 409://Kostel:
                case 412://PredmetMalehoRozsahu:
                case 410://tSynagoga:
                case 303://Vinice:
                case 803://VodniNadrz:
                //                case ElementType.tDobyvaciProstor:
                case 318://NemovitaKulturniPamatka:
                case 703://LoziskoSlatinARaselin:
                case 804://MocalBazina:
                case 307://Pastvina:
                case 701://PovrchovaTezba:
                case 802://VodniTokSireNad2metry:
                case 408://CaraJakoVyplnShodiste:
                case 1060://SymbolVondnihoTokuUzsiNez2m:
                case 1029://SipkaKParcelnimuCisluVolna:
                case 601://KovovyBetovnovyStozar:
                case 420://MostekPropustek:
                //case ElementType.tPredmetMalehoRozzsahuUrcenyStredem:
                case 602://PrihradovyStozar
                case 604://StozarVysilaci:
                case 811://VerejnaStudna:
                case 1033://SipkaKParcelnimuCislu:
                case 103://PBPPBodJednoteneNivelacniSite:
                case 102://PBPPPouzePodzemni:
                case 104://PBPPBodPolohovehoBodovehoPole:
                //                case ElementType.tPBPPStabilizovanyBodNivelacniSite:
                case 1032://CaraProUmisteniSipky:
                case 101:
                case 21700:
                case 405://Budova dřevěná  evidovaná v SPI
                case 411://Předmět malého rozsahu určený středem
                    */
                default:
                    return new Rect(new Point(nodePoint.X - 1.5f, nodePoint.Y - 1.5f), new Size(3f, 3f));
            }
        }
        static private PathImpl GetDrawMark(uint elementId)
        {
            PathImpl path = new PathImpl();
            switch (elementId)
            {
                case 301://OrnaPuda:
                    path.AddLine(new UnitPoint(0, 1.5), new UnitPoint(0, - 1.5));
                    path.AddLine(new UnitPoint(0, 0.5),new UnitPoint(0.5, 1.0));
                    path.AddLine(new UnitPoint(0.5, 1.0),new UnitPoint(1, 1.2));
                    path.AddLine(new UnitPoint(1,1.2),new UnitPoint(1.5,1.25));
                    break;
                case 302://Chmelnice:
                    path.AddLine(new UnitPoint(0.5,1.5),new UnitPoint(0.5,1.5));
                    path.AddLine(new UnitPoint(- 0.5, 1.5), new UnitPoint(0.5, 1.5));
                    break;
                case 304://Zaharada:
                    path.AddArc(new UnitPoint(), 0.75f, 0, -360);
                    path.AddLine(new UnitPoint(0, - 0.75), new UnitPoint(1.75, - 0.75));
                    break;
                case 305://OvocnySad:
                    path.AddArc(new UnitPoint(), 0.75f, 0, 360);
                    path.AddLine(new UnitPoint(0, - 0.75), new UnitPoint(0, - 1.25));
                    path.AddLine(new UnitPoint(0, - 1.25), new UnitPoint(1.0, -1.25));
                    break;
                case 306://TrvalyTravnatyPorost:
                    path.AddLine(new UnitPoint(-0.5, - 0.75), new UnitPoint(-0.5, 0.75));
                    path.AddLine(new UnitPoint(0.5, - 0.75),new UnitPoint(0.5, 0.75));
                    break;
                case 308://tLesniPuda:
                    path.AddLine(new UnitPoint(- 0.75,- 2.0f),new UnitPoint(0, 2.0f));
                    path.AddLine(new UnitPoint(0.75, - 2.0f),new UnitPoint(0, 2.0f));
                    path.AddLine(new UnitPoint(0.75, - 2.0f),new UnitPoint(1.75, - 2.0f));
                    break;
                case 314://tPark:
                    path.AddLine(new UnitPoint(- 1.0f, 0.75f),new UnitPoint(0, - 0.75f));
                    path.AddLine(new UnitPoint(1.0f, 0.75f),new UnitPoint(0, - 0.75f));
                    path.AddArc(new UnitPoint( - 0.8,  - 0.55), 0.2f, 0, 360);
                    path.AddArc(new UnitPoint( 0.8,  - 0.55), 0.2f, 0, 360);
                    break;
                case 315://Hrbitov:
                    path.AddLine(new UnitPoint(0, 1.0f),new UnitPoint(0, - 1.0f));
                    path.AddLine(new UnitPoint(1.0f, 0),new UnitPoint(- 1.0f, 0));
                    break;
                case 316://NeplodnaPuda:
                    path.AddArc(new UnitPoint(0, 0), 0.5f, 0, 360);
                    path.AddLine(new UnitPoint(0.75f, 0),new UnitPoint(- 0.75f, 0));
                    break;
                case 403://BudovaDrevena:
                    path.AddLine(new UnitPoint(- 0.5f, 0),new UnitPoint(0.5f, 0));
                    break;
                case 402://BudovaZdena:
                case 404:
                    path.AddArc( new UnitPoint(0, 0), 0.175f, 0, -360);
                    break;
                case 105://tPBPPHranicniZnak:
                    path.AddArc(new UnitPoint(0, 0), 0.5f, 0, -360);
                    break;
               /* case 409://Kostel:
                case 412://PredmetMalehoRozsahu:
                case 410://tSynagoga:
                case 303://Vinice:
                case 803://VodniNadrz:
                //                case ElementType.tDobyvaciProstor:
                case 318://NemovitaKulturniPamatka:
                case 703://LoziskoSlatinARaselin:
                case 804://MocalBazina:
                case 307://Pastvina:
                case 701://PovrchovaTezba:
                case 802://VodniTokSireNad2metry:
                case 408://CaraJakoVyplnShodiste:
                case 1060://SymbolVondnihoTokuUzsiNez2m:
                case 1029://SipkaKParcelnimuCisluVolna:
                case 601://KovovyBetovnovyStozar:
                case 420://MostekPropustek:
                //case ElementType.tPredmetMalehoRozzsahuUrcenyStredem:
                case 602://PrihradovyStozar
                case 604://StozarVysilaci:
                case 811://VerejnaStudna:
                case 1033://SipkaKParcelnimuCislu:
                case 103://PBPPBodJednoteneNivelacniSite:
                case 102://PBPPPouzePodzemni:
                case 104://PBPPBodPolohovehoBodovehoPole:
                //                case ElementType.tPBPPStabilizovanyBodNivelacniSite:
                case 1032://CaraProUmisteniSipky:
                case 21700:
                case 405://Budova dřevěná  evidovaná v SPI
                case 411://Předmět malého rozsahu určený středem
                case 101:*/
                default:
                    path.AddLine(new UnitPoint(- 1.5f, - 1.5),new UnitPoint(1.5f, 1.5));
                    path.AddLine(new UnitPoint(1.5f, - 1.5),new UnitPoint(- 1.5f, 1.5));
                    break;
            }
            return path;
        }
                        public override void RegisterObject(IVFKMain aOwner)
        {
            switch (VfkMarkType)
            {
                case VfkMarkType.Dpm:
                    {
                        VFKDPMTableItem dpm = Item as VFKDPMTableItem;
                        Item = dpm = aOwner.UpdateDPM(dpm);
                        dpm.TYPPPD_KOD = VfkElement.TYPPPD_KOD;
                        dpm.DPM_TYPE = VfkElement.DpmType.ToString();
                        dpm.VELIKOST = 1;
                        dpm.UHEL = 0;
                        if (Sobr != null)
                        {
                            dpm.BP_ID = Sobr.ID;
                        }
                        else
                        {
                            dpm.SOURADNICE_X = -P1.Y;
                            dpm.SOURADNICE_Y = -P1.X;
                        }
                    } break;
                case VfkMarkType.Op:
                    {
                        VFKOPTableItem op = Item as VFKOPTableItem;
                        Item = op = aOwner.UpdateOP(op);
                        op.TYPPPD_KOD = TYPPPD_KOD;
                        op.OPAR_TYPE = VfkElement.OparType;
                        op.SOURADNICE_X = -P1.Y;
                        op.SOURADNICE_Y = -P1.X;
                        op.VELIKOST = 1;
                        op.UHEL = 0;
                    } break;
                case VfkMarkType.Obbp:
                    {
                        VFKOBBPTableItem obbp = Item as VFKOBBPTableItem;
                        Item = obbp = aOwner.UpdateOBBP(obbp);
                        obbp.TYPPPD_KOD = TYPPPD_KOD;
                        obbp.OBBP_TYPE = VfkElement.ObbpType.ToString();
                        obbp.UHEL = 0;
                        obbp.VELIKOST = 1;
                        if (Sobr != null)
                            obbp.BP_ID = Sobr.ID;
                        else
                        {
                            obbp.SOURADNICE_X = -P1.Y;
                            obbp.SOURADNICE_Y = -P1.X;
                        }
                    } break;
                case VfkMarkType.Ob:
                    {
                        VFKOBTableItem ob = Item as VFKOBTableItem;
                        Item = ob = aOwner.UpdateOB(ob);
                        ob.TYPPPD_KOD = VfkElement.TYPPPD_KOD;
                        ob.OBRBUD_TYPE = VfkElement.ObrBudType.ToString();
                        ob.VELIKOST = 1;
                        ob.UHEL = 0;
                        ob.SOURADNICE_X = -P1.Y;
                        ob.SOURADNICE_Y = -P1.X;
                    } break;
                default:
                    throw new UnExpectException(); 
            }

        }
        public override void DeleteObject(IVFKMain aOwner)
        {
            switch (VfkMarkType)
            {
                case VfkMarkType.Dpm:
                    aOwner.DeleteDPM(Item as VFKDPMTableItem);
                    break;
                case VfkMarkType.Op:
                    aOwner.DeleteOP(Item as VFKOPTableItem);
                    break;
                case VfkMarkType.Obbp:
                    aOwner.DeleteOBBP(Item as VFKOBBPTableItem);
                    break;
                case VfkMarkType.Ob:
                    aOwner.DeleteOB(Item as VFKOBTableItem);
                    break;
                default:
                    throw new UnExpectException(); 
            }
            if (!Item.ItemFromImport())
                Item = null;
        }
        public override bool GetMustBeConnectedWithSnap()
        {
            switch (VfkMarkType)
            {
                case VfkMarkType.Dpm:
                    return VfkElement.DpmType != DpmType.BPP;
                case VfkMarkType.Op:
                    return false;
                case VfkMarkType.Obbp:
                    //Bod PBP - pouze podzemní značka,Bod jednotné nivelační sítě,Stabilizovaný bod technické nivelace,Hraniční znak
                    if (TYPPPD_KOD == 101 || TYPPPD_KOD == 102 || TYPPPD_KOD == 103 || TYPPPD_KOD == 104)
                        return true;
                    return false;
                case VfkMarkType.Ob:
                    return false;
            }
            return false;
        }
        public override void SetVfkElement(VfkElement element)
        {
            base.SetVfkElement(element);
            switch (element.BlockNvf)
            {
                case BlockNvf.DPM:
                    VfkMarkType = VfkMarkType.Dpm;
                    break;
                case BlockNvf.OP:
                    VfkMarkType = VfkMarkType.Op;
                    break;
                case BlockNvf.OBBP:
                    VfkMarkType = VfkMarkType.Obbp;
                    break;
                case BlockNvf.OB:
                    VfkMarkType = VfkMarkType.Ob;
                    break;
                default:
                    throw new UnExpectException(); 
            }
        }
                        private PathImpl GetMarkPath()
        {
            if (!_markCache.ContainsKey(VfkElement.TYPPPD_KOD))
                _markCache[VfkElement.TYPPPD_KOD] = GetDrawMark(VfkElement.TYPPPD_KOD);
            return _markCache[VfkElement.TYPPPD_KOD];
        }
            }
    internal class VfkMarkEdit : VfkMark, IObjectEditInstance
    {
                public void Copy(ActivePointEdit acopy)
        {
            base.Copy(acopy);
        }
        public override IDrawObject Clone()
        {
            var l = new VfkMarkEdit();
            l.Copy(this);
            return l;
        }
                        public IDrawObject GetDrawObject()
        {
            return Clone();
        }
        public PpWindow GetPropPage(IModel aDataMode)
        {
            return null;
        }
        public bool HasPropPage()
        {
            return false;
        }
        public bool ValidateObjectContent()
        {
            return true;
        }
            }
}
