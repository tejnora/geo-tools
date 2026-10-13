using System;
using System.Collections.Generic;
using System.Linq;
using CAD.DTM.Elements;
using CAD.DTM.Elements.Linie;

namespace CAD.DTM.Validation.Rules
{
    public class DtmAttributesGroupRule : DtmCompositeValidationRule
    {
        public override string Code => "1.4.2";
        public override string Name => "Atributové kontroly";

        protected override IEnumerable<DtmValidationRule> CreateChildren()
        {
            yield return new DtmAttributesRule();
            yield return new DtmIcsRule();
            yield return new DtmHeightAccuracyClassRule();
            yield return new DtmUnknownLineTypeRule();
            yield return new DtmTpZpAttributeRule();
        }
    }

    public class DtmAttributesRule : DtmCompositeValidationRule
    {
        public override string Code => "2.1";
        public override string Name => "Kontrola atributů";

        protected override IEnumerable<DtmValidationRule> CreateChildren()
        {
            yield return new DtmAziRule();
            yield return new DtmNewPointAccuracyClassRule();
            yield return new DtmMinimalDateRule();
            yield return new DtmCodelistRule();
        }
    }

    public class DtmAziRule : DtmValidationRule
    {
        public override string Code => "2.1.1";
        public override string Name => "Povinný atribut AZI v doprovodných informacích (číslo oprávnění)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var azi = ctx.ExportCtx?.AZI;
            if (ctx.ExportCtx == null)
            {
                result.IsSkipped = true;
                result.Note = "Doprovodné informace se zadávají až při exportu.";
            }
            else if (string.IsNullOrWhiteSpace(azi))
                result.AddError(null, "Atribut AZI není vyplněn.");
            else if (azi.Contains("/") || !azi.All(char.IsLetterOrDigit))
                result.AddError(null, "Atribut AZI se vyplňuje jako číslo oprávnění bez lomítka a roku vydání.");
        }
    }

    public class DtmNewPointAccuracyClassRule : DtmValidationRule
    {
        public override string Code => "2.1.2";
        public override string Name => "Nové podrobné body (insert) alespoň ve třídě přesnosti 3 (poloha i výška)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var max = ctx.Settings.MaxAccuracyClassForInsert;
            foreach (var item in ctx.PodrobneBody.Where(i => i.Mode == 'i'))
            {
                var a = item.Element.SpolecneAtributyZPS;
                if (a == null) continue;
                if (a.TridaPresnostiPoloha > max)
                    result.AddError(item.Name, $"Třída přesnosti v poloze {a.TridaPresnostiPoloha} je horší než {max}.");
                if (a.TridaPresnostiVyska > max)
                    result.AddError(item.Name, $"Třída přesnosti ve výšce {a.TridaPresnostiVyska} je horší než {max}.");
            }
        }
    }

    public class DtmMinimalDateRule : DtmValidationRule
    {
        public override string Code => "2.1.3";
        public override string Name => "Datumová pole nejsou starší než 1900-01-01";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var min = ctx.Settings.MinDate;
            if (ctx.ExportCtx != null)
            {
                void Test(string name, DateTime d)
                {
                    if (d < min) result.AddError(null, $"{name} ({d:yyyy-MM-dd}) je starší než {min:yyyy-MM-dd}.");
                }
                Test("DatumMereni", ctx.ExportCtx.DatumMereni);
                Test("DatumZpracovani", ctx.ExportCtx.DatumZpracovani);
                Test("DatumOvereni", ctx.ExportCtx.DatumOvereni);
            }
            foreach (var item in ctx.Items)
            {
                var a = item.Element.SpolecneAtributy;
                if (a == null) continue;
                if (a.DatumVkladu != default && a.DatumVkladu < min)
                    result.AddError(item.Name, $"DatumVkladu ({a.DatumVkladu:yyyy-MM-dd}) je starší než {min:yyyy-MM-dd}.");
            }
        }
    }

    public class DtmCodelistRule : DtmValidationRule
    {
        public override string Code => "2.1.4";
        public override string Name => "Soulad s číselníky a povinné atributy (třídy přesnosti, způsob pořízení)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            foreach (var item in ctx.Items.Where(i => i.IsZps))
            {
                var a = item.Element.SpolecneAtributyZPS;
                if (a == null) continue;
                if (a.TridaPresnostiPoloha < 1 || a.TridaPresnostiPoloha > 5)
                    result.AddError(item.Name, $"Třída přesnosti v poloze {a.TridaPresnostiPoloha} není v číselníku (1–5).");
                if (a.TridaPresnostiVyska < 1 || a.TridaPresnostiVyska > 5)
                    result.AddError(item.Name, $"Třída přesnosti ve výšce {a.TridaPresnostiVyska} není v číselníku (1–5).");
                if (a.UrovenUmisteniObjektuZPS < -3 || a.UrovenUmisteniObjektuZPS > 3)
                    result.AddError(item.Name, $"Úroveň umístění {a.UrovenUmisteniObjektuZPS} není v rozsahu -3 až 3.");
            }
        }
    }

    public class DtmIcsRule : DtmValidationRule
    {
        public override string Code => "2.2";
        public override string Name => "Kontrola IČS";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            result.IsSkipped = true;
            result.Note = "Kontrola IČS dosud nebyla spuštěna.";
        }
    }

    public class DtmHeightAccuracyClassRule : DtmValidationRule
    {
        public override string Code => "2.3";
        public override string Name => "Kontrola vyplnění třídy přesnosti ve výšce ZPS (nové podrobné body)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            foreach (var item in ctx.PodrobneBody.Where(i => i.Mode == 'i'))
            {
                var a = item.Element.SpolecneAtributyZPS;
                if (a == null || a.TridaPresnostiVyska < 1)
                    result.AddError(item.Name, "Není vyplněna třída přesnosti ve výšce.");
            }
        }
    }

    public class DtmUnknownLineTypeRule : DtmValidationRule
    {
        public override string Code => "2.4";
        public override string Name => "Kontrola výskytu linií typu nezjištěno nebo jiný (insert, update)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            foreach (var item in ctx.Items.Where(i => i.Mode == 'i' || i.Mode == 'u'))
            {
                switch (item.Element)
                {
                    case DtmHraniceStavbyElement s when s.TypStavby == DtmTypStavbyEnum.Nezjisteno:
                        result.AddError(item.Name, "Hranice stavby, typ stavby: nezjištěno.");
                        break;
                    case DtmHraniceDopravniStavbyPlochyElement d when d.TypDopravniStavbyNeboPlochy == DtmTypDopravniStavbyNeboPlochyEnum.Nezjisteno
                        || d.TypDopravniStavbyNeboPlochy == DtmTypDopravniStavbyNeboPlochyEnum.Jiny:
                        result.AddError(item.Name, $"Hranice dopravní stavby nebo plochy, typ: {d.TypDopravniStavbyNeboPlochy}.");
                        break;
                    case DtmHranicePrirodnihoPoloprirodnihoObjektuElement p when p.TypPrirodnihoPoloprirodnihoObjektu == DtmTypPrirodnihoPoloprirodnihoObjektuEnum.Nezjisteno:
                        result.AddError(item.Name, "Hranice přírodního a polopřírodního objektu, typ: nezjištěno.");
                        break;
                }
            }
        }
    }

    public class DtmTpZpAttributeRule : DtmValidationRule
    {
        public override string Code => "2.5";
        public override string Name => "Kontrola správnosti vyplnění atributu TP a ZP";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            result.IsSkipped = true;
            result.Note = "Pravidla pro TP a ZP nejsou na wiki popsána.";
        }
    }
}
