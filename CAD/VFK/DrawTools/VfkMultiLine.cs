using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using CAD.Canvas;
using CAD.Canvas.DrawTools;
using CAD.Export;
using CAD.UITools;
using CAD.Utils;
using GeoBase.Utils;
using VFK;
using VFK.Tables;

namespace CAD.VFK.DrawTools
{
    
    internal class LineSegment
    {
        enum LineSegmentType
        {
            Sobr,
            Sbm,
        }

        public LineSegment(VFKSOBRTableItem sobr1, VFKSOBRTableItem sobr2, VFKSBPTableItem sbp1, VFKSBPTableItem sbp2)
        {
            Sobr1 = sobr1;
            Sobr2 = sobr2;
            Sbp1 = sbp1;
            Sbp2 = sbp2;
            Type = LineSegmentType.Sobr;
        }

        public LineSegment(VFKSBMTableItem sbm1, VFKSBMTableItem sbm2)
        {
            Sbm1 = sbm1;
            Sbm2 = sbm2;
            Type = LineSegmentType.Sbm;
        }

        public LineSegment(UnitPoint p1, UnitPoint p2)
        {
            P1 = p1;
            P2 = p2;
        }

        private LineSegmentType Type { get; set; }
        private VFKSBMTableItem _sbmp1;

        public VFKSBMTableItem Sbm1
        {
            get { return _sbmp1; }
            set
            {
                _sbmp1 = value;
                P1 = new UnitPoint(-_sbmp1.SOURADNICE_Y, -_sbmp1.SOURADNICE_X);
            }
        }

        private VFKSBMTableItem _sbmp2;

        public VFKSBMTableItem Sbm2
        {
            get { return _sbmp2; }
            set
            {
                _sbmp2 = value;
                P2 = new UnitPoint(-_sbmp2.SOURADNICE_Y, -_sbmp2.SOURADNICE_X);
            }
        }

        private VFKSOBRTableItem _sobr1;

        public VFKSOBRTableItem Sobr1
        {
            get { return _sobr1; }
            set
            {
                _sobr1 = value;
                P1 = new UnitPoint(-_sobr1.SOURADNICE_Y, -_sobr1.SOURADNICE_X);
            }
        }

        private VFKSOBRTableItem _sobr2;

        public VFKSOBRTableItem Sobr2
        {
            get { return _sobr2; }
            set
            {
                _sobr2 = value;
                if (_sobr2 != null)
                    P2 = new UnitPoint(-_sobr2.SOURADNICE_Y, -_sobr2.SOURADNICE_X);
            }
        }

        public VFKSBPTableItem Sbp1 { get; set; }
        public VFKSBPTableItem Sbp2 { get; set; }
        public UnitPoint P1 { get; set; }
        public UnitPoint P2 { get; set; }

        public bool GetIsValid()
        {
            if (Type == LineSegmentType.Sobr)
                return _sobr1 != null && _sobr2 != null;
            return _sbmp1 != null && _sbmp2 != null;
        }
    }

    
    
    public enum VfkMultiLineType
    {
        HP,
        DPM,
        ZVB,
        Ob,
        Op,
        Hbpej
    }

    
    
    public class VfkMultiLineException : Exception
    {
        public VfkMultiLineException(string description)
            : base(description)
        {
        }
    }

    
    
    public class VfkMultiLine : VfkDrawObjectBase, IDrawObject
    {
        
        public VfkMultiLine()
        {
        }

        public VfkMultiLine(VFKMain owner, VFKHPTableItem item, bool fromModifyItems)
        {
            Owner = owner;
            Item = item;
            SetVfkElement(Singletons.VFKElements.GetElement(item.TYPPPD_KOD));
            List<VFKSBPTableItem> sbpPoint;
            if (fromModifyItems)
            {
                sbpPoint = (from n in owner.VFKModifySBPItems
                    where n.HP_ID == item.ID
                    orderby n.PORADOVE_CISLO_BODU
                    select n).ToList();
            }
            else
            {
                sbpPoint = owner.getMultiLineSbpPointsByHpId(item.ID);
            }

            VFKSBPTableItem prevTable = null;
            MultiLineSegments = new List<LineSegment>();
            foreach (VFKSBPTableItem table in sbpPoint)
            {
                if (prevTable == null)
                {
                    prevTable = table;
                    continue;
                }

                var segment = new LineSegment(Owner.getSouradniceBodu(prevTable.BP_ID),
                    Owner.getSouradniceBodu(table.BP_ID),
                    prevTable, table);
                if (!segment.GetIsValid())
                {
                    throw new VfkMultiLineException(string.Format("Vadny soubor. - Nenalezene body u HP line:{0}",
                        Item.ID));
                }

                MultiLineSegments.Add(segment);
                prevTable = table;
            }
        }

