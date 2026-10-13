using System;
using System.Collections.Generic;
using System.Linq;
using CAD.DTM.Elements;
using CAD.DTM.Elements.Linie;

namespace CAD.DTM.Validation.Rules
{
    public class DtmTopologyGroupRule : DtmCompositeValidationRule
    {
        public override string Code => "1.4.3";
        public override string Name => "Topologické kontroly";

        protected override IEnumerable<DtmValidationRule> CreateChildren()
        {
            yield return new DtmVertexPodrobnyBodRule();
            yield return new DtmLineOverlapRule();
            yield return new DtmLineCrossingRule();
            yield return new DtmLineSelfCrossingRule();
            yield return new DtmZeroSegmentLengthRule();
            yield return new DtmLineDuplicityRule();
            yield return new DtmSkippedRule("3.7", "Volné konce linií", "Vyžaduje oblast kompletní ZPS (OKZPS), která není v modelu k dispozici.");
            yield return new DtmPointDuplicityRule();
            yield return new DtmPointProximityRule();
            yield return new DtmShortSegmentRule();
            yield return new DtmSolitaryPodrobnyBodRule();
            yield return new DtmSkippedRule("3.12", "Obvod OKZPS", "Vyžaduje oblast kompletní ZPS (OKZPS), která není v modelu k dispozici.");
            yield return new DtmPointToLineDistanceRule();
        }
    }

    public class DtmExchangeFormatGroupRule : DtmCompositeValidationRule
    {
        readonly DtmValidationRule[] _children;
        public DtmExchangeFormatGroupRule(params DtmValidationRule[] children) { _children = children; }
        public override string Code => "1.4.1";
        public override string Name => "Kontroly výměnného formátu";
        protected override IEnumerable<DtmValidationRule> CreateChildren() => _children;
    }

