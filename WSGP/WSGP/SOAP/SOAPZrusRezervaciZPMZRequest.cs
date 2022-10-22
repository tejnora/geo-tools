namespace WSGP.SOAP
{
    class SOAPZrusRezervaciZPMZRequest
        : ISOAPRqeuest
    {
        readonly string _idRizeniPM;
        readonly string _cisloZpmz;
        readonly string _katuzeKod;

        public SOAPZrusRezervaciZPMZRequest(string idRizeniPM, string cisloZPMZ, string katuzeKod)
        {
            _idRizeniPM = idRizeniPM;
            _cisloZpmz = cisloZPMZ;
            _katuzeKod = katuzeKod;
        }
        public string SoapAction => "http://katastr.cuzk.cz/geows/zrusRezervaciZPMZ";

        public string RawBody
        {
            get
            {
                var body = "<v1:ZrusRezervaciZPMZRequest>";
                body += $"<v1:idRizeniPM>{_idRizeniPM}</v1:idRizeniPM>";
                body += "<v1:rezervCisloZPMZ>";
                body += $"<v1:katuzeKod>{_katuzeKod}</v1:katuzeKod>";
                body += $"<v1:cisloZPMZ>{_cisloZpmz}</v1:cisloZPMZ>";
                body += "</v1:rezervCisloZPMZ>";
                body += "</v1:ZrusRezervaciZPMZRequest>";
                return body;
            }
        }

        public ISOAPResponse CreateResponse(string content)
        {
            return new SOAPZrusRezervaciZPMZResponse(content);
        }
    }
}

/*
 <v1:ZrusRezervaciZPMZRequest>
      <v1:idRizeniPM>22285597010</v1:idRizeniPM>
      <v1:rezervCisloZPMZ>
        <v1:katuzeKod>733857</v1:katuzeKod>
        <v1:cisloZPMZ>1605</v1:cisloZPMZ>
      </v1:rezervCisloZPMZ>
    </v1:ZrusRezervaciZPMZRequest>
    */

