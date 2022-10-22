using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GeoHelper.Converters;
using GeoHelper.FileParses;
using GeoHelper.Printing;
using GeoHelper.Tables.AdditionalGui;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Tables.TablePrintingElements;

namespace GeoHelper.Tables
{
    public partial class MeasureListTable
    {
        public static string TableName = "MeasureListTable";

        public MeasureListTable(IMainWindow mainWindow, IEnumerable<IRecord> records)
            : base(mainWindow)
        {
            InitializeComponent();
            CreateNode(records);
            DataContext = this;
            InitTableBase();
            TableInfoContext = new SeznamMereniInfoDialogContext();
        }

        public MeasureListTable(IMainWindow mainWindow)
            : base(mainWindow)
        {
            InitializeComponent();
            DataContext = this;
            InitTableBase();
            TableInfoContext = new SeznamMereniInfoDialogContext();
        }

        public MeasureListTable(IMainWindow mainWindow, List<TableNodesBase> aNodes)
            : this(mainWindow)
        {
            Nodes = new ObservableCollection<TableNodesBase>(aNodes);
            TableInfoContext = new SeznamMereniInfoDialogContext();
        }

        protected override DataGrid DataGrid
        {
            get { return _dataGrid; }
            set { }
        }

        protected override string GetTableName()
        {
            return TableName;
        }

        protected override bool CanSort()
        {
            return false;
        }

        public override string SaveDialogFilter
        {
            get { return _dictionary.Translate<string>("MeasureListTable.7", "Text"); }
        }

        public override TableNodesBase GetNewNode()
        {
            return new TableMeasureListNode();
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
            var newNode = new TableMeasureListNode();
            var dlg = new MeasureListEditDialog(newNode);
            if (!dlg.ShowDialog().GetValueOrDefault(false)) return;
            if (DataGrid.SelectedItems.Count > 0)
            {
                int index;
                if (DataGrid.SelectedItems[0] is TableNodesBase)
                    index = Nodes.IndexOf((TableNodesBase)DataGrid.SelectedItems[0]);
                else
                    index = Nodes.Count;
                InsertItemRaw(newNode, index);
            }
            else
                InsertItemRaw(newNode, -1);
        }

        public override void OnEdit()
        {
            var sel = _dataGrid.SelectedItem as TableMeasureListNode;
            if (sel == null) return;
            var dlg = new MeasureListEditDialog((TableMeasureListNode)sel.Clone());
            if (!dlg.ShowDialog().GetValueOrDefault(false)) return;
            using (var act = new Action(this))
            {
                act.AddEditedNode(sel);
                sel.AssignNode((TableMeasureListNode)dlg.DataContext);
                MarkModification();
            }
        }

        public override bool GetHasSouborInfo()
        {
            return true;
        }

        public override void OnShowInfoDialog()
        {
            var context = (SeznamMereniInfoDialogContext)TableInfoContext.Clone();
            var dlg = new MeasureListInfoDialog(context);
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                TableInfoContext = dlg.Context;
            }
        }

        void CreateNode(IEnumerable<IRecord> records)
        {
            foreach (var record in records)
            {
                if (record is IStationRecord)
                    Nodes.Add(new TableMeasureListNode(record as IStationRecord));
                else if (record is INodeSideshot)
                    Nodes.Add(new TableMeasureListNode(record as INodeSideshot));
            }
        }

        public override void FillMenuItems(MenuItem menu)
        {
            base.FillMenuItems(menu);
            menu.Items.Add(new Separator());
            var menuItem = new MenuItem { Header = _dictionary.TranslateText("MeasureListTable.8") };
            menuItem.Click += OnShowMessChangeDialog;
            menu.Items.Add(menuItem);
            menu.Items.Add(new Separator());
            menuItem = new MenuItem { Header = _dictionary.TranslateText("MeasureListTable.9") };
            menuItem.Click += OnShowMeasureSettingDialog;
            menu.Items.Add(menuItem);
        }

        public override string GetMenuCaption()
        {
            return _dictionary.TranslateText("MeasureListTable.10");
        }

        public void OnShowMessChangeDialog(object sender, EventArgs args)
        {
            var dlg = new MassChangeMeasureDialog(this);
            dlg.Show();
        }

        public void OnShowMeasureSettingDialog(object sender, EventArgs arts)
        {
            var dlg = new MeasureTableSettingsDialog(this);
            if (!dlg.ShowDialog().GetValueOrDefault(false)) return;
            ColumnVisibility = dlg.ColumnVisibility;
            UpdateTableColumnView();
            MarkModification();
        }

        public override TablePrintSettingBase GetNewPrintSetting(PrintSetting printSetting, ColumnVisibility columnVisibility)
        {
            return new TableMeasurePrintSetting(printSetting, columnVisibility);
        }

        public override IPageElement GetNewHeder(TableInfoContext seznamSouradnicInfo, TablePrintSettingBase ps, PageElemnt.HeaderType headerType)
        {
            return new MeasureHeaderPageElemnt((SeznamMereniInfoDialogContext)seznamSouradnicInfo, (TableMeasurePrintSetting)ps, headerType);
        }

        public override IPageElement GetNewRow(TableNodesBase node, TablePrintSettingBase ps)
        {
            return new MeasureRowPageElemnt((TableMeasureListNode)node, (TableMeasurePrintSetting)ps);
        }

        public override IPageElement GetNewFooter(TablePrintSettingBase ps)
        {
            return new MeasureFooterElement();
        }

        protected override void UpdateTableColumnView()
        {
            if (GetIsSet(ColumnVisibility.Number) && GetIsSet(ColumnVisibility.Prefix))
            {
                var b = new Binding("NumberWithPrefix") { Converter = new CoordinateNumberConverter() };
                _tableColumnNumber.Binding = b;
                _tableColumnNumber.Visibility = Visibility.Visible;
            }
            else if (GetIsSet(ColumnVisibility.Number))
            {
                _tableColumnNumber.Binding = new Binding("Number");
                _tableColumnNumber.Visibility = Visibility.Visible;
            }
            else if (GetIsSet(ColumnVisibility.Prefix))
            {
                _tableColumnNumber.Binding = new Binding("Prefix");
                _tableColumnNumber.Visibility = Visibility.Visible;
            }
            else
            {
                _tableColumnNumber.Visibility = Visibility.Collapsed;
            }
            SetColumnVisibility(ColumnVisibility.Hz, _tableColumnHz);
            SetColumnVisibility(ColumnVisibility.ZenitAngle, _tableColumnZenitAgnle);
            SetColumnVisibility(ColumnVisibility.HorizontalDistance, _tableColumnHorizontalLength);
            SetColumnVisibility(ColumnVisibility.dH, _tableColumnElevationDifferce);
            SetColumnVisibility(ColumnVisibility.Signal, _tableColumnSignal);
            SetColumnVisibility(ColumnVisibility.Description, _tableColumnPrefix);
        }
    }
}