using CAD.DTM.Configuration;

namespace CAD.DTM.Elements
{
    public class DtmElementGeneric
    : DtmBodBaseElement
    {
        DtmElementType _elementType = DtmElementType.Line;
        public override DtmElementType ElementType => _elementType;
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            _elementType = dtmElementOption.ElementType;
        }
    }
}
