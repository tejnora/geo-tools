using CAD.Utils;
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
                familyNode = new FamilyNode { Name = family, Members = new List<LayerNode>(), ParentDialog = this };
                Families.Add(familyNode);
            }
            familyNode.Members.Add(new LayerNode() { IsChecked = isEnabled, Name = groupName, Parent = familyNode });
            familyNode.UpdateCheckboxState();
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

        public void ForceReloadAllData()
        {
            DataContext = null;
            DataContext = this;
        }
    }

    public class FamilyNode
    {
        public DtmLayerManager ParentDialog;
        public string Name { get; set; }
        public List<LayerNode> Members { get; set; }
        bool _isChecked = true;
        bool _isCheckedLock;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isCheckedLock = true;
                _isChecked = value;
                foreach (var node in Members)
                {
                    node.IsChecked = value;
                }
                _isCheckedLock = false;
                ParentDialog.ForceReloadAllData();
            }
        }

        public void UpdateCheckboxState()
        {
            if (Members.Count == 0 || _isCheckedLock)
                return;
            _isChecked = Members[0].IsChecked;
            for (var i = 1; i < Members.Count; i++)
            {
                if (Members[i].IsChecked == _isChecked) continue;
                _isChecked = false;
                ParentDialog.ForceReloadAllData();
                return;
            }
            ParentDialog.ForceReloadAllData();
        }
    }

    public class LayerNode
    {
        public FamilyNode Parent { get; set; }
        public string Name { get; set; }

        bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value)
                    return;
                _isChecked = value;
                Parent?.UpdateCheckboxState();
            }
        }
    }
}
