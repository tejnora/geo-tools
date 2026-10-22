using System.Collections.Generic;
using System.Linq;

namespace CAD.DTM.Validation
{
    public abstract class DtmValidationRule
    {
        public abstract string Code { get; }
        public abstract string Name { get; }
        public virtual string Description => DtmValidationDescriptions.Get(GetType().Name);

        public virtual DtmRuleResult Validate(DtmValidationContext ctx)
        {
            var result = new DtmRuleResult(Code, Name) { Description = Description };
            Check(ctx, result);
            return result;
        }

        protected abstract void Check(DtmValidationContext ctx, DtmRuleResult result);
    }

    // Pravidlo slozene z podskupin; kazda podskupina je samostatne pravidlo.
    public abstract class DtmCompositeValidationRule : DtmValidationRule
    {
        protected abstract IEnumerable<DtmValidationRule> CreateChildren();

        public override DtmRuleResult Validate(DtmValidationContext ctx)
        {
            var result = new DtmRuleResult(Code, Name) { Description = Description };
            Check(ctx, result);
            result.SubResults.AddRange(CreateChildren().Select(c => c.Validate(ctx)));
            return result;
        }

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
        }
    }

    public class DtmValidator
    {
        readonly DtmValidationContext _context;
        readonly IList<DtmValidationRule> _rules;

        public DtmValidator(IDtmMain main, DtmExportCtx exportCtx = null, DtmValidationSettings settings = null)
        {
            _context = new DtmValidationContext(main, exportCtx, settings ?? DtmValidationSettings.Default);
            _rules = new List<DtmValidationRule>
            {
                new Rules.DtmExchangeFormatGroupRule(
                    new Rules.DtmXsdValidationRule(),
                    new Rules.DtmChangeAreaExistsRule(),
                    new Rules.DtmExtentRule(),
                    new Rules.DtmGeometryRule(),
                    new Rules.DtmCoordinatesRule(),
                    new Rules.DtmCoordinatesPrecisionRule(),
                    new Rules.DtmChangesInsideAreaRule()),
                new Rules.DtmAttributesGroupRule(),
                new Rules.DtmTopologyGroupRule(),
                new Rules.DtmDerivedObjectsGroupRule()
            };
        }

        public DtmValidationResult Validate()
        {
            var context = new DtmValidationContext(_context.Main, _context.ExportCtx, _context.Settings);
            return new DtmValidationResult(_rules.Select(r => r.Validate(context)).ToList());
        }
    }
}
