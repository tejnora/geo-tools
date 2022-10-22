using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using GeoBase.Gui;
using GeoBase.Localization;
using GeoBase.Utils;
using GeoCalculations.MethodPoints;
using GeoHelper.Printing;
using GeoHelper.Protocols;
using GeoHelper.Tables.AdditionalGui;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Tables.TablePrintingElements;
using GeoHelper.Tabulky;
using GeoHelper.Utils;
using Utils.PartialStream;

namespace GeoHelper.Tables
{
    [Flags]
    public enum ColumnVisibility : uint
    {
        Prefix = 0x1,
        Number = 0x2,
        Hz = 0x4,
        ZenitAngle = 0x8,
        dH = 0x10,
        Signal = 0x20,
        Description = 0x40,
        HorizontalDistance = 0x80,
        CoordinateX = 0x100,
        CoordinateY = 0x200,
        CoordinateZ = 0x400,
        Quality = 0x800,
        SpolX = 0x1000,
        SpolY = 0x2000,
        SpolQuality = 0x4000,
        All = 0xFFFFFFFF
    }

    public abstract class TableBase : UserControlBase, ILoadSaveMdi, IManipulateItems, IDisposable, IEditMenu, IPrinting, IUndoRedo, ITableBase
    {
        protected TableBase(IMainWindow mainWindow)
        {
            MainWindow = mainWindow;
            ColumnVisibility = ColumnVisibility.All;
            _undoRedo = new UndoRedo(this);
        }

        protected void InitTableBase()
        {
            DataGrid.MouseDown += OnMouseDown;
            DataGrid.MouseUp += OnMouseUp;
            DataGrid.MouseMove += OnMouseMove;
            DataGrid.MouseDoubleClick += OnMouseDoubleClick;
            DataGrid.PreviewKeyDown += OnPreviewKeyDown;
            DataGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            DataGrid.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            DataGrid.Background = Brushes.White;
            DataGrid.RowHeaderWidth = 0;
            DataGrid.SelectionMode = DataGridSelectionMode.Single;
            DataGrid.CanUserSortColumns = false;
            DataGrid.CanUserAddRows = true;
            DataGrid.CanUserDeleteRows = false;
            DataGrid.AutoGenerateColumns = false;
            DataGrid.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
            TableInfoContext = new TableInfoContext();
            if (CanSort())
            {
                ViewNodes = new CollectionViewSource { Source = _nodes };
                ViewNodes.SortDescriptions.Clear();
                ViewNodes.SortDescriptions.Add(new SortDescription("NumberWithPrefixForSorting", ListSortDirection.Ascending));
            }
            DataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.None;
            new DataGridDragDropManager<TableNodesBase>(DataGrid, this, GetTableName());
        }

        ObservableCollection<TableNodesBase> _nodes = new ObservableCollection<TableNodesBase>();
        protected virtual DataGrid DataGrid { get; set; }
        protected readonly LanguageDictionary _dictionary = LanguageConverter.ResolveDictionary();


        public ObservableCollection<TableNodesBase> Nodes
        {
            get { return _nodes; }
            protected set
            {
                _nodes = value;
                if (ViewNodes != null)
                    ViewNodes.Source = _nodes;
            }
        }

        public CollectionViewSource ViewNodes { get; set; }
        public IMainWindow MainWindow { get; set; }
        public ColumnVisibility ColumnVisibility { get; set; }

        protected virtual string GetTableName()
        {
            Debug.Assert(false);
            return string.Empty;
        }

        protected virtual bool CanSort()
        {
            return false;
        }

        bool _mousedown;
        bool _seleted;

        protected void OnMouseDown(object sender, MouseButtonEventArgs args)
        {
            if (args.RightButton != MouseButtonState.Pressed) return;
            var row = UIHelpers.TryFindFromPoint<DataGridRow>((UIElement)sender, args.GetPosition(DataGrid));
            if (row == null || row.IsEditing) return;
            _mousedown = true;
            var node = row.Item as TableNodesBase;
            if (node == null) return;
            _seleted = !node.Selected;
            node.Selected = _seleted;
        }

