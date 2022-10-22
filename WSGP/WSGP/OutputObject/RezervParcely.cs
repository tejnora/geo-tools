using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WSGP.OutputObject
{
    public class RezervParcely
    : RezervPrvkuAbstract
    {
        public string DruhCislovaniPar { get; set; }
        public string KmenoveCislo { get; set; }

        public override void InitFrom(IEnumerable<XElement> parentElement)
        {
            var rezervParcelyXmlElement = parentElement.ElementAt(1).Elements();
            KatuzeKod = rezervParcelyXmlElement.ElementAt(0).Value;
            CisloZPMZ = rezervParcelyXmlElement.ElementAt(1).Value;
            DruhCislovaniPar = rezervParcelyXmlElement.ElementAt(2).Value;
            KmenoveCislo = rezervParcelyXmlElement.ElementAt(3).Value;
        }
    }
}
/*
  <ns0:rezervParcely>
        <ns0:katuzeKod>733857</ns0:katuzeKod>
        <ns0:cisloZPMZ>1605</ns0:cisloZPMZ>
        <ns0:druhCislovaniPar>2</ns0:druhCislovaniPar>
        <ns0:kmenoveCislo>3106</ns0:kmenoveCislo>
      </ns0:rezervParcely>
 */
