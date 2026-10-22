using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using CAD.DTM;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;
using CAD.DTM.Gui;
using CAD.DTM.Validation;
using CAD.DTM.Validation.Rules;
using CAD.GUI;
using NUnit.Framework;

namespace CAD.DtmTests;

[TestFixture, Apartment(ApartmentState.STA)]
public class DtmValidationPanelTests
{
    [Test]
    public void ResultRefreshReplacesTheTreeAndPreservesDescriptionsAndSeverity()
    {
        var panel = new DtmValidationPanel();
        var rule = new DtmRuleResult("3.11", "Vazby bodů") { Description = "Popis kontroly" };
        rule.AddError("Bod 1", "Chybí vazba.");
        panel.SetResult(new DtmValidationResult(new[] { rule }));
        Assert.That(panel.Nodes[0].IsExpanded, Is.True);
        Assert.That(panel.Nodes[0].ToolTip, Is.EqualTo("Popis kontroly"));
        Assert.That(panel.Nodes[0].Children[0].Text, Does.Contain("Bod 1"));
        Assert.That(panel.Summary, Does.Contain("Skupin s chybami: 1"));

        panel.SetResult(new DtmValidationResult(new[] { new DtmRuleResult("3.11", "Vazby bodů") }));
        Assert.That(panel.Nodes[0].Children, Is.Empty);
        Assert.That(panel.Summary, Is.EqualTo("Všechny provedené kontroly byly splněny."));
    }

    [Test]
    public void PaletteSwitchesDocumentsWithoutRunningValidationAndRestoresItsSplit()
    {
        var first = new Document("", null) { IsDtmValidationVisible = true };
        first.DataModel.DtmMain = new DtmMain();
        var second = new Document("", null) { IsDtmValidationVisible = false };
        second.DataModel.DtmMain = new DtmMain();
        var palette = new PropPagePalette();
        var panel = (DtmValidationPanel)palette.FindName("_validationPanel");
        var row = (RowDefinition)palette.FindName("_validationRow");
        var button = (Button)panel.FindName("_runValidation");

        palette.SetValidationDocument(first);
        Assert.That(panel.Visibility, Is.EqualTo(Visibility.Visible));
        Assert.That(button.IsEnabled, Is.True);
        Assert.That(first.DtmValidationResult, Is.Null);
        row.Height = new GridLength(220);
        palette.SetValidationDocument(second);
        Assert.That(panel.Visibility, Is.EqualTo(Visibility.Collapsed));
        Assert.That(row.Height.Value, Is.Zero);
        palette.SetValidationDocument(first);
        Assert.That(panel.Visibility, Is.EqualTo(Visibility.Visible));
        Assert.That(row.Height, Is.EqualTo(new GridLength(220)));
        Assert.That(panel.Result, Is.Null);
        palette.SetValidationDocument(null);
        Assert.That(button.IsEnabled, Is.False);
        Assert.That(panel.Visibility, Is.EqualTo(Visibility.Collapsed));
        Assert.That(panel.Nodes, Is.Empty);
    }

    [Test]
    public void ResultTreeFitsTheNarrowPropertiesPalette()
    {
        var panel = new DtmValidationPanel();
        var rule = new DtmRuleResult("3.1", new string('a', 120));
        for (var i = 0; i < 20; i++) rule.AddWarning("Objekt " + i, new string('b', 120));
        panel.SetResult(new DtmValidationResult(new List<DtmRuleResult> { rule }));
        panel.Measure(new Size(300, 250));
        panel.Arrange(new Rect(0, 0, 300, 250));
        panel.UpdateLayout();
        Assert.That(panel.DesiredSize.Width, Is.LessThanOrEqualTo(300));
        Assert.That(panel.ActualHeight, Is.EqualTo(250));
    }