        public VfkMultiLine(VFKMain owner, VFKZVBTableItem zvbItem, bool fromModifyItems)
        {
            Owner = owner;
            Item = zvbItem;
            SetVfkElement(Singletons.VFKElements.GetElement(zvbItem.TYPPPD_KOD));
            List<VFKSBPTableItem> sbdPoint;
            if (fromModifyItems)
            {
                sbdPoint = (from n in owner.VFKModifySBPItems
                    where n.ZVB_ID == zvbItem.ID
                    orderby n.PORADOVE_CISLO_BODU
                    select n).ToList();
            }
            else
            {
                sbdPoint = (from n in owner.VFKSBPTable.Items
                    where n.ZVB_ID == zvbItem.ID
                    orderby n.PORADOVE_CISLO_BODU
                    select n).ToList();
            }

            MultiLineSegments = new List<LineSegment>();
            VFKSBPTableItem prevTable = null;
            foreach (VFKSBPTableItem table in sbdPoint)
            {
                if (prevTable == null)
                {
                    prevTable = table;
                    continue;
                }

                var segment = new LineSegment(Owner.getSouradniceBodu(prevTable.BP_ID),
                    Owner.getSouradniceBodu(table.BP_ID),
                    prevTable, table);
                if (!segment.GetIsValid())
                {
                    throw new VfkMultiLineException(string.Format("Vadny soubor. - Nenalezene body u ZVB line:{0}",
                        Item.ID));
                }

                MultiLineSegments.Add(segment);
                prevTable = table;
            }
        }

        public VfkMultiLine(VFKMain owner, VFKDPMTableItem dpmItem, bool fromModifyItems)
        {
            Owner = owner;
            Item = dpmItem;
            SetVfkElement(Singletons.VFKElements.GetElement(dpmItem.TYPPPD_KOD));
            if (VfkElement.DpmType == DpmType.HCHU || VfkElement.DpmType == DpmType.HOCHP)
            {
                List<VFKSBMTableItem> sbmPoints;
                if (fromModifyItems)
                {
                    sbmPoints = (from n in Owner.VFKModifySBMItems
                        where n.DPM_ID == Item.ID
                        orderby n.PORADOVE_CISLO_BODU
                        select n).ToList();
                }
                else
                {
                    sbmPoints = (from n in Owner.VFKSBMTable.Items
                        where n.DPM_ID == Item.ID
                        orderby n.PORADOVE_CISLO_BODU
                        select n).ToList();
                }

                VFKSBMTableItem prevTable = null;
                MultiLineSegments = new List<LineSegment>();
                foreach (var table in sbmPoints)
                {
                    if (prevTable == null)
                    {
                        prevTable = table;
                        continue;
                    }

                    var segment = new LineSegment(prevTable, table);
                    if (!segment.GetIsValid())
                    {
                        throw new VfkMultiLineException(string.Format("Vadny soubor. - Nenalezene body u DPM line:{0}",
                            Item.ID));
                    }

                    MultiLineSegments.Add(segment);
                    prevTable = table;
                }
            }
            else
            {
                List<VFKSBPTableItem> sbpPoint;
                if (fromModifyItems)
                {
                    sbpPoint = (from n in Owner.VFKModifySBPItems
                        where n.DPM_ID == Item.ID
                        orderby n.PORADOVE_CISLO_BODU
                        select n).ToList();
                }
                else
                {
                    sbpPoint = owner.getMultiLineSbpPointsByDpmId(Item.ID);
                }

                VFKSBPTableItem prevTable = null;
                MultiLineSegments = new List<LineSegment>();
                foreach (VFKSBPTableItem table in sbpPoint)
                {
                    if (prevTable == null)
                    {
                        prevTable = table;
                        continue;
                    }

                    var segment = new LineSegment(Owner.getSouradniceBodu(prevTable.BP_ID),
                        Owner.getSouradniceBodu(table.BP_ID),
                        prevTable, table);
                    if (!segment.GetIsValid())
                    {
                        throw new VfkMultiLineException(string.Format("Vadny soubor. - Nenalezene body u DPM line:{0}",
                            Item.ID));
                    }

                    MultiLineSegments.Add(segment);
                    prevTable = table;
                }
            }
        }

