using System.Drawing.Printing;
using System.Security;
using System.Windows.Controls;
using System.Windows.Documents.Serialization;
using System.Printing;
using System.Windows.Xps;
using System.Security;

namespace GeoBase.Extensions
{

    public class GoePrintDialog 
    {
/*        public static bool? ShowDialogExt()
        {
            return true;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private XpsDocumentWriter CreateWriter(string description)
        {
            PrintQueue printQueue = null;
            PrintTicket printTicket = null;
            XpsDocumentWriter writer = null;
            //            this.PickCorrectPrintingEnvironment(ref printQueue, ref printTicket);
            new PrintingPermission(PrintingPermissionLevel.DefaultPrinting).Assert();
            try
            {
                if (printQueue != null)
                {
                    printQueue.CurrentJobSettings.Description = description;
                }
                writer = PrintQueue.CreateXpsDocumentWriter(printQueue);
                //PrintDlgPrintTicketEventHandler handler = new PrintDlgPrintTicketEventHandler(printTicket);
                //writer.WritingPrintTicketRequired += new WritingPrintTicketRequiredEventHandler(handler.SetPrintTicket);
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            return writer;            
        }

        [SecurityCritical]
        private bool _dialogInvoked;


        [SecurityCritical]
        private void PickCorrectPrintingEnvironment(ref PrintQueue printQueue, ref PrintTicket printTicket)
        {
            if (!this._dialogInvoked)
            {
                try
                {
                    SecurityHelper.DemandPrintDialogPermissions();
                }
                catch (SecurityException)
                {
                    throw new PrintDialogException(SR.Get("PartialTrustPrintDialogMustBeInvoked"));
                }
            }
            if (this._printQueue == null)
            {
                this._printQueue = this.AcquireDefaultPrintQueue();
            }
            if (this._printTicket == null)
            {
                this._printTicket = this.AcquireDefaultPrintTicket(this._printQueue);
            }
            printQueue = this._printQueue;
            printTicket = this._printTicket;
        }
        */
 

 

    }

}
