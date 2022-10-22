using WSGP.OutputObject;
using System.Collections.Generic;
using System.Linq;

namespace WSGP.SOAP
{

    class ISOAPDetailRizeniPMResponse
        : ISOAPResponseBase
    {
        public ISOAPDetailRizeniPMResponse(string content)
        : base(content)
        {
        }

        public DetailRizeni CreateDataObject()
        {
            var res = new DetailRizeni();
            var detailRizeniPMXmlElement = ParseBody("DetailRizeniPMResponse");
            res.Vysledek = CreateVysledek(detailRizeniPMXmlElement);
            if (!res.IsValid())
                return res;

            res.IdRizeniPM = detailRizeniPMXmlElement.ElementAt(1).Value;
            res.CisloRizeni = detailRizeniPMXmlElement.ElementAt(2).Value;
            res.CisloZakazky = detailRizeniPMXmlElement.ElementAt(3).Value;

            var katuzeKodList = new List<string>();
            foreach (var katuzeKod in detailRizeniPMXmlElement.ElementAt(4).Elements())
            {
                katuzeKodList.Add(katuzeKod.Value);
            }
            res.KatuzeKodList = katuzeKodList;

            var dotceneParcely = new List<string>();
            foreach (var idParcely in detailRizeniPMXmlElement.ElementAt(5).Elements())
            {
                katuzeKodList.Add(idParcely.Value);
            }
            res.DotceneParcely = dotceneParcely;

            var rezervaceCislaZPMZList = new List<RezervCisloZPMZ>();
            foreach (var rezervCisloZPMZ in detailRizeniPMXmlElement.ElementAt(6).Elements())
            {
                var cisloZPMZ = new RezervCisloZPMZ();
                cisloZPMZ.KatuzeKod = rezervCisloZPMZ.Descendants().ElementAt(0).Value;
                cisloZPMZ.CisloZPMZ = rezervCisloZPMZ.Descendants().ElementAt(1).Value;
                rezervaceCislaZPMZList.Add(cisloZPMZ);
            }
            res.RezervCislaZPMZList = rezervaceCislaZPMZList;

            var rezervPoddeleniList = new List<RezervPoddeleni>();
            foreach (var rezervacePoddeleni in detailRizeniPMXmlElement.ElementAt(7).Elements())
            {
                var poddeleni = new RezervPoddeleni();
                poddeleni.KatuzeKod = rezervacePoddeleni.Descendants().ElementAt(0).Value;
                poddeleni.CisloZPMZ = rezervacePoddeleni.Descendants().ElementAt(1).Value;
                poddeleni.DruhCislovaniPar = rezervacePoddeleni.Descendants().ElementAt(2).Value;
                poddeleni.KmenoveCislo = rezervacePoddeleni.Descendants().ElementAt(3).Value;
                poddeleni.PoddeleniCisla = new List<string> { rezervacePoddeleni.Descendants().ElementAt(4).Value };
                rezervPoddeleniList.Add(poddeleni);
            }
            res.RezervPoddeleniList = rezervPoddeleniList;

            return res;
        }
    }
}
/*
<S:Body>
    <ns0:DetailRizeniPMResponse xmlns:ns0="http://katastr.cuzk.cz/geows/types/v2.2" xmlns:ns1="http://katastr.cuzk.cz/commonTypes/v2.2" xmlns:ns10="http://www.w3.org/1999/xlink" xmlns:ns3="http://www.opengis.net/gml/3.2">
      <ns0:vysledek>
        <ns1:zprava kod = "0" uroven="INFORMACE">Požadovaná akce byla úspěšně provedena.</ns1:zprava>
      </ns0:vysledek>
      <ns0:idRizeniPM>22286597010</ns0:idRizeniPM>
      <ns0:cisloRizeni>PM-7/2017-305</ns0:cisloRizeni>
      <ns0:cisloZakazky>Test ITO PM pro ZPG/PGP 2</ns0:cisloZakazky>
      <ns0:katuzeKodList>
        <ns1:katuzeKod>733857</ns1:katuzeKod>
      </ns0:katuzeKodList>
      <ns0:dotceneParcely>
        <ns1:idParcely>2757502305</ns1:idParcely>
      </ns0:dotceneParcely>
      <ns0:rezervCislaZPMZList>
        <ns0:rezervCisloZPMZ>
          <ns0:katuzeKod>733857</ns0:katuzeKod>
          <ns0:cisloZPMZ>1606</ns0:cisloZPMZ>
        </ns0:rezervCisloZPMZ>
      </ns0:rezervCislaZPMZList>
      <ns0:RezervPoddeleniList>
        <ns0:rezervPoddeleni>
          <ns0:katuzeKod>733857</ns0:katuzeKod>
          <ns0:cisloZPMZ>1606</ns0:cisloZPMZ>
          <ns0:druhCislovaniPar>2</ns0:druhCislovaniPar>
          <ns0:kmenoveCislo>1531</ns0:kmenoveCislo>
          <ns0:poddeleniCisla>32</ns0:poddeleniCisla>
        </ns0:rezervPoddeleni>
      </ns0:RezervPoddeleniList>
    </ns0:DetailRizeniPMResponse>
  </S:Body>
*/
