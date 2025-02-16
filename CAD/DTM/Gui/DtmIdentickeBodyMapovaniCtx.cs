using System;
using System.Collections.Generic;
using CAD.DTM.Elements;
using GeoBase.Utils;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace CAD.DTM.Gui
{
    public class DtmIdentickeBodMapovani
    {
        public string ReferencniBod { get; set; }
        public DtmPoint ReferencniBodSouradnice { get; set; }
        public string NamerenyBod { get; set; }
    }

    public class DtmIdentickeBodyMapovaniCtx : DataObjectBase<DtmImportCtx>
    {
        readonly DtmMain _dtmMain;
        static string NA = "N/A";
        public DtmIdentickeBodyMapovaniCtx(DtmMain dtmMain)
            : base(null, new StreamingContext())
        {
            _dtmMain = dtmMain;
            var identickeBody = _dtmMain.getElementsGroup("IdentickyBod");
            if (identickeBody == null)
                return;
            NamereneBody = new List<string> { NA };
            foreach (var bod in identickeBody.GetElementGroups())
            {
                if (!TryGetPointData(bod, out var cislo, out var iBod, NamereneBodyFilter))
                    continue;
                NamereneBody.Add(cislo);
            }
            IdentickeBody = new ObservableCollection<DtmIdentickeBodMapovani>();
            foreach (var bod in identickeBody.GetElementGroups())
            {
                if (!TryGetPointData(bod, out var cislo, out var iBod, p => !p.IsReferencePoint || p.IsDeleted))
                    continue;
                IdentickeBody.Add(new DtmIdentickeBodMapovani
                {
                    ReferencniBod = cislo,
                    ReferencniBodSouradnice = iBod,
                    NamerenyBod = FindNamerenyBod(cislo)
                });
            }
        }
        string FindNamerenyBod(string referencniBod)
        {
            if (!_dtmMain.IndeticalPointsMapping.TryGetValue(referencniBod, out var namerenyBod))
            {
                return NA;
            }
            return NamereneBody.Any(n => n == namerenyBod) ? namerenyBod : NA;
        }
        static bool TryGetPointData(IDtmElement element, out string cislo, out DtmPoint bod, Func<DtmIdentickyBodElement, bool> filter)
        {
            cislo = null;
            bod = null;
            var iBod = element as DtmIdentickyBodElement;
            if (iBod == null || filter(iBod)) return false;
            var iPoint = iBod.Geometry as DtmPointGeometry;
            if (iPoint == null) return false;
            cislo = iBod.CisloBodu;
            bod = iPoint.Point;
            return true;
        }
        public void ApplyChanges()
        {
            _dtmMain.IndeticalPointsMapping.Clear();
            foreach (var ib in IdentickeBody)
            {
                if (ib.NamerenyBod == "N/A")
                    continue;
                _dtmMain.IndeticalPointsMapping[ib.ReferencniBod] = ib.NamerenyBod;
            }
        }
        public readonly PropertyData _itdentickeBody = RegisterProperty("IdentickeBody", typeof(ObservableCollection<DtmIdentickeBodMapovani>), new DtmIdentickeBodMapovani());
        public ObservableCollection<DtmIdentickeBodMapovani> IdentickeBody
        {
            get => GetValue<ObservableCollection<DtmIdentickeBodMapovani>>(_itdentickeBody);
            set => SetValue(_itdentickeBody, value);
        }
        public List<string> NamereneBody { get; set; }

        bool NamereneBodyFilter(DtmIdentickyBodElement p)
        {
            return p.IsReferencePoint || p.IsDeleted;
        }
        public void AutoMap()
        {
            var identickeBody = _dtmMain.getElementsGroup("IdentickyBod");
            for (var i = 0; i < IdentickeBody.Count; i++)
            {
                if (IdentickeBody[i].NamerenyBod != NA)
                    continue;
                var best = double.MaxValue;
                foreach (var bod in identickeBody.GetElementGroups())
                {
                    if (!TryGetPointData(bod, out var bodCislo, out var bodPoint, NamereneBodyFilter))
                        continue;
                    var res = Math.Sqrt(Math.Pow(IdentickeBody[i].ReferencniBodSouradnice.X - bodPoint.X, 2) + Math.Pow(IdentickeBody[i].ReferencniBodSouradnice.Y - bodPoint.Y, 2));
                    if (best < res)
                        continue;
                    best = res;
                    IdentickeBody[i].NamerenyBod = bodCislo;
                }
            }
            //not nice, but it works currently
            var backup = IdentickeBody;
            IdentickeBody = new ObservableCollection<DtmIdentickeBodMapovani>();
            IdentickeBody = backup;
        }

        public IDtmMain DtmMain => _dtmMain;
    }
}
