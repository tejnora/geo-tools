using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeoBase.Utils;
using GeoHelper.Tables;
using GeoHelper.Tables.TableNodes;

namespace GeoHelper.Utils
{
    public class DataGridDragDropManager<ItemType> where ItemType : class
    {
        public DataGridDragDropManager(DataGrid dataGrid, UIElement aPlacemnt, string dataName)
        {
            DataGrid = dataGrid;
            DataName = dataName;
            OwnerControl = aPlacemnt;
            _backupSelectionMode = _dataGrid.SelectionMode;
        }

        readonly DataGridSelectionMode _backupSelectionMode;
        DataGrid _dataGrid;
        UIElement OwnerControl { get; set; }

        string DataName { get; set; }

        public DataGrid DataGrid
        {
            get { return _dataGrid; }
            set
            {
                if (_dataGrid != null)
                {
                    _dataGrid.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                    _dataGrid.MouseLeftButtonUp -= OnMouseLeftButtonUp;
                    _dataGrid.MouseMove -= OnMouseMove;
                }
                _dataGrid = value;
                _dataGrid.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                _dataGrid.MouseLeftButtonUp += OnMouseLeftButtonUp;
                _dataGrid.MouseMove += OnMouseMove;
            }
        }

        bool IsDragging { get; set; }

        void ResetDragDrop()
        {
            IsDragging = false;
            _dataGrid.SelectionMode = _backupSelectionMode;
        }

        public ItemType DraggedItem { get; set; }

        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("OnMousebutton");
            var row = UIHelpers.TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition(_dataGrid));
            if (row == null || row.IsEditing) return;
            IsDragging = true;
            if (row.Item is ItemType)
                DraggedItem = (ItemType)row.Item;
        }

        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDragging || e.LeftButton != MouseButtonState.Pressed) return;
            PerformDragOperation();
        }

        void PerformDragOperation()
        {
            _dataGrid.SelectionMode = DataGridSelectionMode.Single;
            var selectedItem = _dataGrid.SelectedItem as ItemType;
            if (selectedItem == null) return;
            var allowedEffects = DragDropEffects.All;
            var data = new DataObject(DataName, new DragDropDataObjectBase(OwnerControl, selectedItem));
            IsDragging = true;
            if (DragDrop.DoDragDrop(_dataGrid, data, allowedEffects) != DragDropEffects.None)
            {
                _dataGrid.SelectedItem = selectedItem;
            }
            ResetDragDrop();
        }
    }

    public class DragDropDataObjectBase
    {
        public DragDropDataObjectBase(object mdiWindow, object data)
        {
            MdiWindow = mdiWindow;
            Data = data;
        }

        public object MdiWindow { get; set; }
        public object Data { get; set; }
    }

    public static class DragEventArgsEx
    {
        static public bool IsCoordinateListTable(this DragEventArgs args)
        {
            return args.Data.GetDataPresent(CoordinateListTable.TableName) && args.Data != null;
        }

        static public bool IsMeasureListTable(this DragEventArgs args)
        {
            return args.Data.GetDataPresent(MeasureListTable.TableName) && args.Data != null;
        }

        static public void Accept(this DragEventArgs args)
        {
            args.Effects = DragDropEffects.Copy;
            args.Handled = true;
        }

        static public TNode TableNode<TNode>(this DragEventArgs args) where TNode : class
        {
            DragDropDataObjectBase data;
            if (IsCoordinateListTable(args))
            {
                data = (DragDropDataObjectBase)args.Data.GetData(CoordinateListTable.TableName);
            }
            else if (IsMeasureListTable(args))
            {
                data = (DragDropDataObjectBase)args.Data.GetData(MeasureListTable.TableName);
            }
            else
                return null;
            return data.Data as TNode;
        }

        static public TTable Table<TTable>(this DragEventArgs args) where TTable : TableBase
        {
            DragDropDataObjectBase data;
            if (IsCoordinateListTable(args))
            {
                data = (DragDropDataObjectBase)args.Data.GetData(CoordinateListTable.TableName);
            }
            else if (IsMeasureListTable(args))
            {
                data = (DragDropDataObjectBase)args.Data.GetData(MeasureListTable.TableName);
            }
            else
                return null;
            return data.MdiWindow as TTable;

        }
    }

}