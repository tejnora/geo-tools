using System.Collections.Generic;

namespace WSGP.OutputObject
{

    public class DetailRizeni
    : DataObjectBaseAbstract
    {
        public string IdRizeniPM { get; set; }
        public string CisloRizeni { get; set; }
        public string CisloZakazky { get; set; }
        public IList<string> KatuzeKodList { get; set; }
        public IList<string> DotceneParcely { get; set; }
        public IList<RezervCisloZPMZ> RezervCislaZPMZList { get; set; }
        public IList<RezervPoddeleni> RezervPoddeleniList { get; set; }
    }
}
