using System;
using System.Collections.Generic;
using System.Linq;

namespace CAD.DTM.Validation.Rules
{
    public class DtmDerivedObjectsGroupRule : DtmCompositeValidationRule
    {
        public override string Code => "1.4.4";
        public override string Name => "Kontroly odvozených objektů";

        protected override IEnumerable<DtmValidationRule> CreateChildren()
        {
            yield return new DtmMinimalAreaRule();
            yield return new DtmAreaDefinitionPointRule();
            yield return new DtmSkippedRule("4.4", "Hierarchie ploch", "Vyžaduje tabulky hierarchie a oblast kompletní ZPS, které nejsou v modelu k dispozici.");
            yield return new DtmAreaCollisionRule();
            yield return new DtmSkippedRule("4.6", "Kontrola vůči referenčnímu stavu", "Vyžaduje referenční stav topologie, který není v modelu k dispozici.");
            yield return new DtmDefinitionPointWithoutAreaRule();
            yield return new DtmSkippedRule("4.8", "Kontrola odvozených objektů v OKZPS", "Vyžaduje oblast kompletní ZPS (OKZPS).");
            yield return new DtmSkippedRule("4.9", "Kontrola odvozených objektů vůči referenčnímu stavu", "Vyžaduje referenční stav a OKZPS.");
            yield return new DtmSkippedRule("4.10", "Kontrola hran svislých ploch", "Vyžaduje data svislých hran, která nejsou v modelu k dispozici.");
            yield return new DtmSkippedRule("4.11", "Kontrola vzájemné polohy svislých ploch", "Vyžaduje data svislých hran, která nejsou v modelu k dispozici.");
        }
    }

    static class DtmAreaHelper
    {
        public static List<DtmPoint> Ring(DtmValidationItem item)
        {
            var g = item.Geometries.OfType<DtmPolygonGeometry>().FirstOrDefault();
            return g?.Points;
        }
    }