        public VfkMultiLine(VFKMain owner, VFKOBTableItem obItem, bool fromModifyItems)
        {
            Owner = owner;
            Item = obItem;
            SetVfkElement(Singletons.VFKElements.GetElement(obItem.TYPPPD_KOD));
            List<VFKSBPTableItem> sbpPoint;
            if (fromModifyItems)
            {
                sbpPoint = (from n in Owner.VFKModifySBPItems
                    where n.OB_ID == Item.ID
                    orderby n.PORADOVE_CISLO_BODU
                    select n).ToList();
            }
            else
            {
                sbpPoint = owner.getMultiLineSbpPointsByObId(Item.ID);
            }

            VFKSBPTableItem prevTable = null;
            MultiLineSegments = new List<LineSegment>();
            foreach (VFKSBPTableItem table in sbpPoint)
            {
                if (prevTable == null)
                {
                    prevTable = table;
                    continue;
                }

                var segment = new LineSegment(Owner.getSouradniceBodu(prevTable.BP_ID),
                    Owner.getSouradniceBodu(table.BP_ID),
                    prevTable, table);
                if (!segment.GetIsValid())
                {
                    throw new VfkMultiLineException(string.Format("Vadny soubor. - Nenalezene body u DPM line:{0}",
                        Item.ID));
                }

                MultiLineSegments.Add(segment);
                prevTable = table;
            }
        }

        public VfkMultiLine(VFKMain owner, VFKOPTableItem opItem, bool fromModifyItem)
        {
            Owner = owner;
            Item = opItem;
            SetVfkElement(Singletons.VFKElements.GetElement(opItem.TYPPPD_KOD));
            List<VFKSBMTableItem> sbmPoints;
            if (fromModifyItem)
            {
                sbmPoints = (from n in Owner.VFKModifySBMItems
                    where n.OP_ID == Item.ID
                    orderby n.PORADOVE_CISLO_BODU
                    select n).ToList();
            }
            else
            {
                sbmPoints = (from n in Owner.VFKSBMTable.Items
                    where n.OP_ID == Item.ID
                    orderby n.PORADOVE_CISLO_BODU
                    select n).ToList();
            }

            VFKSBMTableItem prevTable = null;
            MultiLineSegments = new List<LineSegment>();
            foreach (var table in sbmPoints)
            {
                if (prevTable == null)
                {
                    prevTable = table;
                    continue;
                }

                var segment = new LineSegment(prevTable, table);
                if (!segment.GetIsValid())
                {
                    throw new VfkMultiLineException(string.Format("Vadny soubor. - Nenalezene body u DPM line:{0}",
                        Item.ID));
                }

                MultiLineSegments.Add(segment);
                prevTable = table;
            }
        }

