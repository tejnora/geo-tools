using CAD.DTM.Configuration;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmBodBaseElement
        : DtmElement
    {
        public string CisloBodu { get; set; }

        public override DtmElementType ElementType => DtmElementType.Bod;

        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
        }

    }
}
