using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox=System.Windows.MessageBox;
using TreeView=System.Windows.Controls.TreeView;

namespace VFK.GUI
{
    public class CadasterViewModel : INotifyPropertyChanged
    {
        #region Data

        readonly ReadOnlyCollection<CadasterViewModel> _children;
        readonly CadasterViewModel _parent;
        readonly CadasterNamesTreeNode _cadasterNamesTreeNode;

        bool _isExpanded;
        bool _isSelected;

        #endregion // Data

        #region Constructors

        public CadasterViewModel(CadasterNamesTreeNode cadasterNamesTreeNode)
            : this(cadasterNamesTreeNode, null)
        {
        }

        private CadasterViewModel(CadasterNamesTreeNode cadasterNamesTreeNode, CadasterViewModel parent)
        {
            _cadasterNamesTreeNode = cadasterNamesTreeNode;
            _parent = parent;

            _children = new ReadOnlyCollection<CadasterViewModel>(
                    (from child in _cadasterNamesTreeNode.Children
                     select new CadasterViewModel(child, this))
                     .ToList<CadasterViewModel>());
        }

        #endregion // Constructors

        #region CadasterNamesTreeNode Properties

        public ReadOnlyCollection<CadasterViewModel> Children
        {
            get { return _children; }
        }

        public string Name
        {
            get { return _cadasterNamesTreeNode.Name; }
        }

        public CadasterNamesTreeNode CadasterNamesTreeNode
        {
            get { return _cadasterNamesTreeNode; }
        }

        #endregion // CadasterNamesTreeNode Properties

        #region Presentation Members

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected

        #region NameContainsText

        public bool NameContainsText(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(this.Name))
                return false;

            return this.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText

        #region Parent

        public CadasterViewModel Parent
        {
            get { return _parent; }
        }

        #endregion // Parent

        #endregion // Presentation Members

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }

    public class CadasterTreeViewModel
    {
        #region Data

        readonly Collection<CadasterViewModel> _firstGeneration;
        readonly ICommand _searchCommand;
        public TreeView OwnerControl
        {
            get; private set;
        }

        IEnumerator<CadasterViewModel> _matchingPeopleEnumerator;
        string _searchText = String.Empty;

        #endregion // Data

        #region Constructor

        public CadasterTreeViewModel(CadasterNamesTreeNode rootCadasterNamesTreeNode, TreeView aTreeView)
        {
            _firstGeneration = new Collection<CadasterViewModel>();
            foreach (var subNode in rootCadasterNamesTreeNode.Children)
            {
                CadasterViewModel item = new CadasterViewModel(subNode);
                _firstGeneration.Add(item);
            }
            OwnerControl = aTreeView;
            _searchCommand = new SearchFamilyTreeCommand(this);
        }

        #endregion // Constructor

        #region Properties

        #region FirstGeneration

        /// <summary>
        /// Returns a read-only collection containing the first person 
        /// in the family tree, to which the TreeView can bind.
        /// </summary>
        public Collection<CadasterViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        #endregion // FirstGeneration

        #region SearchCommand

        /// <summary>
        /// Returns the command used to execute a search in the family tree.
        /// </summary>
        public ICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        private class SearchFamilyTreeCommand : ICommand
        {
            readonly CadasterTreeViewModel _cadasterTree;

            public SearchFamilyTreeCommand(CadasterTreeViewModel cadasterTree)
            {
                _cadasterTree = cadasterTree;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                // I intentionally left these empty because
                // this command never raises the event, and
                // not using the WeakEvent pattern here can
                // cause memory leaks.  WeakEvent pattern is
                // not simple to implement, so why bother.
                add { }
                remove { }
            }

            public void Execute(object parameter)
            {
                _cadasterTree.PerformSearch();
            }
        }

        #endregion // SearchCommand

        #region SearchText

        /// <summary>
        /// Gets/sets a fragment of the name to search for.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value == _searchText)
                    return;

                _searchText = value;