        public VfkMultiLine(VFKMain owner, VFKHBPEJTableItem hbpej, bool fromModifyItems)
        {
            Owner = owner;
            Type = VfkMultiLineType.Hbpej;
            Item = hbpej;
            SetVfkElement(Singletons.VFKElements.GetElement(hbpej.TYPPPD_KOD));
            List<VFKSBMTableItem> sbmPoints;
            if (fromModifyItems)
            {
                sbmPoints = (from n in Owner.VFKModifySBMItems
                    where n.HBPEJ_ID == Item.ID
                    orderby n.PORADOVE_CISLO_BODU
                    select n).ToList();
            }
            else
            {
                sbmPoints = (from n in Owner.VFKSBMTable.Items
                    where n.HBPEJ_ID == Item.ID
                    orderby n.PORADOVE_CISLO_BODU
                    select n).ToList();
            }

            VFKSBMTableItem prevTable = null;
            MultiLineSegments = new List<LineSegment>();
            foreach (var table in sbmPoints)
            {
                if (prevTable == null)
                {
                    prevTable = table;
                    continue;
                }

                var segment = new LineSegment(prevTable, table);
                if (!segment.GetIsValid())
                {
                    throw new VfkMultiLineException(string.Format("Vadny soubor. - Nenalezene body u DPM line:{0}",
                        Item.ID));
                }

                MultiLineSegments.Add(segment);
                prevTable = table;
            }
        }

        
        
        private List<LineSegment> MultiLineSegments { get; set; }

        
        
        private VFKMain Owner { get; set; }
        private VFKDataTableBaseItemWithProp Item { get; set; }
        public VfkMultiLineType Type { get; set; }

        
        
        static int ThresholdPixel = 6;

        static double ThresholdWidth(ICanvas canvas, double objectwidth)
        {
            return ThresholdWidth(canvas, objectwidth, ThresholdPixel);
        }

        public static double ThresholdWidth(ICanvas canvas, double objectwidth, double pixelwidth)
        {
            double minWidth = canvas.ToUnit(pixelwidth);
            double width = Math.Max(objectwidth / 2, minWidth);
            return width;
        }

        public virtual void Copy(VfkMultiLine acopy)
        {
            base.Copy(acopy);
            Selected = acopy.Selected;
            Owner = acopy.Owner;
            Item = acopy.Item;
            Type = acopy.Type;
            MultiLineSegments = acopy.MultiLineSegments;
        }

        
        
        public virtual string Id
        {
            get { return VfkToolBar.VfkMultiLine.Name; }
        }

        public virtual IDrawObject Clone()
        {
            VfkMultiLine l = new VfkMultiLine();
            l.Copy(this);
            return l;
        }

        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            SetVfkElement(Singletons.VFKElements.SelectedSubGroup.SelectedElement);
            if (Type == VfkMultiLineType.Op || Type == VfkMultiLineType.Hbpej ||
                (Type == VfkMultiLineType.DPM &&
                 (VfkElement.DpmType == DpmType.HCHU || VfkElement.DpmType == DpmType.HOCHP)))
            {
                MultiLineSegments = new List<LineSegment>();
                MultiLineSegments.Add(new LineSegment(point, point));
                SetVfkElement(Singletons.VFKElements.SelectedSubGroup.SelectedElement);
                Selected = true;
                return;
            }

            if (snap == null) return;
            MultiLineSegments = new List<LineSegment>();
            MultiLineSegments.Add(new LineSegment(((IVFKTool)snap.Owner).getActivePoint(0),
                ((IVFKTool)snap.Owner).getActivePoint(0), null, null));
            SetVfkElement(Singletons.VFKElements.SelectedSubGroup.SelectedElement);
            Selected = true;
        }

        public Rect GetBoundingRect(ICanvas canvas)
        {
            double thWidth = ThresholdWidth(canvas, Width);
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            if (MultiLineSegments == null || MultiLineSegments.Count == 0)
                return Rect.Empty;
            foreach (var lineSegment in MultiLineSegments)
            {
                minX = Math.Min(Math.Min(lineSegment.P1.X, lineSegment.P2.X), minX);
                minY = Math.Min(Math.Min(lineSegment.P1.Y, lineSegment.P2.Y), minY);
                maxX = Math.Max(Math.Max(lineSegment.P1.X, lineSegment.P2.X), maxX);
                maxY = Math.Max(Math.Max(lineSegment.P1.Y, lineSegment.P2.Y), maxY);
            }

            var p1 = new UnitPoint(minX, minY);
            var p2 = new UnitPoint(maxX, maxY);
            return ScreenUtils.GetRect(p1, p2, thWidth);
        }

        public bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            double thWidth = ThresholdWidth(canvas, Width);
            foreach (var lineSegment in MultiLineSegments)
            {
                if (HitUtil.IsPointInLine(lineSegment.P1, lineSegment.P2, point, thWidth))
                    return true;
            }

