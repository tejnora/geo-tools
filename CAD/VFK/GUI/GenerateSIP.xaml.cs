using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using CAD.Utils;
using VFK.Tables;

namespace VFK.GUI
{
    /// <summary>
    /// Interaction logic for GenerovaniSIP.xaml
    /// </summary>
    public partial class GenerateSIP : Window, INotifyPropertyChanged
    {
        public VFKModifyParcelContext Context
        {
            get; private set;
        }
        public GenerateSIP(VFKModifyParcelContext aContext)
        {
            Context = aContext;
            InitializeComponent();
            _mainGrid.DataContext = this;
        }
        public void OnCancle(object sender, EventArgs args)
        {
            this.Close();
        }
        public void OnOk(object sender, EventArgs args)
        {
            DialogResult = true;
            this.Close();
        }
        public List<VFKPARTableItem> GetNotAssignedParcel()
        {
            IEditParcel ep = Context.IEditParcel;
            List<VFKPARTableItem> notAssignedParcel  = new List<VFKPARTableItem>();
            foreach (var item in ep.GetPARItems())
            {
                if((from n in Context.ParcelNode where n.PAR.ID==item.ID select n).Count()>0)
                {
                    continue;
                }
                notAssignedParcel.Add(item);
            }
            return notAssignedParcel;
        }
        public void OnAddRemoveParcel(object sender, EventArgs args)
        {
            VFKEdidOfParcel editOfParcel = new VFKEdidOfParcel(Context.IEditParcel as IVFKMain);
            editOfParcel.FillDataGrid(GetNotAssignedParcel());
            bool? result = editOfParcel.ShowDialog();
            if (result.GetValueOrDefault(false))
            {
                List<VFKPARTableItem> selectedParceAfter = new List<VFKPARTableItem>();
                editOfParcel.GetSelectedParcel(selectedParceAfter);

                foreach (var par in selectedParceAfter)
                {
                    EditedParcelNode epn = new EditedParcelNode(Context.IEditParcel, par);
                    epn.ParcelModification = EditedParcelNode.ParcelModificationEnum.Cancel;
                    Context.ParcelNode.Add(epn);  
                }
                UpdateSelectedParcel();
            }
        }
        public void OnAddModifyParcel(object sender, EventArgs args)
        {
            VFKEdidOfParcel editOfParcel = new VFKEdidOfParcel(Context.IEditParcel as IVFKMain);
            editOfParcel.FillDataGrid(GetNotAssignedParcel());
            bool? result = editOfParcel.ShowDialog();
            if (result.GetValueOrDefault(false))
            {
                List<VFKPARTableItem> selectedParceAfter = new List<VFKPARTableItem>();
                editOfParcel.GetSelectedParcel(selectedParceAfter);

                foreach (var par in selectedParceAfter)
                {
                    EditedParcelNode epn = new EditedParcelNode(Context.IEditParcel, (VFKPARTableItem)par.Clone());
                    epn.ParcelModification = EditedParcelNode.ParcelModificationEnum.Modify;
                    Context.ParcelNode.Add(epn);
                }
                UpdateSelectedParcel();
            }
        }
        public void OnRemoveParcel(object sender, EventArgs aArgs)
        {
            var sel = SelectedParcelNode;
            if(sel!=null)
            {
                SelectedParcelNode = null;
                Context.ParcelNode.Remove(sel);
                Context.IEditParcel.RestoreBdpAfterRemoveParcelFromEdit(sel.PAR.ID);
            }
            UpdateSelectedParcel();
        }
        public void OnAddNewParcel(object sender, EventArgs args)
        {
            AddNewParcelDialog dialog = new AddNewParcelDialog(Context.IEditParcel);
            dialog.Owner = this;
            dialog.ShowDialog();
            foreach (var parcel in dialog.NewParcels)
            {
                EditedParcelNode epn = new EditedParcelNode(Context.IEditParcel, parcel);
                epn.ParcelModification = EditedParcelNode.ParcelModificationEnum.New;
                Context.ParcelNode.Add(epn);
            }
            UpdateSelectedParcel();
        }
        private void UpdateSelectedParcel()
        {
            if (Context.ParcelNode.Count > 0)
                SelectedParcelNode = Context.ParcelNode[0];
            else
            {
                SelectedParcelNode = null;
            }
        }

