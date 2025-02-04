using System;
using System.Windows.Input;
using CAD.Utils;
using GeoBase.Gui;

namespace VFK.GUI
{
    public partial class KatastralniUzemiDialog : DialogBase
    {
                public KatastralniUzemiDialog()
            : base("KatastralniUzemi")
        {
            InitializeComponent();
            _cadasterTree = new CadasterTreeViewModel(Singletons.CadasterNamesTreeNodeReader.RootNode,_treeView);
            DataContext = _cadasterTree;
        }
                        readonly CadasterTreeViewModel _cadasterTree;  
        public CadasterNamesTreeNode SelectedNode
        {
            get;
            private set;
        }
                        public void OkSearchTextBoxKeyDown(object sender, KeyEventArgs arg)
        {
            if (arg.Key == Key.Enter)
                _cadasterTree.SearchCommand.Execute(null);
        }
        public void OnSelect(object sender, EventArgs arg)
        {
            if (_treeView.SelectedItem == null) 
                return;
            SelectedNode = ((CadasterViewModel)_treeView.SelectedItem).CadasterNamesTreeNode;
            if (SelectedNode.IsRootNode) 
                return;
            DialogResult = true;
            Close();
        }
            }
}
