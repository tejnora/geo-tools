using CAD.DTM.Configuration;

namespace CAD.DTM.Elements
{
    public class DtmBodBaseElement
        : DtmElement
    {
        public string CisloBodu { get; set; }

        public override DtmElementType ElementType => DtmElementType.Point;

        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
        }
    }
}
