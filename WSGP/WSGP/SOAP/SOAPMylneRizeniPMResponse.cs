using WSGP.OutputObject;

namespace WSGP.SOAP
{
    class SOAPMylneRizeniPMResponse
        : ISOAPResponseBase
    {
        public SOAPMylneRizeniPMResponse(string xmlContent) : base(xmlContent)
        {
        }

        public Success CreateDataObject()
        {
            var res = new Success();
            var zalozRizeniPMResponseXmlElement = ParseBody("MylneRizeniPMResponse");
            res.Vysledek = CreateVysledek(zalozRizeniPMResponseXmlElement);
            return res;

        }
    }
}
