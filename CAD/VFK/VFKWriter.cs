using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CAD.VFK.GUI;
using VFK.Tables;
using System.IO;

namespace VFK
{
    public interface IVFKWriter
    {
        void AddHP(VFKHPTableItem aItem);
        void AddSBP(VFKSBPTableItem aItem);
        void AddSbm(VFKSBMTableItem aItem);
        void AddSOBR(VFKSOBRTableItem aItem);
        void AddSPOL(VFKSPOLTableItem aItem);
        void AddDPM(VFKDPMTableItem aItem);
        void AddOP(VFKOPTableItem aItem);
        void AddOB(VFKOBTableItem aItem);
        void AddOBBP(VFKOBBPTableItem aItem);
        void AddZPMZ(VFKZPMZTableItem aItem);
        void AddPAR(VFKPARTableItem aItem);
        void AddBDP(VFKBDPTableItem aItem);
        void AddZVB(VFKZVBTableItem aItem);
        void AddHBPEJ(VFKHBPEJTableItem aItem);
    }
    class VFKWriter : IVFKWriter
    {
        VFKMain _mOwner = null;
        public VFKWriter(VFKMain aOwner)
        {
            _mOwner = aOwner;
            HP = new List<VFKHPTableItem>();
            SBP = new List<VFKSBPTableItem>();
            SOBR = new List<VFKSOBRTableItem>();
            SPOL = new List<VFKSPOLTableItem>();
            DPM = new List<VFKDPMTableItem>();
            OP = new List<VFKOPTableItem>();
            OB = new List<VFKOBTableItem>();
            OBBP =new List<VFKOBBPTableItem>();
            PAR = new List<VFKPARTableItem>();
            BDP = new List<VFKBDPTableItem>();
            ZVB = new List<VFKZVBTableItem>();
            Sbm = new List<VFKSBMTableItem>();
            HBPEJ = new List<VFKHBPEJTableItem>();
        }
        public ExportInfoDialogContext ExportInfoContext { get; set; }
        public void exportVFK(string aLocation)
        {
            ExportInfoContext=new ExportInfoDialogContext();
            _mOwner.WExportHP(this);
            _mOwner.WExportDPM(this);
            _mOwner.WExportOP(this);
            _mOwner.WExportOB(this);
            _mOwner.WExportOBBP(this);
            _mOwner.WExportZPMZ(this);
            _mOwner.WExportPAR(this);
            _mOwner.WExportSOBR(this);
            _mOwner.WExportSPOL(this);
            _mOwner.WExportZVB(this);
            _mOwner.WExportHbpej(this);
            saveToFile(aLocation);
        }
        private void saveToFile(string aLocation)
        {
            try
            {
                Stream fileStream = new FileStream(aLocation, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                var writer = new StreamWriter(fileStream, Encoding.GetEncoding("iso-8859-2"));
                //BEGIN HEADER
                var dateNow = DateTime.Now;
                
                string dateNowString = string.Format("{0:00}.{1:00}.{2:0000} {3:00}:{4:00}:{5:00}", dateNow.Day, dateNow.Month, dateNow.Year, dateNow.Hour,
                                dateNow.Minute, dateNow.Second);
                writer.WriteLine("&HVERZE;\"5.0\"");
                writer.WriteLine(string.Format("&HVYTVORENO;\"{0}\"", dateNowString));
                writer.WriteLine(string.Format("&HPUVOD;\"GEOCAD - {0}\"",Assembly.GetExecutingAssembly().GetName().Version.ToString()));
                writer.WriteLine("&HCODEPAGE;\"WE8ISO8859P2\"");
                writer.WriteLine(string.Format("&HJMENO;\"{0}\"", _mOwner.VFKDataContext.AuthorName));
                List<string> updatedTempItems = new List<string>();
                //END HEADER
                //BEGIN PAR
                Definition def = new Definition("PAR");
                def.SaveDefinitionToStream(writer);
                updatedTempItems.Clear();
                foreach (var par in PAR)
                {
                    writer.WriteLine(def.GetItemDataString(par));
                    if ((from n in PAR where par.ID == n.ID select n).Count() == 2)
                    {
                        if (updatedTempItems.Contains(par.ID))
                            continue;
                        ExportInfoContext.PAR._aktualizovane++;
                        updatedTempItems.Add(par.ID);
                    }
                    else
                        UpdateExportInfo(ExportInfoContext.PAR, par);
                }
                //END PAR
                //BEGIN BUD
                def = new Definition("BUD");
                def.SaveDefinitionToStream(writer);
                //TODO
                //END BUD
                //BEGIN BDP
                def = new Definition("BDP");
                def.SaveDefinitionToStream(writer);
                foreach (var bdp in BDP)
                {
                    writer.WriteLine(def.GetItemDataString(bdp));
                    if (bdp.PRIZNAK_KONTEXTU == 3)
                        ExportInfoContext.BDP._nove++;
                    else if (bdp.PRIZNAK_KONTEXTU == 1)
                        ExportInfoContext.BDP._rusene++;
                }
                //END BDP
                //BEGIN RZO
                def = new Definition("RZO");
                def.SaveDefinitionToStream(writer);
                //TODO
                //END RZO
                //SOBR BEGIN
                def = new Definition("SOBR");
                def.SaveDefinitionToStream(writer);
                foreach (var sobr in SOBR)
                {
                    writer.WriteLine(def.GetItemDataString(sobr));
                    if (sobr.STAV_DAT == 2)
                        ExportInfoContext.SOBR._novy++;
                    else if (sobr.STAV_DAT == 0)
                        ExportInfoContext.SOBR._existujici++;
                }
                //SOBR END
                //SBP BEGIN
                def = new Definition("SBP");
                def.SaveDefinitionToStream(writer);
                foreach (var sbp in SBP)
                {
                    writer.WriteLine(def.GetItemDataString(sbp));
                    if (sbp.PRIZNAK_KONTEXTU == 3)
                        ExportInfoContext.SBP._nove++;
                    else if (sbp.PRIZNAK_KONTEXTU == 1)
                        ExportInfoContext.SBP._rusene++;
                }
                //SBP END
                //SBM BEGIN
                def = new Definition("SBM");
                def.SaveDefinitionToStream(writer);
                foreach (var sbm in Sbm)
                {
                    writer.WriteLine(def.GetItemDataString(sbm));
                    switch(sbm.PRIZNAK_KONTEXTU)
                    {
                        case 1:
                            ExportInfoContext.SBM._zrusen++;
                            break;
                        case 2:
                            ExportInfoContext.SBM._soucasny++;
                            break;
                        case 3:
                            ExportInfoContext.SBM._budouci++;
                            break;
                    }
                }
                //SBM END
                //HP BEGIN
                def= new Definition("HP");
                def.SaveDefinitionToStream(writer);
                updatedTempItems.Clear();
                foreach (var hp in HP)
                {
                    writer.WriteLine(def.GetItemDataString(hp));
                    if ((from n in HP where hp.ID == n.ID select n).Count() == 2)
                    {
                        if (updatedTempItems.Contains(hp.ID))
                            continue;
                        ExportInfoContext.HP._aktualizovane++;
                        updatedTempItems.Add(hp.ID);
                    }
                    else
                        UpdateExportInfo(ExportInfoContext.HP, hp);
                }
                //HP END
                //HBPEJ BEGIN
                def = new Definition("HBPEJ");
                def.SaveDefinitionToStream(writer);
                updatedTempItems.Clear();
                foreach (var hbpej in HBPEJ)
                {
                    writer.WriteLine(def.GetItemDataString(hbpej));
                    if ((from n in HBPEJ where hbpej.ID == n.ID select n).Count() == 2)
                    {
                        if (updatedTempItems.Contains(hbpej.ID))
                            continue;
                        ExportInfoContext.HBPEJ._aktualizovane++;
                        updatedTempItems.Add(hbpej.ID);
                    }
                    else
                        UpdateExportInfo(ExportInfoContext.HBPEJ, hbpej);
                }
                //HBPEJ END
                //OP BEGIN
                def = new Definition("OP");
                def.SaveDefinitionToStream(writer);
                updatedTempItems.Clear();
                foreach (var op in OP)
                {
                    writer.WriteLine(def.GetItemDataString(op));
                    if ((from n in OP where op.ID == n.ID select n).Count() == 2)
                    {
                        if (updatedTempItems.Contains(op.ID))
                            continue;
                        ExportInfoContext.OP._aktualizovane++;
                        updatedTempItems.Add(op.ID);
                    }
                    else
                        UpdateExportInfo(ExportInfoContext.OP, op);
                }
                //OP END
                //OB BEGIN
                def = new Definition("OB");
                def.SaveDefinitionToStream(writer);
                updatedTempItems.Clear();
                foreach (var ob in OB)
                {
                    writer.WriteLine(def.GetItemDataString(ob));
                    if ((from n in OB where ob.ID == n.ID select n).Count() == 2)
                    {
                        if (updatedTempItems.Contains(ob.ID))
                            continue;
                        ExportInfoContext.OB._aktualizovane++;
                        updatedTempItems.Add(ob.ID);
                    }
                    else
                        UpdateExportInfo(ExportInfoContext.OB, ob);
                }
                //OB END
                //DPM BEGIN
                def = new Definition("DPM");
                def.SaveDefinitionToStream(writer);
                updatedTempItems.Clear();
                foreach (var dpm in DPM)
                {
                    writer.WriteLine(def.GetItemDataString(dpm));
                    if ((from n in DPM where dpm.ID == n.ID select n).Count() == 2)
                    {
                        if (updatedTempItems.Contains(dpm.ID))
                            continue;
                        ExportInfoContext.DPM._aktualizovane++;
                        updatedTempItems.Add(dpm.ID);                        
                    }
                    else
                        UpdateExportInfo(ExportInfoContext.DPM, dpm);
                }
                //DPM END
                //ZVB BEGIN
                def = new Definition("ZVB");
                def.SaveDefinitionToStream(writer);
                updatedTempItems.Clear();
                foreach (var zvb in ZVB)
                {
                    writer.WriteLine(def.GetItemDataString(zvb));
                    if ((from n in ZVB where zvb.ID == n.ID select n).Count() == 2)
                    {
                        if (updatedTempItems.Contains(zvb.ID))
                            continue;
                        ExportInfoContext.ZVB._aktualizovane++;
                        updatedTempItems.Add(zvb.ID);                        
                    }
                    else
                        UpdateExportInfo(ExportInfoContext.ZVB, zvb);
                }
                //ZVB END
                //OBBP BEGIN
                def = new Definition("OBBP");
                def.SaveDefinitionToStream(writer);
                updatedTempItems.Clear();
                foreach (var obbp in OBBP)
                {
                    writer.WriteLine(def.GetItemDataString(obbp));
                    if ((from n in OBBP where obbp.ID == n.ID select n).Count() == 2)
                    {
                        if (updatedTempItems.Contains(obbp.ID))
                            continue;
                        ExportInfoContext.OBBP._aktualizovane++;
                        updatedTempItems.Add(obbp.ID);                                               
                    }
                    else
                        UpdateExportInfo(ExportInfoContext.OBBP, obbp);
                }
                //OBBP END
                //BEGION ZPMZ
                def = new Definition("ZPMZ");
                def.SaveDefinitionToStream(writer);
                if(ZPMZ!=null)
                    writer.WriteLine(def.GetItemDataString(ZPMZ));
                ExportInfoContext.ZPMZ++;
                //END ZPMZ
                //SPOL BEGIN
                def = new Definition("SPOL");
                def.SaveDefinitionToStream(writer);
                foreach (var spol in SPOL)
                {
                    writer.WriteLine(def.GetItemDataString(spol));
                    if (spol.STAV_DAT == 2)
                        ExportInfoContext.SPOL._novy++;
                    else if (spol.STAV_DAT == 0)
                        ExportInfoContext.SPOL._existujici++;
                }
                //SPOL END
                writer.Close();
                fileStream.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void UpdateExportInfo(ExportInfoDialogContext.PKStavData aPkStavData, VFKDataTableBaseItemWithProp aItem)
        {
            if (aItem.PRIZNAK_KONTEXTU == 3)
                aPkStavData._nove++;
            else if (aItem.PRIZNAK_KONTEXTU == 1)
                aPkStavData._rusene++;
        }
        #region IVFKWriter
        private List<VFKHPTableItem> HP
        {
            get;
            set;
        }
        public void AddHP(VFKHPTableItem aItem)
        {
            HP.Add(aItem);
        }
        private List<VFKSBPTableItem> SBP
        {
            get;
            set;
        }
        public void AddSBP(VFKSBPTableItem aItem)
        {
            SBP.Add(aItem);
        }
        private List<VFKSBMTableItem> Sbm
        {
            get; set;
        }
        public void AddSbm(VFKSBMTableItem aItem)
        {
            Sbm.Add(aItem);
        }
        private List<VFKSOBRTableItem> SOBR
        {
            get;
            set;
        }
        public void AddSOBR(VFKSOBRTableItem aItem)
        {
            var res = from n in SOBR where n.ID == aItem.ID select n;
            if (res.Count() > 0) return;
            SOBR.Add(aItem);
        }
        private List<VFKSPOLTableItem> SPOL
        {
            get;
            set;
        }
        public void AddSPOL(VFKSPOLTableItem aItem)
        {
            var res = from n in SPOL where n.ID == aItem.ID select n;
            if (res.Count() > 0) return;
            SPOL.Add(aItem);
        }
        private List<VFKDPMTableItem> DPM
        {
            get;
            set;
        }
        public void AddDPM(VFKDPMTableItem aItem)
        {
            var res = from n in DPM where n.ID == aItem.ID select n;
            if (res.Count() > 0) return;
            DPM.Add(aItem);
        }
        private List<VFKOPTableItem> OP
        {
            get;
            set;
        }
        public void AddOP(VFKOPTableItem aItem)
        {
            var res = from n in OP where n.ID == aItem.ID select n;
            if (res.Count() > 0) return;
            OP.Add(aItem);
        }
        private List<VFKOBTableItem> OB
        {
            get;
            set;
        }
        public void AddOB(VFKOBTableItem aItem)
        {
            var res = from n in OB where n.ID == aItem.ID select n;
            if (res.Count() > 0) return;
            OB.Add(aItem);            
        }
        private List<VFKOBBPTableItem> OBBP
        {
            get;
            set;
        }
        public void AddOBBP(VFKOBBPTableItem aItem)
        {
            var res = from n in OBBP where n.ID == aItem.ID select n;
            if (res.Count() > 0) return;
            OBBP.Add(aItem);
        }
        private VFKZPMZTableItem ZPMZ
        {
            get;
            set;
        }
        public void AddZPMZ(VFKZPMZTableItem aItem)
        {
            ZPMZ = aItem;
        }
        private List<VFKPARTableItem> PAR
        {
            get;
            set;
        }
        public void AddPAR(VFKPARTableItem aItem)
        {
            PAR.Add(aItem);
        }
        private List<VFKBDPTableItem> BDP
        {
            get; set;
        }
        public void AddBDP(VFKBDPTableItem aItem)
        {
            BDP.Add(aItem);
        }
        private List<VFKZVBTableItem> ZVB
        {
            get;
            set;
        }
        public void AddZVB(VFKZVBTableItem aItem)
        {
            ZVB.Add(aItem);
        }
        private List<VFKHBPEJTableItem> HBPEJ
        {
            get;
            set;
        }
        public void AddHBPEJ(VFKHBPEJTableItem aItem)
        {
            HBPEJ.Add(aItem);
        }
        #endregion
    }
}
