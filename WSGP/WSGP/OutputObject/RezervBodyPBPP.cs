using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WSGP.OutputObject
{
    public class RezervBodyPBPP
        : RezervPrvkuAbstract
    {
        public string KatuzeKod { get; set; }
        public IList<String> CisloPBPP { get; set; }
        public override void InitFrom(IEnumerable<XElement> parentElement)
        {
            var rezervBodyPBPPXmlElement = parentElement.ElementAt(1).Elements();
            KatuzeKod = rezervBodyPBPPXmlElement.ElementAt(0).Value;
            CisloPBPP = new List<string>();
            for (var i = 1; i < rezervBodyPBPPXmlElement.Count(); i++)
            {
                CisloPBPP.Add(rezervBodyPBPPXmlElement.ElementAt(i).Value);
            }
        }
    }
}

/*
      <ns0:rezervBodyPBPP>
        <ns0:katuzeKod>733857</ns0:katuzeKod>
        <ns0:cisloPBPP>502</ns0:cisloPBPP>
        <ns0:cisloPBPP>503</ns0:cisloPBPP>
      </ns0:rezervBodyPBPP>
 */
