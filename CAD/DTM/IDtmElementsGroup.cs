using CAD.DTM.Gui;
using System.Collections.Generic;

namespace CAD.DTM
{
    public interface IDtmElementsGroup
    {
        string Name { get; }
        IEnumerable<IDtmElement> GetElementGroups();
        void AddElementIfNotExist(IDtmElement dtmElementGetDtmElement, IDtmMain dtmMain);
        bool HasSameElementsForExport();
        void ExportToDtm(IDtmExporter exporter);
    }
}
