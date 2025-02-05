using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CAD.DTM
{
    public class DtmMain
    : IDtmMain
    {
        readonly IDictionary<string, IDtmElementsGroup> _groups = new Dictionary<string, IDtmElementsGroup>();
        public void AddElementGroup(string elementType, IDtmElementsGroup group)
        {
            _groups.Add(elementType, group);
        }

        public IEnumerable<KeyValuePair<string, IDtmElementsGroup>> GetElementGroups()
        {
            return _groups.AsEnumerable();
        }

        public bool Import(DtmImportCtx ctx)
        {
            try
            {
                var importer = new DtmReader(this);
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
    }
}
