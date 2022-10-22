using System.Windows.Controls;
using System.Windows.Documents;

namespace GeoHelper.Printing
{
    public interface IPrinting
    {
        int PageCount { get; }
        void Preprocess(PrintingPaginator paginator);
        UserControl GetPage(int pageNumber);
        DocumentPage GetDocumentPage(int pageNumber);
    }
}