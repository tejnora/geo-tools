using System;
using System.Collections.Generic;
using WSGP.OutputObject;
using System.Xml.Linq;
using System.Linq;

namespace WSGP.SOAP
{
    class SOAPZalozRizeniPMResponse
        : ISOAPResponseBase
    {
        public SOAPZalozRizeniPMResponse(string xmlContent)
        : base(xmlContent)
        {
        }
        public RizeniPM CreateDataObject()
        {
            var res = new RizeniPM();
            var zalozRizeniPMResponseXmlElement = ParseBody("ZalozRizeniPMResponse");
            res.Vysledek = CreateVysledek(zalozRizeniPMResponseXmlElement);
            if (!res.IsValid()) return res;

            var rizeniPMXmlElements = zalozRizeniPMResponseXmlElement.ElementAt(1).Elements();
            res.IdRizeni = rizeniPMXmlElements.ElementAt(0).Value;
            res.CisloRizeni = rizeniPMXmlElements.ElementAt(1).Value;
            res.PraresKod = rizeniPMXmlElements.ElementAt(2).Value;
            res.RizeniTyp = rizeniPMXmlElements.ElementAt(3).Value;
            res.PoradoveCislo = rizeniPMXmlElements.ElementAt(4).Value;
            res.Rok = rizeniPMXmlElements.ElementAt(5).Value;
            return res;
        }
    }
}
/*
    <ns0:ZalozRizeniPMResponse xmlns:ns0="http://katastr.cuzk.cz/geows/types/v2.2" xmlns:ns1="http://katastr.cuzk.cz/commonTypes/v2.2" xmlns:ns10="http://www.w3.org/1999/xlink" xmlns:ns3="http://www.opengis.net/gml/3.2">
      <ns0:vysledek>
        <ns1:zprava kod = "0" uroven="INFORMACE">Požadovaná akce byla úspěšně provedena.</ns1:zprava>
      </ns0:vysledek>
      <ns0:rizeniPM>
        <ns0:idRizeni>50623895010</ns0:idRizeni>
        <ns0:cisloRizeni>PM-1073/2017-203</ns0:cisloRizeni>
        <ns0:praresKod>203</ns0:praresKod>
        <ns0:rizeniTyp>PM</ns0:rizeniTyp>
        <ns0:poradoveCislo>1073</ns0:poradoveCislo>
        <ns0:rok>2017</ns0:rok>
      </ns0:rizeniPM>
    </ns0:ZalozRizeniPMResponse>
*/
