using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using GeoBase.Gui;
using GeoBase.Localization;
using GeoBase.Utils;
using GeoHelper.Controls;
using GeoHelper.FileParses;
using GeoHelper.Tables;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Tabulky;
using GeoHelper.Utils;
using Action = System.Action;

namespace GeoHelper.Tools
{
    public partial class FileTransferDialog : DialogBase
    {
        public FileTransferDialog(IMainWindow aOwner)
            : base("PrenosSouboruDialog")
        {
            InitializeComponent();
            _textBoxData.Document.Blocks.Clear();
            _owner = aOwner;
        }

        readonly object _isCancledDownloadLock = new object();
        readonly IMainWindow _owner;
        bool _isCancledDownload;
        SerialPort _port;
        FileTransferDownloadDialog _stahovaniDatDialog;
        string _ukoncovaciRetezec = string.Empty;

        string _downloadData = string.Empty;
        List<string> _nodesToTopcon;

        void OnNastaveniPrenosu(object sender, RoutedEventArgs e)
        {
            var dialog = new FileTransferSettingsDialog();
            dialog.ShowDialog();
        }

        void OnNacistData(object sender, RoutedEventArgs e)
        {
            _textBoxData.Document.Blocks.Clear();
            try
            {
                _downloadData = string.Empty;
                _stahovaniDatDialog = new FileTransferDownloadDialog(this);
                InitSerialPort(true);
                _port.DataReceived += DataReceived;
                _port.Open();
                lock (_isCancledDownloadLock)
                    _isCancledDownload = false;
                _stahovaniDatDialog.ShowDialog();
                lock (_isCancledDownloadLock)
                    _isCancledDownload = true;
                if (_port.IsOpen)
                    _port.Close();
                UpdateDownloadedData(_downloadData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string updateText = string.Empty;
            _stahovaniDatDialog.BeginDownload();
            lock (_isCancledDownloadLock)
            {
                if (_isCancledDownload)
                    return;
                var bBuffer = new List<byte>();
                while (_port.BytesToRead > 0)
                    bBuffer.Add((byte) _port.ReadByte());
                _downloadData += Encoding.ASCII.GetString(bBuffer.ToArray());
                int indexUkoncovacihoRetezce = _downloadData.IndexOf(_ukoncovaciRetezec);
                if (indexUkoncovacihoRetezce != -1)
                {
                    _downloadData = _downloadData.Substring(0, indexUkoncovacihoRetezce);
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(_stahovaniDatDialog.Close));
                    return;
                }
                int index = _downloadData.LastIndexOf("\r\n");
                if (index != -1)
                {
                    updateText = _downloadData.Substring(0, index + 2);
                    _downloadData = _downloadData.Substring(index + 2);
                }
            }
            if (updateText.Length != 0)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new TimeSpan(0, 0, 0, 0, 500),
                                  new Action<string>(UpdateDownloadedData), updateText);
            }
        }

        void UpdateDownloadedData(string data)
        {
            if (_isCancledDownload)
                return;
            string[] lines = data.Split('\n');
            foreach (string line in lines)
            {
                var par = new Paragraph();
                if (line.Length == 0)
                    continue;
                par.Inlines.Add(new Run(line.Substring(0, line.Length - 1))); //remove \r
                _textBoxData.Document.Blocks.Add(par);
            }
            _textBoxData.ScrollToEnd();
        }

