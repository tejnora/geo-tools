using WSGP.OutputObject;

namespace WSGP.InputObject
{
    public class RezervaceParcela
    : IRezervacePrvku
    {
        public string CisloZPMZ { get; set; }
        public string DruhCislovaniPar { get; set; }
        public string PocetRezParcel { get; set; }

        public string Xml
        {
            get
            {
                var res = "<v1:rezervaceParcela>";
                res += $"<v1:cisloZPMZ>{CisloZPMZ}</v1:cisloZPMZ>";
                res += $"<v1:druhCislovaniPar>{DruhCislovaniPar}</v1:druhCislovaniPar>";
                res += $"<v1:pocetRezParcel>{PocetRezParcel}</v1:pocetRezParcel>";
                res += "</v1:rezervaceParcela>";
                return res;
            }
        }

        public RezervPrvkuAbstract CreateResponseObject()
        {
            return new RezervParcely();
        }
    }
}
/*
<v1:rezervaceParcela>
<v1:cisloZPMZ>1605</v1:cisloZPMZ>
<v1:druhCislovaniPar>2</v1:druhCislovaniPar>
<v1:pocetRezParcel>1</v1:pocetRezParcel>
</v1:rezervaceParcela>
*/
