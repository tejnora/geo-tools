using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WSGP.OutputObject
{
    public class RezervPoddeleni
        : RezervPrvkuAbstract
    {
        public string KatuzeKod { get; set; }
        public string CisloZPMZ { get; set; }
        public string DruhCislovaniPar { get; set; }
        public string KmenoveCislo { get; set; }
        public IList<string> PoddeleniCisla { get; set; }

        public override void InitFrom(IEnumerable<XElement> parentElement)
        {
            var rezervParcelyXmlElement = parentElement.ElementAt(1).Elements();
            KatuzeKod = rezervParcelyXmlElement.ElementAt(0).Value;
            CisloZPMZ = rezervParcelyXmlElement.ElementAt(1).Value;
            DruhCislovaniPar = rezervParcelyXmlElement.ElementAt(2).Value;
            KmenoveCislo = rezervParcelyXmlElement.ElementAt(3).Value;
            PoddeleniCisla = new List<string>();
            for (var i = 4; i < rezervParcelyXmlElement.Count(); i++)
                PoddeleniCisla.Add(rezervParcelyXmlElement.ElementAt(i).Value);
        }
    }
}

/*
  <ns0:rezervPoddeleni>
    <ns0:katuzeKod>733857</ns0:katuzeKod>
    <ns0:cisloZPMZ>1605</ns0:cisloZPMZ>
    <ns0:druhCislovaniPar>2</ns0:druhCislovaniPar>
    <ns0:kmenoveCislo>1531</ns0:kmenoveCislo>
    <ns0:poddeleniCisla>31</ns0:poddeleniCisla>
  </ns0:rezervPoddeleni>
 */
