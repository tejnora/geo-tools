using System.Linq;
using WSGP.OutputObject;

namespace WSGP.SOAP
{
    class SOAPExportVFResponse
       : ISOAPResponseBase
   {
      public SOAPExportVFResponse(string xmlContent) : base(xmlContent)
      {
      }
      public ExportVF CreateDataObject()
      {
         var res = new ExportVF();
         var exportVFResponseXmlElement = ParseBody("ExportVFResponse");
         res.Vysledek = CreateVysledek(exportVFResponseXmlElement);
         if (!res.IsValid())
            return res;
         res.BehId= exportVFResponseXmlElement.ElementAt(1).Value;
         return res;
      }
   }
}
/*
   <ns0:ExportVFResponse xmlns:ns0="http://katastr.cuzk.cz/geows/types/v2.2" xmlns:ns1="http://katastr.cuzk.cz/commonTypes/v2.2" xmlns:ns10="http://www.w3.org/1999/xlink" xmlns:ns3="http://www.opengis.net/gml/3.2">
      <ns0:vysledek>
        <ns1:zprava kod="0" uroven="INFORMACE">Požadovaná akce byla úspěšně provedena.</ns1:zprava>
      </ns0:vysledek>
      <ns0:behId>62541207010</ns0:behId>
    </ns0:ExportVFResponse>
*/