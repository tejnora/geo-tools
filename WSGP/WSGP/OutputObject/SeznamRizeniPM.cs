using System.Collections.Generic;
namespace WSGP.OutputObject
{
    public class SeznamRizeniPM
    : DataObjectBaseAbstract
    {
        public IEnumerable<RizeniPM> RizeniPMList { get; set; }
    }
}
