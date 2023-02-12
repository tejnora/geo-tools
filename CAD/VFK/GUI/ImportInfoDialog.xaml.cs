using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using GeoBase.Gui;

namespace VFK.GUI
{
    public partial class ImportInfoDialog : DialogBase
    {
        #region Constructor
        public ImportInfoDialog(VFKMain aVFKMain)
            : base("ImportInfoDialog")
        {
            _VFKMain = aVFKMain;
            InitializeComponent();
            Context = new ImportInfoDialogContext(aVFKMain);
            _TextBox.Foreground = Brushes.Black;
            _TextBox.Document.PageWidth = 1000;
            _TextBox.FontSize = 12;
        }
        #endregion
        #region Property & Fields
        private VFKMain _VFKMain;
        public ImportInfoDialogContext Context { get; set; }
        #endregion
        #region Methods
        public new bool? ShowDialog()
        {
            StringBuilder content = new StringBuilder();
            content.Append("\n");
            content.Append("==========================================================================\n");
            content.Append(string.Format("Import:{0}\n", Context.JmenoSouboru));
            content.Append("==========================================================================\n");
            content.Append("Informace o VFK:\n");
            content.Append("----------------\n");
            content.Append(string.Format("Verze:\t\t\t{0}\n", Context.Verze));
            content.Append(string.Format("Vytvořeno dne:\t\t{0}\n", Context.Vytvoreno));
            content.Append(string.Format("Vytvořil:\t\t\t{0}\n", Context.Vytvoril));
            content.Append(string.Format("Kódová stránka:\t\t{0}\n", Context.KodovaStranka));
            content.Append(string.Format("Export za období:\t\t{0}\n", Context.ExportZaobdobi));
            content.Append(string.Format("Nepotvrzené GP:\t\t{0}\n", Context.NepotrvzeneGP));
            content.Append(string.Format("Datové skupiny:\t\t{0}\n", Context.DatoveSkupiny));
            content.Append("\n\n");
            content.Append("Statistika Importu:\n");
            content.Append("-------------------\n");
            content.Append("Název importovaného Bloku\tNačteno řádek\tImportováno řádek\n");
            content.Append(getStatString("PAR", Context.PAR, _VFKMain.VFKPARTable.Items.Count));
            content.Append(getStatString("BUD", Context.BUD, _VFKMain.VFKBUDTable.Items.Count));
            content.Append(getStatString("ZPOCHN", Context.ZPOCHN, 0));
            content.Append(getStatString("DRUPOZ", Context.DRUPOZ, 0));
            content.Append(getStatString("ZPVYPO", Context.ZPVYPO, 0));
            content.Append(getStatString("ZDPAZE", Context.ZDPAZE, 0));
            content.Append(getStatString("ZPURVY", Context.ZPURVY, 0));
            content.Append(getStatString("TYPBUD", Context.TYPBUD, 0));
            content.Append(getStatString("MAPLIS", Context.MAPLIS, 0));
            content.Append(getStatString("KATUZE", Context.KATUZE, _VFKMain.VFKKATUZETable.Items.Count));
            content.Append(getStatString("OBCE", Context.OBCE, 0));
            content.Append(getStatString("CABU", Context.CABU, 0));
            content.Append(getStatString("CASOBC", Context.CASOBC, 0));
            content.Append(getStatString("OKRESY", Context.OKRESY, 0));
            content.Append(getStatString("KRAJE", Context.KRAJE, 0));
            content.Append(getStatString("NKRAJE", Context.NKRAJE, 0));
            content.Append(getStatString("RZO", Context.RZO, 0));
            content.Append(getStatString("ZPVYBU", Context.ZPVYBU, 0));
            content.Append(getStatString("BDP", Context.BDP, _VFKMain.VFKBDPTable.Items.Count));
            content.Append(getStatString("OPSUB", Context.OPSUB, 0));
            content.Append(getStatString("VLA", Context.VLA, 0));
            content.Append(getStatString("CHAROS", Context.CHAROS, 0));
            content.Append(getStatString("TEL", Context.TEL, 0));
            content.Append(getStatString("JPV", Context.JPV, 0));
            content.Append(getStatString("TYPRAV", Context.TYPRAV, 0));
            content.Append(getStatString("SOBR", Context.SOBR, _VFKMain.VFKSOBRTable.Items.Count));
            content.Append(getStatString("SBP", Context.SBP, _VFKMain.VFKSBMTable.Items.Count));
            content.Append(getStatString("SBM", Context.SBM, _VFKMain.VFKSBMTable.Items.Count));
            content.Append(getStatString("KODCHB", Context.KODCHB, 0));
            content.Append(getStatString("TYPSOS", Context.TYPSOS, 0));
            content.Append(getStatString("HP", Context.HP, _VFKMain.VFKHPTable.Items.Count));
            content.Append(getStatString("OP", Context.OP, _VFKMain.VFKOPTable.Items.Count));
            content.Append(getStatString("OB", Context.OB, _VFKMain.VFKOBTable.Items.Count));
            content.Append(getStatString("DPM", Context.DPM, _VFKMain.VFKDPMTable.Items.Count));
            content.Append(getStatString("OBBP", Context.OBBP, _VFKMain.VFKOBBPTable.Items.Count));
            content.Append(getStatString("TYPPPD", Context.TYPPPD, 0));
            content.Append(getStatString("ZVB", Context.ZVB, 0));
            content.Append(getStatString("POM", Context.POM, 0));
            content.Append(getStatString("SPOM", Context.SPOM, 0));
            content.Append(getStatString("SPOL", Context.SPOL, _VFKMain.VFKSPOLTable.Items.Count));
            content.Append(getStatString("HBPEJ", Context.HBPEJ, 0));
            content.Append(getStatString("OBPEJ", Context.OBPEJ, 0));
            content.Append(getStatString("NZ", Context.NZ, 0));
            content.Append(getStatString("ZPMZ", Context.ZPMZ, 0));
            content.Append(getStatString("NZZP", Context.NZZP, 0));
            content.Append(getStatString("RECI", Context.RECI, 0));
            content.Append(getStatString("DOCI", Context.DOCI, 0));
            content.Append(getStatString("REZBP", Context.REZBP, 0));
            content.Append(getStatString("OBDEBO", Context.OBDEBO, 0));
            content.Append(getStatString("BUDOBJ", Context.BUDOBJ, 0));
            content.Append(getStatString("ADROBJ", Context.ADROBJ, 0));
            content.Append("\n\n");
            content.Append("Chyby pri importu:\n");
            content.Append("-------------------\n");
            content.Append("Číslo řádky\tPopis\n");
            foreach (var error in Context.Errors)
            {
                content.Append(string.Format("{0}. \t\t{1}\n", error._mNumberOfLine, error._mDescription));
            }
            Paragraph par = new Paragraph();
            par.Inlines.Add(new Run(content.ToString()));
            _TextBox.Document.Blocks.Clear();
            _TextBox.Document.Blocks.Add(par);
            return base.ShowDialog();
        }
        private string getStatString(string aName, UInt32 aReaded, int Imported)
        {
            return string.Format("{0}\t\t\t\t{1}\t\t{2}\n", aName, aReaded, Imported);
        }
        #endregion
    }
    public class ImportInfoDialogContext
    {
        VFKMain _vfkMain;
        public ImportInfoDialogContext(VFKMain aVFKMain)
        {
            _vfkMain = aVFKMain;
            Errors = new List<ErrorInfo>();
            JmenoSouboru = string.Empty;
            Verze = string.Empty;
            Vytvoreno = string.Empty;
            Vytvoril = string.Empty;
            KodovaStranka = string.Empty;
            ExportZaobdobi = string.Empty;
            NepotrvzeneGP = true;
            DatoveSkupiny = string.Empty;
        }
        #region Header
        public string JmenoSouboru { get; set; }
        public string Verze { get; set; }
        public string Vytvoreno { get; set; }
        public string Vytvoril { get; set; }
        public string KodovaStranka { get; set; }
        public string ExportZaobdobi { get; set; }
        public bool NepotrvzeneGP { get; set; }
        public string DatoveSkupiny { get; set; }
        #endregion
        #region Pocet Nactenych Radek
        public UInt32 PAR { get; set; }
        public UInt32 BUD { get; set; }
        public UInt32 ZPOCHN { get; set; }
        public UInt32 DRUPOZ { get; set; }
        public UInt32 ZPVYPO { get; set; }
        public UInt32 ZDPAZE { get; set; }
        public UInt32 ZPURVY { get; set; }
        public UInt32 TYPBUD { get; set; }
        public UInt32 MAPLIS { get; set; }
        public UInt32 KATUZE { get; set; }
        public UInt32 OBCE { get; set; }
        public UInt32 CABU { get; set; }
        public UInt32 CASOBC { get; set; }
        public UInt32 OKRESY { get; set; }
        public UInt32 KRAJE { get; set; }
        public UInt32 NKRAJE { get; set; }
        public UInt32 RZO { get; set; }
        public UInt32 ZPVYBU { get; set; }
        public UInt32 BDP { get; set; }
        public UInt32 OPSUB { get; set; }
        public UInt32 VLA { get; set; }
        public UInt32 CHAROS { get; set; }
        public UInt32 TEL { get; set; }
        public UInt32 JPV { get; set; }
        public UInt32 TYPRAV { get; set; }
        public UInt32 SOBR { get; set; }
        public UInt32 SBP { get; set; }
        public UInt32 SBM { get; set; }
        public UInt32 KODCHB { get; set; }
        public UInt32 TYPSOS { get; set; }
        public UInt32 HP { get; set; }
        public UInt32 OP { get; set; }
        public UInt32 OB { get; set; }
        public UInt32 DPM { get; set; }
        public UInt32 OBBP { get; set; }
        public UInt32 TYPPPD { get; set; }
        public UInt32 ZVB { get; set; }
        public UInt32 POM { get; set; }
        public UInt32 SPOM { get; set; }
        public UInt32 SPOL { get; set; }
        public UInt32 HBPEJ { get; set; }
        public UInt32 OBPEJ { get; set; }
        public UInt32 NZ { get; set; }
        public UInt32 ZPMZ { get; set; }
        public UInt32 NZZP { get; set; }
        public UInt32 RECI { get; set; }
        public UInt32 DOCI { get; set; }
        public UInt32 REZBP { get; set; }
        public UInt32 OBDEBO { get; set; }
        public UInt32 BUDOBJ { get; set; }
        public UInt32 ADROBJ { get; set; }
        public UInt32 HPOLYGDATA { get; set; }
        #endregion
        #region Errors
        public struct ErrorInfo
        {
            public ErrorInfo(string aDescription, int aNumberOfLine, int aError)
            {
                _mDescription = aDescription;
                _mNumberOfLine = aNumberOfLine;
                _mError = aError;
            }
            public string _mDescription;
            public int _mNumberOfLine;
            public int _mError;
        }

        public List<ErrorInfo> Errors { get; set; }
        #endregion
    }
}
