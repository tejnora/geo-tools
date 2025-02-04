using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media.Imaging;
using CAD.Tools;
using GeoBase.Gui;
using GeoBase.Utils;
using VFK;

namespace CAD.VFK.GUI
{
    public partial class VfkLayerManager : DialogBase
    {
                public VfkLayerManager()
            : base("VfkLayerManager")
        {
            InitializeComponent();
            DataContext = this;
            Nodes = new ObservableCollection<VfkLayerGroupNode>();
        }
                        public ObservableCollection<VfkLayerGroupNode> Nodes
        { get; set; }
                        public void AddElement(VfkElement element, bool visble)
        {
            var group = Nodes.FirstOrDefault(n => n.VfkGroup == element.Owner.Owner);
            if (group == null)
            {
                group = new VfkLayerGroupNode(element.Owner.Owner);
                Nodes.Add(group);
            }
            group.AddElement(element, visble);
        }
        public List<NameObjectTwo<VfkElement, bool>> GetElements()
        {
            var elements = new List<NameObjectTwo<VfkElement, bool>>();
            foreach (var node in Nodes)
            {
                node.FillVfkElements(elements);
            }
            return elements;
        }
                        private void OnExpandAll(object sender, RoutedEventArgs e)
        {
            foreach (var node in Nodes)
                node.ExpandAll();
        }

        private void OnUnexpandAll(object sender, RoutedEventArgs e)
        {
            foreach (var node in Nodes)
                node.UnexpandAll();
        }

        private void OnSelectAll(object sender, RoutedEventArgs e)
        {
            foreach (var node in Nodes)
                node.SelectAll();
        }

        private void OnUnselectAll(object sender, RoutedEventArgs e)
        {
            foreach (var node in Nodes)
                node.UnselectAll();
        }
            }

    internal interface IVfkLayerManagerNode
    {
        void ExpandAll();
        void UnexpandAll();
        void SelectAll();
        void UnselectAll();
        void FillVfkElements(List<NameObjectTwo<VfkElement, bool>> elements);
    }

    public class VfkLayerGroupNode : DataObjectBase<VfkLayerGroupNode>, IVfkLayerManagerNode
    {
                public VfkLayerGroupNode(VfkElementGroup vfkGroup)
            : base(null, new StreamingContext())
        {
            Nodes = new ObservableCollection<VfkLayerSubGroupNode>();
            VfkGroup = vfkGroup;
        }
                        public string Name
        {
            get { return VfkGroup.Name; }
        }
        public BitmapImage Image
        {
            get { return VfkGroup.Image; }
        }
        public readonly PropertyData _isExpandedProperty = RegisterProperty("IsExpanded", typeof(bool), false);
        public bool IsExpanded
        {
            get { return GetValue<bool>(_isExpandedProperty); }
            set { SetValue(_isExpandedProperty, value); }
        }
        public VfkElementGroup VfkGroup
        { get; private set; }
        public ObservableCollection<VfkLayerSubGroupNode> Nodes
        { get; set; }
                        public void AddElement(VfkElement element, bool visble)
        {
            var subGroup = Nodes.FirstOrDefault(n => n.VfkSubGroup == element.Owner);
            if (subGroup == null)
            {
                subGroup = new VfkLayerSubGroupNode(element.Owner);
                Nodes.Add(subGroup);
            }
            subGroup.AddElement(element, visble);
        }
                        public void ExpandAll()
        {
            IsExpanded = true;
            foreach (var node in Nodes)
                node.ExpandAll();
        }

        public void UnexpandAll()
        {
            IsExpanded = false;
            foreach (var node in Nodes)
                node.UnexpandAll();
        }

        public void SelectAll()
        {
            foreach (var node in Nodes)
                node.IsVisible = true;
        }

        public void UnselectAll()
        {
            foreach (var node in Nodes)
                node.IsVisible = false;
        }
        public void FillVfkElements(List<NameObjectTwo<VfkElement, bool>> elements)
        {
            foreach (var node in Nodes)
                node.FillVfkElements(elements);
        }
            }

    public class VfkLayerSubGroupNode : DataObjectBase<VfkLayerSubGroupNode>, IVfkLayerManagerNode
    {
                public VfkLayerSubGroupNode(VfkElementSubGroup vfkSubGroup)
            : base(null, new StreamingContext())
        {
            VfkSubGroup = vfkSubGroup;
            Nodes = new ObservableCollection<VfkLayerItemNode>();
        }
                        public string Name
        {
            get { return VfkSubGroup.Description; }
        }
        public readonly PropertyData _isVisibleProperty = RegisterProperty("IsVisible", typeof(bool), false);
        public bool IsVisible
        {
            get { return GetValue<bool>(_isVisibleProperty); }
            set
            {
                if (GetValue<bool>(_isVisibleProperty) != value)
                {
                    SetValue(_isVisibleProperty, value);
                    foreach (var node in Nodes)
                        node.IsVisible = value;
                }
            }
        }
        public readonly PropertyData _isExpandedProperty = RegisterProperty("IsExpanded", typeof(bool), false);
        public bool IsExpanded
        {
            get { return GetValue<bool>(_isExpandedProperty); }
            set { SetValue(_isExpandedProperty, value); }
        }
        public ObservableCollection<VfkLayerItemNode> Nodes
        { get; set; }
        public VfkElementSubGroup VfkSubGroup
        { get; private set; }
                        public void AddElement(VfkElement element, bool visble)
        {
            if (!Nodes.Any(n => n.VfkElement == element))
            {
                Nodes.Add(new VfkLayerItemNode(element, visble));
            }
            if (!IsVisible)
            {
                foreach (var node in Nodes)
                {
                    if (node.IsVisible)
                    {
                        IsVisible = true;
                    }
                }
            }
        }
                        public void ExpandAll()
        {
            IsExpanded = true;
            foreach (var node in Nodes)
                node.IsExpanded = true;
        }

        public void UnexpandAll()
        {
            IsExpanded = false;
            foreach (var node in Nodes)
                node.IsExpanded = false;
        }

        public void SelectAll()
        {
            throw new NotImplementedException();
        }

        public void UnselectAll()
        {
            throw new NotImplementedException();
        }
        public void FillVfkElements(List<NameObjectTwo<VfkElement, bool>> elements)
        {
            foreach (var node in Nodes)
            {
                elements.Add(new NameObjectTwo<VfkElement, bool>(node.Name, node.VfkElement, node.IsVisible));
            }
        }
            }

    public class VfkLayerItemNode : DataObjectBase<VfkLayerItemNode>
    {
                public VfkLayerItemNode(VfkElement element, bool visible)
            : base(null, new StreamingContext())
        {
            VfkElement = element;
            IsVisible = visible;
        }
                        public readonly PropertyData _isExpandedProperty = RegisterProperty("IsExpanded", typeof(bool), false);
        public bool IsExpanded
        {
            get { return GetValue<bool>(_isExpandedProperty); }
            set { SetValue(_isExpandedProperty, value); }
        }
        public readonly PropertyData _isVisibleProperty = RegisterProperty("IsVisible", typeof(bool), false);
        public bool IsVisible
        {
            get { return GetValue<bool>(_isVisibleProperty); }
            set { SetValue(_isVisibleProperty, value); }
        }
        public string Name
        {
            get { return VfkElement.Description; }
        }
        public VfkElement VfkElement
        { get; private set; }
            }
}
