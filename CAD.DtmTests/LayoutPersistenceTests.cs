using System.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using CAD.DTM;
using CAD.DTM.Gui;
using CAD.GUI;
using AvalonDock;
using CAD.UITools;
using GeoBase;
using NUnit.Framework;

namespace CAD.DtmTests;

[TestFixture, Apartment(ApartmentState.STA)]
public class LayoutPersistenceTests
{
    sealed class FirstToolBar : GeoCadToolBar { }
    sealed class SecondToolBar : GeoCadToolBar { }

    sealed class Palette : DockableContent, IDockableContent
    {
        public int ResetCount { get; private set; }
        public void Reset() => ResetCount++;
        public void SetDocked() => State = DockableContentState.Docked;
    }

    [OneTimeSetUp]
    public void DisableUserSettings() => SingletonsBase.Registry.SuppressRegistry = true;

    [Test]
    public void ToolBarRowsAndOrderSurviveManagerRecreation()
    {
        var manager = new GeoCadToolBarManager(null);
        var first = new FirstToolBar();
        var second = new SecondToolBar();
        manager.RegisterToolBar(new ToolBarTray(), first);
        manager.RegisterToolBar(new ToolBarTray(), second);
        first.Band = 2;
        first.BandIndex = 1;
        second.Band = 2;
        second.BandIndex = 0;
        manager.SaveLayout();

        var reopened = new GeoCadToolBarManager(null);
        var restoredSecond = new SecondToolBar();
        var restoredFirst = new FirstToolBar();
        reopened.RegisterToolBar(new ToolBarTray(), restoredSecond);
        reopened.RegisterToolBar(new ToolBarTray(), restoredFirst);
        Assert.Multiple(() =>
        {
            Assert.That(restoredFirst.Band, Is.EqualTo(2));
            Assert.That(restoredFirst.BandIndex, Is.EqualTo(1));
            Assert.That(restoredSecond.Band, Is.EqualTo(2));
            Assert.That(restoredSecond.BandIndex, Is.Zero);
        });
    }

    [Test]
    public void DocumentChangeKeepsActivePaletteInItsExistingPane()
    {
        var environment = (DocumentEnvironemt)System.Activator.CreateInstance(typeof(DocumentEnvironemt), nonPublic: true);
        var active = new Palette { Name = "Active", IsCloseable = true };
        var inactive = new Palette { Name = "Inactive", IsCloseable = true };
        var pane = new DockablePane();
        pane.Items.Add(active);
        pane.Items.Add(inactive);
        active.SetDocked();
        inactive.SetDocked();
        var manager = new DockingManager { Content = pane };
        environment.RegisterPalette(DockablePalette.DtmPropPage, active);
        environment.RegisterPalette(DockablePalette.SeznamSouradnic, inactive);
        environment.HideAllPalette(manager, DockablePalette.DtmPropPage);
        Assert.Multiple(() =>
        {
            Assert.That(active.ContainerPane, Is.SameAs(pane));
            Assert.That(active.State, Is.EqualTo(DockableContentState.Docked));
            Assert.That(active.ResetCount, Is.Zero);
            Assert.That(inactive.State, Is.EqualTo(DockableContentState.Hidden));
            Assert.That(inactive.ResetCount, Is.EqualTo(1));
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ValidationPanelAndPressedButtonRestoreTheSavedPreference(bool visible)
    {
        var registry = SingletonsBase.Registry;
        var previous = registry.getEntry(GeoBase.Utils.Registry.SubKey.kCurrentUser, DtmValidationPanel.VisibilitySettingKey);
        try
        {
            registry.setEntry(GeoBase.Utils.Registry.SubKey.kCurrentUser, DtmValidationPanel.VisibilitySettingKey,
                new GeoBase.Utils.ProgramOption(visible));
            var document = new Document("", null);
            document.DataModel.DtmMain = new DtmMain();
            var properties = new PropPagePalette();
            properties.SetValidationDocument(document);
            var toolbar = new DtmToolBar();
            toolbar.Measure(new System.Windows.Size(1200, 100));
            toolbar.Arrange(new System.Windows.Rect(0, 0, 1200, 100));
            toolbar.UpdateLayout();
            var button = (ToggleButton)toolbar.FindName("_validationToggleButton");
            Assert.That(document.IsDtmValidationVisible, Is.EqualTo(visible));
            Assert.That(((DtmValidationPanel)properties.FindName("_validationPanel")).Visibility,
                Is.EqualTo(visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed));
            Assert.That(button.IsChecked, Is.EqualTo(visible));
            Assert.That(document.DtmValidationResult, Is.Null);

            var context = (DtmToolBarCtx)toolbar.DataContext;
            context.SetDocument(document);
            document.IsDtmValidationVisible = !visible;
            context.SetDocument(document); // Refreshing the same document must also refresh the button.
            Assert.That(button.IsChecked, Is.EqualTo(!visible));
            toolbar.Notify(NotificationType.ToolChanged, null);
            Assert.That(button.IsChecked, Is.EqualTo(!visible));
        }
        finally
        {
            registry.setEntry(GeoBase.Utils.Registry.SubKey.kCurrentUser, DtmValidationPanel.VisibilitySettingKey, previous);
        }
    }
}
