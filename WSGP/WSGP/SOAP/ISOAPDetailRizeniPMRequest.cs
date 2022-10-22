namespace WSGP.SOAP
{
    class ISOAPDetailRizeniPMRequest
        : ISOAPRqeuest
    {
        string _idRizeni;
        bool _rezervace;
        public ISOAPDetailRizeniPMRequest(string idRizeni, bool rezervace)
        {
            _idRizeni = idRizeni;
            _rezervace = rezervace;
        }
        public string SoapAction => "http://katastr.cuzk.cz/geows/detailRizeniPM";

        public string RawBody
        {
            get
            {
                return
                    "<v1:DetailRizeniPMRequest>" +
                        $"<v1:idRizeniPM>{_idRizeni}</v1:idRizeniPM>" +
                        $"<v1:rezervace>{SOAPHelper.BoolToString(_rezervace)}</v1:rezervace>" +
                    "</v1:DetailRizeniPMRequest>";
            }
        }

        public ISOAPResponse CreateResponse(string content)
        {
            return new ISOAPDetailRizeniPMResponse(content);
        }
    }
}
/*
<v1:DetailRizeniPMRequest>
    <v1:idRizeniPM>22286597010</v1:idRizeniPM>
    <v1:rezervace>true</v1:rezervace>
</v1:DetailRizeniPMRequest>
*/
