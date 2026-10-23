using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AvalonDock;
using CAD.DTM;
using GeoBase;
using NUnit.Framework;

namespace CAD.DtmTests;

[TestFixture, Apartment(ApartmentState.STA)]
public class DocumentLifecycleTests
{
    sealed class Workspace : Window
    {
        public DockingManager Manager { get; } = new DockingManager();
        readonly DocumentPane _pane = new DocumentPane();
        public Workspace()
        {
            Manager.Content = _pane;
            Content = Manager;
        }
        public Document GetDocument() => Manager.ActiveDocument as Document;
        void Add(Document document)
        {
            _pane.Items.Add(document);
            document.SetAsActive();
        }
        public void DocumentNew(string name) => Add(new Document(name, null));
        public bool ImportNewDocument(Func<Document, bool> import) =>
            MainWin.ImportNewDocument(() => new Document(string.Empty, null), import, Add);
        public void CloseDocument() => (Manager.ActiveDocument as DocumentContent)?.Close();
    }

    [OneTimeSetUp]
    public void DisableUserSettings() => SingletonsBase.Registry.SuppressRegistry = true;

    [Test]
    public void ImportAddsAndActivatesANewDocumentWithoutClosingExistingDocuments()
    {
        var window = new Workspace();
        window.DocumentNew(string.Empty);
        var first = window.GetDocument();
        first.DataModel.DtmMain = new DtmMain();
        var originalData = first.DataModel.DtmMain;
        var manager = window.Manager;
        Assert.That(window.ImportNewDocument(candidate =>
        {
            Assert.That(window.GetDocument(), Is.SameAs(first));
            Assert.That(manager.Documents, Has.Length.EqualTo(1));
            candidate.DataModel.DtmMain = new DtmMain();
            return true;
        }), Is.True);
        var second = window.GetDocument();
        Assert.That(second, Is.Not.SameAs(first));
        Assert.That(manager.Documents, Has.Length.EqualTo(2));
        Assert.That(first.DataModel.DtmMain, Is.SameAs(originalData));
        Assert.That(window.ImportNewDocument(_ => true), Is.True);
        Assert.That(manager.Documents, Has.Length.EqualTo(3));
        Assert.That(manager.Documents, Does.Contain(first));
        Assert.That(manager.Documents, Does.Contain(second));
        window.Close();
    }

    [Test]
    public void CancelledOrFailedImportLeavesNoTabAndPreservesTheActiveDocument()
    {
        var window = new Workspace();
        window.DocumentNew(string.Empty);
        var original = window.GetDocument();
        var manager = window.Manager;
        Document cancelled = null;
        Assert.That(window.ImportNewDocument(candidate => { cancelled = candidate; return false; }), Is.False);
        Assert.That(cancelled.CanvasCtrl.IsDisposed, Is.True);
        Assert.That(window.GetDocument(), Is.SameAs(original));
        Assert.That(manager.Documents, Has.Length.EqualTo(1));
        Assert.Throws<InvalidOperationException>(() => window.ImportNewDocument(_ => throw new InvalidOperationException("test")));
        Assert.That(window.GetDocument(), Is.SameAs(original));
        Assert.That(manager.Documents, Has.Length.EqualTo(1));
        window.Close();
    }

