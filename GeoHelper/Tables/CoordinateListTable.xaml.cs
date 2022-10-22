using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GeoCalculations.MethodPoints;
using GeoHelper.Converters;
using GeoHelper.FileParses;
using GeoHelper.Plugins;
using GeoHelper.Printing;
using GeoHelper.Protocols;
using GeoHelper.Tables.AdditionalGui;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Tables.TablePrintingElements;

namespace GeoHelper.Tables
{
    public partial class CoordinateListTable
    {
        public static string TableName = "CoordinateListTable";

        public CoordinateListTable(IMainWindow mainWindow)
            : base(mainWindow)
        {
            InitializeComponent();
            InitTableBase();
            _owner = mainWindow;
            _dataGrid.DataContext = this;
            TableInfoContext = new CoordinateListInfoDialogContext();
        }

        public CoordinateListTable(List<TableNodesBase> aNodes, IMainWindow aMainWindow)
            : this(aMainWindow)
        {
            Nodes = new ObservableCollection<TableNodesBase>(aNodes);
        }

        public CoordinateListTable(IMainWindow aMainWindow, List<IRecord> records)
            : this(aMainWindow)
        {
            CreateNode(records);
        }

        readonly IMainWindow _owner;

        protected override DataGrid DataGrid
        {
            get { return _dataGrid; }
        }

        protected override string GetTableName()
        {
            return TableName;
        }

        protected override bool CanSort()
        {
            return true;
        }

        public override string SaveDialogFilter
        {
            get { return _dictionary.Translate<string>("CoordinateListTable.6", "Text"); }
        }

        public override TableNodesBase GetNewNode()
        {
            return new TableCoordinateListNode();
        }

        public override bool CanEdit()
        {
            return true;
        }

        public override bool CanInsert()
        {
            return true;
        }

        public override void OnInsert()
        {
            var newNode = new TableCoordinateListNode();
            var dlg = new CoordinateListEditDialog(newNode);
            if (!dlg.ShowDialog().GetValueOrDefault(false)) return;
            InsertItem(newNode, -1, null);
            MarkModification();
        }

        public override void OnEdit()
        {
            var sel = _dataGrid.SelectedItem as TableCoordinateListNode;
            if (sel == null) return;
            var dlg = new CoordinateListEditDialog((TableCoordinateListNode)sel.Clone());
            if (!dlg.ShowDialog().GetValueOrDefault(false)) return;
            using (var action = new Action(this))
            {
                action.AddEditedNode(sel);
                sel.AssignNode((TableCoordinateListNode)dlg.DataContext);
                MarkModification();
            }
        }

        public override bool AddNewNode(CalculatedPointBase newPoint, IProtocolContext protocolContext)
        {
            InsertItem(new TableCoordinateListNode(newPoint), -1, protocolContext);
            return true;
        }

        public override void OnShowInfoDialog()
        {
            var context = (CoordinateListInfoDialogContext)TableInfoContext.Clone();
            var dlg = new CoordinateListInfoDialog(context);
            if (!dlg.ShowDialog().GetValueOrDefault(false)) return;
            TableInfoContext = dlg.Context;
            MarkModification();
        }

        public override bool GetHasSouborInfo()
        {
            return true;
        }

        public override void InsertItem(TableNodesBase node, int index, IProtocolContext protocolContext)
        {
            using (var action = new Action(this))
            {
                var result = from s in Nodes where s.Number == node.Number && s.Prefix == node.Prefix select s;
                if (result.Any())
                {
                    var dlg = new ExistingPointInCoordinatesDialog((TableCoordinateListNode)result.First(), (TableCoordinateListNode)node, this, protocolContext);
                    dlg.ShowDialog();
                }
                else
                    InsertItemRaw(node, -1);
            }
        }

        public override TablePrintSettingBase GetNewPrintSetting(PrintSetting printSetting, ColumnVisibility columnVisibility)
        {
            return new TableCoordinatePrintSetting(printSetting, columnVisibility);
        }

        public override IPageElement GetNewHeder(TableInfoContext seznamSouradnicInfo, TablePrintSettingBase ps, PageElemnt.HeaderType headerType)
        {
            return new CoordinateHeaderPageElemnt((CoordinateListInfoDialogContext)seznamSouradnicInfo, (TableCoordinatePrintSetting)ps, headerType);
        }

