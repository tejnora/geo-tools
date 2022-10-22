using WSGP.InputObject;
using WSGP.OutputObject;

namespace WSGP.SOAP
{
    class SOAPVytvorRezervaciPrvkuResponse
        : ISOAPResponseBase
    {
        readonly IRezervacePrvku _prvek;

        public SOAPVytvorRezervaciPrvkuResponse(string xmlContent, IRezervacePrvku prvek) : base(xmlContent)
        {
            _prvek = prvek;
        }

        public RezervPrvkuAbstract CreateDataObject()
        {
            var res = _prvek.CreateResponseObject();
            var vytvorRezervaciPrvkuResponseXmlElement = ParseBody("VytvorRezervaciPrvkuResponse");
            res.Vysledek = CreateVysledek(vytvorRezervaciPrvkuResponseXmlElement);
            if (!res.IsValid())
                return res;
            res.InitFrom(vytvorRezervaciPrvkuResponseXmlElement);
            return res;

        }
    }
}

/*
 <ns0:VytvorRezervaciPrvkuResponse xmlns:ns0="http://katastr.cuzk.cz/geows/types/v2.2" xmlns:ns1="http://katastr.cuzk.cz/commonTypes/v2.2" xmlns:ns10="http://www.w3.org/1999/xlink" xmlns:ns3="http://www.opengis.net/gml/3.2">
      <ns0:vysledek>
        <ns1:zprava kod="0" uroven="INFORMACE">Požadovaná akce byla úspěšně provedena.</ns1:zprava>
      </ns0:vysledek>
      <ns0:rezervParcely>
        <ns0:katuzeKod>733857</ns0:katuzeKod>
        <ns0:cisloZPMZ>1605</ns0:cisloZPMZ>
        <ns0:druhCislovaniPar>2</ns0:druhCislovaniPar>
        <ns0:kmenoveCislo>3106</ns0:kmenoveCislo>
      </ns0:rezervParcely>
    </ns0:VytvorRezervaciPrvkuResponse> 
 */
