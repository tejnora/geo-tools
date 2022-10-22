using System.Collections.Generic;

namespace WSGP.SOAP
{
    class SOAPZalozRizeniPMRequest
        : ISOAPRqeuest
    {
        string _cisloZakazky;
        IEnumerable<string> _katuzeKodList;
        IEnumerable<string> _dotceneParcely;

        public SOAPZalozRizeniPMRequest(string cisloZakazky, IEnumerable<string> katuzeKodList, IEnumerable<string> dotceneParcely)
        {
            _cisloZakazky = cisloZakazky;
            _katuzeKodList = katuzeKodList;
            _dotceneParcely = dotceneParcely;
        }
        public string SoapAction => "http://katastr.cuzk.cz/geows/zalozRizeniPM";

        public string RawBody
        {
            get
            {
                var body = "";
                body += "<v1:ZalozRizeniPMRequest>";
                body += $"<v1:cisloZakazky>{_cisloZakazky}</v1:cisloZakazky>";
                body += "<v1:katuzeKodList>";
                foreach (var katuzeKod in _katuzeKodList)
                {
                    body += $"<v2:katuzeKod>{katuzeKod}</v2:katuzeKod>";
                }
                body += "</v1:katuzeKodList>";
                body += "<v1:dotceneParcely>";
                foreach (var idParcely in _dotceneParcely)
                {
                    body += $"<v2:idParcely>{idParcely}</v2:idParcely>";
                }
                body += "</v1:dotceneParcely>";
                body += "</v1:ZalozRizeniPMRequest>";
                return body;
            }
        }

        public ISOAPResponse CreateResponse(string content)
        {
            return new SOAPZalozRizeniPMResponse(content);
        }
    }
}
/*
   <v1:ZalozRizeniPMRequest>
      <v1:cisloZakazky>1/2017</v1:cisloZakazky>
      <v1:katuzeKodList>
        <v2:katuzeKod>628301</v2:katuzeKod>
      </v1:katuzeKodList>
      <v1:dotceneParcely>
        <v2:idParcely>752228203</v2:idParcely>
        <v2:idParcely>752230203</v2:idParcely>
        <v2:idParcely>991254203</v2:idParcely>
      </v1:dotceneParcely>
    </v1:ZalozRizeniPMRequest>
*/