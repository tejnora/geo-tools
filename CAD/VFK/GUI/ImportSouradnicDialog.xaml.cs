using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using CAD.VFK.DrawTools;
using GeoBase.Gui;
using GeoBase.Localization;
using GeoBase.Utils;
using VFK;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CAD.VFK.GUI
{
    public class ImportException : Exception
    {
        public ImportException(string exceptionId, ResourceParams resParams)
        {
            LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
            if (resParams != null)
                _description = dictionary.Translate(exceptionId, "ExceptionText", resParams);
            _description = dictionary.Translate<string>(exceptionId, "ExceptionText");
        }

        private string _description = string.Empty;
        public override string Message
        {
            get { return _description; }
        }
    }

    public partial class ImportSouradnicDialog : DialogBase
    {
        
        public enum ImportTypes
        {
            SouradniceObrazu,
            SouradnicePolohy
        };

                        public ImportSouradnicDialog(VfkActivePointCollection activePoints, Document document)
            : base("VFKImportSouradnicDialog")
        {
            InitializeComponent();
            ActivePoints = activePoints;
            _document = document;
            DataContext = this;
        }
                        Document _document;
        private ImportTypes _importType = ImportTypes.SouradniceObrazu;
        public ImportTypes ImportType
        {
            get { return _importType; }
            set { _importType = value; OnPropertyChanged("ImportType"); }
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
                        private void OnImport(object sender, RoutedEventArgs e)
        {
            if (ActivePoints == null) return;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "TXT files (*.txt)|*.txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result.GetValueOrDefault(false))
            {
                try
                {
                    string[] text = File.ReadAllLines(dlg.FileName, Encoding.ASCII);
                    foreach (var line in text)
                    {
                        if (line.Length == 0) continue;
                        List<string> items = new List<string>();
                        StringBuilder subString = new StringBuilder();
                        for (Int32 i = 0; i < line.Length; i++)
                        {
                            char c = line[i];
                            if (c == ' ')
                            {
                                if (subString.Length != 0)
                                {
                                    items.Add(subString.ToString());
                                    subString.Length = 0;
                                }
                            }
                            else
                            {
                                subString.Append(c);
                            }
                        }
                        if (subString.Length != 0)
                        {
                            items.Add(subString.ToString());
                            subString.Length = 0;
                        }
                        if ((items.Count != 4 && AppendKodChyb) || (!AppendKodChyb && !(items.Count == 3 || items.Count == 4)))
                        {
                            ResourceParams param = new ResourceParams();
                            param.Add("value", line);
                            param.Add("msg", LanguageDictionary.Current.Translate<string>("99", "Text"));
                            LanguageDictionary.Current.ShowMessageBox("97", param, MessageBoxButton.OK,
                                                                      MessageBoxImage.Warning);
                            break;
                        }
                        try
                        {
                            switch (ImportType)
                            {
                                case ImportTypes.SouradniceObrazu:
                                    {
                                        var activePoint = new VfkActivePoint();
                                        var res = from n in ActivePoints
                                                  where n.VfkPointName == items[0].Trim()
                                                  select n;
                                        if (res.Any())
                                        {
                                            var par = new ResourceParams();
                                            par.Add("number", items[0].Trim());
                                            throw new ImportException("E3", par);
                                        }
                                        ActivePoints.Insert(ActivePoints.Count, activePoint);
                                        activePoint.VfkPointName = items[0].Trim();
                                        activePoint.VfkSouradniceObrazuY = items[1].Trim();
                                        activePoint.VfkSouradniceObrazuX = items[2].Trim();
                                        activePoint.VfkSouradnicePolohyY = items[1].Trim();
                                        activePoint.VfkSouradnicePolohyX = items[2].Trim();
                                        if (AppendKodChyb)
                                            activePoint.VfkKkb = items[3].Trim();
                                    } break;
                                case ImportTypes.SouradnicePolohy:
                                    {
                                        var res = from n in ActivePoints
                                                  where n.VfkPointName == items[0].Trim()
                                                  select n;
                                        if (res.Count() == 0)
                                        {
                                            var activePoint = new VfkActivePoint();
                                            ActivePoints.Insert(ActivePoints.Count, activePoint);
                                            activePoint.VfkPointName = items[0].Trim();
                                            activePoint.VfkSouradniceObrazuY = items[1].Trim();
                                            activePoint.VfkSouradniceObrazuX = items[2].Trim();
                                            activePoint.VfkSouradnicePolohyY = items[1].Trim();
                                            activePoint.VfkSouradnicePolohyX = items[2].Trim();
                                            if (AppendKodChyb)
                                                activePoint.VfkKkb = items[3].Trim();
                                        }
                                        else
                                        {
                                            var l = res.ToList();
                                            var activePoint = l[0];
                                            activePoint.VfkSouradnicePolohyY = items[1].Trim();
                                            activePoint.VfkSouradnicePolohyX = items[2].Trim();
                                            if (activePoint.VfkItem.VfkSpolItem.STAV_DAT == VFKMain.SOBR_SPOL_STAV_DAT_NEED_UPDATE)
                                                activePoint.VfkItem.VfkSpolItem.STAV_DAT = VFKMain.SOBR_SPOL_STAV_DAT_NOVY_BOD;
                                            if (AppendKodChyb)
                                                activePoint.VfkKkb = items[3].Trim();
                                        }
                                    } break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        catch (Exception ex)
                        {
                            ResourceParams param = new ResourceParams();
                            param.Add("value", line);
                            param.Add("msg", ex.Message);
                            LanguageDictionary.Current.ShowMessageBox("97", param, MessageBoxButton.OK,
                                                                      MessageBoxImage.Warning);
                            _document.DataModel.StopUndoRedo();
                            _document.DataModel.DoUndo();
                            _document.DataModel.StarUndoRedo();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            OnOkButtonClick(sender, e);
        }
            }
}
