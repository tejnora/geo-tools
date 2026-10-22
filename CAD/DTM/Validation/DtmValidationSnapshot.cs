using System.Collections.Generic;
using System.Linq;
using CAD.DTM.Elements;
using CAD.DTM.Gui;

namespace CAD.DTM.Validation
{
    // Only geometry and level participate in the before/after dependency checks.
    internal sealed class DtmValidationSnapshot
    {
        readonly List<IDtmGeometry> _geometries;
        readonly int _level;

        public DtmValidationSnapshot(DtmElement element)
        {
            _level = new DtmValidationItem { Element = element }.Level;
            _geometries = (element.Geometry?.Geometries ?? new List<IDtmGeometry>())
                .Select(DtmGeometryEditorDialog.Clone).ToList();
        }

        public DtmValidationItem CreateItem(DtmValidationItem current) => new DtmValidationItem
        {
            Element = current.Element, Group = current.Group, Name = current.Name, IsZps = current.IsZps,
            SnapshotLevel = _level,
            SnapshotGeometries = _geometries.Select(DtmGeometryEditorDialog.Clone).ToList()
        };
    }
}
