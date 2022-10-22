namespace WSGP.SOAP
{
    class SOAPMylneRizeniPMRequest
        : ISOAPRqeuest
    {
        readonly string _idRizeniPm;
        readonly string _popis;

        public SOAPMylneRizeniPMRequest(string idRizeniPM, string popis)
        {
            _idRizeniPm = idRizeniPM;
            _popis = popis;
        }

        public string SoapAction => "http://katastr.cuzk.cz/geows/mylneRizeniPM";
        public string RawBody
        {
            get
            {
                var body = "<v1:MylneRizeniPMRequest>";
                body += $"<v1:idRizeniPM>{_idRizeniPm}</v1:idRizeniPM>";
                body += $"<v1:popis>{_popis}</v1:popis>";
                body += "</v1:MylneRizeniPMRequest>";
                return body;
            }
        }
        public ISOAPResponse CreateResponse(string content)
        {
            return new SOAPMylneRizeniPMResponse(content);
        }
    }
}

/*
<v1:MylneRizeniPMRequest>
<v1:idRizeniPM>50623878020</v1:idRizeniPM>
<v1:popis>storno zakázky</v1:popis>
</v1:MylneRizeniPMRequest>
*/