        public override IPageElement GetNewRow(TableNodesBase node, TablePrintSettingBase ps)
        {
            return new CoordinateRowPageElemnt((TableCoordinateListNode)node, (TableCoordinatePrintSetting)ps);
        }

        public override void FillMenuItems(MenuItem menu)
        {
            base.FillMenuItems(menu);
            menu.Items.Add(new Separator());
            var menuItem = new MenuItem { Header = _dictionary.Translate<string>("CoordinateListTable.7", "Text") };
            menuItem.Click += OnShowMassChangesDialog;
            menu.Items.Add(menuItem);
            menuItem = new MenuItem { Header = _dictionary.Translate<string>("CoordinateListTable.8", "Text") };
            menuItem.Click += OnCreateDoubleCoordinates;
            menu.Items.Add(menuItem);
            menu.Items.Add(new Separator());
            menuItem = new MenuItem { Header = _dictionary.Translate<string>("CoordinateListTable.9", "Text") };
            menuItem.Click += OnShowSettingsDialog;
            menu.Items.Add(menuItem);
            menuItem = new MenuItem { Header = _dictionary.Translate<string>("CoordinateListTable.10", "Text") };
            menuItem.Click += OnSendToMicrostation;
            menu.Items.Add(menuItem);
        }

        public override string GetMenuCaption()
        {
            return _dictionary.Translate<string>("CoordinateListTable.11", "Text");
        }

        public void OnShowMassChangesDialog(object sender, EventArgs args)
        {
            new MassChangeCoordinateDialog(this).Show();
        }

        public void OnCreateDoubleCoordinates(object sender, EventArgs args)
        {
            var sel = from n in Nodes where n.Selected select n;
            if (sel.Any())
            {
                _owner.CreateWindowWithDoubleCoordinates(sel.ToList());
            }
            else
                _owner.CreateWindowWithDoubleCoordinates(new List<TableNodesBase>(Nodes));
        }

        public void OnShowSettingsDialog(object sender, EventArgs arts)
        {
            var dlg = new CoordinateTableSttingsDialog(this);
            if (!dlg.ShowDialog().GetValueOrDefault(false)) return;
            ColumnVisibility = dlg.ColumnVisibility;
            UpdateTableColumnView();
            MarkModification();
        }

        public void OnSendToMicrostation(object sender, EventArgs args)
        {
            var selectedItems = (from n in Nodes where n.Selected select n).ToList();
            var plugin = new MicrostationPlugin(selectedItems);
            plugin.Send();
        }

        protected override void UpdateTableColumnView()
        {
            if (GetIsSet(ColumnVisibility.Number) && GetIsSet(ColumnVisibility.Prefix))
            {
                var b = new Binding("NumberWithPrefix") { Converter = new CoordinateNumberConverter() };
                _tableRowCislo.Binding = b;
                _tableRowCislo.Visibility = Visibility.Visible;
            }
            else if (GetIsSet(ColumnVisibility.Number))
            {
                _tableRowCislo.Binding = new Binding("Number");
                _tableRowCislo.Visibility = Visibility.Visible;
            }
            else if (GetIsSet(ColumnVisibility.Prefix))
            {
                _tableRowCislo.Binding = new Binding("Prefix");
                _tableRowCislo.Visibility = Visibility.Visible;
            }
            else
            {
                _tableRowCislo.Visibility = Visibility.Collapsed;
            }
            SetColumnVisibility(ColumnVisibility.CoordinateX, _tableRowSouradniceX);
            SetColumnVisibility(ColumnVisibility.CoordinateY, _tableRowSouradniceY);
            SetColumnVisibility(ColumnVisibility.CoordinateY, _tableRowSouradniceZ);
            SetColumnVisibility(ColumnVisibility.Quality, _tableRowKodKvalita);
            SetColumnVisibility(ColumnVisibility.Description, _tableRowPopis);
        }

        void CreateNode(IEnumerable<IRecord> records)
        {
            foreach (var record in records.OfType<ISouradnice>())
            {
                Nodes.Add(new TableCoordinateListNode(record));
            }
        }
    }
}