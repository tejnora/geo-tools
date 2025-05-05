using GeoBase.Gui;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CAD.DTM.Gui
{
    public partial class DtmLayerManager : DialogBase
    {
        public DtmLayerManager()
            : base("DtmLayerManager", false, true)
        {
            InitializeComponent();
            DataContext = this;
        }

        public void AddElement(string family, string groupName, bool isEnabled)
        {
            var familyNode = Families.FirstOrDefault(n => n.Name == family);
            if (familyNode == null)
            {
                familyNode = new FamilyNode { Name = family, Members = new List<LayerNode>() };
                Families.Add(familyNode);
            }
            familyNode.Members.Add(new LayerNode() { IsChecked = isEnabled, Name = groupName });

        }

        public ObservableCollection<FamilyNode> Families { get; set; } = new ObservableCollection<FamilyNode>();

        void OnConfirm(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            SavePosAndSize();
            Close();
        }

        void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public bool IsVisibleElement(string family, string groupName)
        {
            var familyNode = Families.First(n => n.Name == family);
            var layoutName = familyNode.Members.First(n => n.Name == groupName);
            return layoutName.IsChecked;
        }
    }

    public class FamilyNode : DependencyObject
    {
        public string Name { get; set; }
        public List<LayerNode> Members { get; set; }
        public bool IsChecked { get; set; }
    }

    public class LayerNode : DependencyObject
    {
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}
