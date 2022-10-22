using WSGP.InputObject;

namespace WSGP.SOAP
{
    class SOAPVytvorRezervaciPrvkuRequest
        : ISOAPRqeuest
    {
        string _idRizeni;
        string _katuzeKod;
        readonly IRezervacePrvku _prvek;

        public SOAPVytvorRezervaciPrvkuRequest(string idRizeni, string katuzeKod, IRezervacePrvku prvek)
        {
            _idRizeni = idRizeni;
            _katuzeKod = katuzeKod;
            _prvek = prvek;
        }

        public string SoapAction => "http://katastr.cuzk.cz/geows/vytvorRezervaciPrvku";

        public string RawBody
        {
            get
            {
                return
                    "<v1:VytvorRezervaciPrvkuRequest>" +
                    $"<v1:idRizeniPM>{_idRizeni}</v1:idRizeniPM>" +
                    $"<v1:katuzeKod>{_katuzeKod}</v1:katuzeKod>" +
                    _prvek.Xml +
                    "</v1:VytvorRezervaciPrvkuRequest>";
            }
        }

        public ISOAPResponse CreateResponse(string content)
        {
            return new SOAPVytvorRezervaciPrvkuResponse(content, _prvek);
        }
    }
}
/*
 * <v1:VytvorRezervaciPrvkuRequest>
      <v1:idRizeniPM>22285597010</v1:idRizeniPM>
      <v1:katuzeKod>733857</v1:katuzeKod>
      <v1:rezervaceParcela>
        <v1:cisloZPMZ>1605</v1:cisloZPMZ>
        <v1:druhCislovaniPar>2</v1:druhCislovaniPar>
        <v1:pocetRezParcel>1</v1:pocetRezParcel>
      </v1:rezervaceParcela>
    </v1:VytvorRezervaciPrvkuRequest>
 */
