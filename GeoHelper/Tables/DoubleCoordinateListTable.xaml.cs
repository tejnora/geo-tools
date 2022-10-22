using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GeoHelper.Converters;
using GeoHelper.Printing;
using GeoHelper.Tables.AdditionalGui;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Tables.TablePrintingElements;

namespace GeoHelper.Tables
{
    public partial class DoubleCoordinateListTable
    {
        public static string TableName = "DoubleCoordinateListTable";

        public DoubleCoordinateListTable(IMainWindow mainWindow)
            : base(mainWindow)
        {
            InitializeComponent();
            _dataGrid.DataContext = this;
            InitTableBase();
            TableInfoContext = new CoordinateListInfoDialogContext();
        }

        public DoubleCoordinateListTable(IMainWindow mainWindow, List<TableNodesBase> nodeBase, bool needConvert)
            : this(mainWindow)
        {
            if (needConvert)
            {
                foreach (var node in nodeBase)
                {
                    var newNode = new TableDoubleCoordinateListNode();
                    var nodeSS = (TableCoordinateListNode)node;
                    newNode.AssignNode(nodeSS);
                    newNode.SpolX = nodeSS.X;
                    newNode.SpolY = nodeSS.Y;
                    if (nodeSS.Quality > 4)
                    {
                        newNode.SpolQuality = 0;
                    }
                    else
                    {
                        newNode.Quality = 0;
                        newNode.SpolecnaKvalita = nodeSS.Quality;
                    }
                    Nodes.Add(newNode);
                }
            }
            else
                Nodes = new ObservableCollection<TableNodesBase>(nodeBase);
        }

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
            get { return "Seznam souřadnic dvojí(*.sdv)|*.sdv"; }
        }

        public override TableNodesBase GetNewNode()
        {
            return new TableDoubleCoordinateListNode();
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
            var newNode = new TableDoubleCoordinateListNode();
            var dlg = new DoubleCoordinateEditDialog(newNode);
            if (!dlg.ShowDialog().GetValueOrDefault(false)) return;
            InsertItem(newNode, -1, null);
            MarkModification();
        }

        public override void OnEdit()
        {
            var sel = _dataGrid.SelectedItem as TableDoubleCoordinateListNode;
            if (sel == null) return;
            var dlg = new DoubleCoordinateEditDialog((TableDoubleCoordinateListNode)sel.Clone());
            if (!dlg.ShowDialog().GetValueOrDefault(false)) return;
            using (var action = new Action(this))
            {
                action.AddEditedNode(sel);
                sel.AssignNode((TableDoubleCoordinateListNode)dlg.DataContext);
            }
            MarkModification();
        }

        public override void OnShowInfoDialog()
        {
            var context = (CoordinateListInfoDialogContext)TableInfoContext.Clone();
            var dlg = new CoordinateListInfoDialog(context);
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                TableInfoContext = dlg.Context;
            }
        }

        public override bool GetHasSouborInfo()
        {
            return true;
        }

        public override void FillMenuItems(MenuItem menu)
        {
            base.FillMenuItems(menu);
            menu.Items.Add(new Separator());
            MenuItem menuItem;
            menuItem = new MenuItem { Header = _dictionary.Translate<string>("TwoCoordinateListTable.9", "Text") };
            menuItem.Click += OnShowTableSettingDialog;
            menu.Items.Add(menuItem);
        }

        public override string GetMenuCaption()
        {
            return _dictionary.Translate<string>("TwoCoordinateListTable.10", "Text");
        }

        public void OnShowTableSettingDialog(object sender, EventArgs arts)
        {
            var dlg = new CoordinateTableSttingsDialog(this);
            if (!dlg.ShowDialog().GetValueOrDefault(false)) return;
            ColumnVisibility = dlg.ColumnVisibility;
            UpdateTableColumnView();
            MarkModification();
        }

        public override TablePrintSettingBase GetNewPrintSetting(PrintSetting printSetting, ColumnVisibility columnVisibility)
        {
            return new TableDoubleCoordinatePrintSetting(printSetting, columnVisibility);
        }

        public override IPageElement GetNewHeder(TableInfoContext seznamSouradnicInfo, TablePrintSettingBase ps, PageElemnt.HeaderType headerType)
        {
            return new DoubleCoordinateHeaderPageElemnt((CoordinateListInfoDialogContext)seznamSouradnicInfo, (TableDoubleCoordinatePrintSetting)ps, headerType);
        }

        public override IPageElement GetNewRow(TableNodesBase node, TablePrintSettingBase ps)
        {
            return new DobleCoordinateRowPageElemnt((TableDoubleCoordinateListNode)node, (TableDoubleCoordinatePrintSetting)ps);
        }

        protected override void UpdateTableColumnView()
        {
            if (GetIsSet(ColumnVisibility.Number) && GetIsSet(ColumnVisibility.Prefix))
            {
                var b = new Binding("NumberWithPrefix") {Converter = new CoordinateNumberConverter()};
                _numberWithPrefixColumn.Binding = b;
                _numberWithPrefixColumn.Visibility = Visibility.Visible;
            }
            else if (GetIsSet(ColumnVisibility.Number))
            {
                _numberWithPrefixColumn.Binding = new Binding("Number");
                _numberWithPrefixColumn.Visibility = Visibility.Visible;
            }
            else if (GetIsSet(ColumnVisibility.Prefix))
            {
                _numberWithPrefixColumn.Binding = new Binding("Prefix");
                _numberWithPrefixColumn.Visibility = Visibility.Visible;
            }
            else
            {
                _numberWithPrefixColumn.Visibility = Visibility.Collapsed;
            }
            if (_numberWithPrefixColumn.Visibility == Visibility.Visible)
            {
                var b = new Binding("ActualWidth");
                b.ElementName = "_numberWithPrefixColumn";
                BindingOperations.SetBinding(_uplneCisloColumnHeader, ColumnDefinition.WidthProperty, b);
            }
            else
                _uplneCisloColumnHeader.Width = new GridLength(0);
            SetColumnVisibilityWithHeader(ColumnVisibility.CoordinateX, _sobrXColumn, _sobrXColumnHeader, "_sobrXColumn");
            SetColumnVisibilityWithHeader(ColumnVisibility.CoordinateY, _sobrYColumn, _sobrYColumnHeader, "_sobrYColumn");
            SetColumnVisibilityWithHeader(ColumnVisibility.Quality, _sobrQualityCodeColumn,
                                          _sobrKodKvalitaColumnHeader, "_sobrQualityCodeColumn");
            SetColumnVisibilityWithHeader(ColumnVisibility.SpolX, _spolXcolumn, _spolXcolumnHeader, "_spolXcolumn");
            SetColumnVisibilityWithHeader(ColumnVisibility.SpolY, _spolYcolumn, _spolYcolumnHeader, "_spolYcolumn");
            SetColumnVisibilityWithHeader(ColumnVisibility.SpolQuality, _spolQualityCodeColumn,
                                          _spolKodKvalitaColumnnHeader, "_spolQualityCodeColumn");
        }

        void SetColumnVisibilityWithHeader(ColumnVisibility columnName, DataGridColumn control, ColumnDefinition column,
                                           string elementName)
        {
            SetColumnVisibility(columnName, control);
            if (GetIsSet(columnName))
            {
                if (!BindingOperations.IsDataBound(column, ColumnDefinition.WidthProperty))
                {
                    var b = new Binding("ActualWidth") {ElementName = elementName};
                    BindingOperations.SetBinding(column, ColumnDefinition.WidthProperty, b);
                }
            }
            else
            {
                column.Width = new GridLength(0);
            }
        }
    }
}