    [Test]
    public void SuccessfulDtmFileImportOpensASeparateDtmDocument()
    {
        var window = new Workspace();
        window.DocumentNew(string.Empty);
        var original = window.GetDocument();
        var ctx = new DtmImportCtx
        {
            FileName = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "Samples", "vydej_zps_ref.jvf.xml")
        };
        Assert.That(window.ImportNewDocument(candidate => candidate.DataModel.ImportDtm(ctx)), Is.True);
        Assert.That(window.Manager.Documents, Has.Length.EqualTo(2));
        Assert.That(window.GetDocument(), Is.Not.SameAs(original));
        Assert.That(window.GetDocument().GetDocumentType(), Is.EqualTo(DocumentType.Dtm));
        Assert.That(original.DataModel.DtmMain, Is.Null);
        window.Close();
    }

    static void FinishLayoutCallbacks()
    {
        var frame = new System.Windows.Threading.DispatcherFrame();
        System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(
            System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(() => frame.Continue = false));
        System.Windows.Threading.Dispatcher.PushFrame(frame);
    }

    [Test]
    public void InitialFitWaitsForCanvasSizeAndDoesNotResetLaterUserZoom()
    {
        var document = new Document(string.Empty, null);
        var ctx = new DtmImportCtx
        {
            FileName = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "Samples", "vydej_zps_ref.jvf.xml")
        };
        Assert.That(document.DataModel.ImportDtm(ctx), Is.True);
        document.CanvasCtrl.ClientSize = new System.Drawing.Size(0, 0);
        var originalZoom = document.DataModel.Zoom;
        document.FitViewWhenReady();
        FinishLayoutCallbacks();
        Assert.That(document.DataModel.Zoom, Is.EqualTo(originalZoom));

        _ = document.CanvasCtrl.Handle;
        document.CanvasCtrl.ClientSize = new System.Drawing.Size(800, 600);
        FinishLayoutCallbacks();
        var fittedZoom = document.DataModel.Zoom;
        Assert.That(fittedZoom, Is.GreaterThan(0));
        Assert.That(fittedZoom, Is.Not.EqualTo(originalZoom));
        document.DataModel.Zoom = originalZoom;
        document.CanvasCommand.CommandFitView();
        Assert.That(document.DataModel.Zoom, Is.EqualTo(fittedZoom));

        document.DataModel.Zoom = fittedZoom * 2;
        document.CanvasCtrl.ClientSize = new System.Drawing.Size(1000, 700);
        FinishLayoutCallbacks();
        Assert.That(document.DataModel.Zoom, Is.EqualTo(fittedZoom * 2));
        document.CanvasCtrl.Dispose();
    }

    static FrameworkElement Header(Document document)
    {
        var factory = new FrameworkElementFactory(typeof(Border), "PART_DragArea");
        document.Template = new ControlTemplate(typeof(Document)) { VisualTree = factory };
        document.ApplyTemplate();
        return (FrameworkElement)document.Template.FindName("PART_DragArea", document);
    }

    static void MouseEvent(FrameworkElement header, RoutedEvent routedEvent, MouseButton button = MouseButton.Middle)
    {
        header.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, button) { RoutedEvent = routedEvent });
    }

    static void Click(FrameworkElement header)
    {
        MouseEvent(header, UIElement.PreviewMouseDownEvent);
        MouseEvent(header, UIElement.PreviewMouseUpEvent);
    }

    [Test]
    public void MiddleClickClosesTheInactiveTabAndUsesTheCancellableClosePath()
    {
        var window = new Workspace();
        window.DocumentNew(string.Empty);
        var first = window.GetDocument();
        window.DocumentNew(string.Empty);
        var second = window.GetDocument();
        var manager = window.Manager;
        var header = Header(first);
        System.EventHandler<System.ComponentModel.CancelEventArgs> cancel = (_, e) => e.Cancel = true;
        first.Closing += cancel;
        Click(header);
        Assert.That(manager.Documents, Has.Length.EqualTo(2));
        Assert.That(window.GetDocument(), Is.SameAs(second));
        first.Closing -= cancel;
        Click(header);
        Assert.That(manager.Documents, Has.Length.EqualTo(1));
        Assert.That(manager.Documents[0], Is.SameAs(second));
        Assert.That(window.GetDocument(), Is.SameAs(second));
        Click(Header(second));
        Assert.That(manager.Documents, Is.Empty);
        Assert.That(window.GetDocument(), Is.Null);
        Assert.DoesNotThrow(() => window.CloseDocument());
        window.Close();
    }

    [Test]
    public void MiddleClickMustStartAndEndOnTheSameCurrentHeader()
    {
        var window = new Workspace();
        window.DocumentNew(string.Empty);
        var document = window.GetDocument();
        var manager = window.Manager;
        var oldHeader = Header(document);
        MouseEvent(oldHeader, UIElement.PreviewMouseUpEvent);
        MouseEvent(oldHeader, UIElement.PreviewMouseDownEvent, MouseButton.Right);
        MouseEvent(oldHeader, UIElement.PreviewMouseUpEvent, MouseButton.Right);
        MouseEvent(oldHeader, UIElement.PreviewMouseDownEvent);
        oldHeader.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, Environment.TickCount) { RoutedEvent = UIElement.MouseLeaveEvent });
        MouseEvent(oldHeader, UIElement.PreviewMouseUpEvent);
        Assert.That(manager.Documents, Has.Length.EqualTo(1));
        var newHeader = Header(document);
        Click(oldHeader);
        Assert.That(manager.Documents, Has.Length.EqualTo(1));
        Click(newHeader);
        Assert.That(manager.Documents, Is.Empty);
        window.Close();
    }
}