using WSGP.OutputObject;

namespace WSGP.InputObject
{
    public class RezervacePBPP
        : IRezervacePrvku
    {
        public string PocetRezPBPP { get; set; }

        public string Xml
        {
            get
            {
                var res = "<v1:rezervacePBPP>";
                res += $"<v1:pocetRezPBPP>{PocetRezPBPP}</v1:pocetRezPBPP>";
                res += "</v1:rezervacePBPP>";
                return res;
            }
        }
        public RezervPrvkuAbstract CreateResponseObject()
        {
            return new RezervBodyPBPP();
        }
    }
}

/*
 <v1:rezervacePBPP>
    <v1:pocetRezPBPP>2</v1:pocetRezPBPP>
  </v1:rezervacePBPP>
 */