        protected void OnMouseUp(object sender, MouseButtonEventArgs args)
        {
            _mousedown = false;
        }

        protected void OnMouseMove(object sender, MouseEventArgs args)
        {
            if (!_mousedown) return;
            if (args.RightButton != MouseButtonState.Pressed) return;
            var row = UIHelpers.TryFindFromPoint<DataGridRow>((UIElement)sender, args.GetPosition(DataGrid));
            if (row == null || row.IsEditing) return;
            var node = row.Item as TableNodesBase;
            if (node == null) return;
            node.Selected = _seleted;
        }

        protected void OnMouseDoubleClick(object sender, MouseButtonEventArgs args)
        {
            var cell = DataGrid.CurrentCell;
            if (cell.Column is DataGridTemplateColumn)
            {
                if (cell.Item != null)
                {
                    var node = (TableNodesBase)cell.Item;
                    node.Locked = !node.Locked;
                    return;
                }
            }
            OnEdit();
        }

        protected void OnPreviewKeyDown(object sender, KeyEventArgs args)
        {
            switch (args.Key)
            {
                case Key.Delete:
                    Delete(false);
                    args.Handled = true;
                    break;
                case Key.Enter:
                    OnEdit();
                    args.Handled = true;
                    break;
                case Key.Add:
                    {
                        foreach (var node in Nodes)
                        {
                            node.Selected = true;
                        }
                    }
                    break;
                case Key.Subtract:
                    {
                        foreach (var node in Nodes)
                        {
                            node.Selected = false;
                        }
                    }
                    break;
            }
        }

        protected TableInfoContext TableInfoContext { get; set; }
        public virtual FileInfo FileName { get; set; }
        public Stream FileStream { get; set; }

        public bool GetIsModifed()
        {
            return _modfied;
        }

        public void RestModifedFlag()
        {
            _modfied = false;
        }

        public void Serialize(Stream aWriter)
        {
            var infoStream = PartialStreamFactory.CreateWritePartialStream(aWriter);
            infoStream.WriteInt32(1);
            TableInfoContext.Serialize(infoStream);
            infoStream.WriteInt32((int)ColumnVisibility);
            var swp = (IStreamWithParent)infoStream;
            var parent = swp.GetParentStream();
            infoStream.Dispose();
            aWriter = parent;
            var w = new BinaryWriter(aWriter);
            w.Write((UInt32)Nodes.Count);
            foreach (var node in Nodes)
            {
                node.Serialize(w);
            }
        }

        public virtual void Deserialize(Stream aReader)
        {
            _modfied = false;
            FileStream = aReader;
            var infoStream = PartialStreamFactory.CreateReadPartialStream(aReader);
            infoStream.ReadInt32();
            TableInfoContext.FileName = FileName.FullName;
            TableInfoContext.Deserialize(infoStream);
            if (infoStream.Position < infoStream.Length)
                ColumnVisibility = (ColumnVisibility)infoStream.ReadInt32();
            var swp = (IStreamWithParent)infoStream;
            var parent = swp.GetParentStream();
            infoStream.Dispose();
            aReader = parent;
            Nodes = new ObservableCollection<TableNodesBase>();
            var r = new BinaryReader(aReader);
            var count = r.ReadUInt32();
            for (uint i = 0; i < count; i++)
            {
                var newNode = GetNewNode();
                newNode.Deserialize(r);
                Nodes.Add(newNode);
            }
            UpdateTableColumnView();
        }

        public virtual string SaveDialogFilter
        {
            get { return string.Empty; }
        }

        public virtual TableNodesBase GetNewNode()
        {
            return null;
        }

        public bool CanCopy()
        {
            return true;
        }

