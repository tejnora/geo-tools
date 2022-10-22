using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GeoHelper.Printing
{
    public class PrintingPaginator : DocumentPaginator
    {
        public PrintingPaginator(IPrinting printDocument, PrintSetting printSetting)
        {
            _printDocument = printDocument;
            PrintSetting = printSetting;
            _pageSize = new Size(printSetting.PaperSizeWidth, printSetting.PaperSizeHeigth);
            _printDocument.Preprocess(this);
        }

        readonly IPrinting _printDocument;
        public bool DocumentPageDirect { get; set; }
        public PrintSetting PrintSetting { get; set; }

        Size _pageSize;

        public override bool IsPageCountValid
        {
            get { return true; }
        }

        public override int PageCount
        {
            get { return _printDocument.PageCount; }
        }

        public override Size PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public override IDocumentPaginatorSource Source
        {
            get { return null; }
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            if (DocumentPageDirect)
            {
                return _printDocument.GetDocumentPage(pageNumber);
            }
            UserControl page = _printDocument.GetPage(pageNumber);
            page.Width = PageSize.Width;
            page.Height = PageSize.Height;
            page.Margin = new Thickness(PageElemnt.toPixel(PrintSetting.Margins.Left),
                                        PageElemnt.toPixel(PrintSetting.Margins.Top),
                                        PageElemnt.toPixel(PrintSetting.Margins.Right),
                                        PageElemnt.toPixel(PrintSetting.Margins.Bottom));
            page.Measure(PageSize);
            page.Arrange(new Rect(new Point(0, 0), PageSize));

            return new DocumentPage(page);
        }
    }
}