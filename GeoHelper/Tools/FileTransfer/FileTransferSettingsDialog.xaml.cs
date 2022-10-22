using System;
using System.IO.Ports;
using System.Windows;
using GeoBase.Gui;
using GeoBase.Utils;
using GeoHelper.Utils;

namespace GeoHelper.Tools
{
    public partial class FileTransferSettingsDialog : DialogBase
    {
        public enum BitsPerSecs
        {
            B110,
            B150,
            B300,
            B600,
            B1200,
            B2400,
            B4800,
            B9600,
            B19200
        }

        public enum DataBits
        {
            DB7,
            DB8
        }

        public enum Ports
        {
            COM1,
            COM2,
            COM3,
            COM4
        }

        public FileTransferSettingsDialog()
            : base("PrenosSoboruNastaveniDialog")
        {
            InitializeComponent();
            Load();
            DataContext = this;
        }

        BitsPerSecs _bitsPerSec;
        DataBits _dataBit;
        Parity _parityType;
        Ports _port;
        StopBits _stopBits;
        string _ukoncovaciRetezecCteni;
        string _ukoncovaciRetezecZapis;

        public Ports Port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged("Port");
            }
        }

        public BitsPerSecs BitsPerSec
        {
            get { return _bitsPerSec; }
            set
            {
                _bitsPerSec = value;
                OnPropertyChanged("BitsPerSec");
            }
        }

        public DataBits DataBit
        {
            get { return _dataBit; }
            set
            {
                _dataBit = value;
                OnPropertyChanged("DataBits");
            }
        }

        public Parity ParityType
        {
            get { return _parityType; }
            set
            {
                _parityType = value;
                OnPropertyChanged("ParityType");
            }
        }

        public StopBits StopBitsType
        {
            get { return _stopBits; }
            set
            {
                _stopBits = value;
                OnPropertyChanged("StopBitsType");
            }
        }

        public string UkoncovaciRetezecCteni
        {
            get { return _ukoncovaciRetezecCteni; }
            set
            {
                _ukoncovaciRetezecCteni = value;
                OnPropertyChanged("UkoncovaciRetezecCteni");
            }
        }

        public string UkoncovaciRetezecZapis
        {
            get { return _ukoncovaciRetezecZapis; }
            set
            {
                _ukoncovaciRetezecZapis = value;
                OnPropertyChanged("UkoncovaciRetezecZapis");
            }
        }

        void Load()
        {
            ProgramOption op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                              "PrenosSouboruNastaveniDialog/Port");
            Port = (Ports)Enum.Parse(typeof(Ports), op.getString("COM1"), true);
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "PrenosSouboruNastaveniDialog/BitsPerSec");
            BitsPerSec = (BitsPerSecs)Enum.Parse(typeof(BitsPerSecs), op.getString("B9600"), true);
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "PrenosSouboruNastaveniDialog/DataBit");
            DataBit = (DataBits)Enum.Parse(typeof(DataBits), op.getString("DB8"), true);
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "PrenosSouboruNastaveniDialog/ParityType");
            ParityType = (Parity)Enum.Parse(typeof(Parity), op.getString("None"), true);
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                "PrenosSouboruNastaveniDialog/StopBitsType");
            StopBitsType = (StopBits)Enum.Parse(typeof(StopBits), op.getString("One"), true);
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                "PrenosSouboruNastaveniDialog/UkoncovaciRetezecCteni");
            UkoncovaciRetezecCteni = op.getString("<4>");
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                "PrenosSouboruNastaveniDialog/UkoncovaciRetezecZapis");
            UkoncovaciRetezecZapis = op.getString("<4>");
        }

        void Save()
        {
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "PrenosSouboruNastaveniDialog/Port",
                                           new ProgramOption(Enum.GetName(typeof(Ports), Port)));
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "PrenosSouboruNastaveniDialog/BitsPerSec",
                                           new ProgramOption(Enum.GetName(typeof(BitsPerSecs), BitsPerSec)));
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "PrenosSouboruNastaveniDialog/DataBit",
                                           new ProgramOption(Enum.GetName(typeof(DataBits), DataBit)));
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "PrenosSouboruNastaveniDialog/ParityType",
                                           new ProgramOption(Enum.GetName(typeof(Parity), ParityType)));
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "PrenosSouboruNastaveniDialog/StopBitsType",
                                           new ProgramOption(Enum.GetName(typeof(StopBits), StopBitsType)));
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser,
                                           "PrenosSouboruNastaveniDialog/UkoncovaciRetezecCteni",
                                           new ProgramOption(UkoncovaciRetezecCteni));
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser,
                                           "PrenosSouboruNastaveniDialog/UkoncovaciRetezecZapis",
                                           new ProgramOption(UkoncovaciRetezecZapis));
        }

        protected override void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            Save();
        }

    }
}