        public void Copy()
        {
            var context = new CopyPasteContext();
            var selectedNodes = (from s in Nodes where s.Selected select s).ToList();
            if (selectedNodes.Count > 0)
                context.Nodes = selectedNodes;
            else if (DataGrid.SelectedItems.Count > 0)
            {
                context.Nodes = new List<TableNodesBase>();
                context.Nodes.Add((TableNodesBase)DataGrid.SelectedItems[0]);
            }
            if (context.Nodes != null)
            {
                Clipboard.SetData("GeoHelper\\" + GetTableName(), context);
            }
        }

        public bool CanPaste()
        {
            return Clipboard.ContainsData("GeoHelper\\" + GetTableName());
        }

        public void Paste()
        {
            if (!Clipboard.ContainsData("GeoHelper\\" + GetTableName()))
                return;
            try
            {
                var context = Clipboard.GetData("GeoHelper\\" + GetTableName()) as CopyPasteContext;
                var index = DataGrid.SelectedIndex;
                if (index == -1)
                    index = 0;
                using (new Action(this))
                {
                    foreach (var node in context.Nodes)
                    {
                        InsertItem(node, index, null);
                        index++;
                    }
                }
            }
            catch (Exception)
            {
                Debug.Assert(false);
            }
        }

        public bool CanCut()
        {
            return DataGrid.SelectedItem != null;
        }

        public void Cut()
        {
            Copy();
            Delete(true);
        }

        public virtual bool CanEdit()
        {
            return false;
        }

        public virtual bool CanInsert()
        {
            return false;
        }

        public virtual void OnInsert()
        {
        }

        public virtual bool CanDelete()
        {
            return DataGrid.SelectedItem != null;
        }

