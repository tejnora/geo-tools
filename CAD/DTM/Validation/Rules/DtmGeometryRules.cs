using System.Collections.Generic;
using System.Linq;
using CAD.DTM.Elements;

namespace CAD.DTM.Validation.Rules
{
    public class DtmGeometryRule : DtmCompositeValidationRule
    {
        public override string Code => "1.4";
        public override string Name => "Kontrola geometrií";

        protected override IEnumerable<DtmValidationRule> CreateChildren()
        {
            yield return new DtmGeometryExistsRule();
            yield return new DtmGeometryAllowedTypesRule();
            yield return new DtmGeometryMatchesElementRule();
            yield return new DtmPointGeometryRule();
            yield return new DtmLineGeometryRule();
            yield return new DtmPolygonGeometryRule();
        }
    }

    public class DtmGeometryExistsRule : DtmValidationRule
    {
        public override string Code => "1.4.1";
        public override string Name => "Objekt má geometrii";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            foreach (var item in ctx.Items.Where(i => !i.Geometries.Any()))
                result.AddError(item.Name, "Objekt nemá geometrii.");
        }
    }

    public class DtmGeometryAllowedTypesRule : DtmValidationRule
    {
        public override string Code => "1.4.2";
        public override string Name => "Povolené typy geometrie (bod, linie, plocha; bez oblouků)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            foreach (var item in ctx.Items)
                foreach (var g in item.Geometries)
                    if (!(g is DtmPointGeometry) && !(g is DtmCurveGeometry) && !(g is DtmPolygonGeometry))
                        result.AddError(item.Name, $"Nepovolený typ geometrie {g.GetType().Name}.");
        }
    }

    public class DtmGeometryMatchesElementRule : DtmValidationRule
    {
        public override string Code => "1.4.3";
        public override string Name => "Typ geometrie odpovídá typu objektu";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            foreach (var item in ctx.Items)
            {
                var geometries = item.Geometries.ToList();
                if (item.Element is DtmElement element && element.SchemaAttributes != null)
                {
                    var actual = geometries.Select(g => g switch
                    {
                        DtmPointGeometry => "pointProperty",
                        DtmMultiCurveGeometry => "multiCurveProperty",
                        DtmCurveGeometry => "curveProperty",
                        DtmSurfaceGeometry => "surfaceProperty",
                        _ => "unknown"
                    });
                    if (!actual.SequenceEqual(element.SchemaAttributes.Definition.Geometries))
                        result.AddError(item.Name, "Geometrie neodpovídá XSD: " + string.Join(", ", element.SchemaAttributes.Definition.Geometries));
                    continue;
                }
                switch (item.Element.ElementType)
                {
                    case DtmElementType.Bod:
                    case DtmElementType.DefinicniBod:
                        if (geometries.Any(g => !(g is DtmPointGeometry)))
                            result.AddError(item.Name, "Bodový objekt musí mít bodovou geometrii.");
                        break;
                    case DtmElementType.Linie:
                        if (geometries.Any(g => !(g is DtmCurveGeometry)))
                            result.AddError(item.Name, "Liniový objekt musí mít liniovou geometrii.");
                        break;
                    case DtmElementType.Plocha:
                        if (geometries.Any(g => !(g is DtmPolygonGeometry)))
                            result.AddError(item.Name, "Plošný objekt musí mít plošnou geometrii.");
                        break;
                }
            }
        }
    }

    public class DtmPointGeometryRule : DtmValidationRule
    {
        public override string Code => "1.4.4";
        public override string Name => "Bodová geometrie";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            foreach (var item in ctx.Items)
                foreach (var g in item.Geometries.OfType<DtmPointGeometry>())
                    if (g.Point == null)
                        result.AddError(item.Name, "Bodová geometrie nemá souřadnice.");
        }
    }

    public class DtmLineGeometryRule : DtmValidationRule
    {
        public override string Code => "1.4.5";
        public override string Name => "Liniová geometrie (nejméně 2 body)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            foreach (var item in ctx.Items)
                foreach (var g in item.Geometries.OfType<DtmCurveGeometry>().SelectMany(g => g is DtmMultiCurveGeometry multi ? multi.Curves : new[] { g }))
                    if ((g.Points?.Count ?? 0) < 2)
                        result.AddError(item.Name, "Liniová geometrie musí mít nejméně 2 body.");
        }
    }

    public class DtmPolygonGeometryRule : DtmValidationRule
    {
        public override string Code => "1.4.6";
        public override string Name => "Plošná geometrie (uzavřená, nejméně 4 body)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            foreach (var item in ctx.Items)
                foreach (var g in item.Geometries.OfType<DtmPolygonGeometry>())
                {
                    foreach (var p in new[] { g.Points }.Concat(g.InteriorRings))
                    {
                        if (p == null || p.Count < 4 || p[0].X != p[p.Count - 1].X || p[0].Y != p[p.Count - 1].Y)
                            result.AddError(item.Name, "Plošná geometrie musí být uzavřená a mít nejméně 4 body.");
                    }
                }
        }
    }
}
