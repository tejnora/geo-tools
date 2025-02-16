using System.Collections.Generic;
using CAD.DTM.Elements;

namespace CAD.DTM
{
    public interface IDtmMain
    {
        void AddElementGroup(string elementType, IDtmElementsGroup group);
        IDtmElementsGroup getElementsGroup(string elementType);
        IEnumerable<KeyValuePair<string, IDtmElementsGroup>> GetElementGroups();
        void AddElementIfNotExist(string groupName, IDtmElement dtmElementGetDtmElement);
        string AllocateUniqueId(string name);
        DtmUdajeOVydeji UdajeOVydeji { get; set; }

        Dictionary<string, string> IndeticalPointsMapping { get; set; }

        DtmIdentickyBodElement GetIdentickyBod(string cislo, bool referencni);
    }
}
