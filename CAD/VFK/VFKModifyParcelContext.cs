using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using CAD.Utils;
using VFK.GUI;
using VFK.Tables;

namespace VFK
{
    [Serializable]
    public class VFKModifyParcelContext : ISerializable
    {
                public VFKModifyParcelContext()
        {

        }
        public VFKModifyParcelContext(IEditParcel aEditParcel)
        {
            ParcelNode = new ObservableCollection<EditedParcelNode>();
            IEditParcel = aEditParcel;
        }
                        public ObservableCollection<EditedParcelNode> ParcelNode
        {
            get;
            set;
        }

        private IEditParcel _IEditParcel;
        public IEditParcel IEditParcel
        {
            get
            {
                return _IEditParcel;
            }
            set
            {
                _IEditParcel = value;
                if (ParcelNode != null)
                {
                    foreach (var parcelNode in ParcelNode)
                    {
                        parcelNode.IEditParcel = value;
                    }
                }

            }
        }
                        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("ParcelNodesCount", ParcelNode.Count, typeof(int));
            for (int i = 0; i < ParcelNode.Count; i++)
            {
                info.AddValue("ParcelNode" + i.ToString(), ParcelNode[i], typeof(EditedParcelNode));
            }
        }
        public VFKModifyParcelContext(SerializationInfo info, StreamingContext ctxt)
        {
            ParcelNode = new ObservableCollection<EditedParcelNode>();
            int count = info.GetInt32("ParcelNodesCount");
            for (int i = 0; i < count; i++)
            {
                EditedParcelNode node =
                    info.GetValue("ParcelNode" + i.ToString(), typeof(EditedParcelNode)) as EditedParcelNode;
                ParcelNode.Add(node);
            }
        }
            }

    [Serializable]
    public class EditedParcelNode : INotifyPropertyChanged, ISerializable
    {
                public EditedParcelNode()
        {

        }
        public EditedParcelNode(IEditParcel aEditParcel, VFKPARTableItem par)
        {
            PAR = par;
            IEditParcel = aEditParcel;
            BDPA = new ObservableCollection<BDPANode>();
            foreach (var s in aEditParcel.getExistBDPAItems(PAR))
            {
                BDPA.Add(new BDPANode(s));
            }
        }
                        public IEditParcel IEditParcel
        {
            get;
            set;
        }
        public enum ParcelModificationEnum
        {
            New = 1,
            Modify = 2,
            Cancel = 3
        }
        public VFKPARTableItem PAR
        {
            get;
            set;
        }
        public ParcelModificationEnum ParcelModification
        {
            get;
            set;
        }
        public bool IsEditAble
        {
            get
            {
                if (PAR == null) return false;
                return ParcelModification == ParcelModificationEnum.New ||
                       ParcelModification == ParcelModificationEnum.Modify;
            }
        }
        public string KatastralniUzemi
        {
            get
            {
                if (PAR == null) return string.Empty;
                return PAR.KATUZE_KOD.ToString();
            }
            set
            {

            }
        }
        public string CisloParcely
        {
            get
            {
                if (PAR == null) return string.Empty;
                if (PAR.PODDELENI_CISLA_PAR == UInt32.MaxValue)
                    return PAR.PAR_TYPE.Substring(1) + " " + PAR.KMENOVE_CISLO_PAR.ToString();
                return PAR.PAR_TYPE.Substring(1) + " " + PAR.KMENOVE_CISLO_PAR.ToString() + "/" + PAR.PODDELENI_CISLA_PAR.ToString();
            }
            set
            {

            }
        }
        public UInt32 Vymera
        {
            get
            {
                if (PAR == null) return 0;
                return PAR.VYMERA_PARCELY;
            }
            set
            {
                if (PAR == null) return;
                PAR.VYMERA_PARCELY = value;
            }
        }
        public UInt32 FSU
        {
            get
            {
                if (PAR == null) return 0;
                return PAR.KATUZE_KOD;
            }
            set
            {
                if (PAR == null) return;
                PAR.KATUZE_KOD = value;
            }
        }
        public List<LandTypeNode> LandTypeNodes
        {
            get
            {
                return Singletons.LandTypes.LandTypeNodes;
            }
        }
        public LandTypeNode LandType
        {
            get
            {
                if (PAR == null) return null;
                return Singletons.LandTypes.LandTypeNodes.Find((type) => type.KOD == PAR.DRUPOZ_KOD);
            }
            set
            {
                if (PAR == null || value == null) return;
                PAR.DRUPOZ_KOD = value.KOD;
                OnPropertyChanged("LandUseNodes");
                OnPropertyChanged("LandUse");
            }
        }
        public List<LandUseNode> LandUseNodes
        {
            get
            {
                if (PAR == null) return null;
                var landTyp = Singletons.LandTypes.LandTypeNodes.Find((type) => type.KOD == PAR.DRUPOZ_KOD);
                return landTyp.LandUseNode;
            }
            set
            {

            }
        }
        public LandUseNode LandUse
        {
            get
            {
                if (PAR == null) return null;
                var landTyp = Singletons.LandTypes.LandTypeNodes.Find((type) => type.KOD == PAR.DRUPOZ_KOD);
                return landTyp.LandUseNode.Find((type) => type.KOD == PAR.ZPVYPA_KOD);
            }
            set
            {
                if (PAR == null || value == null) return;
                PAR.ZPVYPA_KOD = value.KOD;
            }
        }
        public List<DeterminateeAreaTypeNode> DeterminateAreaTypes
        {
            get
            {
                return Singletons.DeterminateeAreaTypes.DeterminateeAreaNodes;
            }
            set
            {

            }
        }
        public DeterminateeAreaTypeNode DeterminateAreaType
        {
            get
            {
                if (PAR == null) return null;
                var datn =
                    Singletons.DeterminateeAreaTypes.DeterminateeAreaNodes.Find((type) => type.KOD == PAR.ZPURVY_KOD);
                return datn;
            }
            set
            {
                if (PAR == null || value == null) return;
                PAR.ZPURVY_KOD = value.KOD;
            }
        }
        public List<VFKMAPLISTableItem> MapLists
        {
            get
            {
                if (PAR == null) return null;
                return IEditParcel.GetMapListTable().Items;
            }
            set
            {

            }
        }
        public VFKMAPLISTableItem MapList
        {
            get
            {
                if (PAR == null || PAR.MAPLIS_KOD == null || PAR.MAPLIS_KOD.Length == 0) return null;
                var mapList = from n in IEditParcel.GetMapListTable().Items where n.ID == PAR.MAPLIS_KOD select n;
                if (mapList.Any())
                    return mapList.First();
                return null;
            }
            set
            {
                if (PAR == null || value == null) return;
                PAR.MAPLIS_KOD = value.ID;
            }
        }
        public string OwningNumber
        {
            get
            {
                if (PAR == null) return string.Empty;
                if (PAR.TEL_ID != null && PAR.TEL_ID.Length != 0)
                {
                    var tel = IEditParcel.GetTELItem(PAR.TEL_ID);
                    if (tel != null) return tel.CISLO_TEL;
                }
                return string.Empty;
            }
            set
            {

            }
        }

        public string HouseIsIncluded
        {
            get { return PAR.SOUCASTI; }
            set { PAR.SOUCASTI = value; }
        }

        public ObservableCollection<BDPANode> BDPA
        {
            get;
            private set;
        }

        private BDPANode _SelectedBDPA;
        public BDPANode SelectedBDPA
        {
            get
            {
                return _SelectedBDPA;
            }
            set
            {
                _SelectedBDPA = value;
                OnPropertyChanged("SelectedBDPA");
                OnPropertyChanged("CanRemoveAllBDPA");
                OnPropertyChanged("CanRemoveSelectedBDPA");
            }
        }
        public bool CanRemoveAllBDPA
        {
            get
            {
                if (!IsEditAble) return false;
                if (BDPA == null || BDPA.Count == 0) return false;
                return true;
            }
        }
        public bool CanRemoveSelectedBDPA
        {
            get
            {
                if (!IsEditAble || SelectedBDPA == null) return false;
                return true;
            }
        }
                        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
                        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("ParcelModification", ParcelModification, typeof(ParcelModificationEnum));
            info.AddValue("Parcele", PAR);
            info.AddValue("BDPACount", BDPA.Count);
            for (int i = 0; i < BDPA.Count; i++)
            {
                info.AddValue("BDPA" + i.ToString(), BDPA[i]);
            }
        }
        public EditedParcelNode(SerializationInfo info, StreamingContext ctxt)
        {
            ParcelModification = (ParcelModificationEnum)info.GetValue("ParcelModification", typeof(ParcelModificationEnum));
            PAR = info.GetValue("Parcele", typeof(VFKPARTableItem)) as VFKPARTableItem;
            int count = info.GetInt32("BDPACount");
            BDPA = new ObservableCollection<BDPANode>();
            for (int i = 0; i < count; i++)
            {
                BDPA.Add(info.GetValue("BDPA" + i.ToString(), typeof(BDPANode)) as BDPANode);
            }
        }
        
    }

    [Serializable]
    public class BDPANode : INotifyPropertyChanged, ISerializable
    {
                public VFKBDPTableItem BDPA
        {
            get;
            private set;
        }
        public BDPANode(VFKBDPTableItem bdp)
        {
            BDPA = bdp;
        }
        public string BPEJ_KOD
        {
            get
            {
                return BDPA.BPEJ_KOD;
            }
            set
            {
                BDPA.BPEJ_KOD = value;
            }
        }
        public string VYMERA
        {
            get
            {
                return BDPA.VYMERA.ToString();
            }
            set
            {
                try
                {
                    BDPA.VYMERA = Convert.ToUInt32(value);
                }
                catch (Exception)
                {
                    MessageBox.Show("Hodna vymery je spatna.");
                    throw;
                }
            }
        }
                        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("BDPA", BDPA, typeof(VFKBDPTableItem));
        }
        public BDPANode(SerializationInfo info, StreamingContext ctxt)
        {
            BDPA = info.GetValue("BDPA", typeof(VFKBDPTableItem)) as VFKBDPTableItem;
        }
                        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
            }
}
