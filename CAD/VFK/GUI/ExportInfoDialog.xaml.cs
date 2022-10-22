using System;
using System.Text;
using System.Windows.Documents;
using GeoBase.Gui;

namespace CAD.VFK.GUI
{
    public partial class ExportInfoDialog : DialogBase
    {
        #region Constructor
        public ExportInfoDialog(ExportInfoDialogContext context)
            : base("ExportInfoDialog")
        {
            InitializeComponent();
            Context = context;
        }
        #endregion
        #region Property
        public ExportInfoDialogContext Context { get; set; }
        #endregion
        #region Methods
        public new bool? ShowDialog()
        {
            StringBuilder content = new StringBuilder();
            content.Append("\n");
            content.Append("Statistika exportu:\n");
            content.Append("------------------------------------------------------------------------------------------------------------------\n");
            content.Append(string.Format("Čas exportu:\t\t{0}\n", Context.CasExportu));
            content.Append("\n\n");
            content.Append("Statistika Exportu:\n");
            content.Append("------------------------------------------------------------------------------------------------------------------\n");
            content.Append("Nazev\t\t\tNový\t\tRušený\t\tAktualizovaný\n");
            content.Append(getStatString("PAR", Context.PAR));
            content.Append(getStatString("BUD", Context.BUD));
            content.Append(getStatString("RZO", Context.RZO));
            content.Append(getStatString("SBP", Context.SBP));
            content.Append(getStatString("SBM", Context.SBM));
            content.Append(getStatString("HP", Context.HP));
            content.Append(getStatString("OP", Context.OP));
            content.Append(getStatString("OB", Context.OB));
            content.Append(getStatString("DPM", Context.DPM));
            content.Append(getStatString("OBBP", Context.OBBP));
            content.Append(getStatString("ZVB", Context.ZVB));
            content.Append(getStatString("BDP", Context.BDP));
            content.Append(getStatString("HBPEJ", Context.HBPEJ));
            content.Append("------------------------------------------------------------------------------------------------------------------\n");
            content.Append("Nazev\t\t\tExistující\t\tNový\n");
            content.Append(getStatString("SOBR", Context.SOBR));
            content.Append(getStatString("SPOL", Context.SPOL));
            content.Append("------------------------------------------------------------------------------------------------------------------\n");
            content.Append("Nazev\t\t\tRušený\t\tSoučasný\tBudoucí\n");
            content.Append(getStatString("SBM", Context.SBM));
            content.Append("------------------------------------------------------------------------------------------------------------------\n");
            content.Append("Nazev\t\t\tNový\n");            
            content.Append(getStatString("ZPMZ", Context.ZPMZ));
            content.Append("\n\n");
            Paragraph par = new Paragraph();
            par.Inlines.Add(new Run(content.ToString()));
            _TextBox.Document.Blocks.Clear();
            _TextBox.Document.Blocks.Add(par);
            return base.ShowDialog();
        }
        private string getStatString(string aName, ExportInfoDialogContext.PKStavData aExported)
        {
            return string.Format("{0}\t\t\t{1}\t\t{2}\t\t{3}\n", aName, aExported._nove,aExported._rusene,aExported._aktualizovane);
        }
        private string getStatString(string aName, Int32 aExported)
        {
            return string.Format("{0}\t\t\t{1}\n", aName, aExported);
        }
        private string getStatString(string aName, ExportInfoDialogContext.SOBR_SPOL_StavData aExported)
        {
            return string.Format("{0}\t\t\t{1}\t\t{2}\n", aName, aExported._existujici, aExported._novy);
        }

        private string getStatString(string aName, ExportInfoDialogContext.SBM_StavData aExported)
        {
            return string.Format("{0}\t\t\t{1}\t\t{2}\t\t{3}\n", aName, aExported._zrusen, aExported._soucasny,aExported._budouci);
        }
        #endregion
    }

    public class ExportInfoDialogContext
    {
        public ExportInfoDialogContext()
        {
            PAR = new PKStavData();
            BUD = new PKStavData();
            RZO = new PKStavData();
            SBP = new PKStavData();
            HP = new PKStavData();
            OP = new PKStavData();
            OB = new PKStavData();
            DPM = new PKStavData();
            OBBP = new PKStavData();
            ZVB = new PKStavData();
            SPOL = new SOBR_SPOL_StavData();
            SOBR = new SOBR_SPOL_StavData();
            SBM = new SBM_StavData();
            BDP = new PKStavData();
            HBPEJ = new PKStavData();
        }
        public DateTime CasExportu
        {
            get; set;
        }

        public class PKStavData
        {
            public Int32 _nove;
            public Int32 _rusene;
            public Int32 _aktualizovane;
        }
        public class SOBR_SPOL_StavData
        {
            public Int32 _novy;
            public Int32 _existujici;
        }
        public class SBM_StavData
        {
            public Int32 _zrusen;
            public Int32 _soucasny;
            public Int32 _budouci;
        }
        #region Pocet Exportovanych Elementu
        public PKStavData PAR { get; set; }
        public PKStavData BUD { get; set; }
        public PKStavData RZO { get; set; }
        public PKStavData BDP { get; set; }
        public SOBR_SPOL_StavData SOBR { get; set; }
        public PKStavData SBP { get; set; }
        public SBM_StavData SBM { get; set; }
        public PKStavData HP { get; set; }
        public PKStavData HBPEJ { get; set; }
        public PKStavData OP { get; set; }
        public PKStavData OB { get; set; }
        public PKStavData DPM { get; set; }
        public PKStavData OBBP { get; set; }
        public Int32 ZPMZ { get; set; }
        public PKStavData ZVB { get; set; }
        public PKStavData NZ { get; set; }
        public SOBR_SPOL_StavData SPOL { get; set; }
        #endregion
    }
}