        private EditedParcelNode _SelectedParcelNode;
        public EditedParcelNode SelectedParcelNode
        {
            get
            {
                return _SelectedParcelNode;
            }
            set
            {
                _SelectedParcelNode = value;
                OnPropertyChangedAll();
                OnPropertyChanged("VymBPEJ");
                OnPropertyChanged("VymNebon");
            }
        }
        public void onAddBDPA(object sender, EventArgs arg)
        {
            try
            {
                string kod = _BDPA_BPEJ_KOD.Text;
                UInt32 vymera = Convert.ToUInt32(_BDPA_VYMERA.Text);
                var bdp = Context.IEditParcel.GetNewBdp();
                bdp.VYMERA = vymera;
                bdp.BPEJ_KOD = kod;
                bdp.PAR_ID = SelectedParcelNode.PAR.ID;
                SelectedParcelNode.BDPA.Add(new BDPANode(bdp));
                OnPropertyChanged("VymBPEJ");
                OnPropertyChanged("VymNebon");
            }
            catch (Exception)
            {

                MessageBox.Show("Mate spatene vyplenou vymeru, nebo bpej kod.");
            }
        }
        public void OnRemoveSelectedBDPA(object sender, EventArgs args)
        {
            if(SelectedParcelNode==null || SelectedParcelNode.SelectedBDPA==null)
                return;
            Context.IEditParcel.DeleteBdp(SelectedParcelNode.SelectedBDPA.BDPA);
            SelectedParcelNode.BDPA.Remove(SelectedParcelNode.SelectedBDPA);
            OnPropertyChanged("VymBPEJ");
            OnPropertyChanged("VymNebon");
        }
        public void OnRemoveSelectedBDPAAll(object sender, EventArgs args)
        {
            foreach (var bdp in SelectedParcelNode.BDPA)
                Context.IEditParcel.DeleteBdp(bdp.BDPA);
            SelectedParcelNode.BDPA.Clear();
            OnPropertyChanged("VymBPEJ");
            OnPropertyChanged("VymNebon");
        }
        public string VymBPEJ
        {
            get
            {
                if (SelectedParcelNode == null) return string.Empty;
                UInt64 vym = 0;
                foreach (var bdp in SelectedParcelNode.BDPA)
                {
                    vym += bdp.BDPA.VYMERA;
                }
                return vym.ToString();
            }
        }
        public string VymNebon
        {
            get
            {
                if (SelectedParcelNode == null) return string.Empty;
                UInt64 vym = SelectedParcelNode.PAR.VYMERA_PARCELY;
                foreach (var bdp in SelectedParcelNode.BDPA)
                {
                    vym -= bdp.BDPA.VYMERA;
                }
                return vym.ToString();
            }
        }
        #region INotifyPropertyChanged
        private void OnPropertyChangedAll()
        {
            OnPropertyChanged("SelectedParcelNode");
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        #endregion
    }


    [ValueConversion(typeof(EditedParcelNode.ParcelModificationEnum), typeof(String))]
    public class ParcelModificationEnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            EditedParcelNode.ParcelModificationEnum pm = (EditedParcelNode.ParcelModificationEnum)value;
            switch (pm)
            {
                case EditedParcelNode.ParcelModificationEnum.New:
                    return "N";
                case EditedParcelNode.ParcelModificationEnum.Modify:
                    return "A";
                case EditedParcelNode.ParcelModificationEnum.Cancel:
                    return "R";
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break; ;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            String pm = (String)value;
            if (pm == "N") return EditedParcelNode.ParcelModificationEnum.New;
            if (pm == "A") return EditedParcelNode.ParcelModificationEnum.Cancel;
            return EditedParcelNode.ParcelModificationEnum.Cancel;
        }
    }
}