        void OnNahraniData(object sender, RoutedEventArgs e)
        {
            _nodesToTopcon = GetContentAsStringList();
            int errorIndex = 0;
            if (!Validate(_nodesToTopcon, ref errorIndex))
            {
                LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
                var par = new ResourceParams();
                par.Add("radek", errorIndex.ToString());
                MessageBox.Show(this, dictionary.Translate("393", "Text", par),
                                dictionary.Translate<string>("393", "Caption"), MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }
            var dlg = new ProgressBarDialog(TransferToTopcon);
            dlg.ShowDialog();
        }

        void TransferToTopcon(ProgressBarDialog dialog, CancellationToken ct)
        {
            try
            {
                InitSerialPort(true);
                _port.Open();
                double count = _textBoxData.Document.Blocks.Count;
                double counter = 0;
                foreach (string line in _nodesToTopcon)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(line + "\r\n");
                    _port.Write(bytes, 0, bytes.Length);
                    if (ct.IsCancellationRequested)
                        break;
                    dialog.ProgressBarValue = ++counter/count*100.0;
                    Thread.Sleep(500);
                }
                byte[] bytess = Encoding.ASCII.GetBytes(_ukoncovaciRetezec);
                _port.Write(bytess, 0, bytess.Length);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                                                                            {
                                                                                MessageBox.Show(this, ex.Message,
                                                                                                "Error",
                                                                                                MessageBoxButton.OK,
                                                                                                MessageBoxImage.Error);
                                                                                dialog.Close();
                                                                            }));
                return;
            }
            finally
            {
                if (_port.IsOpen)
                    _port.Close();
            }
            dialog.ProgressBarValue = 100;
        }

        protected void OnVytvoritSeznamMereni(object sender, EventArgs args)
        {
            try
            {
                var parser = new NiconRowParser();
                parser.parseLines(GetContentAsStringList().ToArray());
                _owner.AddTable(new MeasureListTable(_owner, parser.Records));
            }
            catch (ParseException ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected void OnVytvoritSeznamSouradnic(object sender, EventArgs args)
        {
            try
            {
                var parser = new NiconRowParser();
                parser.parseLines(GetContentAsStringList().ToArray());
                _owner.AddTable(new CoordinateListTable(_owner, parser.Records));
            }
            catch (ParseException ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        List<string> GetContentAsStringList()
        {
            var result = new List<string>();
            foreach (Block par in _textBoxData.Document.Blocks)
            {
                if (!(par is Paragraph))
                    continue;
                var pargraph = (Paragraph) par;
                string line = string.Empty;
                foreach (Inline text in pargraph.Inlines)
                {
                    if (!(text is Run))
                        continue;
                    var textText = text as Run;
                    line += textText.Text;
                }
                result.Add(line);
            }
            return result;
        }

        bool Validate(List<string> items, ref int errorIndex)
        {
            foreach (string item in items)
            {
                errorIndex++;
                if (item.Length == 0)
                    items.Remove(item);
                else
                {
                    //082002120002 700900.190 1050492.530 0.000
                    var reg =
                        new Regex("^[0-9]{1,12} [0-9]{1,6}.[0-9]{1,3} [0-9]{1,7}.[0-9]{1,3} [0-9]{1,10}.[0-9]{1,3}$");
                    if (!reg.IsMatch(item))
                        return false;
                }
            }
            return true;
        }

        void InitSerialPort(bool cteni)
        {
            ProgramOption op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                              "PrenosSouboruNastaveniDialog/Port");
            string port = op.getString("COM1");
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "PrenosSouboruNastaveniDialog/BitsPerSec");
            int boudRate = int.Parse(op.getString("B9600").Substring(1));
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "PrenosSouboruNastaveniDialog/DataBit");
            int dataBit = int.Parse(op.getString("DB8").Substring(2));
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "PrenosSouboruNastaveniDialog/ParityType");
            var parity = (Parity) Enum.Parse(typeof (Parity), op.getString("None"), true);
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                "PrenosSouboruNastaveniDialog/StopBitsType");
            var stopBits = (StopBits) Enum.Parse(typeof (StopBits), op.getString("One"), true);

            if (cteni)
            {
                op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                    "PrenosSouboruNastaveniDialog/UkoncovaciRetezecCteni");
                _ukoncovaciRetezec = ProcessUkoncovaciRetezec(op.getString("<4>"));
            }
            else
            {
                op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                    "PrenosSouboruNastaveniDialog/UkoncovaciRetezecZapis");
                _ukoncovaciRetezec = ProcessUkoncovaciRetezec(op.getString("<4>"));
            }
            _port = new SerialPort(port, boudRate, parity, dataBit, stopBits);
        }

        void OnVymazatTextBoxData(object sender, RoutedEventArgs e)
        {
            _textBoxData.Document.Blocks.Clear();
        }

        string ProcessUkoncovaciRetezec(string value)
        {
            int hexaDeep = 0;
            string hexaSubString = string.Empty;
            string result = string.Empty;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '<')
                    hexaDeep++;
                else if (c == '>')
                {
                    hexaDeep--;
                    if (hexaDeep == 0)
                    {
                        try
                        {
                            int hexValue = Convert.ToInt32(hexaSubString, 16);
                            result += (char) hexValue;
                        }
                        catch (Exception)
                        {
                            result += hexaSubString;
                        }
                        finally
                        {
                            hexaSubString = string.Empty;
                        }
                    }
                }
                else if (hexaDeep > 0)
                    hexaSubString += c;
                else
                    result += c;
            }
            return result;
        }

        void OnTextBoxDataDragEnter(object sender, DragEventArgs args)
        {
            if (args.Data.GetDataPresent("SeznamSouradnicTabulka"))
            {
                args.Effects = DragDropEffects.Copy;
                args.Handled = true;
            }
        }

        void OnTextBoxDragOwer(object sender, DragEventArgs args)
        {
            if (args.Data.GetDataPresent("SeznamSouradnicTabulka"))
            {
                args.Effects = DragDropEffects.Copy;
                args.Handled = true;
            }
        }


        void OnTextBoxDataDrop(object sender, DragEventArgs args)
        {
            if (args.Data.GetDataPresent("SeznamSouradnicTabulka"))
            {
                var table = (DragDropDataObjectBase) args.Data.GetData("SeznamSouradnicTabulka");
                var nodes = new List<TableNodesBase>(((TableBase) table.MdiWindow).Nodes);
                var selectedNodes =
                    (from n in nodes
                     where n.Selected
                     select new {UplneCislo = n.NumberWithPrefix, ((TableCoordinateListNode) n).Y, ((TableCoordinateListNode) n).X, ((TableCoordinateListNode) n).Z}).ToList();
                foreach (var node in selectedNodes)
                {
                    string line = string.Format(CultureInfo.InvariantCulture, "{0} {1:0.000} {2:0.000} {3:0.000}",
                                                node.UplneCislo, double.IsNaN(node.Y) ? 0 : node.Y,
                                                double.IsNaN(node.X) ? 0 : node.X, double.IsNaN(node.Z) ? 0 : node.Z);
                    var par = new Paragraph();
                    par.Inlines.Add(new Run(line));
                    _textBoxData.Document.Blocks.Add(par);
                }
            }
        }
    }
}