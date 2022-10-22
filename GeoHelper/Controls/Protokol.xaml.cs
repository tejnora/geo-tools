using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using GeoCalculations.MethodPoints;
using GeoHelper.Printing;
using GeoHelper.Protocols;
using GeoHelper.Tabulky;
using GeoHelper.Utils;
using Utils.PartialStream;

namespace GeoHelper.Controls
{
    public partial class Protokol : ILoadSaveMdi, IManipulateItems, IPrinting, IDisposable
    {
        readonly IMainWindow _mainWindow;

        public Protokol(IMainWindow aMainWindow)
        {
            InitializeComponent();
            _mainWindow = aMainWindow;
        }

        public FileInfo FileName { get; set; }
        public Stream FileStream { get; set; }

        public void RestModifedFlag()
        {
            _modfied = false;
        }

        public void Serialize(Stream aWriter)
        {
            FileStream = aWriter;
            aWriter.WriteInt32(1);
            var subStream = PartialStreamFactory.CreateWritePartialStream(aWriter);
            var range = new TextRange(_text.Document.ContentStart, _text.Document.ContentEnd);
            range.Save(subStream, DataFormats.Text);
            subStream.Dispose();
        }

        public void Deserialize(Stream aReader)
        {
            _modfied = false;
            FileStream = aReader;
            Debug.Assert(aReader.ReadInt32() == 1);
            var subStream = PartialStreamFactory.CreateReadPartialStream(aReader);
            var range = new TextRange(_text.Document.ContentStart, _text.Document.ContentEnd);
            range.Load(subStream, DataFormats.Text);
            subStream.Dispose();
        }

        public string SaveDialogFilter
        {
            get { return "Protokol (*.prot)|*.prot"; }
        }

        public bool GetIsModifed()
        {
            return _modfied;
        }

        public bool CanEdit()
        {
            return false;
        }

        public bool CanDelete()
        {
            return true;
        }

        public bool CanInsert()
        {
            return false;
        }

        public void OnInsert()
        {
        }

        public void Delete(bool skipWarnning)
        {
        }

        public void OnEdit()
        {
        }

        public bool CanCopy()
        {
            return true;
        }

        public void Copy()
        {
        }

        public bool CanPaste()
        {
            return true;
        }

        public void Paste()
        {
        }

        public bool CanCut()
        {
            return true;
        }

        public void Cut()
        {
        }

        public bool CanUndo()
        {
            return true;
        }

        public void Undo()
        {
        }

        public bool CanRedo()
        {
            return true;
        }

        public void Redo()
        {
        }

        public void OnShowInfoDialog()
        {
        }

        public bool GetHasSouborInfo()
        {
            return false;
        }

        public void AppendText(FlowDocument document)
        {
            if (document.Blocks.Count == 0) return;
            MarkModification();
            var clone = new FlowDocument();
            using (var stream = new MemoryStream())
            {
                var source = new TextRange(document.ContentStart, document.ContentEnd);
                source.Save(stream, DataFormats.Xaml);

                var target = new TextRange(clone.ContentStart, clone.ContentEnd);
                target.Load(stream, DataFormats.Xaml);
            }
            var range = new TextRange(_text.Document.ContentStart, _text.Document.ContentEnd);
            if (range.Text == "\r\n")
            {
                _text.Document.Blocks.Clear();
            }
            while (clone.Blocks.Count > 0)
            {
                var b = clone.Blocks.ElementAt(0);
                clone.Blocks.Remove(b);
                _text.Document.Blocks.Add(b);
            }
            _text.ScrollToEnd();
        }

        public bool AddNewNode(CalculatedPointBase newPoint, IProtocolContext protocolContext)
        {
            return false;
        }

        DocumentPaginatorWrapper _paginator;

