using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

using VFK;
using VFK.Tables;

namespace VFK.GUI
{
    /// <summary>
    /// Interaction logic for VFKEdidOfParcel.xaml
    /// </summary>

    public partial class VFKEdidOfParcel : Window
    {
        IVFKMain _IVFKMain = null;
        public ObservableCollection<DataGridItems> DataGridItems 
        { 
            get; 
            set; 
        }
        public VFKEdidOfParcel(IVFKMain aVFKMain)
        {
            InitializeComponent();
            _IVFKMain = aVFKMain;
        }

        public void FillDataGrid(List<VFKPARTableItem> aItems)
        {
            DataGridItems  = new ObservableCollection<DataGridItems>();
            VFKKATUZETableItem katuze=null;
            foreach (VFKPARTableItem table in aItems)
            {
                DataGridItems .Add(new DataGridItems(table));
                if (katuze==null || katuze.KOD != table.KATUZE_KOD)
                {
                    katuze = _IVFKMain.getKATUZE(table.KATUZE_KOD);
                    if(katuze!=null)
                        _mKatastralniUzemi.Items.Add(katuze.NAZEV);
                }
            }
            _mKatastralniUzemi.SelectedIndex = 0;
            _mDataGrid.DataContext = this;
        }

        private void _Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void _Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _mSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in DataGridItems)
            {
                item.Edit = true;
            }
            _mDataGrid.DataContext = null;
            _mDataGrid.DataContext = this;
        }

        private void _mCancelAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in DataGridItems)
            {
                item.Edit = false;
            }
            _mDataGrid.DataContext = null;
            _mDataGrid.DataContext = this;
        }
        public void  GetSelectedParcel(List<VFKPARTableItem> aSelectedItems)
        {
            foreach (var item in DataGridItems)
            {
                if(item.Edit)
                    aSelectedItems.Add(item.VFKPARItem);
            }
        }
        public void SetSelectedParcel(List<VFKPARTableItem> aSelectedItems)
        {
            foreach (var list in aSelectedItems)
            {
                var gridItem = from n in DataGridItems where n.VFKPARItem.ID == list.ID select n;
                gridItem.First().Edit = true;
            }
        }
    }

    public class DataGridItems
    {
        public DataGridItems(VFKPARTableItem aPointer)
        {
            if (aPointer.PODDELENI_CISLA_PAR == UInt32.MaxValue)
                CisloParcely = aPointer.PAR_TYPE.Substring(1) + " " + aPointer.KMENOVE_CISLO_PAR.ToString();
            else
                CisloParcely = aPointer.PAR_TYPE.Substring(1) + " " + aPointer.KMENOVE_CISLO_PAR.ToString() + "/" + aPointer.PODDELENI_CISLA_PAR.ToString();
            VFKPARItem = aPointer;
        }
        public bool Edit
        {
            get;
            set;
        }
        public string CisloParcely
        {
            get;
            set;
        }
        public VFKPARTableItem VFKPARItem
        {
            get; set;
        }
    }
}
