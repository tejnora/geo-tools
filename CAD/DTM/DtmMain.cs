using CAD.DTM.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace CAD.DTM
{
    public class DtmMain
    : IDtmMain
    {
        readonly IDictionary<string, IDtmElementsGroup> _groups = new Dictionary<string, IDtmElementsGroup>();
        int _idAllocator = 1;
        public void AddElementGroup(string elementType, IDtmElementsGroup group)
        {
            _groups.Add(elementType, group);
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
            return $"ID{_idAllocator}_{DtmConfigurationSingleton.Instance.ElementSetting[name].CodeSuffix}";
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
    }
}