        public void Preprocess(PrintingPaginator paginator)
        {
            paginator.DocumentPageDirect = true;
            var clone = new FlowDocument
                            {
                                PageHeight = paginator.PageSize.Height,
                                PageWidth = paginator.PageSize.Width,
                                ColumnWidth = paginator.PageSize.Width,
                                Background = Brushes.White,
                                PagePadding = new Thickness(0)
                            };
            using (var stream = new MemoryStream())
            {
                var source = new TextRange(_text.Document.ContentStart, _text.Document.ContentEnd);
                source.Save(stream, DataFormats.Xaml);

                var target = new TextRange(clone.ContentStart, clone.ContentEnd);
                target.Load(stream, DataFormats.Xaml);
            }
            foreach (var c in clone.Blocks)
            {
                c.Margin = new Thickness(0);
                c.Padding = new Thickness(0);
            }
            var documentPaginator = ((IDocumentPaginatorSource)clone).DocumentPaginator;
            _paginator = new DocumentPaginatorWrapper(documentPaginator, paginator.PageSize, new Size(50, 50));
            _paginator.ComputePageCount();
        }

        public int PageCount
        {
            get { return _paginator.PageCount; }
        }

        public UserControl GetPage(int pageNumber)
        {
            Debug.Assert(false);
            return null;
        }

        public DocumentPage GetDocumentPage(int pageNumber)
        {
            return _paginator.GetPage(pageNumber);
        }

        bool _modfied = true;

        protected void MarkModification()
        {
            if (!_modfied)
            {
                _mainWindow.SetModifiedFlag(this);
                _modfied = true;
            }
        }

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                if (FileStream != null)
                {
                    FileStream.Close();
                    FileStream = null;
                }
            }
            _disposed = true;
        }
        ~Protokol()
        {
            Dispose(false);
        }

        public void OnKeyDown(object sender, KeyEventArgs args)
        {
            MarkModification();
        }
    }

    public class DocumentPaginatorWrapper : DocumentPaginator
    {
        readonly Size m_PageSize;
        readonly DocumentPaginator m_Paginator;
        Size m_Margin;

        public DocumentPaginatorWrapper(DocumentPaginator paginator, Size pageSize, Size margin)
        {
            m_PageSize = pageSize;
            m_Margin = margin;
            m_Paginator = paginator;
            m_Paginator.PageSize = new Size(m_PageSize.Width - margin.Width * 2,
                                            m_PageSize.Height - margin.Height * 2);
        }

        public override bool IsPageCountValid
        {
            get { return m_Paginator.IsPageCountValid; }
        }

        public override int PageCount
        {
            get { return m_Paginator.PageCount; }
        }

        public override Size PageSize
        {
            get { return m_Paginator.PageSize; }
            set { m_Paginator.PageSize = value; }
        }

        public override IDocumentPaginatorSource Source
        {
            get { return m_Paginator.Source; }
        }

        Rect Move(Rect rect)
        {
            if (rect.IsEmpty)
            {
                return rect;
            }
            return new Rect(rect.Left + m_Margin.Width, rect.Top + m_Margin.Height, rect.Width, rect.Height);
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            var page = m_Paginator.GetPage(pageNumber);
            // Create a wrapper visual for transformation and add extras
            var newpage = new ContainerVisual();
            //var title = new DrawingVisual();
            /*            using (DrawingContext ctx = title.RenderOpen())
                        {
                            if (m_Typeface == null)
                            {
                                m_Typeface = new Typeface("Times New Roman");
                            }
                            FormattedText text = new FormattedText("Page " + (pageNumber + 1),
                                System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                                m_Typeface, 14, Brushes.Black);
                            ctx.DrawText(text, new Node(0, -96 / 4)); // 1/4 inch above page content
                        }*/
            var smallerPage = new ContainerVisual();
            smallerPage.Children.Add(page.Visual);
            /*            smallerPage.Transform = new MatrixTransform(0.95, 0, 0, 0.95,
                            0.025 * page.ContentBox.Width, 0.025 * page.ContentBox.Height);*/
            newpage.Children.Add(smallerPage);
            //            newpage.Children.Add(title);
            newpage.Transform = new TranslateTransform(m_Margin.Width, m_Margin.Height);
            return new DocumentPage(newpage, m_PageSize, Move(page.BleedBox), Move(page.ContentBox));
        }
    }
}