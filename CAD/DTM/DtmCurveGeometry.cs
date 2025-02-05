
using System.Collections.Generic;
using System.Windows.Documents;

namespace CAD.DTM
{
    class DtmCurveGeometry
    : IDtmGeometry
    {
        public string Id { get; set; }
        public string SrsName { get; set; }
        public int SrsDimension { get; set; }
        public List<DtmPoint> Points { get; set; }
    }
}
