using CAD.DTM.Elements;

namespace CAD.DTM.Validation.Rules
{
    public sealed class DtmSchemaAttributesRule : DtmValidationRule
    {
        public override string Code => "2.1.0";
        public override string Name => "Povinné atributy a datové typy podle XSD";
        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            foreach (var item in ctx.Items)
                if (item.Element is DtmElement element && element.SchemaAttributes != null)
                    foreach (var error in element.SchemaAttributes.Validate()) result.AddObjectError(item, error);
        }
    }
}
