using System.Collections.Generic;

namespace CAD.DTM
{
    public interface IDtmElementsGroup
    {
        IEnumerable<IDtmElement> GetElementGroups();
    }
}
