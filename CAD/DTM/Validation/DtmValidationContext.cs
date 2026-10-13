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
        public int Level
        {
            get
            {
                if (Element.SpolecneAtributyZPS != null) return Element.SpolecneAtributyZPS.UrovenUmisteniObjektuZPS;
                if (Element.SpolecneAtributyObjektuZPS_TI != null) return Element.SpolecneAtributyObjektuZPS_TI.UrovenUmisteniObjektuTI;
                return 0;
            }
        }
        public char Mode => Element.EvaluateZapisObjektuForGui();
        public DtmElementType Type => Element.ElementType;
        public IEnumerable<DtmPoint> Points => Geometries.SelectMany(DtmValidationContext.GetPoints);
        public IEnumerable<DtmPoint> LinePoints => Geometries.OfType<DtmCurveGeometry>().SelectMany(g => g.Points ?? new List<DtmPoint>());
        public IEnumerable<IDtmGeometry> Geometries => Element.Geometry?.Geometries ?? Enumerable.Empty<IDtmGeometry>();
    }

    public class DtmValidationContext
    {
        List<DtmValidationItem> _items;
        List<DtmValidationItem> _target;

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
        public IReadOnlyList<DtmValidationItem> TargetItems => _target ?? (_target = CollectItems(e => !e.IsDeleted));

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
                case DtmPointGeometry p:
                    if (p.Point != null) yield return p.Point;
                    break;
                case DtmCurveGeometry c:
                    foreach (var pt in c.Points ?? new List<DtmPoint>()) yield return pt;
                    break;
                case DtmPolygonGeometry pg:
                    foreach (var pt in pg.Points ?? new List<DtmPoint>()) yield return pt;
                    break;
            }
        }

        public static bool HasZ(IDtmGeometry geometry) =>
            geometry is DtmPointGeometry || geometry is DtmCurveGeometry || geometry.SrsDimension == 3;

        public List<DtmPoint> GetChangeArea()
        {
            var polygon = Main.UdajeOZmenach?.Polygon ?? Main.UdajeOVydeji?.Polygon;
            return polygon?.Points != null && polygon.Points.Count >= 3 ? polygon.Points : null;
        }
    }
}
