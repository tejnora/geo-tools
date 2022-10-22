using WSGP.OutputObject;

namespace WSGP.InputObject
{
    public class RezervacePoddeleni
    : IRezervacePrvku
    {
        public string CisloZPMZ { get; set; }
        public string DruhCislovaniPar { get; set; }
        public string KmenoveCislo { get; set; }
        public string PocetRezPoddeleni { get; set; }

        public string Xml
        {
            get
            {
                var res = "<v1:rezervacePoddeleni>";
                res += $"<v1:cisloZPMZ>{CisloZPMZ}</v1:cisloZPMZ>";
                res += $"<v1:druhCislovaniPar>{DruhCislovaniPar}</v1:druhCislovaniPar>";
                res += $"<v1:kmenoveCislo>{KmenoveCislo}</v1:kmenoveCislo>";
                res += $"<v1:pocetRezPoddeleni>{PocetRezPoddeleni}</v1:pocetRezPoddeleni>";
                res += "</v1:rezervacePoddeleni>";
                return res;

            }
        }
        public RezervPrvkuAbstract CreateResponseObject()
        {
            return new RezervPoddeleni();
        }

    }
}
/*
<v1:rezervacePoddeleni>
<v1:cisloZPMZ>1605</v1:cisloZPMZ>
<v1:druhCislovaniPar>2</v1:druhCislovaniPar>
<v1:kmenoveCislo>1531</v1:kmenoveCislo>
<v1:pocetRezPoddeleni>1</v1:pocetRezPoddeleni>
</v1:rezervacePoddeleni>
*/
