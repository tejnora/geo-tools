using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CAD.DTM.Elements;

namespace CAD.DTM.Gui
{
    /// <summary>Edits a private copy; cancelling never changes the drawing.</summary>
    public sealed class DtmGeometryEditorDialog : Window
    {
        readonly IList<IDtmGeometry> _target;
        readonly List<string> _allowedGeometries;
        readonly List<IDtmGeometry> _draft;
        readonly ComboBox _parts = new() { MinWidth = 260, DisplayMemberPath = "Label" };
        readonly DataGrid _grid = new() { AutoGenerateColumns = false, CanUserAddRows = true, CanUserDeleteRows = true };
        readonly TextBlock _error = new() { Foreground = System.Windows.Media.Brushes.Firebrick, TextWrapping = TextWrapping.Wrap };
        sealed record Part(string Label, IDtmGeometry Geometry, List<DtmPoint> Points);

        public DtmGeometryEditorDialog(DtmElement element) : this(element, element.Geometry.Geometries, element.SchemaAttributes.Definition.Geometries) { }

        internal DtmGeometryEditorDialog(DtmElement element, IList<IDtmGeometry> target, IEnumerable<string> allowedGeometries)
        {
            _target = target;
            _allowedGeometries = allowedGeometries.ToList();
            _draft = target.Select(Clone).ToList();
            Title = "Geometrie – " + element.Options.ObjektovyTypNazev;
            Width = 700; Height = 480; WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var panel = new DockPanel { Margin = new Thickness(10) };
            var bar = new WrapPanel();
            bar.Children.Add(_parts);
            var addRing = Button("Přidat otvor", AddRing);
            var addPart = Button("Přidat část / obvod", AddPart);
            var removePart = Button("Odebrat část", RemovePart);
            bar.Children.Add(addRing); bar.Children.Add(addPart); bar.Children.Add(removePart);
            DockPanel.SetDock(bar, Dock.Top); panel.Children.Add(bar);
            var actions = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            actions.Children.Add(Button("Použít", Apply));
            actions.Children.Add(Button("Zrušit", () => DialogResult = false));
            DockPanel.SetDock(actions, Dock.Bottom); panel.Children.Add(actions);
            DockPanel.SetDock(_error, Dock.Bottom); panel.Children.Add(_error);
            foreach (var coordinate in new[] { "X", "Y", "Z" })
                _grid.Columns.Add(new DataGridTextColumn { Header = coordinate, Binding = new Binding(coordinate) { UpdateSourceTrigger = UpdateSourceTrigger.LostFocus, StringFormat = "G17", ConverterCulture = System.Globalization.CultureInfo.CurrentCulture, ValidatesOnExceptions = true }, Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            panel.Children.Add(_grid);
            Content = panel;
            _parts.SelectionChanged += (_, _) => _grid.ItemsSource = (_parts.SelectedItem as Part)?.Points;
            Refresh();
        }

        static Button Button(string label, Action action)
        {
            var button = new Button { Content = label, Margin = new Thickness(4), Padding = new Thickness(6) };
            button.Click += (_, _) => action();
            return button;
        }

        void Refresh()
        {
            var parts = new List<Part>();
            foreach (var geometry in _draft)
                switch (geometry)
                {
                    case DtmPointGeometry point: parts.Add(new Part("Bod", point, new List<DtmPoint> { point.Point })); break;
                    case DtmMultiCurveGeometry multi:
                        if (multi.Members.Count == 0) multi.Members.Add(new DtmCurveGeometry { Points = multi.Points });
                        for (var i = 0; i < multi.Members.Count; i++) parts.Add(new Part("Obvod / část " + (i + 1), multi.Members[i], multi.Members[i].Points));
                        break;
                    case DtmCurveGeometry curve: parts.Add(new Part("Linie", curve, curve.Points)); break;
                    case DtmPolygonGeometry polygon:
                        parts.Add(new Part("Vnější hranice plochy", polygon, polygon.Points));
                        for (var i = 0; i < polygon.InteriorRings.Count; i++) parts.Add(new Part("Otvor " + (i + 1), polygon, polygon.InteriorRings[i]));
                        break;
                }
            _parts.ItemsSource = parts;
            _parts.SelectedIndex = parts.Count - 1;
        }

        void AddRing()
        {
            if ((_parts.SelectedItem as Part)?.Geometry is not DtmPolygonGeometry polygon) return;
            polygon.InteriorRings.Add(new List<DtmPoint>());
            Refresh();
        }
        void AddPart()
        {
            if (!_allowedGeometries.Contains("multiCurveProperty")) return;
            var multi = _draft.OfType<DtmMultiCurveGeometry>().FirstOrDefault();
            if (multi == null)
            {
                multi = new DtmMultiCurveGeometry { Id = "G" + Guid.NewGuid().ToString("N"), Points = new List<DtmPoint>() };
                _draft.Add(multi);
            }
            multi.Members.Add(new DtmCurveGeometry { Points = new List<DtmPoint>() });
            Refresh();
        }
        void RemovePart()
        {
            if (_parts.SelectedItem is not Part part) return;
            if (part.Geometry is DtmPolygonGeometry polygon && polygon.InteriorRings.Contains(part.Points)) polygon.InteriorRings.Remove(part.Points);
            else
                foreach (var multi in _draft.OfType<DtmMultiCurveGeometry>().ToList())
                {
                    if (multi.Members.Count == 1 && ReferenceEquals(multi.Members[0].Points, part.Points))
                    {
                        _error.Text = "XSD vyžaduje obvod objektu; poslední část nelze odebrat.";
                        return;
                    }
                    multi.Members.RemoveAll(m => ReferenceEquals(m.Points, part.Points));
                    if (multi.Members.Count == 0) _draft.Remove(multi);
                }
            Refresh();
        }

        void Apply()
        {
            if (!_grid.CommitEdit(DataGridEditingUnit.Cell, true) || !_grid.CommitEdit(DataGridEditingUnit.Row, true)) return;
            // A point uses its one-row list, so disallow adding/removing its only coordinate.
            foreach (var part in _parts.Items.OfType<Part>())
            {
                if (part.Points.Any(p => !double.IsFinite(p.X) || !double.IsFinite(p.Y) || !double.IsFinite(p.Z))) { _error.Text = "Souřadnice musí být konečná čísla."; return; }
                if (part.Geometry is DtmPointGeometry point)
                {
                    if (part.Points.Count != 1) { _error.Text = "Bod musí mít právě jednu souřadnici."; return; }
                    point.Point = part.Points[0];
                }
                else if (part.Geometry is DtmPolygonGeometry)
                {
                    if (part.Points.Count < 3) { _error.Text = "Prstenec musí mít alespoň tři vrcholy."; return; }
                    var a = part.Points[0]; var b = part.Points[^1];
                    if (a.X != b.X || a.Y != b.Y || a.Z != b.Z) part.Points.Add((DtmPoint)a.Clone());
                    if (part.Points.Count < 4) { _error.Text = "Prstenec musí mít alespoň tři různé vrcholy."; return; }
                }
                else if (part.Points.Count < 2) { _error.Text = "Část linie musí mít alespoň dva vrcholy."; return; }
            }
            var result = new List<IDtmGeometry>();
            foreach (var draft in _draft)
            {
                var original = _target.FirstOrDefault(g => g.GetType() == draft.GetType() && g.Id == draft.Id);
                if (original == null) { result.Add(draft); continue; }
                CopyCoordinates(draft, original);
                result.Add(original);
            }
            _target.Clear();
            foreach (var geometry in result) _target.Add(geometry);
            DialogResult = true;
        }

        static void CopyCoordinates(IDtmGeometry source, IDtmGeometry target)
        {
            switch (source)
            {
                case DtmPointGeometry point: ((DtmPointGeometry)target).Point = point.Point; break;
                case DtmMultiCurveGeometry multi:
                    var destination = (DtmMultiCurveGeometry)target;
                    destination.Members = multi.Members;
                    destination.Points = multi.Members[0].Points;
                    break;
                case DtmCurveGeometry curve: ((DtmCurveGeometry)target).Points = curve.Points; break;
                case DtmPolygonGeometry polygon:
                    ((DtmPolygonGeometry)target).Points = polygon.Points;
                    ((DtmPolygonGeometry)target).InteriorRings = polygon.InteriorRings;
                    break;
            }
        }

        public static IDtmGeometry Clone(IDtmGeometry source)
        {
            static List<DtmPoint> Points(IEnumerable<DtmPoint> values) => (values ?? Enumerable.Empty<DtmPoint>()).Select(p => (DtmPoint)p.Clone()).ToList();
            IDtmGeometry copy = source switch
            {
                DtmPointGeometry point => new DtmPointGeometry { Point = (DtmPoint)point.Point.Clone() },
                DtmMultiCurveGeometry multi => new DtmMultiCurveGeometry { Points = Points(multi.Points), Members = multi.Curves.Select(c => (DtmCurveGeometry)Clone(c)).ToList() },
                DtmCurveGeometry curve => new DtmCurveGeometry { Points = Points(curve.Points) },
                DtmSurfaceGeometry surface => new DtmSurfaceGeometry { Points = Points(surface.Points), InteriorRings = surface.InteriorRings.Select(Points).ToList() },
                DtmPolygonGeometry polygon => new DtmPolygonGeometry { Points = Points(polygon.Points), InteriorRings = polygon.InteriorRings.Select(Points).ToList() },
                _ => throw new ArgumentException("Neznámý typ geometrie.")
            };
            copy.Id = source.Id; copy.SrsDimension = source.SrsDimension; copy.SrsName = source.SrsName;
            return copy;
        }
    }
}