    static void SetValidationResult(Document document)
    {
        typeof(Document).GetProperty("DtmValidationResult", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .SetValue(document, new DtmValidationResult(new[] { new DtmRuleResult("1", "Test") }));
    }

    [Test]
    public void CanvasChangesUndoRedoAndPropertiesMarkValidationStaleUntilItRunsAgain()
    {
        var document = new Document("", null);
        var main = document.DataModel.DtmMain = new DtmMain();
        var point = AddPoint(main, -500000);
        var layer = new DtmDrawingLayerMain(main);
        document.DataModel.AddLayer(layer);
        SetValidationResult(document);
        var panel = new DtmValidationPanel();
        panel.SetDocument(document);
        var warning = (Border)panel.FindName("_staleWarning");
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Collapsed));
        var drawing = layer.FindObject(point);
        document.DataModel.AddSelectedObject(drawing);
        Assert.That(document.IsDtmValidationStale, Is.False);

        document.DataModel.DeleteObjects(new[] { drawing });
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Visible));
        SetValidationResult(document);
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Collapsed));
        Assert.That(document.DataModel.DoUndo(), Is.True);
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Visible));
        SetValidationResult(document);
        Assert.That(document.DataModel.DoRedo(), Is.True);
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Visible));
        SetValidationResult(document);
        document.DataModel.NotifyContentChanged();
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Visible));
        SetValidationResult(document);
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Collapsed));
    }

    [Test]
    public void StaleWarningFollowsTheDocumentIncludingBeforeFirstValidation()
    {
        var first = new Document("", null);
        first.DataModel.DtmMain = new DtmMain();
        var second = new Document("", null);
        second.DataModel.DtmMain = new DtmMain();
        first.DataModel.NotifyContentChanged();
        Assert.That(first.IsDtmValidationStale, Is.True);
        SetValidationResult(first);
        SetValidationResult(second);
        first.DataModel.NotifyContentChanged();
        var panel = new DtmValidationPanel();
        var warning = (Border)panel.FindName("_staleWarning");
        panel.SetDocument(first);
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Visible));
        panel.SetDocument(second);
        SetValidationResult(first);
        first.DataModel.NotifyContentChanged();
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Collapsed));
        panel.SetDocument(first);
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Visible));
        panel.SetDocument(null);
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Collapsed));
    }

    [Test]
    public void FirstOpeningShowsWarningUntilFirstValidation()
    {
        var document = new Document("", null);
        document.DataModel.DtmMain = new DtmMain();
        var panel = new DtmValidationPanel();
        panel.SetDocument(document);
        var warning = (Border)panel.FindName("_staleWarning");
        Assert.That(document.DtmValidationResult, Is.Null);
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Visible));
        SetValidationResult(document);
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Collapsed));
        document.DataModel.NotifyContentChanged();
        Assert.That(warning.Visibility, Is.EqualTo(Visibility.Visible));
    }

    static DtmElement AddPoint(DtmMain main, double x)
    {
        var element = DtmConfigurationSingleton.Instance.CreateType("PodrobnyBodZPS");
        ((DtmBodBaseElement)element).CisloBodu = "1"; // Intentionally identical display names.
        element.Geometry = new DtmGeometryGroup { Geometries = new List<IDtmGeometry>
        {
            new DtmPointGeometry { Point = new DtmPoint { X = x, Y = -1000000, Z = 100 }, SrsDimension = 3 }
        } };
        if (main.getElementsGroup("PodrobnyBodZPS") == null)
            main.AddElementGroup("PodrobnyBodZPS", new DtmElementsGroup("PodrobnyBodZPS"));
        ((DtmElementsGroup)main.getElementsGroup("PodrobnyBodZPS")).AddElement(element);
        return element;
    }

    [Test]
    public void ClickingAnIssueSelectsItsExactDrawingAndLoadsProperties()
    {
        var document = new Document("", null);
        var main = document.DataModel.DtmMain = new DtmMain();
        var first = AddPoint(main, -500000);
        var second = AddPoint(main, -500010);
        document.DataModel.AddLayer(new DtmDrawingLayerMain(main));
        var properties = new PropPagePalette();
        DocumentEnvironemt.Instance.RegisterPalette(DockablePalette.DtmPropPage, properties);
        var panel = new DtmValidationPanel();
        panel.SetDocument(document);
        var rule = new DtmSolitaryPodrobnyBodRule().Validate(new DtmValidationContext(main, null, DtmValidationSettings.Default));
        Assert.That(rule.Issues.Select(i => i.ObjectName).Distinct().Count(), Is.EqualTo(1));
        Assert.That(rule.Issues[1].Element, Is.SameAs(second));
        panel.SetResult(new DtmValidationResult(new[] { rule }));
        var tree = ((DockPanel)panel.Content).Children.OfType<TreeView>().Single();
        var issue = panel.Nodes[0].Children[1];
        Assert.That(document.SelectDtmValidationObject(first), Is.True);
        tree.RaiseEvent(new RoutedPropertyChangedEventArgs<object>(null, issue, TreeView.SelectedItemChangedEvent));
        var drawing = document.DataModel.SelectedObjects.Single();
        Assert.That(((IDtmDrawingElement)drawing).GetDtmElement, Is.SameAs(second));
        Assert.That(document.DataModel.FocusetObject, Is.SameAs(drawing));
        Assert.That(drawing.Selected, Is.True);
        Assert.That(((StackPanel)properties.FindName("_stack")).Children.OfType<DtmPropPage>().Count(), Is.EqualTo(1));

        // Repeating a click also restores selection after selecting another object.
        document.SelectDtmValidationObject(first);
        tree.RaiseEvent(new RoutedPropertyChangedEventArgs<object>(issue, issue, TreeView.SelectedItemChangedEvent));
        Assert.That(((IDtmDrawingElement)document.DataModel.SelectedObjects.Single()).GetDtmElement, Is.SameAs(second));

        document.SelectDtmValidationObject(first);
        second.IsDeleted = true;
        tree.RaiseEvent(new RoutedPropertyChangedEventArgs<object>(null, issue, TreeView.SelectedItemChangedEvent));
        Assert.That(((IDtmDrawingElement)document.DataModel.SelectedObjects.Single()).GetDtmElement, Is.SameAs(first));

        // A general rule heading or a file-level error must leave selection intact.
        var general = new DtmRuleResult("1.1", "XSD");
        general.AddError(null, "Chyba souboru.");
        panel.SetResult(new DtmValidationResult(new[] { general }));
        tree.RaiseEvent(new RoutedPropertyChangedEventArgs<object>(null, panel.Nodes[0].Children[0], TreeView.SelectedItemChangedEvent));
        tree.RaiseEvent(new RoutedPropertyChangedEventArgs<object>(null, panel.Nodes[0], TreeView.SelectedItemChangedEvent));
        Assert.That(((IDtmDrawingElement)document.DataModel.SelectedObjects.Single()).GetDtmElement, Is.SameAs(first));
    }

    [Test]
    public void ObjectErrorsAndWarningsKeepIdentityInTheTree()
    {
        var main = new DtmMain();
        var element = AddPoint(main, 0);
        var context = new DtmValidationContext(main, null, DtmValidationSettings.Default);
        var error = new DtmCoordinatesRule().Validate(context);
        element.ZapisObjektu = 'u';
        var warning = new DtmOriginalStateAvailableRule().Validate(context);
        var panel = new DtmValidationPanel();
        panel.SetResult(new DtmValidationResult(new[] { error, warning }));
        Assert.That(panel.Nodes[0].Children[0].Element, Is.SameAs(element));
        Assert.That(panel.Nodes[1].Children[0].Element, Is.SameAs(element));
    }

    [Test]
    public void ObjectsFromAnotherDocumentOrHiddenLayersCannotBeSelected()
    {
        var main = new DtmMain();
        var element = AddPoint(main, -500000);
        var layer = new DtmDrawingLayerMain(main);
        Assert.That(layer.FindObject(element), Is.Not.Null);
        var foreign = AddPoint(new DtmMain(), -500000);
        Assert.That(layer.FindObject(foreign), Is.Null);
        layer.Visible = false;
        Assert.That(layer.FindObject(element), Is.Null);
    }
}
