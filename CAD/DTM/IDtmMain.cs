using System.Collections.Generic;

namespace CAD.DTM
{
    public interface IDtmMain
    {
        void AddElementGroup(string elementType, IDtmElementsGroup group);
        IEnumerable<KeyValuePair<string, IDtmElementsGroup>> GetElementGroups();
        void AddElementIfNotExist(string groupName, IDtmElement dtmElementGetDtmElement);
        string AllocateUniqueId(string name);
    }
}