    public class DtmSkippedRule : DtmValidationRule
    {
        readonly string _note;
        public DtmSkippedRule(string code, string name, string note)
        {
            Code = code;
            Name = name;
            _note = note;
        }
        public override string Code { get; }
        public override string Name { get; }
        public override string Description
        {
            get
            {
                var d = DtmValidationDescriptions.Get("Skipped:" + Code);
                return d == null ? _note : d + "\n\nNeprovádí se: " + _note;
            }
        }

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            result.IsSkipped = true;
            result.Note = _note;
        }
    }

    internal class DtmSegment
    {
        public DtmValidationItem Item;
        public int Index;
        public DtmPoint A, B;
        public string Set;
        public double MinX, MaxX, MinY, MaxY;
    }

    internal static class DtmTopologyHelper
    {
        public const int MaxIssues = 500;

        public static string SetKey(DtmValidationContext ctx, DtmValidationItem item)
        {
            var key = ctx.Settings.ConstructionLineGroups.Contains(item.Group) ? "K" : item.Group;
            string special = null;
            if (item.Element is DtmHraniceDopravniStavbyPlochyElement d)
            {
                if (d.TypDopravniStavbyNeboPlochy == DtmTypDopravniStavbyNeboPlochyEnum.PlochaMostniKonstrukce) special = "most";
                else if (d.TypDopravniStavbyNeboPlochy == DtmTypDopravniStavbyNeboPlochyEnum.ProvozniPlochaTunelu) special = "tunel";
                else if (d.TypDopravniStavbyNeboPlochy == DtmTypDopravniStavbyNeboPlochyEnum.ProvozniPlochaPodchodu) special = "podchod";
            }
            return item.Level + "|" + (special ?? key);
        }

        public static List<DtmSegment> Segments(DtmValidationContext ctx, IEnumerable<DtmValidationItem> items)
        {
            var list = new List<DtmSegment>();
            foreach (var item in items)
            {
                var set = SetKey(ctx, item);
                foreach (var g in item.Geometries.OfType<DtmCurveGeometry>())
                {
                    var pts = g.Points;
                    if (pts == null) continue;
                    for (var i = 0; i + 1 < pts.Count; i++)
                        list.Add(new DtmSegment
                        {
                            Item = item, Index = i, A = pts[i], B = pts[i + 1], Set = set,
                            MinX = Math.Min(pts[i].X, pts[i + 1].X), MaxX = Math.Max(pts[i].X, pts[i + 1].X),
                            MinY = Math.Min(pts[i].Y, pts[i + 1].Y), MaxY = Math.Max(pts[i].Y, pts[i + 1].Y)
                        });
                }
            }
            return list;
        }

        // Dvojice usecek s prekryvajicimi se obalkami (sweep podle X).
        public static IEnumerable<Tuple<DtmSegment, DtmSegment>> Pairs(List<DtmSegment> segs, double eps)
        {
            var sorted = segs.OrderBy(s => s.MinX).ToList();
            for (var i = 0; i < sorted.Count; i++)
            {
                var a = sorted[i];
                for (var j = i + 1; j < sorted.Count && sorted[j].MinX <= a.MaxX + eps; j++)
                {
                    var b = sorted[j];
                    if (b.MinY > a.MaxY + eps || a.MinY > b.MaxY + eps) continue;
                    yield return Tuple.Create(a, b);
                }
            }
        }

        public static bool IsEndpoint(DtmSegment s, int pointIndex, DtmPoint p, double eps, int pointCount)
        {
            return false;
        }

        public static string Coord(double x, double y) => $"[{x:0.00}; {y:0.00}]";
    }

    public class DtmVertexPodrobnyBodRule : DtmValidationRule
    {
        public override string Code => "3.1";
        public override string Name => "Každý vrchol bodu a linie má podrobný bod se shodnými souřadnicemi XYZ a úrovní";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var pb = new HashSet<string>(ctx.PodrobneBody.SelectMany(i => i.Points.Select(p => i.Level + "|" + DtmGeometryMath.Key3D(p))));
            var count = 0;
            foreach (var item in ctx.PointObjects.Concat(ctx.Lines))
            {
                foreach (var p in item.Geometries.Where(g => g is DtmPointGeometry || g is DtmCurveGeometry).SelectMany(DtmValidationContext.GetPoints))
                {
                    if (pb.Contains(item.Level + "|" + DtmGeometryMath.Key3D(p))) continue;
                    if (count++ < DtmTopologyHelper.MaxIssues)
                        result.AddError(item.Name, $"Vrchol {DtmTopologyHelper.Coord(p.X, p.Y)} Z={p.Z:0.00} nemá podrobný bod.");
                }
            }
        }
    }

    public class DtmLineOverlapRule : DtmValidationRule
    {
        public override string Code => "3.2";
        public override string Name => "Překrytí linií (shodný úsek ve 2D)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var eps = ctx.Settings.GeometryTolerance;
            var count = 0;
            foreach (var pair in DtmTopologyHelper.Pairs(DtmTopologyHelper.Segments(ctx, ctx.Lines), eps))
            {
                var a = pair.Item1;
                var b = pair.Item2;
                if (a.Set != b.Set || a.Item == b.Item) continue;
                var rel = DtmGeometryMath.Relate(a.A, a.B, b.A, b.B, eps);
                if (rel.Kind != DtmSegmentRelationKind.Overlap) continue;
                // zcela shodne linie patri do kontroly duplicity 3.6
                if (IsIdentical(a.Item, b.Item)) continue;
                if (count++ < DtmTopologyHelper.MaxIssues)
                    result.AddError(a.Item.Name, $"Překrývá se s {b.Item.Name} v místě {DtmTopologyHelper.Coord(rel.X, rel.Y)}.");
            }
        }

        public static bool IsIdentical(DtmValidationItem a, DtmValidationItem b)
        {
            var ka = a.LinePoints.Select(DtmGeometryMath.Key2D).ToList();
            var kb = b.LinePoints.Select(DtmGeometryMath.Key2D).ToList();
            if (ka.Count != kb.Count) return false;
            return ka.SequenceEqual(kb) || ka.AsEnumerable().Reverse().SequenceEqual(kb);
        }
    }

    public class DtmLineCrossingRule : DtmValidationRule
    {
        public override string Code => "3.3";
        public override string Name => "Křížení linií (bodový průsečík je povolen jen v koncových bodech obou linií)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var eps = ctx.Settings.GeometryTolerance;
            var count = 0;
            foreach (var pair in DtmTopologyHelper.Pairs(DtmTopologyHelper.Segments(ctx, ctx.Lines), eps))
            {
                var a = pair.Item1;
                var b = pair.Item2;
                if (a.Set != b.Set || a.Item == b.Item) continue;
                var rel = DtmGeometryMath.Relate(a.A, a.B, b.A, b.B, eps);
                if (rel.Kind != DtmSegmentRelationKind.Point) continue;
                var p = new DtmPoint { X = rel.X, Y = rel.Y };
                if (IsEnd(a.Item, p, eps) && IsEnd(b.Item, p, eps)) continue;
                if (count++ < DtmTopologyHelper.MaxIssues)
                    result.AddError(a.Item.Name, $"Kříží se s {b.Item.Name} v místě {DtmTopologyHelper.Coord(rel.X, rel.Y)}.");
            }
        }

        static bool IsEnd(DtmValidationItem item, DtmPoint p, double eps)
        {
            foreach (var g in item.Geometries.OfType<DtmCurveGeometry>())
            {
                if (g.Points == null || g.Points.Count == 0) continue;
                if (DtmGeometryMath.Distance2D(g.Points[0], p) <= eps || DtmGeometryMath.Distance2D(g.Points[g.Points.Count - 1], p) <= eps)
                    return true;
            }
            return false;
        }
    }

    public class DtmLineSelfCrossingRule : DtmValidationRule
    {
        public override string Code => "3.4";
        public override string Name => "Samokřížení linie";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var eps = ctx.Settings.GeometryTolerance;
            foreach (var item in ctx.Lines)
            {
                foreach (var g in item.Geometries.OfType<DtmCurveGeometry>())
                {
                    var pts = g.Points;
                    if (pts == null) continue;
                    var found = false;
                    for (var i = 0; i + 1 < pts.Count && !found; i++)
                        for (var j = i + 2; j + 1 < pts.Count && !found; j++)
                        {
                            var rel = DtmGeometryMath.Relate(pts[i], pts[i + 1], pts[j], pts[j + 1], eps);
                            if (rel.Kind == DtmSegmentRelationKind.None) continue;
                            // uzavrena linie: prvni a posledni usecka se dotykaji v koncovem bode
                            var closing = i == 0 && j + 2 == pts.Count && DtmGeometryMath.Distance2D(pts[0], pts[pts.Count - 1]) <= eps
                                && rel.Kind == DtmSegmentRelationKind.Point
                                && DtmGeometryMath.Distance2D(new DtmPoint { X = rel.X, Y = rel.Y }, pts[0]) <= eps;
                            if (closing) continue;
                            found = true;
                            result.AddError(item.Name, $"Linie se kříží sama se sebou v místě {DtmTopologyHelper.Coord(rel.X, rel.Y)}.");
                        }
                }
            }
        }
    }

    public class DtmZeroSegmentLengthRule : DtmValidationRule
    {
        public override string Code => "3.5";
        public override string Name => "Nulová 3D délka segmentu linie";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var count = 0;
            foreach (var s in DtmTopologyHelper.Segments(ctx, ctx.Lines))
            {
                if (DtmGeometryMath.Distance3D(s.A, s.B) > 1e-6) continue;
                if (count++ < DtmTopologyHelper.MaxIssues)
                    result.AddError(s.Item.Name, $"Segment {s.Index + 1} má nulovou délku v místě {DtmTopologyHelper.Coord(s.A.X, s.A.Y)}.");
            }
        }
    }

    public class DtmLineDuplicityRule : DtmValidationRule
    {
        public override string Code => "3.6";
        public override string Name => "Duplicita linií (shodné XY; shodné Z je chyba, rozdíl Z menší než 0,12 m varování)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var lines = ctx.Lines.ToList();
            var groups = lines.GroupBy(l => l.Level + "|" + DtmTopologyHelper.SetKey(ctx, l) + "|" + Canonical(l));
            foreach (var grp in groups.Where(g => g.Count() > 1))
            {
                var items = grp.ToList();
                for (var i = 0; i < items.Count; i++)
                    for (var j = i + 1; j < items.Count; j++)
                    {
                        var maxDz = MaxDz(items[i], items[j]);
                        if (maxDz < 0.005)
                            result.AddError(items[i].Name, $"Duplicitní linie s {items[j].Name} (shodné XYZ).");
                        else if (maxDz < ctx.Settings.MinDuplicateZDifference)
                            result.AddWarning(items[i].Name, $"Linie se shoduje s {items[j].Name} v XY, rozdíl Z je {maxDz:0.00} m.");
                    }
            }
        }

        static string Canonical(DtmValidationItem l)
        {
            var a = string.Join(";", l.LinePoints.Select(DtmGeometryMath.Key2D));
            var b = string.Join(";", l.LinePoints.Reverse().Select(DtmGeometryMath.Key2D));
            return string.CompareOrdinal(a, b) <= 0 ? a : b;
        }

        static double MaxDz(DtmValidationItem a, DtmValidationItem b)
        {
            var pa = a.LinePoints.ToList();
            var pb = b.LinePoints.ToList();
            if (!pa.Select(DtmGeometryMath.Key2D).SequenceEqual(pb.Select(DtmGeometryMath.Key2D)))
                pb.Reverse();
            return pa.Zip(pb, (x, y) => Math.Abs(x.Z - y.Z)).DefaultIfEmpty(0).Max();
        }
    }

    public class DtmPointDuplicityRule : DtmValidationRule
    {
        public override string Code => "3.8";
        public override string Name => "Duplicita bodů (podrobné body a bodové objekty ve 3D, definiční body ve 2D v rámci úrovně)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var points = ctx.PodrobneBody.Concat(ctx.PointObjects)
                .SelectMany(i => i.Points.Select(p => new { Item = i, P = p }))
                .GroupBy(x => x.Item.Group + "|" + DtmGeometryMath.Key3D(x.P));
            foreach (var g in points)
            {
                var list = g.ToList();
                var distinct = list.Select(x => x.Item).Distinct().ToList();
                if (distinct.Count > 1)
                    result.AddError(distinct[0].Name, $"Duplicitní bod s {distinct[1].Name} v místě {DtmTopologyHelper.Coord(list[0].P.X, list[0].P.Y)} Z={list[0].P.Z:0.00}.");
            }
            foreach (var g in ctx.DefinitionPoints.GroupBy(d => d.Level + "|" + string.Join(";", d.Points.Select(DtmGeometryMath.Key2D))))
            {
                var list = g.ToList();
                if (list.Count > 1)
                    result.AddError(list[0].Name, $"Duplicitní definiční bod s {list[1].Name}.");
            }
        }
    }

    public class DtmPointProximityRule : DtmValidationRule
    {
        public override string Code => "3.9";
        public override string Name => "Bodové objekty a podrobné body blíž než 0,05 m (3D)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var min = ctx.Settings.MinPointDistance;
            var pts = ctx.PodrobneBody.Concat(ctx.PointObjects)
                .SelectMany(i => i.Points.Select(p => new { Item = i, P = p })).OrderBy(x => x.P.X).ToList();
            var count = 0;
            for (var i = 0; i < pts.Count; i++)
                for (var j = i + 1; j < pts.Count && pts[j].P.X - pts[i].P.X < min; j++)
                {
                    if (pts[i].Item == pts[j].Item) continue;
                    var d = DtmGeometryMath.Distance3D(pts[i].P, pts[j].P);
                    if (d <= 0.0005 || d >= min) continue;
                    if (count++ < DtmTopologyHelper.MaxIssues)
                        result.AddWarning(pts[i].Item.Name, $"Vzdálenost od {pts[j].Item.Name} je {d:0.000} m, v místě {DtmTopologyHelper.Coord(pts[i].P.X, pts[i].P.Y)}.");
                }
        }
    }

    public class DtmShortSegmentRule : DtmValidationRule
    {
        public override string Code => "3.10";
        public override string Name => "Segmenty linií kratší než 0,05 m";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var count = 0;
            foreach (var s in DtmTopologyHelper.Segments(ctx, ctx.Lines))
            {
                var d = DtmGeometryMath.Distance3D(s.A, s.B);
                if (d <= 1e-6 || d >= ctx.Settings.MinSegmentLength) continue;
                if (count++ < DtmTopologyHelper.MaxIssues)
                    result.AddWarning(s.Item.Name, $"Segment {s.Index + 1} má délku {d:0.000} m v místě {DtmTopologyHelper.Coord(s.A.X, s.A.Y)}.");
            }
        }
    }

    public class DtmSolitaryPodrobnyBodRule : DtmValidationRule
    {
        public override string Code => "3.11";
        public override string Name => "Solitérní podrobný bod v oblasti změny";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var area = ctx.GetChangeArea();
            var used = new HashSet<string>(ctx.PointObjects.Concat(ctx.Lines)
                .SelectMany(i => i.Points.Select(p => i.Level + "|" + DtmGeometryMath.Key3D(p))));
            foreach (var item in ctx.PodrobneBody)
            {
                foreach (var p in item.Points)
                {
                    if (area != null && area.Count >= 3 && !DtmGeometryMath.IsInsideOrOnBoundary(p, area, ctx.Settings.OnBoundaryTolerance)) continue;
                    if (!used.Contains(item.Level + "|" + DtmGeometryMath.Key3D(p)))
                        result.AddError(item.Name, $"Podrobný bod {DtmTopologyHelper.Coord(p.X, p.Y)} Z={p.Z:0.00} není vrcholem žádného objektu.");
                }
            }
        }
    }

    public class DtmPointToLineDistanceRule : DtmValidationRule
    {
        public override string Code => "3.13";
        public override string Name => "Minimální vzdálenost bodu od linie (3D) 0,05 m";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var min = ctx.Settings.MinPointToLineDistance;
            var segs = DtmTopologyHelper.Segments(ctx, ctx.Lines);
            var pts = ctx.PodrobneBody.Concat(ctx.PointObjects)
                .SelectMany(i => i.Points.Select(p => new { Item = i, P = p })).ToList();
            var count = 0;
            foreach (var s in segs)
            {
                foreach (var x in pts)
                {
                    if (x.P.X < s.MinX - min || x.P.X > s.MaxX + min || x.P.Y < s.MinY - min || x.P.Y > s.MaxY + min) continue;
                    if (x.Item.Level != s.Item.Level) continue;
                    var d = DtmGeometryMath.DistanceToSegment3D(x.P, s.A, s.B);
                    // body lezici ve vrcholu linie (d ~ 0) jsou v poradku
                    if (d <= 0.0005 || d >= min) continue;
                    if (count++ < DtmTopologyHelper.MaxIssues)
                        result.AddWarning(x.Item.Name, $"Bod je {d:0.000} m od linie {s.Item.Name} v místě {DtmTopologyHelper.Coord(x.P.X, x.P.Y)}.");
                }
            }
        }
    }
}
