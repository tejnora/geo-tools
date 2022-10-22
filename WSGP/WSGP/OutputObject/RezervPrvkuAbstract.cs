using System.Collections.Generic;
using System.Xml.Linq;

namespace WSGP.OutputObject
{
    public abstract class RezervPrvkuAbstract
    : DataObjectBaseAbstract
    {
        public string KatuzeKod { get; set; }
        public string CisloZPMZ { get; set; }

        public abstract void InitFrom(IEnumerable<XElement> parentElement);
    }
}
