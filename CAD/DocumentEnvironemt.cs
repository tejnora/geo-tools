using AvalonDock;
using System.Collections.Generic;

namespace CAD
{
    enum DockablePalette
    {
        SeznamSouradnic,
        DtmPropPage
    }

    class DocumentEnvironemt
    {
        static readonly DocumentEnvironemt _instance = new DocumentEnvironemt();

        readonly Dictionary<DockablePalette, DockableContent> _palette = new Dictionary<DockablePalette, DockableContent>();
        DocumentEnvironemt()
        {
        }
        public static DocumentEnvironemt Instance => _instance;

        public DockableContent GetPalette(DockablePalette type)
        {
            return _palette[type];
        }

        public void RegisterPalette(DockablePalette type, DockableContent content)
        {
            _palette[type] = content;
        }

        public void HideAllPalette(DockingManager manager)
        {
            foreach (var dockableContent in _palette)
            {
                if (dockableContent.Value.State == DockableContentState.Hidden)
                    continue;
                manager.Hide(dockableContent.Value);
                ((IDockableContent)dockableContent.Value).Reset();
            }
        }

    }
}
