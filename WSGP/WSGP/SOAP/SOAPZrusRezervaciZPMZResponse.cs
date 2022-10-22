using WSGP.OutputObject;

namespace WSGP.SOAP
{
    class SOAPZrusRezervaciZPMZResponse
        : ISOAPResponseBase
    {
        public SOAPZrusRezervaciZPMZResponse(string xmlContent)
        : base(xmlContent)
        {
        }
        public Success CreateDataObject()
        {
            var res = new Success();
            var zalozRizeniPMResponseXmlElement = ParseBody("ZrusRezervaciZPMZResponse");
            res.Vysledek = CreateVysledek(zalozRizeniPMResponseXmlElement);
            return res;
        }
    }
}
/*
  <S:Body>
    <ns0:ZrusRezervaciZPMZResponse xmlns:ns0="http://katastr.cuzk.cz/geows/types/v2.2" xmlns:ns1="http://katastr.cuzk.cz/commonTypes/v2.2" xmlns:ns10="http://www.w3.org/1999/xlink" xmlns:ns3="http://www.opengis.net/gml/3.2">
      <ns0:vysledek>
        <ns1:zprava kod = "0" uroven="INFORMACE">Požadovaná akce byla úspěšně provedena.</ns1:zprava>
      </ns0:vysledek>
    </ns0:ZrusRezervaciZPMZResponse>
  </S:Body>
*/
