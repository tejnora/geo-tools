namespace WSGP.SOAP
{
    class SOAPSeznamRizeniPMRequest
        : ISOAPRqeuest
    {
        public string SoapAction => "http://katastr.cuzk.cz/geows/seznamRizeniPM";
        public string RawBody => "<v1:SeznamRizeniPMRequest/>";
        public ISOAPResponse CreateResponse(string content)
        {
            return new SOAPSeznamRizeniPMResponse(content);
        }
    }
}
