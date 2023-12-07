using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;
using CAD.Canvas;
using CAD.VFK;
using CAD.VFK.DrawTools;
using CAD.VFK.GUI;
using GeoBase.Localization;
using GeoBase.Utils;
using VFK.Tables;
using CAD.Utils;
using VFK.GUI;


namespace VFK
{
    public interface IVFKMain
    {
        VFKPARTable VFKPARTable
        {
            get;
        }
        VFKKATUZETableItem getKATUZE(UInt32 aKodKN);
        VFKKATUZETableItem getSelectedKATUZE();
        List<VfkProxyActivePoint> GetProxyActivePoins();
        List<VfkMultiLine> GetDravingLineObjects();
        List<VfkMark> GetDravingMarkObjects();
        List<VfkText> GetDrawingTextObjects();

        VFKSOBRTableItem updateSOBR(VFKSOBRTableItem value);
        VFKSPOLTableItem updateSPOL(VFKSPOLTableItem value, VFKSOBRTableItem aAssociatetTable);

        VFKSBPTableItem UpdateSBP(VFKSBPTableItem aValue);
        VFKHPTableItem UpdateHP(VFKHPTableItem aValue);
        VFKDPMTableItem UpdateDPM(VFKDPMTableItem aValue);
        VFKOPTableItem UpdateOP(VFKOPTableItem aValue);
        VFKOBBPTableItem UpdateOBBP(VFKOBBPTableItem aValue);
        VFKOBTableItem UpdateOB(VFKOBTableItem aValue);
        VFKZVBTableItem UpdateZVB(VFKZVBTableItem aValue);
        VFKSBMTableItem UpdateSbm(VFKSBMTableItem aValue);
        VFKHBPEJTableItem UpdateHbpej(VFKHBPEJTableItem aValue);


        bool CanDeleteSobrAndSpol(string aSobrId);


        void DeleteSBP(VFKSBPTableItem aItem);
        void DeleteHP(VFKHPTableItem aItem);
        void DeleteDPM(VFKDPMTableItem aItem);
        void DeleteOP(VFKOPTableItem aItem);
        void DeleteOBBP(VFKOBBPTableItem aItem);
        void DeleteOB(VFKOBTableItem aItem);
        void DeleteZVB(VFKZVBTableItem aItem);
        void DeleteSbm(VFKSBMTableItem aItem);
        void DeleteHbpej(VFKHBPEJTableItem aItem);
        void DeleteSobr(VFKSOBRTableItem item);
        void DeleteSpol(VFKSPOLTableItem item);
        //
        void WExportHP(IVFKWriter aWriter);
        void WExportDPM(IVFKWriter aWriter);
        void WExportOP(IVFKWriter aWriter);
        void WExportOB(IVFKWriter aWriter);
        void WExportOBBP(IVFKWriter aWriter);
        void WExportZPMZ(IVFKWriter aWriter);
        void WExportPAR(IVFKWriter aWriter);
        void WExportSOBR(IVFKWriter aWriter);
        void WExportSPOL(IVFKWriter aWriter);
        void WExportZVB(IVFKWriter aWriter);
        void WExportHbpej(IVFKWriter aWriter);
    }
    public interface IEditParcel
    {
        VFKMAPLISTable GetMapListTable();
        VFKTELTableItem GetTELItem(string aTelId);
        List<VFKPARTableItem> GetPARItems();
        VFKPARTableItem GetNewParcel();

        VFKBDPTableItem GetNewBdp();
        void DeleteBdp(VFKBDPTableItem bdp);
        void DeleteAllBdp(string parId);
        void RestoreBdpAfterRemoveParcelFromEdit(string parId);