    public class DtmMinimalAreaRule : DtmValidationRule
    {
        public override string Code => "4.1";
        public override string Name => "Minimální velikost plochy 0,05 m²";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            foreach (var item in ctx.Areas.Where(DtmValidationContext.IsChanged))
            {
                var ring = DtmAreaHelper.Ring(item);
                if (ring == null || ring.Count < 3) continue;
                var a = DtmGeometryMath.Area2D(ring);
                if (a < ctx.Settings.MinAreaSize)
                    result.AddObjectError(item, $"Plocha má výměru {a:0.000} m².");
            }
        }
    }

    public class DtmAreaDefinitionPointRule : DtmValidationRule
    {
        public override string Code => "4.2";
        public override string Name => "Každá plocha obsahuje právě jeden definiční bod stejné úrovně";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var defs = ctx.DefinitionPoints.SelectMany(d => d.Points.Select(p => new { Item = d, P = p })).ToList();
            var originalDefs = ctx.OriginalItems.Where(d => d.IsZps && d.Type == DtmElementType.DefinicniBod)
                .SelectMany(d => d.Points.Select(p => new { Item = d, P = p })).ToList();
            foreach (var area in ctx.Areas)
            {
                var ring = DtmAreaHelper.Ring(area);
                if (ring == null || ring.Count < 3) continue;
                var count = defs.Count(d => d.Item.Level == area.Level && DtmGeometryMath.IsInsideOrOnBoundary(d.P, ring, ctx.Settings.GeometryTolerance));
                if (!DtmValidationContext.IsChanged(area))
                {
                    var original = ctx.OriginalOf(area);
                    var originalRing = original == null ? null : DtmAreaHelper.Ring(original);
                    if (originalRing == null || originalRing.Count < 3) continue;
                    var before = originalDefs.Count(d => d.Item.Level == original.Level && DtmGeometryMath.IsInsideOrOnBoundary(d.P, originalRing, ctx.Settings.GeometryTolerance));
                    // Report a newly missing point, or a new/worsened excess of points.
                    if (!(count == 0 && before > 0) && !(count > 1 && (before <= 1 || count > before))) continue;
                }
                if (count == 0)
                    result.AddObjectError(area, "Plocha neobsahuje definiční bod.");
                else if (count > 1)
                    result.AddObjectError(area, $"Plocha obsahuje {count} definičních bodů.");
            }
        }
    }

    public class DtmAreaCollisionRule : DtmValidationRule
    {
        public override string Code => "4.5";
        public override string Name => "Kolize ploch (překryv ploch ve stejné úrovni)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var areas = ctx.Areas.Select(a => new { Item = a, Ring = DtmAreaHelper.Ring(a) })
                .Where(a => a.Ring != null && a.Ring.Count >= 3)
                .Select(a => new { a.Item, a.Ring, Box = DtmGeometryMath.Box(a.Ring) }).ToList();
            var tol = ctx.Settings.GeometryTolerance;
            for (var i = 0; i < areas.Count; i++)
                for (var j = i + 1; j < areas.Count; j++)
                {
                    var a = areas[i];
                    var b = areas[j];
                    if (!DtmValidationContext.IsChangedPair(a.Item, b.Item)) continue;
                    if (a.Item.Level != b.Item.Level || a.Item.Group != b.Item.Group) continue;
                    if (!DtmGeometryMath.BoxesOverlap(a.Box, b.Box, tol)) continue;
                    if (Collides(a.Ring, b.Ring, tol) || Collides(b.Ring, a.Ring, tol))
                        result.AddObjectError(a.Item, $"Plocha se překrývá s {b.Item.Name}.");
                }
        }

        // vrchol jedne plochy lezi uvnitr druhe, nebo se hrany protinaji mimo vrcholy
        static bool Collides(IList<DtmPoint> a, IList<DtmPoint> b, double tol)
        {
            if (a.Any(p => DtmGeometryMath.IsStrictlyInside(p, b, tol)))
                return true;
            for (var i = 0; i < a.Count; i++)
            {
                var a1 = a[i];
                var a2 = a[(i + 1) % a.Count];
                for (var j = 0; j < b.Count; j++)
                {
                    var b1 = b[j];
                    var b2 = b[(j + 1) % b.Count];
                    var rel = DtmGeometryMath.Relate(a1, a2, b1, b2, tol);
                    if (rel.Kind != DtmSegmentRelationKind.Point) continue;
                    var p = new DtmPoint { X = rel.X, Y = rel.Y };
                    var atVertex = new[] { a1, a2, b1, b2 }.Any(v => DtmGeometryMath.Distance2D(v, p) <= tol);
                    if (!atVertex) return true;
                }
            }
            return false;
        }
    }

    public class DtmDefinitionPointWithoutAreaRule : DtmValidationRule
    {
        public override string Code => "4.7";
        public override string Name => "Definiční bod bez plochy";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var areas = ctx.Areas.Select(a => new { a.Level, Ring = DtmAreaHelper.Ring(a) })
                .Where(a => a.Ring != null && a.Ring.Count >= 3).ToList();
            var originalAreas = ctx.OriginalItems.Where(a => a.IsZps && a.Type == DtmElementType.Plocha)
                .Select(a => new { a.Level, Ring = DtmAreaHelper.Ring(a) })
                .Where(a => a.Ring != null && a.Ring.Count >= 3).ToList();
            foreach (var d in ctx.DefinitionPoints)
            {
                foreach (var p in d.Points)
                {
                    if (!DtmValidationContext.IsChanged(d) && !originalAreas.Any(a => a.Level == d.Level && DtmGeometryMath.IsInsideOrOnBoundary(p, a.Ring, ctx.Settings.GeometryTolerance))) continue;
                    if (!areas.Any(a => a.Level == d.Level && DtmGeometryMath.IsInsideOrOnBoundary(p, a.Ring, ctx.Settings.GeometryTolerance)))
                        result.AddObjectError(d, $"Definiční bod {DtmTopologyHelper.Coord(p.X, p.Y)} neleží v žádné ploše.");
                }
            }
        }
    }
}
