using System.Collections.Generic;

namespace CAD.DTM
{
    interface IDtmMain
    {
        void AddElementGroup(string elementType, IDtmElementsGroup group);
        IEnumerable<KeyValuePair<string, IDtmElementsGroup>> GetElementGroups();
    }
}