        public virtual void Delete(bool skipWarnning)
        {
            using (new Action(this))
            {
                var selectedNodes = (from s in Nodes where s.Selected select s).ToList();
                if (selectedNodes.Count > 0)
                {
                    var par = new ResourceParams();
                    par.Add("count", selectedNodes.Count.ToString(CultureInfo.InvariantCulture));
                    if (skipWarnning || _dictionary.ShowMessageBox("TableBase.0", par, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        for (var i = 0; i < selectedNodes.Count; i++)
                        {
                            DeleteNodeRaw(selectedNodes[i], i == selectedNodes.Count - 1, false);
                        }
                    }
                }
                else if (DataGrid.SelectedItems.Count > 0)
                {
                    if (skipWarnning || _dictionary.ShowMessageBox("TableBase.1", null, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        var selItem = (TableNodesBase)DataGrid.SelectedItems[0];
                        DeleteNodeRaw(selItem, true, false);
                    }
                }
            }
        }

        public virtual void OnEdit()
        {
        }

        public virtual bool AddNewNode(CalculatedPointBase newPoint, IProtocolContext protocolContext)
        {
            return false;
        }

        public virtual void OnShowInfoDialog()
        {
        }

        public virtual bool GetHasSouborInfo()
        {
            return false;
        }

        public bool CanUndo()
        {
            return _undoRedo != null && _undoRedo.CanUndo();
        }

        public void Undo()
        {
            _undoRedo.DoUndo();
        }

        public bool CanRedo()
        {
            return _undoRedo != null && _undoRedo.CanRedo();
        }

        public void Redo()
        {
            _undoRedo.DoRedo();
        }

        public bool DeleteNodeRaw(TableNodesBase selItem, bool changeDataGridSelecteion, bool skipWarnning)
        {
            if (selItem.Locked)
            {
                _dictionary.ShowMessageBox("TableBase.2", null, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            var index = Nodes.IndexOf(selItem);
            DeleteItemRaw(index, changeDataGridSelecteion);
            return true;
        }

        public virtual void InsertItem(TableNodesBase node, int index, IProtocolContext protocol)
        {
            if (index != -1)
                InsertItemRaw(node, index);
            else if (DataGrid.SelectedIndex != -1)
                InsertItemRaw(node, DataGrid.SelectedIndex);
            else
                InsertItemRaw(node, -1);
        }

        public void InsertItemRaw(TableNodesBase node, int index)
        {
            using (var action = new Action(this))
            {
                Nodes.Insert(index != -1 ? index : Nodes.Count, node);
                MarkModification();
                DataGrid.UpdateLayout();
                SelectNode(node);
                action.AddAddedItem(node);
            }
        }

        public void DeleteItemRaw(int index, bool changeDataSelection)
        {
            using (var action = new Action(this))
            {
                action.AddDeletedItem(Nodes[index]);
                Nodes.RemoveAt(index);
                MarkModification();
                DataGrid.UpdateLayout();
                if (!changeDataSelection) return;
                if (index < Nodes.Count)
                    SelectNode(Nodes[index]);
                else if (Nodes.Count > 0)
                    SelectNode(Nodes[Nodes.Count - 1]);
            }
        }

        public void EditNode(TableNodesBase newNode, int index)
        {
            using (var action = new Action(this))
            {
                action.AddEditedNode(Nodes[index]);
                Nodes[index].AssignNode(newNode);
            }
        }

        [Serializable]
        class CopyPasteContext
        {
            public List<TableNodesBase> Nodes;
        }

        bool _modfied = true;

        protected void SortNodes()
        {
        }

        public void MarkModification()
        {
            if (_modfied) return;
            MainWindow.SetModifiedFlag(this);
            _modfied = true;
        }

        protected virtual void UpdateTableColumnView()
        {
        }

        protected bool GetIsSet(ColumnVisibility column)
        {
            return (ColumnVisibility & column) == column;
        }

        protected void SetColumnVisibility(ColumnVisibility columnName, DataGridColumn control)
        {
            control.Visibility = GetIsSet(columnName) ? Visibility.Visible : Visibility.Collapsed;
        }

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                if (FileStream != null)
                {
                    FileStream.Close();
                    FileStream = null;
                }
            }
            _disposed = true;
        }
        ~TableBase()
        {
            Dispose(false);
        }

        RoutedCommand _deleteCommand;
        RoutedCommand _editCommand;
        RoutedCommand _findCommand;
        RoutedCommand _infoCommand;
        RoutedCommand _insertCommand;
        bool _needConstructMenu = true;

        public virtual void FillMenuItems(MenuItem menu)
        {
            menu.Items.Clear();
            if (_needConstructMenu)
            {
                ConstructMenu();
                _needConstructMenu = false;
            }
            var dictionary = LanguageConverter.ResolveDictionary();
            menu.Visibility = Visibility.Visible;
            menu.Header = GetMenuCaption();
            menu.Items.Add(new MenuItem { Header = dictionary.Translate<string>("TableBase.3", "Text"), Command = _insertCommand });
            menu.Items.Add(new MenuItem { Header = dictionary.Translate<string>("TableBase.4", "Text"), Command = _deleteCommand });
            menu.Items.Add(new MenuItem { Header = dictionary.Translate<string>("TableBase.5", "Text"), Command = _editCommand });
            menu.Items.Add(new MenuItem { Header = dictionary.Translate<string>("TableBase.6", "Text"), Command = _findCommand });
            menu.Items.Add(new Separator());
            menu.Items.Add(new MenuItem { Header = dictionary.Translate<string>("TableBase.7", "Text"), Command = _infoCommand });
        }

        public virtual string GetMenuCaption()
        {
            Debug.Assert(false);
            return string.Empty;
        }

        void ConstructMenu()
        {
            var ownerWindow = MainWindow.GetMdiWindow(this);
            var mainControl = MainWindow as Control;
            foreach (var b in mainControl.CommandBindings)
            {
                DataGrid.CommandBindings.Add(b as CommandBinding);
            }
            _insertCommand = new RoutedCommand("InsertItem", ownerWindow.GetType());
            BindCommand(new CommandBinding(_insertCommand, OnInsertItem));
            ownerWindow.InputBindings.Add(new KeyBinding(_insertCommand, Key.Insert, ModifierKeys.None));

            _deleteCommand = new RoutedCommand("DeleteItem", ownerWindow.GetType());
            BindCommand(new CommandBinding(_deleteCommand, OnDeleteItem, CanDeleteItem));
            ownerWindow.InputBindings.Add(new KeyBinding(_deleteCommand, Key.Delete, ModifierKeys.None));

            _editCommand = new RoutedCommand("EditItem", ownerWindow.GetType());
            BindCommand(new CommandBinding(_editCommand, OnEditItem, CanEdiItem));
            ownerWindow.InputBindings.Add(new KeyBinding(_editCommand, Key.Enter, ModifierKeys.None));

            _infoCommand = new RoutedCommand("InfoTable", ownerWindow.GetType());
            ownerWindow.CommandBindings.Add(new CommandBinding(_infoCommand, OnInfoTableEdit, CanInfoTable));

            _findCommand = new RoutedCommand("FindTableItem", ownerWindow.GetType());
            BindCommand(new CommandBinding(_findCommand, OnFindItemDialog));
            ownerWindow.InputBindings.Add(new KeyBinding(_findCommand, Key.F, ModifierKeys.Control));
        }

        protected void BindCommand(CommandBinding command)
        {
            var ownerWindow = MainWindow.GetMdiWindow(this);
            var mainControl = MainWindow as Control;
            ownerWindow.CommandBindings.Add(command);
            mainControl.CommandBindings.Add(command);
        }

        void OnInsertItem(object sender, EventArgs args)
        {
            OnInsert();
        }

        void CanDeleteItem(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = DataGrid.SelectedItem != null && CanInsert();
            args.Handled = true;
        }

        void OnDeleteItem(object sender, EventArgs args)
        {
            Delete(false);
        }

        void CanEdiItem(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = DataGrid.SelectedItem != null && CanEdit();
            args.Handled = true;
        }

        void OnEditItem(object sender, EventArgs args)
        {
            OnEdit();
        }

        void CanInfoTable(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = GetHasSouborInfo();
            args.Handled = true;
        }

        void OnInfoTableEdit(object sender, EventArgs args)
        {
            OnShowInfoDialog();
        }

        void OnFindItemDialog(object sender, EventArgs args)
        {
            var dlg = new FindPointDialog(this);
            dlg.ShowDialog();
            var row = (DataGridRow)DataGrid.ItemContainerGenerator.ContainerFromItem(DataGrid.SelectedValue);
            if (row != null)
                row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        int _matchedNodeIdx = -1;
        List<TableNodesBase> _matchedNodes;

        public void SelectNodeWith(string cisloBodu)
        {
            _matchedNodes = (from n in Nodes where n.Number == cisloBodu select n).ToList();
            _matchedNodeIdx = -1;
            SelectNodeNext();
        }

        public void SelectNodeNext()
        {
            if (_matchedNodes == null || _matchedNodes.Count == 0)
                return;
            if (_matchedNodeIdx + 1 > _matchedNodes.Count) return;
            _matchedNodeIdx++;
            if (_matchedNodeIdx == _matchedNodes.Count)
                _matchedNodeIdx = 0;
            SelectNode(_matchedNodes[_matchedNodeIdx]);
        }

        protected void SelectNode(TableNodesBase node)
        {
            DataGrid.SelectedValue = node;
            var row = (DataGridRow)DataGrid.ItemContainerGenerator.ContainerFromItem(node);
            if (row != null)
            {
                row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            DataGrid.ScrollIntoView(DataGrid.SelectedValue);
        }

        List<IPageElement> _pageItems;
        List<int> _pageOffsets;

        public void Preprocess(PrintingPaginator paginator)
        {
            var ps = GetNewPrintSetting(paginator.PrintSetting, ColumnVisibility);
            //remove 2 pixel for table border form both sides
            var pageHeigth = PageElemnt.toMM(paginator.PageSize.Height - 2)
                                - paginator.PrintSetting.Margins.Top - paginator.PrintSetting.Margins.Bottom;
            if (pageHeigth <= 0)
                return;

            _pageItems = new List<IPageElement>();
            _pageOffsets = new List<int>();
            _pageOffsets.Add(0);
            _pageItems.Add(GetNewHeder(TableInfoContext, ps, PageElemnt.HeaderType.FirstHeader));
            var currentHeigth = _pageItems.Last().GetSize();
            if (currentHeigth > pageHeigth)
            {
                PageCount = 0;
                return;
            }
            PageCount = 1;
            var pageElementOffset = 1;
            List<TableNodesBase> sortedNode;
            sortedNode = CanSort() ? (from n in Nodes orderby n.NumberWithPrefixForSorting ascending select n).ToList() : Nodes.ToList();
            for (var i = 0; i < sortedNode.Count; i++)
            {
                if (paginator.PrintSetting.Polozka == PrintSetting.Polozky.Oznacene && !sortedNode[i].Selected)
                    continue;
                var rpe = GetNewRow(sortedNode[i], ps);
                currentHeigth += rpe.GetSize();
                if (currentHeigth > pageHeigth)
                {
                    _pageItems.Add(GetNewFooter(ps));
                    pageElementOffset++;
                    _pageOffsets.Add(pageElementOffset);
                    //begin new page
                    _pageItems.Add(GetNewHeder(TableInfoContext as CoordinateListInfoDialogContext, ps,
                                               PageElemnt.HeaderType.Header));
                    pageElementOffset++;
                    currentHeigth = _pageItems.Last().GetSize();
                    PageCount++;
                    i--;
                    continue;
                }
                _pageItems.Add(rpe);
                pageElementOffset++;
            }
            _pageItems.Add(GetNewFooter(ps));
            pageElementOffset++;
            _pageOffsets.Add(pageElementOffset);
        }

        public int PageCount { get; set; }

        public UserControl GetPage(int pageNumber)
        {
            return new PageElementBase(_pageItems, _pageOffsets[pageNumber], _pageOffsets[pageNumber + 1]);
        }

        public DocumentPage GetDocumentPage(int pageNumber)
        {
            return null;
        }

        public virtual TablePrintSettingBase GetNewPrintSetting(PrintSetting printSetting,
                                                                ColumnVisibility columnVisibility)
        {
            Debug.Assert(false);
            return null;
        }

        public virtual IPageElement GetNewHeder(TableInfoContext seznamSouradnicInfo, TablePrintSettingBase ps,
                                                PageElemnt.HeaderType headerType)
        {
            Debug.Assert(false);
            return null;
        }

        public virtual IPageElement GetNewRow(TableNodesBase node, TablePrintSettingBase ps)
        {
            Debug.Assert(false);
            return null;
        }

        public virtual IPageElement GetNewFooter(TablePrintSettingBase ps)
        {
            return new FooterElement();
        }

        readonly UndoRedo _undoRedo;
        int _undoRedoCounter;

        public void BeginUndo()
        {
            if (_undoRedoCounter == 0)
                _undoRedo.BeginUndo();
            _undoRedoCounter++;
        }

        public void AddDeletedItem(TableNodesBase node)
        {
            _undoRedo.AddDeletedItem(node);
        }

        public void AddAddedItem(TableNodesBase node)
        {
            _undoRedo.AddAddedItem(node);
        }

        public void AddEditedNode(TableNodesBase node)
        {
            _undoRedo.AddEditedNode(node);
        }

        public void EndUndo()
        {
            _undoRedoCounter--;
            if (_undoRedoCounter == 0)
                _undoRedo.EndUndo();
        }

        public TableNodesBase GetNode(string uplneCislo)
        {
            var res = (from n in Nodes where uplneCislo == n.NumberWithPrefix select n).ToList();
            return !res.Any() ? null : res[0];
        }
    }
}