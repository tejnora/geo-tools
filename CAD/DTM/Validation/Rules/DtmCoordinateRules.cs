using System;
using System.Collections.Generic;
using System.Linq;
using CAD.DTM.Validation;

namespace CAD.DTM.Validation.Rules
{
    public class DtmCoordinatesRule : DtmValidationRule
    {
        public override string Code => "1.5";
        public override string Name => "Kontrola souřadnic (X, Y, Z)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var s = ctx.Settings;
            foreach (var item in ctx.Items)
            {
                var isDefPoint = item.Element.ElementType == DtmElementType.DefinicniBod;
                foreach (var g in item.Geometries)
                {
                    foreach (var p in DtmValidationContext.GetPoints(g))
                    {
                        if (double.IsNaN(p.X) || double.IsNaN(p.Y) || p.X < s.MinX || p.X > s.MaxX || p.Y < s.MinY || p.Y > s.MaxY)
                            result.AddError(item.Name, $"X,Y ({p.X:0.00} {p.Y:0.00}) mimo povolený rozsah / 3. kvadrant S-JTSK.");
                        if (item.IsZps && DtmValidationContext.HasZ(g))
                        {
                            var minZ = isDefPoint ? s.MinZDefinicniBod : s.MinZ;
                            if (double.IsNaN(p.Z) || p.Z < minZ || p.Z > s.MaxZ)
                                result.AddError(item.Name, $"Z ({p.Z:0.00}) mimo povolený rozsah {minZ}–{s.MaxZ} m n. m.");
                        }
                    }
                }
            }
        }
    }

    public class DtmCoordinatesPrecisionRule : DtmValidationRule
    {
        public override string Code => "1.6";
        public override string Name => "Kontrola přesnosti souřadnic na cm";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            bool IsCm(double v)
            {
                var cm = v * 100;
                return Math.Abs(cm - Math.Round(cm)) <= ctx.Settings.PrecisionTolerance;
            }
            foreach (var item in ctx.Items)
                foreach (var g in item.Geometries)
                    foreach (var p in DtmValidationContext.GetPoints(g))
                    {
                        var hasZ = DtmValidationContext.HasZ(g);
                        if (!IsCm(p.X) || !IsCm(p.Y) || (hasZ && !IsCm(p.Z)))
                            result.AddError(item.Name, $"Souřadnice ({p.X} {p.Y}{(hasZ ? " " + p.Z : "")}) mají vyšší přesnost než cm.");
                    }
        }
    }

    public class DtmChangesInsideAreaRule : DtmValidationRule
    {
        public override string Code => "1.7";
        public override string Name => "Kontrola umístění změn v JVF DTM v oblasti změny";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var area = ctx.GetChangeArea();
            if (area == null)
            {
                result.IsSkipped = true;
                result.Note = "Oblast změny není zadána (při exportu se vytvoří z dat).";
                return;
            }
            foreach (var item in ctx.Items)
            {
                var type = item.Element.ElementType;
                if (type == DtmElementType.Plocha || type == DtmElementType.Obvod) continue;
                foreach (var g in item.Geometries)
                {
                    var outside = DtmValidationContext.GetPoints(g).FirstOrDefault(p => !IsInsideOrOnBoundary(p, area, ctx.Settings.OnBoundaryTolerance));
                    if (outside != null)
                        result.AddError(item.Name, $"Bod ({outside.X:0.00} {outside.Y:0.00}) leží mimo oblast změny.");
                }
            }
        }

        static bool IsInsideOrOnBoundary(DtmPoint p, IList<DtmPoint> ring, double tolerance)
        {
            var inside = false;
            for (int i = 0, j = ring.Count - 1; i < ring.Count; j = i++)
            {
                var a = ring[i];
                var b = ring[j];
                if (DistanceToSegment(p, a, b) <= tolerance)
                    return true;
                if ((a.Y > p.Y) != (b.Y > p.Y) && p.X < (b.X - a.X) * (p.Y - a.Y) / (b.Y - a.Y) + a.X)
                    inside = !inside;
            }
            return inside;
        }

        static double DistanceToSegment(DtmPoint p, DtmPoint a, DtmPoint b)
        {
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            var len2 = dx * dx + dy * dy;
            var t = len2 == 0 ? 0 : Math.Max(0, Math.Min(1, ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / len2));
            var cx = a.X + t * dx;
            var cy = a.Y + t * dy;
            return Math.Sqrt((p.X - cx) * (p.X - cx) + (p.Y - cy) * (p.Y - cy));
        }
    }
}
