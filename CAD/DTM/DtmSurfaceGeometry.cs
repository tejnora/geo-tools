using System;
using CAD.DTM.Gui;

namespace CAD.DTM
{
    class DtmSurfaceGeometry
    : IDtmGeometry
    {
        public IDtmGeometry BaseGeometry { get; set; }
        public string Id
        {
            get => BaseGeometry.Id;
            set => BaseGeometry.Id = value;
        }
        public void ExportToDtm(IDtmExporter exporter)
        {
            throw new NotImplementedException();
        }
    }
}