        VFKDataContext GetDataContext();
        List<VFKBDPTableItem> getExistBDPAItems(VFKPARTableItem aPar);
        VFKModifyParcelContext ParcelContext
        { get; set; }
    }
    [Serializable]
    public class VFKMain : IVFKMain, IEditParcel, ISerializable, IDeserializationCallback
    {
        public enum StavData
        {
            Pritomnoust = 0,
            Budoucnost = 1
        }
        #region Constats
        private const UInt32 PRIZNAK_KONTEXTU_N = 3;
        private const UInt32 PRIZNAK_KONTEXTU_Z = 1;
        public const Int32 STAV_DAT_PRITOMNOST = 0;
        public const Int32 STAV_DAT_BUDOUCNOST = 1;
        //blok SOBR and SPOL
        public const Int32 SOBR_SPOL_STAV_DAT_NOVY_BOD = 2;
        private const Int32 SOBR_SPOL_STAV_DAT_EXISTUJICI = 0;
        public const Int32 SOBR_SPOL_STAV_DAT_NEED_UPDATE = Int32.MaxValue;
        //blok SBM
        private const UInt32 SBM_PRIZNAK_KONTEXTU_Z = 1;
        private const UInt32 SBM_PRIZNAK_KONTEXTU_S = 2;
        private const UInt32 SBM_PRIZNAK_KONTEXTU_N = 3;
        #endregion
        public enum VFKDataTypes
        {
            itDKM,
            itKMD
        };
        #region Constructors
        public VFKMain(VFKDataContext aDataContext, IModel model)
        {
            VFKDataContext = aDataContext;
            Header = new Header();
            VFKPARTable = new VFKPARTable();
            VFKBUDTable = new VFKBUDTable();
            VFKBDPTable = new VFKBDPTable();
            VFKRZOTable = new VFKRZOTable();
            VFKSOBRTable = new VFKSOBRTable();
            VFKSBPTable = new VFKSBPTable();
            VFKSBMTable = new VFKSBMTable();
            VFKHPTable = new VFKHPTable();
            VFKZVBTable = new VFKZVBTable();
            VFKOBTable = new VFKOBTable();
            VFKOPTable = new VFKOPTable();
            VFKDPMTable = new VFKDPMTable();
            VFKOBBPTable = new VFKOBBPTable();
            VFKZPMZTable = new VFKZPMZTable();
            VFKSPOLTable = new VFKSPOLTable();
            VFKKATUZETable = new VFKKATUZETable();
            VFKMAPLISTable = new VFKMAPLISTable();
            VFKMAPLISTable.addTableData(new VFKMAPLISTableItem() { ID = null, OZNACENI_MAPOVEHO_LISTU = "-----------", });
            VfkTelTable = new VFKTELTable();
            VFKHBPEJTable = new VFKHBPEJTable();
            VFKOBPEJTable = new VFKOBPEJTable();
            VFKDataType = VFKDataTypes.itDKM;

            VFKModifySBPItems = VFKModifySBPItems = new List<VFKSBPTableItem>();
            Model = model;
        }
        #endregion
        #region Property
        public VFKDataContext VFKDataContext
        {
            get;
            set;
        }
        public IModel Model
        {
            get;
            private set;
        }
        #endregion
        #region Tables
        public Header Header
        {
            get;
            set;
        }
        public VFKPARTable VFKPARTable
        {
            get;
            set;
        }
        public VFKBUDTable VFKBUDTable
        {
            get;
            set;
        }
        public VFKBDPTable VFKBDPTable
        {
            get;
            set;
        }
        public VFKRZOTable VFKRZOTable
        {
            get;
            set;
        }
        public VFKSOBRTable VFKSOBRTable
        {
            get;
            set;
        }
        public VFKSBPTable VFKSBPTable
        {
            get;
            set;
        }
        public VFKSBMTable VFKSBMTable
        {
            get;
            set;
        }
        public VFKHPTable VFKHPTable
        {
            get;
            set;
        }
        public VFKZVBTable VFKZVBTable
        {
            get;
            set;
        }
        public VFKOBTable VFKOBTable
        {
            get;
            set;
        }
        public VFKOPTable VFKOPTable
        {
            get;
            set;
        }
        public VFKDPMTable VFKDPMTable
        {
            get;
            set;
        }
        public VFKOBBPTable VFKOBBPTable
        {
            get;
            set;
        }
        public VFKZPMZTable VFKZPMZTable
        {
            get;
            set;
        }
        public VFKSPOLTable VFKSPOLTable
        {
            get;
            set;
        }
        public VFKKATUZETable VFKKATUZETable
        {
            get;
            set;
        }
        public VFKMAPLISTable VFKMAPLISTable
        {
            get;
            set;
        }
        public VFKTELTable VfkTelTable
        {
            get;
            set;
        }
        public VFKHBPEJTable VFKHBPEJTable
        {
            get;
            set;
        }
        public VFKOBPEJTable VFKOBPEJTable
        {
            get;
            set;
        }
        #endregion
        #region Import
        public void VFKOpenFile()
        {
            ImportInfoDialog importDialog = new ImportInfoDialog(this);
            importDialog.Context.JmenoSouboru = VFKDataContext.FileName;
            VFKReader reader = new VFKReader(this, importDialog.Context);
            reader.parseFile(VFKDataContext.FileName);
            importDialog.ShowDialog();
        }
        #endregion
        #region ExportVFK
        public VFKDataTypes VFKDataType
        {
            get;
            set;
        }
        public void ExportFile(string aFileName)
        {
            VFKWriter writer = new VFKWriter(this);
            writer.exportVFK(aFileName);
            ExportInfoDialog dlg = new ExportInfoDialog(writer.ExportInfoContext);
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
        }
        #endregion
        #region KATUZE
        public VFKKATUZETableItem getKATUZE(UInt32 aKodKN)
        {
            var katuze = from n in VFKKATUZETable.Items where n.KOD == aKodKN select n;
            foreach (VFKKATUZETableItem table in katuze)
            {
                return table;
            }
            return null;
        }
        public VFKKATUZETableItem getSelectedKATUZE()
        {
            return VFKKATUZETable.Items[0];
        }
        #endregion
        #region DrawingMethos
        public List<VfkProxyActivePoint> GetProxyActivePoins()
        {
            List<VfkProxyActivePoint> points = new List<VfkProxyActivePoint>();
            var spolTableCache = new Dictionary<string, List<int>>(VFKSPOLTable.Items.Count);
            var counter = 0;
            foreach (var item in VFKSPOLTable.Items)
            {
                List<int> items;
                if (!spolTableCache.TryGetValue(item.ID, out items))
                {
                    items = new List<int>();
                    spolTableCache[item.ID] = items;
                }
                items.Add(counter);
                counter++;
            }

            foreach (var item in VFKSOBRTable.Items)
            {
                VFKSPOLTableItem spolItem = null;
                if (VFKSPOLTable.Items != null)
                {
                    List<int> cItems;
                    if (!spolTableCache.TryGetValue(item.ID, out cItems))
                    {
                        spolItem = updateSPOL(null, item);
                        spolItem.STAV_DAT = SOBR_SPOL_STAV_DAT_NEED_UPDATE;
                    }
                    else if (cItems.Count == 1)
                    {
                        spolItem = VFKSPOLTable.Items[cItems[0]];
                    }
                    else
                    {
                        throw new ArgumentException("Bylo nalezeno prilis mnoho Sbor bodu.");
                    }
                }
                points.Add(new VfkProxyActivePoint(item, spolItem, this));
            }
            return points;
        }
        public List<VfkMultiLine> GetDravingLineObjects()
        {
            CreateProxyObjects();
            return VFKMultiLinesDrawingObjects;
        }
        public List<VfkMark> GetDravingMarkObjects()
        {
            CreateProxyObjects();
            return VFKMarksDrawingObjects;
        }
        public List<VfkText> GetDrawingTextObjects()
        {
            CreateProxyObjects();
            return VFKTextsDrawingObjects;
        }
        private List<VfkMultiLine> VFKMultiLinesDrawingObjects
        {
            get;
            set;
        }
        private List<VfkMark> VFKMarksDrawingObjects
        {
            get;
            set;
        }
        private List<VfkText> VFKTextsDrawingObjects
        {
            get;
            set;
        }
        private bool _useImportCache = false;
        private Dictionary<string, int> _sobrCahce = new Dictionary<string, int>();
        private Dictionary<string, List<int>> _hp2SBPTableCache = new Dictionary<string, List<int>>();
        private Dictionary<string, List<int>> _dpm2SBPTableCache = new Dictionary<string, List<int>>();
        private Dictionary<string, List<int>> _ob2SBPTableCache = new Dictionary<string, List<int>>();
        private void CreateImportCache()
        {
            _sobrCahce.Clear();
            var i = 0;
            foreach (var item in VFKSOBRTable.Items)
            {
                _sobrCahce.Add(item.ID, i++);
            }
            i = 0;
            _hp2SBPTableCache.Clear();
            _dpm2SBPTableCache.Clear();
            _ob2SBPTableCache.Clear();

            foreach (var item in VFKSBPTable.Items)
            {
                List<int> hpIds;
                if (!_hp2SBPTableCache.TryGetValue(item.HP_ID, out hpIds))
                {
                    hpIds = new List<int>();
                    _hp2SBPTableCache[item.HP_ID] = hpIds;
                }
                hpIds.Add(i);
                List<int> dpmIds;
                if (!_dpm2SBPTableCache.TryGetValue(item.DPM_ID, out dpmIds))
                {
                    dpmIds = new List<int>();
                    _dpm2SBPTableCache[item.DPM_ID] = dpmIds;
                }
                dpmIds.Add(i);

                List<int> obIds;
                if (!_ob2SBPTableCache.TryGetValue(item.OB_ID, out obIds))
                {
                    obIds = new List<int>();
                    _ob2SBPTableCache[item.OB_ID] = obIds;
                }
                obIds.Add(i);
                ++i;
            }
            _useImportCache = true;
        }
        private void ClearCache()
        {
            _sobrCahce.Clear();
            _hp2SBPTableCache.Clear();
            _dpm2SBPTableCache.Clear();
            _ob2SBPTableCache.Clear();
            _useImportCache = false;
        }
        private bool _mCreatedProxyObjects = false;
        private void CreateProxyObjects()
        {
            if (_mCreatedProxyObjects) return;
            CreateImportCache();
            _mCreatedProxyObjects = true;
            VFKMultiLinesDrawingObjects = new List<VfkMultiLine>();
            VFKMarksDrawingObjects = new List<VfkMark>();
            VFKTextsDrawingObjects = new List<VfkText>();
            UInt32 stavDataShow = (UInt32)VFKDataContext.CilImportuEntrie.StavData;

            //            VFKModifySBPItems
            //            Dictio
            //BEGIN HP
            foreach (var line in VFKHPTable.Items)
            {
                if (line.STAV_DAT != stavDataShow) continue;
                if (_VFKModifyHPItems.Exists(n => n.ID == line.ID && n.PRIZNAK_KONTEXTU == PRIZNAK_KONTEXTU_Z))
                    continue;
                VFKMultiLinesDrawingObjects.Add(new VfkMultiLine(this, line, false));
            }
            foreach (var line in _VFKModifyHPItems)
            {
                if (line.PRIZNAK_KONTEXTU != PRIZNAK_KONTEXTU_N)
                    continue;
                VFKMultiLinesDrawingObjects.Add(new VfkMultiLine(this, line, true));
            }
            //END HP
            //BEGIN ZVB
            foreach (var line in VFKZVBTable.Items)
            {
                if (line.STAV_DAT != stavDataShow) continue;
                if (_VFKModifyZVBItems.Exists(n => n.ID == line.ID && n.PRIZNAK_KONTEXTU == PRIZNAK_KONTEXTU_Z))
                    continue;
                VFKMultiLinesDrawingObjects.Add(new VfkMultiLine(this, line, false));
            }
            foreach (var line in _VFKModifyZVBItems)
            {
                if (line.PRIZNAK_KONTEXTU != PRIZNAK_KONTEXTU_N)
                    continue;
                VFKMultiLinesDrawingObjects.Add(new VfkMultiLine(this, line, true));
            }
            //END ZVB
            //BEGIN DPM
            foreach (var dpm in VFKDPMTable.Items)
            {
                if (dpm.STAV_DAT != stavDataShow) continue;
                if (_VFKModifyDPMItems.Exists(n => n.ID == dpm.ID && n.PRIZNAK_KONTEXTU == PRIZNAK_KONTEXTU_Z))
                    continue;
                CreateDPMGraphicsElement(dpm, false);
            }
            foreach (var dpm in _VFKModifyDPMItems)
            {
                if (dpm.PRIZNAK_KONTEXTU != PRIZNAK_KONTEXTU_N)
                    continue;
                CreateDPMGraphicsElement(dpm, true);
            }
            //END  DPM
            //BEGIN OP
            foreach (var op in VFKOPTable.Items)
            {
                if (op.STAV_DAT != stavDataShow) continue;
                if (_VFKModifyOPItems.Exists(n => n.ID == op.ID && n.PRIZNAK_KONTEXTU == PRIZNAK_KONTEXTU_Z))
                    continue;
                CreateOPGraphicsElement(op, false);
            }
            foreach (var opItem in _VFKModifyOPItems)
            {
                if (opItem.PRIZNAK_KONTEXTU != PRIZNAK_KONTEXTU_N)
                    continue;
                CreateOPGraphicsElement(opItem, true);
            }
            //END OP
            //BEGIN OBBP
            foreach (var obbp in VFKOBBPTable.Items)
            {
                if (obbp.STAV_DAT != stavDataShow) continue;
                if (_VFKModifyOBBPItems.Exists(n => n.ID == obbp.ID && n.PRIZNAK_KONTEXTU == PRIZNAK_KONTEXTU_Z))
                    continue;
                CreateOBBPGraphicsElement(obbp, false);
            }
            foreach (var obbpItem in _VFKModifyOBBPItems)
            {
                if (obbpItem.PRIZNAK_KONTEXTU != PRIZNAK_KONTEXTU_N)
                    continue;
                CreateOBBPGraphicsElement(obbpItem, true);
            }
            //END OBBP
            //BEGIN OB
            foreach (var ob in VFKOBTable.Items)
            {
                if (ob.STAV_DAT != stavDataShow) continue;
                if (_VFKModifyOBItems.Exists(n => n.ID == ob.ID && n.PRIZNAK_KONTEXTU == PRIZNAK_KONTEXTU_Z))
                    continue;
                CreateOBGraphicsElement(ob, false);
            }
            foreach (var obItem in _VFKModifyOBItems)
            {
                if (obItem.PRIZNAK_KONTEXTU != PRIZNAK_KONTEXTU_N)
                    continue;
                CreateOBGraphicsElement(obItem, true);
            }
            //END OB
            //BEGIN HBPEJ
            foreach (var hbpej in VFKHBPEJTable.Items)
            {
                if (hbpej.STAV_DAT != stavDataShow) continue;
                if (_VFKModifyHBPEJItems.Exists(n => n.ID == hbpej.ID && n.PRIZNAK_KONTEXTU == PRIZNAK_KONTEXTU_Z))
                    continue;
                VFKMultiLinesDrawingObjects.Add(new VfkMultiLine(this, hbpej, false));
                //Debug.Assert(hbpej.BPEJ_KOD_HRANICE_2.Length==0); TODO
            }
            foreach (var hbpejItem in _VFKModifyHBPEJItems)
            {
                if (hbpejItem.PRIZNAK_KONTEXTU != PRIZNAK_KONTEXTU_N)
                    continue;
                VFKMultiLinesDrawingObjects.Add(new VfkMultiLine(this, hbpejItem, true));
            }
            //END HBPEJ
            //BEGIN OBPEJ
            foreach (var obpej in VFKOBPEJTable.Items)
            {
                if (obpej.STAV_DAT != stavDataShow) continue;
                VFKTextsDrawingObjects.Add(new VfkText(this, obpej));
            }
            //TODO
            //END OBPEJ
            ClearCache();
        }

