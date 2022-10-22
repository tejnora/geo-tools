namespace WSGP.SOAP
{
    class SOAPRezervaceZPMZRequest
        : ISOAPRqeuest
    {
        readonly string _idRizeniPm;
        readonly string _katuzeKod;
        public SOAPRezervaceZPMZRequest(string idRizeniPM, string katuzeKod)
        {
            _idRizeniPm = idRizeniPM;
            _katuzeKod = katuzeKod;
        }
        public string SoapAction => "http://katastr.cuzk.cz/geows/vytvorRezervaciZPMZ";
        public string RawBody
        {
            get
            {
                var body = "<v1:VytvorRezervaciZPMZRequest>";
                body += $"<v1:idRizeniPM>{_idRizeniPm}</v1:idRizeniPM>";
                body += $"<v1:katuzeKod>{_katuzeKod}</v1:katuzeKod>";
                body += "</v1:VytvorRezervaciZPMZRequest>";
                return body;
            }
        }
        public ISOAPResponse CreateResponse(string content)
        {
            return new SOAPRezervaceZPMZResponse(content);
        }

    }
}
/*
 <soapenv:Body>
    <v1:VytvorRezervaciZPMZRequest>
      <v1:idRizeniPM>50623895010</v1:idRizeniPM>
      <v1:katuzeKod>628301</v1:katuzeKod>
    </v1:VytvorRezervaciZPMZRequest>
  </soapenv:Body>
  */
