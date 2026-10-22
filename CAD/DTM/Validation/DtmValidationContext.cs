using System;
using System.Collections.Generic;
using System.Linq;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;

namespace CAD.DTM.Validation
{
    public class DtmValidationItem
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public IDtmElement Element { get; set; }
        public bool IsZps { get; set; }
        internal int? SnapshotLevel { get; set; }
        internal IReadOnlyList<IDtmGeometry> SnapshotGeometries { get; set; }
        public int Level
        {
            get
            {
                if (SnapshotLevel.HasValue) return SnapshotLevel.Value;
                if (Element is DtmElement element && element.SchemaAttributes != null)
                {
                    element.SchemaAttributes.SynchronizeLegacyValues();
                    var levelAttribute = element.SchemaAttributes.Fields().FirstOrDefault(f => f.Attribute.Name.StartsWith("UrovenUmisteniObjektu", StringComparison.Ordinal));
                    if (levelAttribute.Path != null && int.TryParse(element.SchemaAttributes.Get<string>(levelAttribute.Path), out var level)) return level;
                }
                if (Element.SpolecneAtributyZPS != null) return Element.SpolecneAtributyZPS.UrovenUmisteniObjektuZPS;
                if (Element.SpolecneAtributyObjektuZPS_TI != null) return Element.SpolecneAtributyObjektuZPS_TI.UrovenUmisteniObjektuTI;
                return 0;
            }
        }
        public char Mode => Element.EvaluateZapisObjektuForGui();
        public DtmElementType Type => Element.ElementType;
        public IEnumerable<DtmPoint> Points => Geometries.SelectMany(DtmValidationContext.GetPoints);
        public IEnumerable<DtmPoint> LinePoints => Geometries.OfType<DtmCurveGeometry>().SelectMany(DtmValidationContext.GetPoints);
        public IEnumerable<IDtmGeometry> Geometries => SnapshotGeometries ?? (IEnumerable<IDtmGeometry>)Element.Geometry?.Geometries ?? Enumerable.Empty<IDtmGeometry>();
    }

    public class DtmValidationContext
    {
        List<DtmValidationItem> _items;
        List<DtmValidationItem> _target;
        List<DtmValidationItem> _original;

        public DtmValidationContext(IDtmMain main, DtmExportCtx exportCtx, DtmValidationSettings settings)
        {
            Main = main;
            ExportCtx = exportCtx;
            Settings = settings;
        }

        public IDtmMain Main { get; }
        public DtmExportCtx ExportCtx { get; }
        public DtmValidationSettings Settings { get; }

        public IReadOnlyList<DtmValidationItem> Items => _items ?? (_items = CollectItems(e => e.ExportToOutput));

        // Cilovy stav: vsechny nesmazane objekty (vcetne referencnich).
        public IReadOnlyList<DtmValidationItem> TargetItems => _target ?? (_target = CollectItems(e => !e.IsDeleted && e.EvaluateZapisObjektuForGui() != 'd'));

        public IReadOnlyList<DtmValidationItem> OriginalItems => _original ?? (_original = CollectOriginalItems());
        public IEnumerable<DtmValidationItem> ChangedItems => TargetItems.Where(IsChanged);
        public IEnumerable<DtmValidationItem> DeletedItems => Items.Where(i => i.Mode == 'd');
        public static bool IsChanged(DtmValidationItem item) => item.Element.ExportToOutput && (item.Mode == 'i' || item.Mode == 'u');
        public static bool IsChangedPair(DtmValidationItem a, DtmValidationItem b) => IsChanged(a) || IsChanged(b);
        public DtmValidationItem OriginalOf(DtmValidationItem item) => OriginalItems.FirstOrDefault(i => ReferenceEquals(i.Element, item.Element));

        List<DtmValidationItem> CollectOriginalItems()
        {
            var result = new List<DtmValidationItem>();
            foreach (var item in CollectItems(e => true))
                if (item.Element is DtmElement element && element.ValidationOriginal != null)
                    result.Add(element.ValidationOriginal.CreateItem(item));
            return result;
        }

        internal bool IsPodrobnyBod(DtmValidationItem i) => i.IsZps && Settings.PodrobnyBodGroups.Contains(i.Group);
        internal bool IsPointObject(DtmValidationItem i) => i.IsZps && i.Type == DtmElementType.Bod
            && !Settings.PodrobnyBodGroups.Contains(i.Group) && !Settings.PointsExcludedFromTopology.Contains(i.Group);
        internal bool IsLine(DtmValidationItem i) => i.IsZps && i.Type == DtmElementType.Linie && i.LinePoints.Count() >= 2;

        public IEnumerable<DtmValidationItem> ZpsTarget => TargetItems.Where(i => i.IsZps);
        public IEnumerable<DtmValidationItem> PodrobneBody => ZpsTarget.Where(i => Settings.PodrobnyBodGroups.Contains(i.Group));
        public IEnumerable<DtmValidationItem> PointObjects => ZpsTarget.Where(i => i.Type == DtmElementType.Bod
            && !Settings.PodrobnyBodGroups.Contains(i.Group) && !Settings.PointsExcludedFromTopology.Contains(i.Group));
        public IEnumerable<DtmValidationItem> DefinitionPoints => ZpsTarget.Where(i => i.Type == DtmElementType.DefinicniBod);
        public IEnumerable<DtmValidationItem> Lines => ZpsTarget.Where(i => i.Type == DtmElementType.Linie && i.LinePoints.Count() >= 2);
        public IEnumerable<DtmValidationItem> Areas => ZpsTarget.Where(i => i.Type == DtmElementType.Plocha);

        List<DtmValidationItem> CollectItems(Func<IDtmElement, bool> filter)
        {
            var items = new List<DtmValidationItem>();
            foreach (var group in Main.GetElementGroups())
            {
                DtmConfigurationSingleton.Instance.ElementSetting.TryGetValue(group.Key, out var option);
                var isZps = option == null || option.ObsahovaCast == Settings.ZpsContentPart;
                var index = 0;
                foreach (var element in group.Value.GetElementGroups())
                {
                    index++;
                    if (!filter(element)) continue;
                    var name = group.Key;
                    if (element is DtmBodBaseElement bod && !string.IsNullOrEmpty(bod.CisloBodu))
                        name += " " + bod.CisloBodu;
                    else
                        name += " #" + index;
                    items.Add(new DtmValidationItem { Group = group.Key, Name = name, Element = element, IsZps = isZps });
                }
            }
            return items;
        }

        public static IEnumerable<DtmPoint> GetPoints(IDtmGeometry geometry)
        {
            switch (geometry)
            {
                case DtmMultiCurveGeometry multi:
                    foreach (var member in multi.Curves)
                        foreach (var pt in member.Points ?? new List<DtmPoint>()) yield return pt;
                    break;
                case DtmPointGeometry p:
                    if (p.Point != null) yield return p.Point;
                    break;
                case DtmCurveGeometry c:
                    foreach (var pt in c.Points ?? new List<DtmPoint>()) yield return pt;
                    break;
                case DtmPolygonGeometry pg:
                    foreach (var pt in pg.Points ?? new List<DtmPoint>()) yield return pt;
                    foreach (var ring in pg.InteriorRings)
                        foreach (var pt in ring) yield return pt;
                    break;
            }
        }

        public static bool HasZ(IDtmGeometry geometry) => geometry.SrsDimension == 3;

        public List<DtmPoint> GetChangeArea()
        {
            var polygon = Main.UdajeOZmenach?.Polygon ?? Main.UdajeOVydeji?.Polygon;
            return polygon?.Points != null && polygon.Points.Count >= 3 ? polygon.Points : null;
        }
    }
}
