using CAD.DTM.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using CAD.DTM.Elements;

namespace CAD.DTM
{
    public class DtmMain
    : IDtmMain
    {
        readonly IDictionary<string, IDtmElementsGroup> _groups = new Dictionary<string, IDtmElementsGroup>();
        int _idAllocator = 1;

        public DtmMain()
        {
            IndeticalPointsMapping = new Dictionary<string, string>();
        }
        public void AddElementGroup(string elementType, IDtmElementsGroup group)
        {
            _groups.Add(elementType, group);
        }

        public IDtmElementsGroup getElementsGroup(string elementType)
        {
            _groups.TryGetValue(elementType, out var group);
            return group;
        }

        public IEnumerable<KeyValuePair<string, IDtmElementsGroup>> GetElementGroups()
        {
            return _groups.AsEnumerable();
        }

        public void AddElementIfNotExist(string groupName, IDtmElement dtmElementGetDtmElement)
        {
            if (!_groups.ContainsKey(groupName))
            {
                _groups[groupName] = new DtmElementsGroup(groupName);
            }
            _groups[groupName].AddElementIfNotExist(dtmElementGetDtmElement, this);
        }

        public string AllocateUniqueId(string name)
        {
            if(DtmConfigurationSingleton.Instance.ElementSetting.ContainsKey(name))
                return $"ID{_idAllocator++}_{DtmConfigurationSingleton.Instance.ElementSetting[name].CodeSuffix}";
            return $"ID{_idAllocator++}_{name}";
        }

        public DtmUdajeOVydeji UdajeOVydeji { get; set; }
        public Dictionary<string, string> IndeticalPointsMapping { get; set; }
        public DtmIdentickyBodElement GetIdentickyBod(string cislo, bool referencni)
        {
            var identickeBody = getElementsGroup("IdentickyBod");
            if (identickeBody == null)
                return null;
            var point = identickeBody.GetElementGroups().First(n =>
            {
                var p = n as DtmIdentickyBodElement;
                return p.IsReferencePoint == referencni && !p.IsDeleted && cislo == p.CisloBodu;
            });
            return (DtmIdentickyBodElement)point;
        }


        public bool Import(DtmImportCtx ctx)
        {
            try
            {
                var importer = new DtmImporter(this);
                importer.ParseFile(ctx.FileName);
                //((DtmElementsGroup)_groups["IdentickyBod"]).Save();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool Export(DtmExportCtx ctx)
        {
            try
            {
                var importer = new DtmExporter(this);
                importer.CreateFile(ctx);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public IList<IDtmElement> ImportPoints(DtmImportPointsCtx ctx)
        {
            var points = new List<IDtmElement>();
            var lines = File.ReadAllLines(ctx.FileName);
            foreach (var line in lines)
            {
                var items = Regex.Split(line, @"\s{2,}");
                if (items.Length != 4)
                    throw new ArgumentOutOfRangeException("Format it is not correct.");
                var element = DtmConfigurationSingleton.Instance.CreateType(ctx.PointTypeSelected);
                element.Geometry = new DtmPointGeometry
                {
                    Point = new DtmPoint("-" + items[1], "-" + items[2], items[3])
                };
                element.CisloBodu = items[0];
                AddElementIfNotExist(ctx.PointTypeSelected, element);
                points.Add(element);
            }
            return points;
        }
    }
}
