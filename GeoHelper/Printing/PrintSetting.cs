using System;
using System.Drawing.Printing;
using Margins = GeoBase.Utils.Margins;

namespace GeoHelper.Printing
{
    public class PrintSetting
    {
        public enum Polozky
        {
            Vsechny,
            Oznacene
        }

        [Flags]
        public enum PrintItemsEnum : uint
        {
            InformaceOSoubor = 0x01
        }

        public PrintSetting()
        {
            Margins = new Margins {Bottom = 15.0, Left = 15.0, Right = 15.0, Top = 15.0};
            Polozka = Polozky.Vsechny;
            PrinterSettings = new PrinterSettings();
            PrintInformaceOSoubor = true;
        }

        PrintItemsEnum _printItems;
        public Margins Margins { get; set; }
        public Polozky Polozka { get; set; }
        public PrinterSettings PrinterSettings { get; set; }

        public double PrintableAreaWidth
        {
            //wpf is 96 pixel per inch and this is in 100 pixel per inch propably
            get { return PrinterSettings.DefaultPageSettings.PrintableArea.Width*0.96; }
        }

        public double PrintableAreaHeight
        {
            get { return PrinterSettings.DefaultPageSettings.PrintableArea.Height*0.96; }
        }

        public double PaperSizeWidth
        {
            get { return PrinterSettings.DefaultPageSettings.PaperSize.Width*0.96; }
        }

        public double PaperSizeHeigth
        {
            get { return PrinterSettings.DefaultPageSettings.PaperSize.Height*0.96; }
        }

        public bool PrintInformaceOSoubor
        {
            get { return GetIsSet(PrintItemsEnum.InformaceOSoubor); }
            set { Set(PrintItemsEnum.InformaceOSoubor, value); }
        }

        bool GetIsSet(PrintItemsEnum column)
        {
            return (_printItems & column) == column;
        }

        void Set(PrintItemsEnum column, bool value)
        {
            if (value)
                _printItems |= column;
            else
            {
                _printItems ^= column;
            }
        }
    }
}