                _matchingPeopleEnumerator = null;
            }
        }

        #endregion // SearchText

        #endregion // Properties

        #region Search Logic

        void PerformSearch()
        {
            if (_matchingPeopleEnumerator == null || !_matchingPeopleEnumerator.MoveNext())
                this.VerifyMatchingPeopleEnumerator();

            var person = _matchingPeopleEnumerator.Current;

            if (person == null)
                return;

            // Ensure that this person is in view.
            if (person.Parent != null)
                person.Parent.IsExpanded = true;

            person.IsSelected = true;
        }

        void VerifyMatchingPeopleEnumerator()
        {
            IEnumerable<CadasterViewModel> matches=null;
            foreach (var item in _firstGeneration)
            {
                if (matches == null)
                    matches = FindMatches(_searchText, item);
                else
                    matches=matches.Union(FindMatches(_searchText, item));
            }
            _matchingPeopleEnumerator = matches.GetEnumerator();
            if (!_matchingPeopleEnumerator.MoveNext())
            {
                MessageBox.Show(
                    "No matching names were found.",
                    "Try Again",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            }
        }

        IEnumerable<CadasterViewModel> FindMatches(string searchText, CadasterViewModel cadaster)
        {
            if (cadaster.NameContainsText(searchText))
                yield return cadaster;

            foreach (CadasterViewModel child in cadaster.Children)
                foreach (CadasterViewModel match in this.FindMatches(searchText, child))
                    yield return match;
        }

        #endregion // Search Logic
    }

    public class CadasterNamesTreeNode
    {
        readonly List<CadasterNamesTreeNode> _children = new List<CadasterNamesTreeNode>();
        public IList<CadasterNamesTreeNode> Children
        {
            get { return _children; }
        }
        public CadasterNamesTreeNode()
        {
            IsRootNode = false;
        }
        public string Name { get; set; }
        public string KU_NAZEV{ get; set;}
        public string OKRES_NAZEV{ get; set;}
        public string OBEC_NAZEV{ get; set;}
        public string KU_KOD{ get; set;}
        public string KU_PRAC{ get; set;}
        public string CISELNA_RADA{ get; set;}
        public string MAPA { get; set; }
        public bool IsRootNode { get; set; }

    }
    public class CadasterNamesTreeNodeReader
    {
        public CadasterNamesTreeNode RootNode
        {
            get; private set;
        }
        private CadasterNamesTreeNodeReader()
        {
            RootNode = new CadasterNamesTreeNode();
            RootNode.Name = "root";
            using (BinaryReader reader = new BinaryReader(File.Open(AppDomain.CurrentDomain.BaseDirectory + "\\Data\\SC_SEZNAMKUKRA.dat", FileMode.Open)))
            {
                Int32 count = reader.ReadInt32();
                CadasterNamesTreeNode dntn = new CadasterNamesTreeNode();
                dntn.Name = "A";
                dntn.IsRootNode = true;
                CadasterNamesTreeNode UNode = null;
                CadasterNamesTreeNode U2Node = null;
                for (Int32 i = 0; i < count; i++)
                {
                    CadasterNamesTreeNode node = new CadasterNamesTreeNode();
                    node.KU_NAZEV = reader.ReadString();
                    node.OKRES_NAZEV = reader.ReadString();
                    node.OBEC_NAZEV = reader.ReadString();
                    node.KU_KOD = reader.ReadString();
                    node.KU_PRAC = reader.ReadString();
                    node.CISELNA_RADA = reader.ReadString();
                    node.MAPA = reader.ReadString();
                    node.Name = node.KU_NAZEV;
                    String fc = node.KU_NAZEV.Substring(0, 2);
                    fc=fc.ToUpper();
                    if (fc.Substring(0,2)=="CH")
                        fc = "CH";
                    else
                    {
                        fc = fc.Substring(0, 1);
                    }
                    if (fc == "U")
                    {
                        if(UNode==null)
                        {
                            UNode=new CadasterNamesTreeNode();
                            UNode.Name = "U";
                            UNode.IsRootNode = true;
                            RootNode.Children.Add(UNode);
                            U2Node=new CadasterNamesTreeNode();
                            U2Node.Name = "Ú";
                            U2Node.IsRootNode = true;
                            RootNode.Children.Add(U2Node);
                        }
                        UNode.Children.Add(node);
                    }
                    else if (fc == "Ú")
                    {
                        if (UNode == null)
                        {
                            UNode = new CadasterNamesTreeNode();
                            UNode.Name = "U";
                            UNode.IsRootNode = true;
                            RootNode.Children.Add(UNode);
                            U2Node = new CadasterNamesTreeNode();
                            U2Node.Name = "Ú";
                            U2Node.IsRootNode = true;
                            RootNode.Children.Add(U2Node);
                        }
                        U2Node.Children.Add(node);
                    }
                    else
                    {
                        if (fc != dntn.Name)
                        {
                            RootNode.Children.Add(dntn);
                            dntn = new CadasterNamesTreeNode();
                            dntn.IsRootNode = true;
                            dntn.Name = fc;
                        }
                        dntn.Children.Add(node);
                    }
                }
                RootNode.Children.Add(dntn);
            }
        }
    }
}
