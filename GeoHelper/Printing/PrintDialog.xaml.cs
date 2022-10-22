using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Printing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Xps;
using GeoBase.Gui;

namespace GeoHelper.Printing
{
    public partial class PrintDialog : DialogBase
    {
        public PrintDialog()
            : base("PrintDialog", true)
        {
            InitializeComponent();
            DisableDialogResize();
            PrintSetting = new PrintSetting();
            DataContext = this;
        }

        public List<string> PrinterNames { get; set; }
        public PrintSetting PrintSetting { get; set; }

        XpsDocumentWriter CreateWriter(string jobName)
        {
            var printServer = new LocalPrintServer();
            PrintQueue printQueue = printServer.GetPrintQueue(PrintSetting.PrinterSettings.PrinterName);
            printQueue.CurrentJobSettings.CurrentPrintTicket.PageMediaSize =
                new PageMediaSize(PrintSetting.PrintableAreaWidth, PrintSetting.PrintableAreaHeight);
            printQueue.CurrentJobSettings.Description = jobName;
            return PrintQueue.CreateXpsDocumentWriter(printQueue);
        }

        public new bool? ShowDialog()
        {
            FillPrintersAndSetDefaultPrinter();
            bool? result = base.ShowDialog();
            return result;
        }

        public void PrintDocument(DocumentPaginator documentPaginator, string description)
        {
            if (documentPaginator == null)
            {
                throw new ArgumentNullException("documentPaginator");
            }
            XpsDocumentWriter docWriter = CreateWriter(description);

            if (docWriter != null)
            {
                documentPaginator.PageSize = new Size(PrintSetting.PrintableAreaWidth, PrintSetting.PrintableAreaHeight);
                docWriter.Write(documentPaginator);
            }
        }

        void FillPrintersAndSetDefaultPrinter()
        {
            var printServer = new LocalPrintServer();
            PrintQueueCollection printQueuesOnLocalServer = printServer.GetPrintQueues();
            PrinterNames = new List<string>();
            foreach (PrintQueue printer in printQueuesOnLocalServer)
            {
                PrinterNames.Add(printer.Name);
            }
        }

        void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var source = PresentationSource.FromVisual(this) as HwndSource;
            OpenPrinterPropertiesDialog(PrintSetting.PrinterSettings, source.Handle);
        }

        const Int32 DM_OUT_BUFFER = 14;

        [DllImport("kernel32.dll")]
        static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        static extern bool GlobalFree(IntPtr hMem);

        [DllImport("winspool.Drv", EntryPoint = "DocumentPropertiesW", SetLastError = true, ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        static extern int DocumentProperties(IntPtr hwnd, IntPtr hPrinter,
                                             [MarshalAs(UnmanagedType.LPWStr)] string pDeviceName, IntPtr pDevModeOutput,
                                             IntPtr pDevModeInput, int fMode);

        public void OpenPrinterPropertiesDialog(PrinterSettings printerSettings, IntPtr pHandle)
        {
            IntPtr hDevMode = printerSettings.GetHdevmode();
            IntPtr pDevMode = GlobalLock(hDevMode);
            Int32 fMode = 0;
            int sizeNeeded = DocumentProperties(pHandle, IntPtr.Zero, printerSettings.PrinterName, pDevMode, pDevMode,
                                                fMode);
            IntPtr devModeData = Marshal.AllocHGlobal(sizeNeeded);

            fMode = DM_OUT_BUFFER;

            DocumentProperties(pHandle, IntPtr.Zero, printerSettings.PrinterName, devModeData, pDevMode, fMode);
            GlobalUnlock(hDevMode);
            printerSettings.SetHdevmode(devModeData);
            printerSettings.DefaultPageSettings.SetHdevmode(devModeData);
            GlobalFree(hDevMode);
            Marshal.FreeHGlobal(devModeData);
        }
    }
}