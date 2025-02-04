using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using CAD.VFK.DrawTools;
using GeoBase.Gui;
using Microsoft.Win32;
using VFK;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CAD.VFK.GUI
{
    public partial class ExportSouradnicDialog : DialogBase
    {
        
        public enum ExportTypes
        {
            SouradniceObrazu,
            SouradnicePolohy
        };

                        public ExportSouradnicDialog(VfkActivePointCollection activePoints)
            : base("VFKExportSouradnicDialog")
        {
            InitializeComponent();
            ActivePoints = activePoints;
            DataContext = this;
        }
                        private ExportTypes _exportType = ExportTypes.SouradniceObrazu;
        public ExportTypes ExportType
        {
            get { return _exportType; }
            set { _exportType = value; OnPropertyChanged("ExportType"); }
        }

        private bool _appendKodChyb = true;
        public bool AppendKodChyb
        {
            get { return _appendKodChyb; }
            set { _appendKodChyb = value; OnPropertyChanged("AppendKodChyb"); }
        }
        private VfkActivePointCollection ActivePoints
        {
            get;
            set;
        }
                        private void OnExport(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "TXT files (*.txt)|*.txt";
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                try
                {
                    Stream fileStream = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                    var writer = new StreamWriter(fileStream, Encoding.GetEncoding("iso-8859-2"));
                    foreach (var point in ActivePoints)
                    {
                        string line = point.VfkItem.PointFullName;
                        switch (ExportType)
                        {
                            case ExportTypes.SouradniceObrazu:
                                line += " " + AdjustToString(point.VfkItem.SouradniceObrazuY, 9);
                                line += " " + AdjustToString(point.VfkItem.SouradniceObrazuX, 10);
                                break;
                            case ExportTypes.SouradnicePolohy:
                                if (point.VfkItem.VfkSpolItem.STAV_DAT == VFKMain.SOBR_SPOL_STAV_DAT_NEED_UPDATE)
                                    continue;
                                line += " " + AdjustToString(point.VfkItem.VfkSouradnicePolohyY, 9);
                                line += " " + AdjustToString(point.VfkItem.VfkSouradnicePolohyX, 10);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        if (AppendKodChyb)
                            line += " " + point.VfkItem.VfkKkb;
                        writer.WriteLine(line);
                    }
                    writer.Close();
                    fileStream.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            OnOkButtonClick(sender, e);
        }
        private string AdjustToString(string value, int pad)
        {
            var number = double.Parse(value);
            var res = string.Format(CultureInfo.InvariantCulture, "{0:0.00}", number);
            return res.PadLeft(pad, '0');
        }
            }
}