            return false;
        }

        public bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint)
        {
            if (anyPoint)
            {
                foreach (var lineSegment in MultiLineSegments)
                {
                    if (HitUtil.LineIntersectWithRect(lineSegment.P1, lineSegment.P2, rect))
                        return true;
                }

                return false;
            }

            Rect boundingrect = GetBoundingRect(canvas);
            return rect.Contains(boundingrect);
        }

        public virtual void Draw(ICanvas canvas, Rect unitrect)
        {
            Color color = Color;
            Pen pen = canvas.CreatePen(color, (float)Width);
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            foreach (var segment in MultiLineSegments)
            {
                canvas.DrawLine(canvas, pen, segment.P1, segment.P2);
                if (Highlighted)
                    canvas.DrawLine(canvas, DrawUtils.SelectedPen, segment.P1, segment.P2);
                if (Selected)
                {
                    canvas.DrawLine(canvas, DrawUtils.SelectedPen, segment.P1, segment.P2);
                    if (segment.P1.IsEmpty == false)
                        DrawUtils.DrawNode(canvas, segment.P1);
                    if (segment.P2.IsEmpty == false)
                        DrawUtils.DrawNode(canvas, segment.P2);
                }
            }
        }

        public virtual void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            MultiLineSegments.Last().Sobr2 = null;
            MultiLineSegments.Last().P2 = point;
        }

        public virtual eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            Selected = false;
            if (Type == VfkMultiLineType.Op || Type == VfkMultiLineType.Hbpej ||
                (Type == VfkMultiLineType.DPM &&
                 (VfkElement.DpmType == DpmType.HCHU || VfkElement.DpmType == DpmType.HOCHP)))
            {
                MultiLineSegments.Last().P2 = point;
            }
            else
                MultiLineSegments.Last().Sobr2 = ((IVFKTool)snappoint.Owner).getActivePoint(0);

            if (Control.ModifierKeys == Keys.Control)
                return eDrawObjectMouseDown.Done;
            return eDrawObjectMouseDown.DoneRepeat;
        }

        public void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
        }

        public virtual void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }

        public UnitPoint RepeatStartingPoint
        {
            get { return MultiLineSegments.Last().P2; }
        }

        public INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            return null;
        }

        public virtual ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobjs,
            Type[] runningsnaptypes, Type usersnaptype)
        {
            return null;
        }

        public void Move(UnitPoint offset)
        {
        }

        public bool getSelectDrawToolCreate()
        {
            return false;
        }

        public virtual string GetInfoAsString()
        {
            return string.Format("VfkMultiLine, Typ={0}, Pocet Bodu={1}, Typppd_kod={2}", VfkElement,
                MultiLineSegments.Count, VfkElement.TYPPPD_KOD);
        }

        public void Export(IExport export)
        {
            if (MultiLineSegments.Count == 0)
                return;
            if (MultiLineSegments.Count == 1)
            {
                LineSegment line = MultiLineSegments[0];
                export.AddLine(line.P1, line.P2, Color, Width);
                return;
            }

            UnitPoint[] points = new UnitPoint[MultiLineSegments.Count + 1];
            for (int i = 0; i < MultiLineSegments.Count; i++)
                points[i] = MultiLineSegments[i].P1;
            points[points.Length - 1] = MultiLineSegments[MultiLineSegments.Count - 1].P2;
            export.AddPolyline(ref points, Color, Width);
        }

        
        
        public override void RegisterObject(IVFKMain aOwner)
        {
            bool newItem = Item == null;
            if (Type == VfkMultiLineType.Op || Type == VfkMultiLineType.Hbpej ||
                (Type == VfkMultiLineType.DPM &&
                 (VfkElement.DpmType == DpmType.HCHU || VfkElement.DpmType == DpmType.HOCHP)))
            {
                foreach (var segment in MultiLineSegments)
                {
                    UnitPoint backup = segment.P1;
                    segment.Sbm1 = aOwner.UpdateSbm(segment.Sbm1);
                    segment.P1 = backup;
                    if (segment == MultiLineSegments.Last())
                    {
                        backup = segment.P2;
                        segment.Sbm2 = aOwner.UpdateSbm(segment.Sbm2);
                        segment.P2 = backup;
                    }
                }
            }
            else
            {
                foreach (var segment in MultiLineSegments)
                {
                    segment.Sbp1 = aOwner.UpdateSBP(segment.Sbp1);
                    if (segment == MultiLineSegments.Last())
                        segment.Sbp2 = aOwner.UpdateSBP(segment.Sbp2);
                }
            }

            switch (Type)
            {
                case VfkMultiLineType.HP:
                {
                    VFKHPTableItem hp = Item as VFKHPTableItem;
                    Item = hp = aOwner.UpdateHP(hp);
                    if (newItem)
                    {
                        UInt64 number = 1;
                        foreach (var segment in MultiLineSegments)
                        {
                            segment.Sbp1.HP_ID = hp.ID;
                            segment.Sbp1.PORADOVE_CISLO_BODU = number;
                            segment.Sbp1.BP_ID = segment.Sobr1.ID;
                            segment.Sbp1.PARAMETRY_SPOJENI = "3";
                            number++;
                            if (segment == MultiLineSegments.Last())
                            {
                                segment.Sbp2.HP_ID = hp.ID;
                                segment.Sbp2.PORADOVE_CISLO_BODU = number;
                                segment.Sbp2.BP_ID = segment.Sobr2.ID;
                                segment.Sbp2.PARAMETRY_SPOJENI = "3";
                                number++;
                            }
                        }

                        hp.TYPPPD_KOD = TYPPPD_KOD;
                    }
                }
                    break;
                case VfkMultiLineType.DPM:
                {
                    VFKDPMTableItem dpm = Item as VFKDPMTableItem;
                    Item = dpm = aOwner.UpdateDPM(dpm);
                    if (newItem)
                    {
                        UInt64 number = 1;
                        if (VfkElement.DpmType == DpmType.HCHU || VfkElement.DpmType == DpmType.HOCHP)
                        {
                            foreach (var segment in MultiLineSegments)
                            {
                                segment.Sbm1.DPM_ID = dpm.ID;
                                segment.Sbm1.PORADOVE_CISLO_BODU = number;
                                segment.Sbm1.SOURADNICE_X = -segment.P1.Y;
                                segment.Sbm1.SOURADNICE_Y = -segment.P1.X;
                                segment.Sbm1.PARAMETRY_SPOJENI = "3";
                                number++;
                                if (segment == MultiLineSegments.Last())
                                {
                                    segment.Sbm2.DPM_ID = dpm.ID;
                                    segment.Sbm2.PORADOVE_CISLO_BODU = number;
                                    segment.Sbm2.PARAMETRY_SPOJENI = "3";
                                    segment.Sbm2.SOURADNICE_X = -segment.P1.Y;
                                    segment.Sbm2.SOURADNICE_Y = -segment.P1.X;
                                    number++;
                                }
                            }
                        }
                        else
                        {
                            foreach (var segment in MultiLineSegments)
                            {
                                segment.Sbp1.DPM_ID = dpm.ID;
                                segment.Sbp1.PORADOVE_CISLO_BODU = number;
                                segment.Sbp1.BP_ID = segment.Sobr1.ID;
                                segment.Sbp1.PARAMETRY_SPOJENI = "3";
                                number++;
                                if (segment == MultiLineSegments.Last())
                                {
                                    segment.Sbp2.DPM_ID = dpm.ID;
                                    segment.Sbp2.PORADOVE_CISLO_BODU = number;
                                    segment.Sbp2.BP_ID = segment.Sobr2.ID;
                                    segment.Sbp2.PARAMETRY_SPOJENI = "3";
                                    number++;
                                }
                            }
                        }

                        dpm.TYPPPD_KOD = TYPPPD_KOD;
                        dpm.DPM_TYPE = VfkElement.DpmType.ToString();
                    }
                }
                    break;
                case VfkMultiLineType.ZVB:
                {
                    VFKZVBTableItem zvb = Item as VFKZVBTableItem;
                    Item = zvb = aOwner.UpdateZVB(zvb);
                    if (newItem)
                    {
                        UInt64 number = 1;
                        foreach (var segment in MultiLineSegments)
                        {
                            segment.Sbp1.ZVB_ID = zvb.ID;
                            segment.Sbp1.PORADOVE_CISLO_BODU = number;
                            segment.Sbp1.BP_ID = segment.Sobr1.ID;
                            segment.Sbp1.PARAMETRY_SPOJENI = "3";
                            number++;
                            if (segment == MultiLineSegments.Last())
                            {
                                segment.Sbp2.ZVB_ID = zvb.ID;
                                segment.Sbp2.PORADOVE_CISLO_BODU = number;
                                segment.Sbp2.BP_ID = segment.Sobr2.ID;
                                segment.Sbp2.PARAMETRY_SPOJENI = "3";
                                number++;
                            }
                        }

                        zvb.TYPPPD_KOD = TYPPPD_KOD;
                    }
                }
                    break;
                case VfkMultiLineType.Ob:
                {
                    VFKOBTableItem ob = Item as VFKOBTableItem;
                    Item = ob = aOwner.UpdateOB(ob);
                    if (newItem)
                    {
                        UInt64 number = 1;
                        foreach (var segment in MultiLineSegments)
                        {
                            segment.Sbp1.OB_ID = ob.ID;
                            segment.Sbp1.PORADOVE_CISLO_BODU = number;
                            segment.Sbp1.BP_ID = segment.Sobr1.ID;
                            segment.Sbp1.PARAMETRY_SPOJENI = "3";
                            number++;
                            if (segment == MultiLineSegments.Last())
                            {
                                segment.Sbp2.OB_ID = ob.ID;
                                segment.Sbp2.PORADOVE_CISLO_BODU = number;
                                segment.Sbp2.BP_ID = segment.Sobr2.ID;
                                segment.Sbp2.PARAMETRY_SPOJENI = "3";
                                number++;
                            }
                        }

                        ob.TYPPPD_KOD = VfkElement.TYPPPD_KOD;
                        ob.OBRBUD_TYPE = VfkElement.ObrBudType.ToString();
                    }
                }
                    break;
                case VfkMultiLineType.Op:
                {
                    VFKOPTableItem op = Item as VFKOPTableItem;
                    Item = op = aOwner.UpdateOP(op);
                    if (newItem)
                    {
                        UInt64 number = 1;
                        foreach (var segment in MultiLineSegments)
                        {
                            segment.Sbm1.OP_ID = op.ID;
                            segment.Sbm1.PORADOVE_CISLO_BODU = number;
                            segment.Sbm1.SOURADNICE_X = -segment.P1.Y;
                            segment.Sbm1.SOURADNICE_Y = -segment.P1.X;
                            segment.Sbm1.PARAMETRY_SPOJENI = "3";
                            number++;
                            if (segment == MultiLineSegments.Last())
                            {
                                segment.Sbm2.OP_ID = op.ID;
                                segment.Sbm2.PORADOVE_CISLO_BODU = number;
                                segment.Sbm2.PARAMETRY_SPOJENI = "3";
                                segment.Sbm2.SOURADNICE_X = -segment.P1.Y;
                                segment.Sbm2.SOURADNICE_Y = -segment.P1.X;
                                number++;
                            }
                        }

                        op.TYPPPD_KOD = TYPPPD_KOD;
                        op.OPAR_TYPE = VfkElement.OparType;
                    }
                }
                    break;
                case VfkMultiLineType.Hbpej:
                {
                    VFKHBPEJTableItem hbpej = Item as VFKHBPEJTableItem;
                    Item = hbpej = aOwner.UpdateHbpej(hbpej);
                    if (newItem)
                    {
                        UInt64 number = 1;
                        foreach (var segment in MultiLineSegments)
                        {
                            segment.Sbm1.HBPEJ_ID = hbpej.ID;
                            segment.Sbm1.PORADOVE_CISLO_BODU = number;
                            segment.Sbm1.SOURADNICE_X = -segment.P1.Y;
                            segment.Sbm1.SOURADNICE_Y = -segment.P1.X;
                            segment.Sbm1.PARAMETRY_SPOJENI = "3";
                            number++;
                            if (segment == MultiLineSegments.Last())
                            {
                                segment.Sbm2.HBPEJ_ID = hbpej.ID;
                                segment.Sbm2.PORADOVE_CISLO_BODU = number;
                                segment.Sbm2.PARAMETRY_SPOJENI = "3";
                                segment.Sbm2.SOURADNICE_X = -segment.P1.Y;
                                segment.Sbm2.SOURADNICE_Y = -segment.P1.X;
                                number++;
                            }
                        }

                        hbpej.TYPPPD_KOD = TYPPPD_KOD;
                    }
                }
                    break;
                default:
                    throw new VfkMultiLineException("Neni naprogramovano.");
            }
        }

        public override void DeleteObject(IVFKMain aOwner)
        {
            if (Type == VfkMultiLineType.Op || Type == VfkMultiLineType.Hbpej ||
                (Type == VfkMultiLineType.DPM &&
                 (VfkElement.DpmType == DpmType.HCHU || VfkElement.DpmType == DpmType.HOCHP)))
            {
                foreach (var segment in MultiLineSegments)
                {
                    aOwner.DeleteSbm(segment.Sbm1);
                    if (segment == MultiLineSegments.Last())
                        aOwner.DeleteSbm(segment.Sbm2);
                }
            }
            else
            {
                foreach (var segment in MultiLineSegments)
                {
                    aOwner.DeleteSBP(segment.Sbp1);
                    if (segment == MultiLineSegments.Last())
                        aOwner.DeleteSBP(segment.Sbp2);
                }
            }

            switch (Type)
            {
                case VfkMultiLineType.HP:
                    aOwner.DeleteHP(Item as VFKHPTableItem);
                    break;
                case VfkMultiLineType.DPM:
                    aOwner.DeleteDPM(Item as VFKDPMTableItem);
                    break;
                case VfkMultiLineType.ZVB:
                    aOwner.DeleteZVB(Item as VFKZVBTableItem);
                    break;
                case VfkMultiLineType.Ob:
                    aOwner.DeleteOB(Item as VFKOBTableItem);
                    break;
                case VfkMultiLineType.Op:
                    aOwner.DeleteOP(Item as VFKOPTableItem);
                    break;
                case VfkMultiLineType.Hbpej:
                    aOwner.DeleteHbpej(Item as VFKHBPEJTableItem);
                    break;
                default:
                    throw new VfkMultiLineException("Neni naprogramovano.");
            }

            if (!Item.ItemFromImport())
                Item = null;
        }

        public override bool GetMustBeConnectedWithSnap()
        {
            if (Singletons.VFKElements.SelectedSubGroup.SelectedElement.TYPPPD_KOD == 1032) //Čára pro umístění šipky
                return false;
            if (Singletons.VFKElements.SelectedSubGroup.SelectedElement.DpmType == DpmType.HCHU ||
                Singletons.VFKElements.SelectedSubGroup.SelectedElement.DpmType == DpmType.HOCHP)
                return false;
            if (Singletons.VFKElements.SelectedSubGroup.GroupId == 15) //HBPEJ
                return false;
            return true;
        }

        public override void SetVfkElement(VfkElement vfkElement)
        {
            base.SetVfkElement(vfkElement);
            UseLayerColor = false;
            BlockNvf type = vfkElement.BlockNvf;
            if (type == BlockNvf.HP)
                Type = VfkMultiLineType.HP;
            else if (type == BlockNvf.DPM)
                Type = VfkMultiLineType.DPM;
            else if (type == BlockNvf.ZVB)
                Type = VfkMultiLineType.ZVB;
            else if (type == BlockNvf.OB)
                Type = VfkMultiLineType.Ob;
            else if (type == BlockNvf.HBPEJ)
                Type = VfkMultiLineType.Hbpej;
            else if (type == BlockNvf.OP)
                Type = VfkMultiLineType.Op;
            else
            {
                throw new VfkMultiLineException(string.Format("Typ cary:{0} neni podporovan.", type));
            }
        }

            }

    }