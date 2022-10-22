using System.Collections.ObjectModel;
using System.Windows;
using CAD.Utils;

namespace VFK.GUI
{
    /// <summary>
    /// Interaction logic for ElementsProperties.xaml
    /// </summary>
    public partial class ElementsProperties : Window
    {
        public ElementsProperties()
        {
            InitializeComponent();
            DataGridItems = new ObservableCollection<VfkElement>();
            AddElements(Singletons.VFKElements.VfkLineElementGroup.SubGroupItems);
            AddElements(Singletons.VFKElements.VfkMarkElementGroup.SubGroupItems);
            AddElements(Singletons.VFKElements.VfkNumberElementGroup.SubGroupItems);
            _mDataGrid.DataContext = this;
        }

        public ObservableCollection<VfkElement> DataGridItems
        {
            get; 
            set;
        }

        private void AddElements(ObservableCollection<VfkElementSubGroup> aGroups)
        {
            foreach (var @group in aGroups)
            {
                foreach (var element in group.Elements)
                {
                    DataGridItems.Add(element);                    
                }
            }
        }
    }
}
