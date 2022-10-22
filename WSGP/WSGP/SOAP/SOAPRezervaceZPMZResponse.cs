using System;
using WSGP.OutputObject;
using System.Linq;

namespace WSGP.SOAP
{
    class SOAPRezervaceZPMZResponse
        : ISOAPResponseBase
    {
        public SOAPRezervaceZPMZResponse(string xmlContent)
        : base(xmlContent)
        {
        }

        public RezervCisloZPMZ CreateDataObject()
        {
            var res = new RezervCisloZPMZ();
            var rezervCisloZpmzXmlElement = ParseBody("VytvorRezervaciZPMZResponse");
            res.Vysledek = CreateVysledek(rezervCisloZpmzXmlElement);
            if (res.IsValid())
            {
                var rezervCisloZPMZElements = rezervCisloZpmzXmlElement.ElementAt(1).Elements();
                res.KatuzeKod = rezervCisloZPMZElements.ElementAt(0).Value;
                res.CisloZPMZ = rezervCisloZPMZElements.ElementAt(1).Value;
            }

            return res;
        }
    }
}

/*
<ns0:rezervCisloZPMZ>
<ns0:katuzeKod>628301</ns0:katuzeKod>
<ns0:cisloZPMZ>328</ns0:cisloZPMZ>
</ns0:rezervCisloZPMZ>
*/
