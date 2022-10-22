using System;
using WSGP.OutputObject;
using System.Collections.Generic;
using System.Linq;

namespace WSGP.SOAP
{
    class SOAPSeznamRizeniPMResponse
        : ISOAPResponseBase
    {
        public SOAPSeznamRizeniPMResponse(string xmlContent)
        : base(xmlContent)
        {
        }

        public SeznamRizeniPM CreateDataObject()
        {
            var res = new SeznamRizeniPM();
            var seznamRizeniPMXmlElement = ParseBody("SeznamRizeniPMResponse");
            res.Vysledek = CreateVysledek(seznamRizeniPMXmlElement);
            if (!res.IsValid()) return res;

            var rizeniPMListXmlElement = seznamRizeniPMXmlElement.ElementAt(1).Elements();
            var seznamRizeni = new List<RizeniPM>();
            foreach (var rizeniElement in rizeniPMListXmlElement)
            {
                var rizeni = new RizeniPM();
                rizeni.IdRizeni = rizeniElement.Element(ns0 + "idRizeniPM").Value;
                rizeni.CisloRizeni = rizeniElement.Element(ns0 + "cisloRizeni").Value;
                rizeni.CisloZakazky = rizeniElement.Element(ns0 + "cisloZakazky").Value;
                seznamRizeni.Add(rizeni);
            }
            res.RizeniPMList = seznamRizeni;
            return res;
        }
    }
}
/*
<?xml version="1.0" encoding="utf-8"?>
<S:Envelope xmlns:env="http://schemas.xmlsoap.org/soap/envelope/" xmlns:S="http://schemas.xmlsoap.org/soap/envelope/">
  <S:Header />
  <S:Body>
    <ns0:SeznamRizeniPMResponse xmlns:ns0="http://katastr.cuzk.cz/geows/types/v2.2" xmlns:ns1="http://katastr.cuzk.cz/commonTypes/v2.2" xmlns:ns10="http://www.w3.org/1999/xlink" xmlns:ns3="http://www.opengis.net/gml/3.2">
      <ns0:vysledek>
        <ns1:zprava kod="0" uroven="INFORMACE">Požadovaná akce byla úspěšně provedena.</ns1:zprava>
      </ns0:vysledek>
      <ns0:rizeniPMList>
        <ns0:rizeniPM>
          <ns0:idRizeniPM>50623876010</ns0:idRizeniPM>
          <ns0:cisloRizeni>PM-670/2017-508</ns0:cisloRizeni>
          <ns0:cisloZakazky>2mm</ns0:cisloZakazky>
        </ns0:rizeniPM>
        <ns0:rizeniPM>
          <ns0:idRizeniPM>50623889010</ns0:idRizeniPM>
          <ns0:cisloRizeni>PM-894/2017-504</ns0:cisloRizeni>
          <ns0:cisloZakazky>Průřezový test JJ</ns0:cisloZakazky>
        </ns0:rizeniPM>
      </ns0:rizeniPMList>
    </ns0:SeznamRizeniPMResponse>
  </S:Body>
</S:Envelope>
 */