        public void CreateDPMGraphicsElement(VFKDPMTableItem dpm, bool fromModifyItems)
        {
            VfkElement element = Singletons.VFKElements.GetElement(dpm.TYPPPD_KOD);
            switch (element.DpmType)
            {
                case DpmType.LDPP:
                case DpmType.LPP:
                    VFKMultiLinesDrawingObjects.Add(new VfkMultiLine(this, dpm, fromModifyItems));
                    break;
                case DpmType.BPP:
                case DpmType.MEZ:
                case DpmType.BDPP:
                    VFKMarksDrawingObjects.Add(new VfkMark(this, dpm));
                    break;
                case DpmType.TPP:
                    VFKTextsDrawingObjects.Add(new VfkText(this, dpm));
                    break;
                case DpmType.HCHU:
                case DpmType.HOCHP:
                    //todo
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }
        public void CreateOPGraphicsElement(VFKOPTableItem op, bool fromModifyItems)
        {
            switch (op.OPAR_TYPE)
            {
                case "ZDP":
                case "SPC":
                    if (op.TYPPPD_KOD == 1032)
                    {
                        VFKMultiLinesDrawingObjects.Add(new VfkMultiLine(this, op, fromModifyItems));
                    }
                    else
                        VFKMarksDrawingObjects.Add(new VfkMark(this, op));
                    break;
                case "PC":
                case "PPC":
                    VFKTextsDrawingObjects.Add(new VfkText(this, op));
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }
        public void CreateOBBPGraphicsElement(VFKOBBPTableItem obbp, bool fromModifyItems)
        {
            switch (obbp.OBBP_TYPE)
            {
                case "ZB":
                    {
                        VFKMarksDrawingObjects.Add(new VfkMark(this, obbp));
                    }
                    break;
                case "CB":
                    {
                        VFKTextsDrawingObjects.Add(new VfkText(this, obbp));
                    }
                    break;
                default:
                    {
                        Debug.Assert(false);
                    }
                    break;
            }
        }
        public void CreateOBGraphicsElement(VFKOBTableItem ob, bool fromModifyItems)
        {
            switch (ob.OBRBUD_TYPE)
            {
                case "OB":
                case "ZDB":
                    {
                        //Obvod budovy evidované v SPI
                        if (ob.TYPPPD_KOD == 21700)
                            VFKMultiLinesDrawingObjects.Add(new VfkMultiLine(this, ob, fromModifyItems));
                        else
                            VFKMarksDrawingObjects.Add(new VfkMark(this, ob));
                    }
                    break;
                default:
                    {
                        Debug.Assert(false);
                        break;
                    }
            }
        }
        #endregion
        #region private methods
        private void InitDataTableBaseItems(VFKDataTableBaseItemWithProp aItem, VFKDataTableBase aOwnerTable)
        {
            aItem.PRIZNAK_KONTEXTU = PRIZNAK_KONTEXTU_N;
            aItem.STAV_DAT = STAV_DAT_BUDOUCNOST;
            aItem.ID = aOwnerTable.getMaxValueFromBlock().ToString();
            aOwnerTable.UpdateMaxValue(aItem.ID);
        }
        private void InitDataRemoveTableBaseItem(VFKDataTableBaseItemWithProp aItem)
        {
            aItem.PRIZNAK_KONTEXTU = PRIZNAK_KONTEXTU_Z;
            aItem.STAV_DAT = STAV_DAT_BUDOUCNOST;
        }
        private void InitBeforeExport(VFKDataTableBaseItemWithProp aItem)
        {
            switch (aItem.PRIZNAK_KONTEXTU)
            {
                case PRIZNAK_KONTEXTU_N:
                    aItem.DATUM_VZNIKU = DateTime.Now;
                    aItem.STAV_DAT = STAV_DAT_BUDOUCNOST;
                    break;
                case PRIZNAK_KONTEXTU_Z:
                    aItem.STAV_DAT = STAV_DAT_PRITOMNOST;
                    break;
                default:
                    throw new UnExpectException();
            }
        }
        #endregion
        #region DPM
        private List<VFKDPMTableItem> _VFKModifyDPMItems = new List<VFKDPMTableItem>();
        public VFKDPMTableItem UpdateDPM(VFKDPMTableItem aValue)
        {
            if (aValue == null)
            {
                VFKDPMTableItem newItem = new VFKDPMTableItem();
                InitDataTableBaseItems(newItem, VFKDPMTable);
                newItem.KATUZE_KOD = VFKDataContext.FSU;
                newItem.VZTAZNY_BOD = UInt32.MaxValue;
                newItem.VELIKOST = double.MaxValue;
                newItem.UHEL = double.MaxValue;
                _VFKModifyDPMItems.Add(newItem);
                return newItem;
            }
            if (aValue.STAV_DAT == STAV_DAT_BUDOUCNOST)
            {
                var dpm = (from n in _VFKModifyDPMItems where n.ID == aValue.ID select n).ToList();
                if (dpm.Count == 0)
                {
                    _VFKModifyDPMItems.Add(aValue);
                    return aValue;
                }
                if (dpm.Count() == 1)
                {
                    dpm.ToList()[0] = aValue;
                    return aValue;
                }
            }
            else if (aValue.STAV_DAT == STAV_DAT_PRITOMNOST)
            {//UndoRedo, item form import
                _VFKModifyDPMItems.RemoveAll(n => n.ID == aValue.ID);
                return aValue;
            }
            throw new UnExpectException();
        }
        public void DeleteDPM(VFKDPMTableItem aItem)
        {
            var item = from n in _VFKModifyDPMItems where n.ID == aItem.ID select n;
            if (item.Count() > 0)
            {
                Debug.Assert(item.Count() == 1);
                _VFKModifyDPMItems.Remove(item.First());
                return;
            }
            item = from n in VFKDPMTable.Items where n.ID == aItem.ID select n;
            Debug.Assert(item.Count() == 1);
            var newItem = (VFKDPMTableItem)item.First().Clone();
            InitDataRemoveTableBaseItem(newItem);
            _VFKModifyDPMItems.Add(newItem);
        }
        public void WExportDPM(IVFKWriter aWriter)
        {
            foreach (var dpmT in _VFKModifyDPMItems)
            {
                var dpm = (VFKDPMTableItem)dpmT.Clone();
                InitBeforeExport(dpm);
                aWriter.AddDPM(dpm);
                if (dpm.DPM_TYPE == "HCHU" || dpm.DPM_TYPE == "HOCHP")
                {
                    var sbms = from n in VFKModifySBMItems where n.DPM_ID == dpm.ID select n;
                    foreach (var sbm in sbms)
                    {
                        WExportSbm(aWriter, sbm);
                    }
                }
                else if (dpm.DPM_TYPE == "MEZ")
                {
                    var sobr = from n in VFKSOBRTable.Items where n.ID == dpm.BP_ID select n;
                    WExportSOBR(aWriter, sobr.First(), IsItemRemoved(dpm));
                }
                else
                {
                    var sbps = from n in VFKModifySBPItems where n.DPM_ID == dpm.ID select n;
                    foreach (var sbp in sbps)
                    {
                        WExportSBP(aWriter, sbp);
                    }
                }
            }
        }
        #endregion
        #region OP
        private List<VFKOPTableItem> _VFKModifyOPItems = new List<VFKOPTableItem>();
        public VFKOPTableItem UpdateOP(VFKOPTableItem aValue)
        {
            if (aValue == null)
            {
                VFKOPTableItem newItem = new VFKOPTableItem();
                InitDataTableBaseItems(newItem, VFKOPTable);
                _VFKModifyOPItems.Add(newItem);
                newItem.VELIKOST = double.MaxValue;
                newItem.UHEL = double.MaxValue;
                return newItem;
            }
            if (aValue.STAV_DAT == STAV_DAT_BUDOUCNOST)
            {
                var op = (from n in _VFKModifyOPItems where n.ID == aValue.ID select n).ToList();
                if (op.Count == 0)
                {
                    _VFKModifyOPItems.Add(aValue);
                    return aValue;
                }
                if (op.Count() == 1)
                {
                    op.ToList()[0] = aValue;
                    return aValue;
                }
            }
            else if (aValue.STAV_DAT == STAV_DAT_PRITOMNOST)
            {//UndoRedo, item form import
                _VFKModifyOPItems.RemoveAll(n => n.ID == aValue.ID);
                return aValue;
            }
            throw new UnExpectException();
        }
        public void DeleteOP(VFKOPTableItem aItem)
        {
            var item = from n in _VFKModifyOPItems where n.ID == aItem.ID select n;
            if (item.Count() > 0)
            {
                Debug.Assert(item.Count() == 1);
                _VFKModifyOPItems.Remove(item.First());
                return;
            }
            item = from n in VFKOPTable.Items where n.ID == aItem.ID select n;
            Debug.Assert(item.Count() == 1);
            var newItem = (VFKOPTableItem)item.First().Clone();
            InitDataRemoveTableBaseItem(newItem);
            _VFKModifyOPItems.Add(newItem);
        }
        public void WExportOP(IVFKWriter aWriter)
        {
            foreach (var opT in _VFKModifyOPItems)
            {
                var op = (VFKOPTableItem)opT.Clone();
                InitBeforeExport(op);
                op.PAR_ID = "0";//todo
                aWriter.AddOP(op);
                if (op.TYPPPD_KOD == 1032)
                {
                    var sbms = (from n in VFKModifySBMItems where n.OP_ID == opT.ID select n).ToList();
                    foreach (var smbItem in sbms)
                    {
                        WExportSbm(aWriter, smbItem);
                    }
                }
                else
                {
                    var sbps = (from n in VFKModifySBPItems where n.OB_ID == opT.ID select n).ToList();
                    foreach (var sbp in sbps)
                    {
                        WExportSBP(aWriter, sbp);
                    }
                }
            }
        }
        #endregion
        #region OBBP
        private List<VFKOBBPTableItem> _VFKModifyOBBPItems = new List<VFKOBBPTableItem>();
        public VFKOBBPTableItem UpdateOBBP(VFKOBBPTableItem aValue)
        {
            if (aValue == null)
            {
                VFKOBBPTableItem newItem = new VFKOBBPTableItem();
                InitDataTableBaseItems(newItem, VFKOBBPTable);
                _VFKModifyOBBPItems.Add(newItem);
                newItem.VELIKOST = double.MaxValue;
                newItem.UHEL = double.MaxValue;
                return newItem;
            }
            if (aValue.STAV_DAT == STAV_DAT_BUDOUCNOST)
            {
                var obbp = (from n in _VFKModifyOBBPItems where n.ID == aValue.ID select n).ToList();
                if (obbp.Count == 0)
                {
                    _VFKModifyOBBPItems.Add(aValue);
                    return aValue;
                }
                if (obbp.Count() == 1)
                {
                    obbp.ToList()[0] = aValue;
                    return aValue;
                }
            }
            else if (aValue.STAV_DAT == STAV_DAT_PRITOMNOST)
            {//UndoRedo, item form import
                _VFKModifyOBBPItems.RemoveAll(n => n.ID == aValue.ID);
                return aValue;
            }
            throw new UnExpectException();
        }
        public void DeleteOBBP(VFKOBBPTableItem aItem)
        {
            var item = from n in _VFKModifyOBBPItems where n.ID == aItem.ID select n;
            if (item.Count() > 0)
            {
                Debug.Assert(item.Count() == 1);
                _VFKModifyOBBPItems.Remove(item.First());
                return;
            }
            item = from n in VFKOBBPTable.Items where n.ID == aItem.ID select n;
            Debug.Assert(item.Count() == 1);
            var newItem = (VFKOBBPTableItem)item.First().Clone();
            InitDataRemoveTableBaseItem(newItem);
            _VFKModifyOBBPItems.Add(newItem);
        }
        public void WExportOBBP(IVFKWriter aWriter)
        {
            foreach (var obT in _VFKModifyOBBPItems)
            {
                VFKOBBPTableItem obbp = (VFKOBBPTableItem)obT.Clone();
                InitBeforeExport(obbp);
                aWriter.AddOBBP(obbp);
                var sobrs = from n in VFKSOBRTable.Items where n.ID == obbp.BP_ID select n;
                foreach (var sobr in sobrs)
                {
                    WExportSOBR(aWriter, sobr, IsItemRemoved(obbp));
                }
            }
        }
        #endregion
        #region OB
        private List<VFKOBTableItem> _VFKModifyOBItems = new List<VFKOBTableItem>();
        public VFKOBTableItem UpdateOB(VFKOBTableItem aValue)
        {
            if (aValue == null)
            {
                VFKOBTableItem newItem = new VFKOBTableItem();
                InitDataTableBaseItems(newItem, VFKOBTable);
                _VFKModifyOBItems.Add(newItem);
                newItem.BUD_ID = "0";//tod
                newItem.VELIKOST = double.MaxValue;
                newItem.UHEL = double.MaxValue;

                return newItem;
            }
            if (aValue.STAV_DAT == STAV_DAT_BUDOUCNOST)
            {
                var ob = (from n in _VFKModifyOBItems where n.ID == aValue.ID select n).ToList();
                if (ob.Count == 0)
                {
                    _VFKModifyOBItems.Add(aValue);
                    return aValue;
                }
                if (ob.Count() == 1)
                {
                    ob.ToList()[0] = aValue;
                    return aValue;
                }
            }
            else if (aValue.STAV_DAT == STAV_DAT_PRITOMNOST)
            {//UndoRedo, item form import
                _VFKModifyOBItems.RemoveAll(n => n.ID == aValue.ID);
                return aValue;
            }
            throw new UnExpectException();
        }
        public void DeleteOB(VFKOBTableItem aItem)
        {
            var item = from n in _VFKModifyOBItems where n.ID == aItem.ID select n;
            if (item.Count() > 0)
            {
                Debug.Assert(item.Count() == 1);
                _VFKModifyOBItems.Remove(item.First());
                return;
            }
            item = from n in VFKOBTable.Items where n.ID == aItem.ID select n;
            Debug.Assert(item.Count() == 1);
            var newItem = (VFKOBTableItem)item.First().Clone();
            InitDataRemoveTableBaseItem(newItem);
            _VFKModifyOBItems.Add(newItem);
        }
        public void WExportOB(IVFKWriter aWriter)
        {
            foreach (var obT in _VFKModifyOBItems)
            {
                VFKOBTableItem ob = (VFKOBTableItem)obT.Clone();
                InitBeforeExport(ob);
                aWriter.AddOB(ob);
                var sbps = from n in VFKModifySBPItems where n.OB_ID == ob.ID select n;
                foreach (var sbp in sbps)
                {
                    WExportSBP(aWriter, sbp);
                }
            }
        }
        #endregion
        #region HBPEJ
        private List<VFKHBPEJTableItem> _VFKModifyHBPEJItems = new List<VFKHBPEJTableItem>();
        public VFKHBPEJTableItem UpdateHbpej(VFKHBPEJTableItem aValue)
        {
            if (aValue == null)
            {
                VFKHBPEJTableItem newItem = new VFKHBPEJTableItem();
                InitDataTableBaseItems(newItem, VFKOBTable);
                _VFKModifyHBPEJItems.Add(newItem);
                return newItem;
            }
            if (aValue.STAV_DAT == STAV_DAT_BUDOUCNOST)
            {
                var hbpej = (from n in _VFKModifyHBPEJItems where n.ID == aValue.ID select n).ToList();
                if (hbpej.Count == 0)
                {
                    _VFKModifyHBPEJItems.Add(aValue);
                    return aValue;
                }
                if (hbpej.Count() == 1)
                {
                    hbpej.ToList()[0] = aValue;
                    return aValue;
                }
            }
            else if (aValue.STAV_DAT == STAV_DAT_PRITOMNOST)
            {//UndoRedo, item form import
                _VFKModifyHBPEJItems.RemoveAll(n => n.ID == aValue.ID);
                return aValue;
            }
            throw new UnExpectException();
        }
        public void DeleteHbpej(VFKHBPEJTableItem aItem)
        {
            var item = from n in _VFKModifyHBPEJItems where n.ID == aItem.ID select n;
            if (item.Count() > 0)
            {
                Debug.Assert(item.Count() == 1);
                _VFKModifyHBPEJItems.Remove(item.First());
                return;
            }
            item = from n in VFKHBPEJTable.Items where n.ID == aItem.ID select n;
            Debug.Assert(item.Count() == 1);
            var newItem = (VFKHBPEJTableItem)item.First().Clone();
            InitDataRemoveTableBaseItem(newItem);
            _VFKModifyHBPEJItems.Add(newItem);
        }
        public void WExportHbpej(IVFKWriter aWriter)
        {
            foreach (var obT in _VFKModifyHBPEJItems)
            {
                VFKHBPEJTableItem hbpej = (VFKHBPEJTableItem)obT.Clone();
                InitBeforeExport(hbpej);
                aWriter.AddHBPEJ(hbpej);
                var sbms = from n in VFKModifySBMItems where n.HBPEJ_ID == hbpej.ID select n;
                foreach (var smb in sbms)
                {
                    WExportSbm(aWriter, smb);
                }
            }
        }
        #endregion
        #region SBP
        public List<VFKSBPTableItem> VFKModifySBPItems
        {
            get;
            set;
        }
        public VFKSBPTableItem UpdateSBP(VFKSBPTableItem aValue)
        {
            if (aValue == null)
            {
                VFKSBPTableItem newItem = new VFKSBPTableItem();
                InitDataTableBaseItems(newItem, VFKSBPTable);
                VFKModifySBPItems.Add(newItem);
                return newItem;
            }
            if (aValue.STAV_DAT == STAV_DAT_BUDOUCNOST)
            {
                var sbp = (from n in VFKModifySBPItems where n.ID == aValue.ID select n).ToList();
                if (sbp.Count == 0)
                {
                    VFKModifySBPItems.Add(aValue);
                    return aValue;
                }
                if (sbp.Count() == 1)
                {
                    sbp.ToList()[0] = aValue;
                    return aValue;
                }
            }
            else if (aValue.STAV_DAT == STAV_DAT_PRITOMNOST)
            {//UndoRedo, item form import
                VFKModifySBPItems.RemoveAll(n => n.ID == aValue.ID);
                return aValue;
            }
            throw new UnExpectException();
        }
        public void DeleteSBP(VFKSBPTableItem aItem)
        {
            var item = from n in VFKModifySBPItems where n.ID == aItem.ID select n;
            if (item.Count() > 0)
            {
                Debug.Assert(item.Count() == 1);
                VFKModifySBPItems.Remove(item.First());
                return;
            }
            item = from n in VFKSBPTable.Items where n.ID == aItem.ID select n;
            Debug.Assert(item.Count() == 1);
            var newItem = (VFKSBPTableItem)item.First().Clone();
            InitDataRemoveTableBaseItem(newItem);
            VFKModifySBPItems.Add(newItem);
        }
        private void WExportSBP(IVFKWriter aWriter, VFKSBPTableItem aItem)
        {
            aItem = (VFKSBPTableItem)aItem.Clone();
            InitBeforeExport(aItem);
            var sobrs = from n in VFKSOBRTable.Items where n.ID == aItem.BP_ID select n;
            System.Diagnostics.Debug.Assert(sobrs.Count() == 1);
            foreach (var tempSobr in sobrs)
            {
                WExportSOBR(aWriter, tempSobr, IsItemRemoved(aItem));
            }
            //            aItem.ZVB_ID = "0";//todo from ISKN so empty
            aWriter.AddSBP(aItem);
        }

        bool IsItemRemoved(VFKDataTableBaseItemWithProp aItem)
        {
            return aItem.PRIZNAK_KONTEXTU == PRIZNAK_KONTEXTU_Z && aItem.STAV_DAT == STAV_DAT_PRITOMNOST;
        }

        #endregion
        #region Sbm
        public List<VFKSBMTableItem> VFKModifySBMItems = new List<VFKSBMTableItem>();
        public VFKSBMTableItem UpdateSbm(VFKSBMTableItem aValue)
        {
            if (aValue == null)
            {
                VFKSBMTableItem newItem = new VFKSBMTableItem();
                newItem.PRIZNAK_KONTEXTU = SBM_PRIZNAK_KONTEXTU_N;
                newItem.DATUM_VZNIKU = DateTime.Now;
                VFKModifySBMItems.Add(newItem);
                return newItem;
            }
            if (aValue.PRIZNAK_KONTEXTU == SBM_PRIZNAK_KONTEXTU_N)
            {
                var sbm = (from n in VFKModifySBMItems
                           where n.DPM_ID == aValue.DPM_ID && n.HBPEJ_ID == aValue.HBPEJ_ID
                                 && n.OP_ID == aValue.OP_ID && n.PARAMETRY_SPOJENI == aValue.PARAMETRY_SPOJENI
                                 && n.PORADOVE_CISLO_BODU == aValue.PORADOVE_CISLO_BODU &&
                                 n.SOURADNICE_X == aValue.SOURADNICE_X
                                 && n.SOURADNICE_Y == aValue.SOURADNICE_Y
                           select n).ToList();
                if (sbm.Count == 0)
                {
                    VFKModifySBMItems.Add(aValue);
                    return aValue;
                }
                if (sbm.Count() == 1)
                {
                    sbm.ToList()[0] = aValue;
                    return aValue;
                }
            }
            else if (aValue.PRIZNAK_KONTEXTU == SBM_PRIZNAK_KONTEXTU_S)
            {//UndoRedo, item form import
                VFKModifySBMItems.RemoveAll(n => n.DPM_ID == aValue.DPM_ID && n.HBPEJ_ID == aValue.HBPEJ_ID
                                                  && n.OP_ID == aValue.OP_ID &&
                                                  n.PARAMETRY_SPOJENI == aValue.PARAMETRY_SPOJENI
                                                  && n.PORADOVE_CISLO_BODU == aValue.PORADOVE_CISLO_BODU &&
                                                  n.SOURADNICE_X == aValue.SOURADNICE_X
                                                  && n.SOURADNICE_Y == aValue.SOURADNICE_Y);
                return aValue;
            }
            throw new UnExpectException();
        }
        public void DeleteSbm(VFKSBMTableItem aItem)
        {
            var item = from n in VFKModifySBMItems
                       where n.DPM_ID == aItem.DPM_ID && n.HBPEJ_ID == aItem.HBPEJ_ID
                             && n.OP_ID == aItem.OP_ID && n.PARAMETRY_SPOJENI == aItem.PARAMETRY_SPOJENI
                             && n.PORADOVE_CISLO_BODU == aItem.PORADOVE_CISLO_BODU &&
                             n.SOURADNICE_X == aItem.SOURADNICE_X
                             && n.SOURADNICE_Y == aItem.SOURADNICE_Y
                       select n;
            if (item.Count() > 0)
            {
                Debug.Assert(item.Count() == 1);
                VFKModifySBMItems.Remove(item.First());
                return;
            }
            item = from n in VFKSBMTable.Items
                   where n.DPM_ID == aItem.DPM_ID && n.HBPEJ_ID == aItem.HBPEJ_ID
                         && n.OP_ID == aItem.OP_ID && n.PARAMETRY_SPOJENI == aItem.PARAMETRY_SPOJENI
                         && n.PORADOVE_CISLO_BODU == aItem.PORADOVE_CISLO_BODU &&
                         n.SOURADNICE_X == aItem.SOURADNICE_X
                         && n.SOURADNICE_Y == aItem.SOURADNICE_Y
                   select n;
            Debug.Assert(item.Count() == 1);
            var newItem = (VFKSBMTableItem)item.First().Clone();
            newItem.PRIZNAK_KONTEXTU = SBM_PRIZNAK_KONTEXTU_Z;
            VFKModifySBMItems.Add(newItem);
        }
        private void WExportSbm(IVFKWriter aWriter, VFKSBMTableItem aItem)
        {
            VFKSBMTableItem item = (VFKSBMTableItem)aItem.Clone();
            if (item.PRIZNAK_KONTEXTU == SBM_PRIZNAK_KONTEXTU_N)
                item.DATUM_VZNIKU = DateTime.Now;
            else if (item.PRIZNAK_KONTEXTU == SBM_PRIZNAK_KONTEXTU_Z)
                item.DATUM_ZANIKU = DateTime.Now;
            aWriter.AddSbm(item);
        }
        #endregion
        #region HP
        private List<VFKHPTableItem> _VFKModifyHPItems = new List<VFKHPTableItem>();
        public VFKHPTableItem UpdateHP(VFKHPTableItem aValue)
        {
            if (aValue == null)
            {
                VFKHPTableItem newItem = new VFKHPTableItem();
                InitDataTableBaseItems(newItem, VFKHPTable);
                _VFKModifyHPItems.Add(newItem);
                return newItem;
            }
            if (aValue.STAV_DAT == STAV_DAT_BUDOUCNOST)
            {
                var hp = (from n in _VFKModifyHPItems where n.ID == aValue.ID select n).ToList();
                if (hp.Count == 0)
                {
                    _VFKModifyHPItems.Add(aValue);
                    return aValue;
                }
                if (hp.Count() == 1)
                {
                    hp.ToList()[0] = aValue;
                    return aValue;
                }
            }
            else if (aValue.STAV_DAT == STAV_DAT_PRITOMNOST)
            {//UndoRedo, item form import
                _VFKModifyHPItems.RemoveAll(n => n.ID == aValue.ID);
                return aValue;
            }
            throw new UnExpectException();
        }
        public void DeleteHP(VFKHPTableItem aItem)
        {
            var item = from n in _VFKModifyHPItems where n.ID == aItem.ID select n;
            if (item.Count() > 0)
            {
                Debug.Assert(item.Count() == 1);
                _VFKModifyHPItems.Remove(item.First());
                return;
            }
            item = from n in VFKHPTable.Items where n.ID == aItem.ID select n;
            Debug.Assert(item.Count() == 1);
            var newItem = (VFKHPTableItem)item.First().Clone();
            InitDataRemoveTableBaseItem(newItem);
            _VFKModifyHPItems.Add(newItem);
        }
        public void WExportHP(IVFKWriter aWriter)
        {
            foreach (var hpT in _VFKModifyHPItems)
            {
                VFKHPTableItem hp = (VFKHPTableItem)hpT.Clone();
                InitBeforeExport(hp);
                hp.PAR_ID_1 = "0";
                aWriter.AddHP(hp);
                var sbps = from n in VFKModifySBPItems where n.HP_ID == hp.ID select n;
                Debug.Assert(sbps.Count() >= 2);
                foreach (var tempSbp in sbps)
                {
                    WExportSBP(aWriter, tempSbp);
                }
            }
        }
        #endregion
        #region ZVB
        private List<VFKZVBTableItem> _VFKModifyZVBItems = new List<VFKZVBTableItem>();
        public VFKZVBTableItem UpdateZVB(VFKZVBTableItem aValue)
        {
            if (aValue == null)
            {
                VFKZVBTableItem newItem = new VFKZVBTableItem();
                newItem.KATUZE_KOD = VFKDataContext.FSU;
                InitDataTableBaseItems(newItem, VFKZVBTable);
                _VFKModifyZVBItems.Add(newItem);
                return newItem;
            }
            if (aValue.STAV_DAT == STAV_DAT_BUDOUCNOST)
            {
                var zvb = (from n in _VFKModifyZVBItems where n.ID == aValue.ID select n).ToList();
                if (zvb.Count == 0)
                {
                    _VFKModifyZVBItems.Add(aValue);
                    return aValue;
                }
                if (zvb.Count() == 1)
                {
                    zvb.ToList()[0] = aValue;
                    return aValue;
                }
            }
            else if (aValue.STAV_DAT == STAV_DAT_PRITOMNOST)
            {//UndoRedo, item form import
                _VFKModifyZVBItems.RemoveAll(n => n.ID == aValue.ID);
                return aValue;
            }
            throw new UnExpectException();
        }
        public void DeleteZVB(VFKZVBTableItem aItem)
        {
            var item = from n in _VFKModifyZVBItems where n.ID == aItem.ID select n;
            if (item.Count() > 0)
            {
                Debug.Assert(item.Count() == 1);
                _VFKModifyZVBItems.Remove(item.First());
                return;
            }
            item = from n in VFKZVBTable.Items where n.ID == aItem.ID select n;
            Debug.Assert(item.Count() == 1);
            var newItem = (VFKZVBTableItem)item.First().Clone();
            InitDataRemoveTableBaseItem(newItem);
            _VFKModifyZVBItems.Add(newItem);
        }
        public void WExportZVB(IVFKWriter aWriter)
        {
            foreach (var zvbT in _VFKModifyZVBItems)
            {
                VFKZVBTableItem zvb = (VFKZVBTableItem)zvbT.Clone();
                InitBeforeExport(zvb);
                aWriter.AddZVB(zvb);
                var sbps = from n in VFKModifySBPItems where n.ZVB_ID == zvb.ID select n;
                Debug.Assert(sbps.Count() == 2);
                foreach (var tempSbp in sbps)
                {
                    WExportSBP(aWriter, tempSbp);
                }
            }
        }
        #endregion
        #region SPOL
        public bool CanDeleteSobrAndSpol(string aSobrId)
        {
            var sobr = VFKSOBRTable.Items.FindAll(x => x.ID == aSobrId);
            if (sobr.ToArray()[0].ItemFromImport())
                return false;
            var result = from n in VFKModifySBPItems where n.BP_ID == aSobrId select n;
            if (result.Any())
                return false;
            return true;
        }
        public VFKSPOLTableItem updateSPOL(VFKSPOLTableItem value, VFKSOBRTableItem aAssociatetTable)
        {
            if (value == null)
            {
                VFKSPOLTableItem newItem = new VFKSPOLTableItem();
                newItem.ID = aAssociatetTable.ID;
                newItem.STAV_DAT = SOBR_SPOL_STAV_DAT_NOVY_BOD;
                newItem.KATUZE_KOD = aAssociatetTable.KATUZE_KOD;
                newItem.CISLO_ZPMZ = aAssociatetTable.CISLO_ZPMZ;
                newItem.KATUZE_KOD_MER = aAssociatetTable.KATUZE_KOD;
                newItem.CISLO_ZPMZ_MER = VFKDataContext.CisloZPMZ;
                newItem.KODCHB_KOD = UInt32.MaxValue;
                newItem.UPLNE_CISLO = aAssociatetTable.UPLNE_CISLO;
                newItem.CISLO_BODU = aAssociatetTable.CISLO_BODU;
                newItem.CISLO_TL = aAssociatetTable.CISLO_TL;
                VFKSPOLTable.addTableData(newItem);
                return newItem;
            }
            Debug.Assert(!VFKSPOLTable.Items.Contains(value));
            VFKSPOLTable.addTableData(value);
            return value;
        }
        public void DeleteSpol(VFKSPOLTableItem value)
        {
            var spol = (from n in VFKSPOLTable.Items where n.ID == value.ID select n).ToList();
            Debug.Assert(spol.Count == 1 && spol[0].STAV_DAT == SOBR_SPOL_STAV_DAT_NOVY_BOD);
            VFKSPOLTable.Items.RemoveAll(x => x.ID == value.ID);
        }
        public void WExportSPOL(IVFKWriter aWriter)
        {
            foreach (var spolT in VFKSPOLTable.Items)
            {
                var spol = (VFKSPOLTableItem)spolT.Clone();
                if (spol.ItemFromImport())
                    continue;
                if (spol.STAV_DAT == SOBR_SPOL_STAV_DAT_NEED_UPDATE) continue;
                //if(spol.KODCHB_KOD==8)continue; ?????
                spol.STAV_DAT = SOBR_SPOL_STAV_DAT_NOVY_BOD;
                aWriter.AddSPOL(spol);
            }
        }
        private void WExportSPOL(IVFKWriter aWriter, VFKSPOLTableItem aItem, bool aParentIsRemoved)
        {
            var spol = (VFKSPOLTableItem)aItem.Clone();
            if (spol.ItemFromImport())
            {
                spol.STAV_DAT = SOBR_SPOL_STAV_DAT_EXISTUJICI;
                aWriter.AddSPOL(spol);
            }
            else
            {
                if (spol.STAV_DAT == SOBR_SPOL_STAV_DAT_NEED_UPDATE)
                {
                    if (aParentIsRemoved) return;
                    ResourceParams par = new ResourceParams();
                    par.Add("number", spol.UPLNE_CISLO.ToString());
                    var res = LanguageDictionary.Current.ShowMessageBox("142", par, MessageBoxButton.OKCancel,
                                  MessageBoxImage.Error);
                    if (res == MessageBoxResult.Cancel)
                    {
                        throw new ExportExcetiption(LanguageDictionary.Current.Translate("E1", "Text",
                                                                                         par));
                    }
                    return;
                }
                spol.STAV_DAT = SOBR_SPOL_STAV_DAT_NOVY_BOD;
                aWriter.AddSPOL(spol);
            }
        }
        #endregion
        #region SOBR
        public VFKSOBRTableItem getSouradniceBodu(string aId)
        {
            if (_useImportCache)
            {
                if (_sobrCahce.ContainsKey(aId))
                {
                    return VFKSOBRTable.Items[_sobrCahce[aId]];
                }
                throw new UnExpectException();
            }
            var point = from n in VFKSOBRTable.Items where n.ID == aId select n;
            VFKSOBRTableItem retValue = null;
            foreach (var p in point)
            {
                System.Diagnostics.Debug.Assert(retValue == null);
                retValue = p;
            }
            System.Diagnostics.Debug.Assert(retValue != null);
            return retValue;
        }

        public List<VFKSBPTableItem> getMultiLineSbpPointsByHpId(string hpId)
        {
            List<VFKSBPTableItem> sbpPoint;
            if (_useImportCache)
            {
                List<int> values;
                if (!_hp2SBPTableCache.TryGetValue(hpId, out values))
                    throw new ExportExcetiption("Incorrect import data.");
                sbpPoint = new List<VFKSBPTableItem>(values.Count);
                sbpPoint.AddRange(values.Select(value => VFKSBPTable.Items[value]));
                sbpPoint = sbpPoint.OrderBy(n => n.PORADOVE_CISLO_BODU).ToList();
            }
            else
            {
                sbpPoint = (from n in VFKSBPTable.Items
                            where n.HP_ID == hpId
                            orderby n.PORADOVE_CISLO_BODU
                            select n).ToList();
            }
            return sbpPoint;
        }

        public List<VFKSBPTableItem> getMultiLineSbpPointsByDpmId(string dpmId)
        {
            List<VFKSBPTableItem> sbpPoint;
            if (_useImportCache)
            {
                List<int> values;
                if (!_dpm2SBPTableCache.TryGetValue(dpmId, out values))
                    throw new ExportExcetiption("Incorrect import data.");
                sbpPoint = new List<VFKSBPTableItem>(values.Count);
                sbpPoint.AddRange(values.Select(value => VFKSBPTable.Items[value]));
                sbpPoint = sbpPoint.OrderBy(n => n.PORADOVE_CISLO_BODU).ToList();
            }
            else
            {
                sbpPoint = (from n in VFKSBPTable.Items
                            where n.DPM_ID == dpmId
                            orderby n.PORADOVE_CISLO_BODU
                            select n).ToList();
            }
            return sbpPoint;
        }

        public List<VFKSBPTableItem> getMultiLineSbpPointsByObId(string obId)
        {
            List<VFKSBPTableItem> sbpPoint;
            if (_useImportCache)
            {
                List<int> values;
                if (!_ob2SBPTableCache.TryGetValue(obId, out values))
                    throw new ExportExcetiption("Incorrect import data.");
                sbpPoint = new List<VFKSBPTableItem>(values.Count);
                sbpPoint.AddRange(values.Select(value => VFKSBPTable.Items[value]));
                sbpPoint = sbpPoint.OrderBy(n => n.PORADOVE_CISLO_BODU).ToList();
            }
            else
            {
                sbpPoint = (from n in VFKSBPTable.Items
                            where n.OB_ID == obId
                            orderby n.PORADOVE_CISLO_BODU
                            select n).ToList();
            }
            return sbpPoint;
        }

        public VFKSOBRTableItem updateSOBR(VFKSOBRTableItem value)
        {
            if (value == null)
            {
                VFKSOBRTableItem newItem = new VFKSOBRTableItem();
                newItem.ID = VFKSOBRTable.getMaxValueFromBlock().ToString();
                newItem.KATUZE_KOD = VFKDataContext.FSU;
                newItem.CISLO_TL = UInt32.MaxValue;
                newItem.CISLO_ZPMZ = VFKDataContext.CisloZPMZ;
                newItem.STAV_DAT = SOBR_SPOL_STAV_DAT_NOVY_BOD;
                VFKSOBRTable.addTableData(newItem);
                return newItem;
            }
            Debug.Assert(!VFKSOBRTable.Items.Contains(value));
            VFKSOBRTable.addTableData(value);
            return value;
        }
        public void DeleteSobr(VFKSOBRTableItem value)
        {
            var sobr = (from n in VFKSOBRTable.Items where n.ID == value.ID select n).ToList();
            Debug.Assert(sobr.Count == 1 && sobr[0].STAV_DAT == SOBR_SPOL_STAV_DAT_NOVY_BOD);
            VFKSOBRTable.Items.RemoveAll(x => x.ID == value.ID);
        }
        private void WExportSOBR(IVFKWriter aWriter, VFKSOBRTableItem aItem, bool aParentIsRemoved)
        {
            aItem = (VFKSOBRTableItem)aItem.Clone();
            if (aItem.ItemFromImport())
            {
                aItem.STAV_DAT = SOBR_SPOL_STAV_DAT_EXISTUJICI;
                aWriter.AddSOBR(aItem);
            }
            else
            {
                aItem.STAV_DAT = SOBR_SPOL_STAV_DAT_NOVY_BOD;
                aWriter.AddSOBR(aItem);
            }
            var spols = from n in VFKSPOLTable.Items where n.ID == aItem.ID select n;
            if (spols.Count() != 1)
            {
                ResourceParams par = new ResourceParams();
                par.Add("number", aItem.UPLNE_CISLO.ToString());
                throw new ExportExcetiption(LanguageDictionary.Current.Translate("E2", "Text", par));
            }
            WExportSPOL(aWriter, spols.First(), aParentIsRemoved);
        }
        public void WExportSOBR(IVFKWriter aWriter)
        {
            foreach (var sobrT in VFKSOBRTable.Items)
            {
                if (sobrT.ItemFromImport())
                    continue;
                var sobr = (VFKSOBRTableItem)sobrT.Clone();
                sobr.STAV_DAT = SOBR_SPOL_STAV_DAT_NOVY_BOD;
                aWriter.AddSOBR(sobr);
            }
        }
        #endregion
        #region ZPMZ
        public void WExportZPMZ(IVFKWriter aWriter)
        {
            VFKZPMZTableItem zpmz = new VFKZPMZTableItem();
            zpmz.CISLO_ZPMZ = VFKDataContext.CisloZPMZ;
            zpmz.KATUZE_KOD = VFKDataContext.FSU;
            zpmz.STAV_ZPMZ = 1;
            zpmz.TYPSOS_KOD = (UInt32)VFKDataContext.SouradnicovySystem;
            aWriter.AddZPMZ(zpmz);
        }
        #endregion
        #region Bdp
        List<VFKBDPTableItem> _bdbModifyTableItems = new List<VFKBDPTableItem>();
        public VFKBDPTableItem GetNewBdp()
        {
            VFKBDPTableItem bdp = new VFKBDPTableItem();
            bdp.STAV_DAT = STAV_DAT_BUDOUCNOST;
            bdp.PRIZNAK_KONTEXTU = PRIZNAK_KONTEXTU_N;
            bdp.VYMERA = UInt32.MaxValue;
            _bdbModifyTableItems.Add(bdp);
            return bdp;
        }
        public void DeleteBdp(VFKBDPTableItem bdp)
        {
            if (bdp.ItemFromImport())
            {
                VFKBDPTableItem deltedBdp = (VFKBDPTableItem)bdp.Clone();
                deltedBdp.PRIZNAK_KONTEXTU = PRIZNAK_KONTEXTU_Z;
                deltedBdp.STAV_DAT = STAV_DAT_BUDOUCNOST;
                _bdbModifyTableItems.Add(deltedBdp);
            }
            else
            {
                _bdbModifyTableItems.Remove(bdp);
            }
        }
        public void DeleteAllBdp(string parId)
        {
            var bdps = (from n in VFKBDPTable.Items where n.PAR_ID == parId select n).ToList();
            foreach (var bdp in bdps)
            {
                DeleteBdp(bdp);
            }
        }
        public void RestoreBdpAfterRemoveParcelFromEdit(string parId)
        {
            _bdbModifyTableItems.RemoveAll(x => x.PAR_ID == parId);
        }
        public List<VFKBDPTableItem> getExistBDPAItems(VFKPARTableItem aPar)
        {
            var returnBdp =
                (from n in VFKBDPTable.Items where n.PAR_ID == aPar.ID && n.STAV_DAT == STAV_DAT_PRITOMNOST select n).
                    ToList();
            foreach (var bdpModify in _bdbModifyTableItems)
            {
                if (bdpModify.PAR_ID != aPar.ID)
                    continue;
                if (bdpModify.PRIZNAK_KONTEXTU == PRIZNAK_KONTEXTU_N)
                    returnBdp.Add(bdpModify);
                if (bdpModify.PRIZNAK_KONTEXTU == PRIZNAK_KONTEXTU_Z)
                {
                    var ret = (from n in returnBdp
                               where
                                   n.BPEJ_KOD == bdpModify.BPEJ_KOD && n.PAR_ID == bdpModify.PAR_ID &&
                                   n.RIZENI_ID_VZNIKU == bdpModify.RIZENI_ID_VZNIKU &&
                                   n.RIZENI_ID_ZANIKU == bdpModify.RIZENI_ID_ZANIKU &&
                                   n.VYMERA == bdpModify.VYMERA
                               select n).ToList();
                    if (ret.Count > 0)
                        returnBdp.Remove(ret.First());
                }
            }
            return returnBdp;
        }
        private void WExportBdp(IVFKWriter aWriter, VFKPARTableItem par)
        {
            var bdps = from n in _bdbModifyTableItems where n.PAR_ID == par.ID select n;
            foreach (var bdp in bdps)
            {
                var bdpc = (VFKBDPTableItem)bdp.Clone();
                switch (bdpc.PRIZNAK_KONTEXTU)
                {
                    case PRIZNAK_KONTEXTU_N:
                        bdpc.DATUM_VZNIKU = DateTime.Now;
                        bdpc.STAV_DAT = STAV_DAT_BUDOUCNOST;
                        break;
                    case PRIZNAK_KONTEXTU_Z:
                        bdpc.STAV_DAT = STAV_DAT_PRITOMNOST;
                        break;
                    default:
                        throw new UnExpectException();
                }
                aWriter.AddBDP(bdpc);
            }
        }
        #endregion
        #region IEditParcel PAR
        public VFKMAPLISTable GetMapListTable()
        {
            return VFKMAPLISTable;
        }
        public VFKTELTableItem GetTELItem(string aTelId)
        {
            var tel = from n in VfkTelTable.Items where n.ID == aTelId select n;
            if (tel.Count() == 0)
                return null;
            else
            {
                return tel.First();
            }
        }
        public List<VFKPARTableItem> GetPARItems()
        {

            UInt32 stavDataShow = (UInt32)VFKDataContext.CilImportuEntrie.StavData;
            return (from n in VFKPARTable.Items where n.STAV_DAT == stavDataShow select n).ToList();
        }
        public VFKPARTableItem GetNewParcel()
        {
            VFKPARTableItem newPar = new VFKPARTableItem();
            newPar.ID = VFKPARTable.getMaxValueFromBlock().ToString();
            VFKPARTable.UpdateMaxValue(newPar.ID);
            newPar.STAV_DAT = STAV_DAT_BUDOUCNOST;
            newPar.PRIZNAK_KONTEXTU = PRIZNAK_KONTEXTU_N;
            newPar.KATUZE_KOD = UInt32.MaxValue;
            newPar.KATUZE_KOD_PUV = UInt32.MaxValue;
            newPar.DRUH_CISLOVANI_PAR = UInt32.MaxValue;
            newPar.KMENOVE_CISLO_PAR = UInt32.MaxValue;
            newPar.ZDPAZE_KOD = UInt32.MaxValue;
            newPar.PODDELENI_CISLA_PAR = UInt32.MaxValue;
            newPar.DIL_PARCELY = UInt32.MaxValue;
            newPar.ZPURVY_KOD = 0;
            newPar.DRUPOZ_KOD = UInt32.MaxValue;
            newPar.ZPVYPA_KOD = UInt32.MaxValue;
            newPar.TYP_PARCELY = UInt32.MaxValue;
            newPar.VYMERA_PARCELY = 0;
            newPar.CENA_NEMOVITOSTI = UInt64.MaxValue;
            newPar.IDENT_BUD = "n";
            newPar.SOUCASTI = "n";
            newPar.PS_ID = "";
            newPar.IDENT_PS = "n";
            return newPar;
        }
        public VFKDataContext GetDataContext()
        {
            return VFKDataContext;
        }
        VFKModifyParcelContext _VFKModifyParcelContext = null;
        public VFKModifyParcelContext ParcelContext
        {
            get
            {
                if (_VFKModifyParcelContext == null)
                    _VFKModifyParcelContext = new VFKModifyParcelContext(this);
                return _VFKModifyParcelContext;
            }
            set
            {
                _VFKModifyParcelContext = value;
            }
        }
        public void WExportPAR(IVFKWriter aWriter)
        {
            if (_VFKModifyParcelContext == null) return;
            foreach (var parT in _VFKModifyParcelContext.ParcelNode)
            {
                switch (parT.ParcelModification)
                {
                    case EditedParcelNode.ParcelModificationEnum.New:
                        {
                            VFKPARTableItem par = (VFKPARTableItem)parT.PAR.Clone();
                            InitBeforeExport(par);
                            aWriter.AddPAR(par);
                        }
                        break;
                    case EditedParcelNode.ParcelModificationEnum.Modify:
                        {
                            var oPar = from n in VFKPARTable.Items where n.ID == parT.PAR.ID select n;
                            Debug.Assert(oPar.Count() != 0);
                            //remove old
                            VFKPARTableItem par = (VFKPARTableItem)oPar.First().Clone();
                            par.STAV_DAT = STAV_DAT_PRITOMNOST;
                            par.PRIZNAK_KONTEXTU = PRIZNAK_KONTEXTU_Z;
                            aWriter.AddPAR(par);
                            //add modify
                            par = (VFKPARTableItem)parT.PAR.Clone();
                            par.STAV_DAT = STAV_DAT_BUDOUCNOST;
                            par.PRIZNAK_KONTEXTU = PRIZNAK_KONTEXTU_N;
                            aWriter.AddPAR(par);
                        }
                        break;
                    case EditedParcelNode.ParcelModificationEnum.Cancel:
                        {
                            var oPar = from n in VFKPARTable.Items where n.ID == parT.PAR.ID select n;
                            System.Diagnostics.Debug.Assert(oPar.Count() != 0);
                            VFKPARTableItem par = (VFKPARTableItem)oPar.First().Clone();
                            par.STAV_DAT = STAV_DAT_PRITOMNOST;
                            par.PRIZNAK_KONTEXTU = PRIZNAK_KONTEXTU_Z;
                            aWriter.AddPAR(par);
                            var bdps = from n in VFKBDPTable.Items where n.PAR_ID == par.ID select n;
                            foreach (var bdp in bdps)
                            {
                                var bdpc = (VFKBDPTableItem)bdp.Clone();
                                bdpc.PRIZNAK_KONTEXTU = PRIZNAK_KONTEXTU_Z;
                                bdpc.STAV_DAT = STAV_DAT_PRITOMNOST;
                                aWriter.AddBDP(bdpc);
                            }
                        }
                        break;
                }
                WExportBdp(aWriter, parT.PAR);
            }
        }
        #endregion
        #region Serialzece & Deserializace
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("VFKDataContext", VFKDataContext, typeof(VFKDataContext));
            info.AddValue("Header", Header, typeof(Header));
            info.AddValue("VFKPARTable", VFKPARTable, typeof(VFKPARTable));
            info.AddValue("VFKBUDTable", VFKBUDTable, typeof(VFKBUDTable));
            info.AddValue("VFKBDPTable", VFKBDPTable, typeof(VFKBDPTable));
            info.AddValue("VFKRZOTable", VFKRZOTable, typeof(VFKRZOTable));
            info.AddValue("VFKSOBRTable", VFKSOBRTable, typeof(VFKSOBRTable));
            info.AddValue("VFKSBPTable", VFKSBPTable, typeof(VFKSBPTable));
            info.AddValue("VFKSBMTable", VFKSBMTable, typeof(VFKSBMTable));
            info.AddValue("VFKHPTable", VFKHPTable, typeof(VFKHPTable));
            info.AddValue("VFKZVBTable", VFKZVBTable, typeof(VFKZVBTable));
            info.AddValue("VFKOBTable", VFKOBTable, typeof(VFKOBTable));
            info.AddValue("VFKOPTable", VFKOPTable, typeof(VFKOPTable));
            info.AddValue("VFKDPMTable", VFKDPMTable, typeof(VFKDPMTable));
            info.AddValue("VFKOBBPTable", VFKOBBPTable, typeof(VFKOBBPTable));
            info.AddValue("VFKZPMZTable", VFKZPMZTable, typeof(VFKZPMZTable));
            info.AddValue("VFKSPOLTable", VFKSPOLTable, typeof(VFKSPOLTable));
            info.AddValue("VFKKATUZETable", VFKKATUZETable, typeof(VFKKATUZETable));
            info.AddValue("VFKMAPLISTable", VFKMAPLISTable, typeof(VFKMAPLISTable));
            info.AddValue("VFKTELTable", VfkTelTable, typeof(VFKTELTable));
            info.AddValue("VFKHBPEJTable", VFKHBPEJTable, typeof(VFKHBPEJTable));
            info.AddValue("VFKOBPEJTable", VFKOBPEJTable, typeof(VFKOBPEJTable));

            info.AddValue("VFKModifyDPMItems", _VFKModifyDPMItems, typeof(List<VFKDPMTableItem>));
            info.AddValue("VFKModifyOPItems", _VFKModifyOPItems, typeof(List<VFKOPTableItem>));
            info.AddValue("VFKModifyOBBPItems", _VFKModifyOBBPItems, typeof(List<VFKOBBPTableItem>));
            info.AddValue("VFKModifyOBItems", _VFKModifyOBItems, typeof(List<VFKOBTableItem>));
            info.AddValue("VFKModifyHBPEJItems", _VFKModifyHBPEJItems, typeof(List<VFKHBPEJTableItem>));
            info.AddValue("VFKModifySBPItems", VFKModifySBPItems, typeof(List<VFKSBPTableItem>));
            info.AddValue("VFKModifySBMItems", VFKModifySBMItems, typeof(List<VFKSBMTableItem>));
            info.AddValue("VFKModifyHPItems", _VFKModifyHPItems, typeof(List<VFKHPTableItem>));
            info.AddValue("VFKModifyZVBItems", _VFKModifyZVBItems, typeof(List<VFKZVBTableItem>));
            info.AddValue("VFKModifyBDPItems", _bdbModifyTableItems, typeof(List<VFKBDPTableItem>));
            info.AddValue("VFKModifyParcelContext", _VFKModifyParcelContext, typeof(VFKModifyParcelContext));
        }
        public VFKMain(SerializationInfo info, StreamingContext ctxt)
        {
            VFKDataContext = (VFKDataContext)info.GetValue("VFKDataContext", typeof(VFKDataContext));
            Header = info.GetValue("Header", typeof(Header)) as Header;
            VFKPARTable = info.GetValue("VFKPARTable", typeof(VFKPARTable)) as VFKPARTable;
            VFKBUDTable = info.GetValue("VFKBUDTable", typeof(VFKBUDTable)) as VFKBUDTable;
            VFKBDPTable = info.GetValue("VFKBDPTable", typeof(VFKBDPTable)) as VFKBDPTable;
            VFKRZOTable = info.GetValue("VFKRZOTable", typeof(VFKRZOTable)) as VFKRZOTable;
            VFKSOBRTable = info.GetValue("VFKSOBRTable", typeof(VFKSOBRTable)) as VFKSOBRTable;
            VFKSBPTable = info.GetValue("VFKSBPTable", typeof(VFKSBPTable)) as VFKSBPTable;
            VFKSBMTable = info.GetValue("VFKSBMTable", typeof(VFKSBMTable)) as VFKSBMTable;
            VFKHPTable = info.GetValue("VFKHPTable", typeof(VFKHPTable)) as VFKHPTable;
            VFKZVBTable = info.GetValue("VFKZVBTable", typeof(VFKZVBTable)) as VFKZVBTable;
            VFKOBTable = info.GetValue("VFKOBTable", typeof(VFKOBTable)) as VFKOBTable;
            VFKOPTable = info.GetValue("VFKOPTable", typeof(VFKOPTable)) as VFKOPTable;
            VFKDPMTable = info.GetValue("VFKDPMTable", typeof(VFKDPMTable)) as VFKDPMTable;
            VFKOBBPTable = info.GetValue("VFKOBBPTable", typeof(VFKOBBPTable)) as VFKOBBPTable;
            VFKZPMZTable = info.GetValue("VFKZPMZTable", typeof(VFKZPMZTable)) as VFKZPMZTable;
            VFKSPOLTable = info.GetValue("VFKSPOLTable", typeof(VFKSPOLTable)) as VFKSPOLTable;
            VFKKATUZETable = info.GetValue("VFKKATUZETable", typeof(VFKKATUZETable)) as VFKKATUZETable;
            VFKMAPLISTable = info.GetValue("VFKMAPLISTable", typeof(VFKMAPLISTable)) as VFKMAPLISTable;
            VfkTelTable = info.GetValue("VFKTELTable", typeof(VFKTELTable)) as VFKTELTable;
            VFKHBPEJTable = info.GetValue("VFKHBPEJTable", typeof(VFKHBPEJTable)) as VFKHBPEJTable;
            VFKOBPEJTable = info.GetValue("VFKOBPEJTable", typeof(VFKOBPEJTable)) as VFKOBPEJTable;

            _VFKModifyDPMItems = info.GetValue("VFKModifyDPMItems", typeof(List<VFKDPMTableItem>)) as List<VFKDPMTableItem>;
            _VFKModifyOPItems = info.GetValue("VFKModifyOPItems", typeof(List<VFKOPTableItem>)) as List<VFKOPTableItem>;
            _VFKModifyOBBPItems = info.GetValue("VFKModifyOBBPItems", typeof(List<VFKOBBPTableItem>)) as List<VFKOBBPTableItem>;
            _VFKModifyOBItems = info.GetValue("VFKModifyOBItems", typeof(List<VFKOBTableItem>)) as List<VFKOBTableItem>;
            _VFKModifyHBPEJItems = info.GetValue("VFKModifyHBPEJItems", typeof(List<VFKHBPEJTableItem>)) as List<VFKHBPEJTableItem>;
            VFKModifySBPItems = info.GetValue("VFKModifySBPItems", typeof(List<VFKSBPTableItem>)) as List<VFKSBPTableItem>;
            VFKModifySBMItems = info.GetValue("VFKModifySBMItems", typeof(List<VFKSBMTableItem>)) as List<VFKSBMTableItem>;
            _VFKModifyHPItems = info.GetValue("VFKModifyHPItems", typeof(List<VFKHPTableItem>)) as List<VFKHPTableItem>;
            _VFKModifyZVBItems = info.GetValue("VFKModifyZVBItems", typeof(List<VFKZVBTableItem>)) as List<VFKZVBTableItem>;
            _bdbModifyTableItems = info.GetValue("VFKModifyBDPItems", typeof(List<VFKBDPTableItem>)) as List<VFKBDPTableItem>;
            _VFKModifyParcelContext = info.GetValue("VFKModifyParcelContext", typeof(VFKModifyParcelContext)) as VFKModifyParcelContext;
        }
        #endregion
        #region IDeserializationCallback Members
        public void OnDeserialization(object sender)
        {
            if (_VFKModifyParcelContext != null)
                _VFKModifyParcelContext.IEditParcel = this;
        }
        #endregion

        public void ImportEditedParcel()
        {
            if (VFKDataContext.CilImportuEntrie.StavData != StavData.Budoucnost) return;
            _bdbModifyTableItems = VFKBDPTable.Items;
            VFKBDPTable.ReinitItems();
            var modifiedParcels = VFKPARTable.Items.GroupBy((n) => n.ID);
            foreach (var modifiedParcel in modifiedParcels)
            {
                var par = modifiedParcel.ToList();
                EditedParcelNode newNode = null;
                switch (par.Count)
                {
                    case 1:
                        {
                            newNode = new EditedParcelNode(this, par[0]);
                            newNode.ParcelModification = par[0].PRIZNAK_KONTEXTU == PRIZNAK_KONTEXTU_N
                                ? EditedParcelNode.ParcelModificationEnum.New
                                : EditedParcelNode.ParcelModificationEnum.Cancel;
                        }
                        break;
                    case 2:
                        {
                            newNode = new EditedParcelNode(this, par[0].PRIZNAK_KONTEXTU == PRIZNAK_KONTEXTU_N ? par[0] : par[1]);
                            newNode.ParcelModification = EditedParcelNode.ParcelModificationEnum.Modify;
                        }
                        break;
                    default:
                        continue;
                }
                ParcelContext.ParcelNode.Add(newNode);
            }
        }
    }
}
