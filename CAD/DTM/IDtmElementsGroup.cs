using CAD.DTM.Gui;
using System.Collections.Generic;
using CAD.DTM.Configuration;

namespace CAD.DTM
{
    public interface IDtmElementsGroup
    {
        string Name { get; }
        IEnumerable<IDtmElement> GetElementGroups();
        void AddElementIfNotExist(IDtmElement dtmElementGetDtmElement);
        bool HasSameElementsToExport();
        void ExportToDtm(IDtmExporter exporter, DtmElementOption option);
    }
}
