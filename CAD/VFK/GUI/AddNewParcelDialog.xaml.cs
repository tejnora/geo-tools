using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeoBase.Gui;
using VFK.Tables;

namespace VFK.GUI
{
    public partial class AddNewParcelDialog : DialogBase
    {
                public AddNewParcelDialog ()
            :base("AddNewParcelDialog")
        {
            
        }
        public AddNewParcelDialog(IEditParcel aEP)
            : base("AddNewParcelDialog")
        {
            _editParce = aEP;
            InitializeComponent();
            NewParcels = new List<VFKPARTableItem>();
            _FSU.Text = aEP.GetDataContext().FSU.ToString();
        }

                        private IEditParcel _editParce;
        public List<VFKPARTableItem> NewParcels
        {
            get; 
            private set;
        }
                        public void OnAddNewParcel(object sender, EventArgs args)
        {
            ValidationRule val = new CAD.Validators.ParcelNumberValidator();
            if (val.Validate(_ParcelNumber.Text, null) != ValidationResult.ValidResult || _FSU.Text.Length == 0)
            {
                MessageBox.Show("Nějaké položky jsou špatně vyplněné.");
                return;
            }
            VFKPARTableItem newPar = _editParce.GetNewParcel();
            if(_parcelType.SelectedIndex==0)
            {
                newPar.DRUH_CISLOVANI_PAR = 2;//Pozemkova parcela
                newPar.PAR_TYPE = "PKN";
            }
            else if(_parcelType.SelectedIndex==1)
            {
                newPar.DRUH_CISLOVANI_PAR = 1;//stavebni parcela
                newPar.PAR_TYPE = "PKN";
            }
            else
            {
                System.Diagnostics.Debug.Assert(false);
            }
            newPar.KATUZE_KOD = Convert.ToUInt32(_FSU.Text);
            string[] numbers = _ParcelNumber.Text.Split('/');
            newPar.KMENOVE_CISLO_PAR = Convert.ToUInt32(numbers[0]);
            if(numbers.Length==2)
            {
                newPar.PODDELENI_CISLA_PAR = Convert.ToUInt32(numbers[1]);
            }

            NewParcels.Add(newPar);
        }
            }
}
