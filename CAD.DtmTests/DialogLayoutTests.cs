using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CAD.DTM;
using CAD.DTM.Gui;
using GeoBase;
using GeoBase.Localization;
using NUnit.Framework;

namespace CAD.DtmTests;

[TestFixture, Apartment(ApartmentState.STA), NonParallelizable]
public class DialogLayoutTests
{
    Window _owner;
    Window _previousMainWindow;

    [OneTimeSetUp]
    public void InitializeDialogEnvironment()
    {
        if (Application.Current == null)
            _ = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        _previousMainWindow = Application.Current.MainWindow;
        var owner = _owner = new Window { ShowInTaskbar = false };
        _ = new System.Windows.Interop.WindowInteropHelper(owner).EnsureHandle();
        Application.Current.MainWindow = owner;
        SingletonsBase.Registry.SuppressRegistry = true;
        var culture = CultureInfo.GetCultureInfo("cs-CZ");
        LanguageDictionary.RegisterDictionary(culture, new XmlLanguageDictionary(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "Languages", "cs-CZ.xml"), string.Empty));
        LanguageContext.Instance.Culture = culture;
    }

    [OneTimeTearDown]
    public void RestoreDialogEnvironment()
    {
        Application.Current.MainWindow = _previousMainWindow;
        _owner.Close();
    }

    static Window Create(string name) => name switch
    {
        "dtm-import" => new DtmImportDialog(new DtmImportCtx()),
        "dtm-import-points" => new DtmImportPointsDialog(new DtmImportPointsCtx()),
        "dtm-export" => new DtmExportDialog(new DtmExportCtx()),
        "vfk-import" => new global::VFK.GUI.VFKImportDialog(new global::VFK.VFKDataContext()),
        "vfk-export" => new CAD.VFK.GUI.VFKExportDialog(new global::VFK.VFKDataContext()),
        "vfk-coordinates" => new CAD.VFK.GUI.SouradniceDialog(new global::VFK.VfkProxyActivePoint(
            new global::VFK.Tables.VFKSOBRTableItem(), new global::VFK.Tables.VFKSPOLTableItem(), null)),
        "vfk-new-parcel" => NewParcelDialog(),
        "vfk-import-coordinates" => new CAD.VFK.GUI.ImportSouradnicDialog(null, null),
        "vfk-export-coordinates" => new CAD.VFK.GUI.ExportSouradnicDialog(null),
        _ => throw new ArgumentOutOfRangeException(nameof(name))
    };

    static Window NewParcelDialog()
    {
        var dialog = new global::VFK.GUI.AddNewParcelDialog();
        dialog.InitializeComponent();
        return dialog;
    }

    static IEnumerable<T> Descendants<T>(DependencyObject root) where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is T item) yield return item;
            foreach (var descendant in Descendants<T>(child)) yield return descendant;
        }
    }

    static FrameworkElement Layout(Window dialog, double width, double height)
    {
        var content = (FrameworkElement)dialog.Content;
        content.Measure(new Size(width, height));
        content.Arrange(new Rect(0, 0, width, height));

        return content;
    }

    static void Preview(Window dialog, FrameworkElement content, string name)
    {
        var width = (int)Math.Ceiling(content.ActualWidth + content.Margin.Left + content.Margin.Right);
        var height = (int)Math.Ceiling(content.ActualHeight + content.Margin.Top + content.Margin.Bottom);
        var visual = new DrawingVisual();
        using (var drawing = visual.RenderOpen())
        {
            drawing.DrawRectangle(dialog.Background, null, new Rect(0, 0, width, height));
            var brush = new VisualBrush(content)
            {
                ViewboxUnits = BrushMappingMode.Absolute,
                Viewbox = new Rect(VisualTreeHelper.GetOffset(content).X, VisualTreeHelper.GetOffset(content).Y, content.ActualWidth, content.ActualHeight),
                Stretch = Stretch.Fill
            };
            drawing.DrawRectangle(brush, null, new Rect(content.Margin.Left, content.Margin.Top, content.ActualWidth, content.ActualHeight));
        }
        var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(visual);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        var directory = Path.Combine(TestContext.CurrentContext.TestDirectory, "DialogPreviews");
        Directory.CreateDirectory(directory);
        using var file = File.Create(Path.Combine(directory, name + ".png"));
        encoder.Save(file);
    }

    [TestCase("dtm-import")]
    [TestCase("dtm-import-points")]
    [TestCase("dtm-export")]
    [TestCase("vfk-import")]
    [TestCase("vfk-export")]
    [TestCase("vfk-coordinates")]
    [TestCase("vfk-new-parcel")]
    [TestCase("vfk-import-coordinates")]
    [TestCase("vfk-export-coordinates")]
    public void FieldsGrowHorizontallyAndKeepTheirHeight(string name)
    {
        var dialog = Create(name);
        dialog.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.DataBind, new Action(() => { }));
        var content = Layout(dialog, dialog.MinWidth - 16, dialog.MinHeight - 38);
        var fields = Descendants<TextBox>(content).Where(t => t.GetType() == typeof(TextBox)).ToArray();
        Assert.That(Descendants<Button>(content), Is.Not.Empty);
        var narrowWidths = fields.Select(t => t.ActualWidth).ToArray();
        var originalHeights = fields.Select(t => t.ActualHeight).ToArray();
        Assert.That(originalHeights, Is.All.GreaterThanOrEqualTo(30));
        content = Layout(dialog, dialog.MinWidth + 284, dialog.MinHeight + 162);
        for (var i = 0; i < fields.Length; i++)
        {
            Assert.That(fields[i].ActualWidth, Is.GreaterThan(narrowWidths[i] + 200), name);
            Assert.That(fields[i].ActualHeight, Is.EqualTo(originalHeights[i]).Within(0.1), name);
        }
        Preview(dialog, content, name + "-wide");
        content = Layout(dialog, Math.Max(dialog.Width, dialog.MinWidth) - 16, Math.Max(dialog.Height, dialog.MinHeight) - 38);
        foreach (var field in fields)
        {
            var bounds = field.TransformToAncestor(content).TransformBounds(new Rect(field.RenderSize));
            Assert.That(bounds.Bottom, Is.LessThanOrEqualTo(content.ActualHeight), name);
            Assert.That(bounds.Right, Is.LessThanOrEqualTo(content.ActualWidth), name);
            Assert.That(field.Margin.Top + field.Margin.Bottom, Is.GreaterThanOrEqualTo(8));
        }
        foreach (var button in Descendants<Button>(content))
        {
            if (!button.IsVisible && button.Visibility != Visibility.Visible) continue;
            var bounds = button.TransformToAncestor(content).TransformBounds(new Rect(button.RenderSize));
            Assert.That(bounds.Bottom, Is.LessThanOrEqualTo(content.ActualHeight + 0.1), name + " button bottom");
            Assert.That(bounds.Right, Is.LessThanOrEqualTo(content.ActualWidth + 0.1), name + " button right");
        }
        Preview(dialog, content, name);
        dialog.Close();
    }
}