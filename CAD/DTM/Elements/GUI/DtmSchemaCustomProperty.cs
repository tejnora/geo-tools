using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CAD.DTM.Configuration;
using CAD.GUI;
using CAD.DTM.Gui;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace CAD.DTM.Elements.GUI
{
    public sealed class DtmSchemaCustomProperty : IDtmCustomProperty
    {
        readonly DtmSchemaAttributes _attributes;
        readonly string _path;
        readonly DtmAttributeDefinition _definition;
        readonly bool _required;

        public DtmSchemaCustomProperty(DtmSchemaAttributes attributes, string path, DtmAttributeDefinition definition, bool required)
        {
            _attributes = attributes;
            _path = path;
            _definition = definition;
            _required = required;
        }

        public string Name
        {
            get
            {
                var parts = _path.Split('/');
                var prefix = parts.Length == 1 ? "" : parts[0] switch
                {
                    "SpolecneAtributyVsechObjektu" => "Obecné / ",
                    "SpolecneAtributyObjektuDefinicnichBodu" => "Definiční bod / ",
                    _ => parts[0].Replace("SpolecneAtributyObjektu", "").Replace("SpolecneAtributy", "") + " / "
                };
                return prefix + Regex.Replace(parts[^1], @"(?<=[a-z])(?=[A-Z])", " ") + (_required ? " *" : "");
            }
        }
        public IPropPage PropPage { get; set; }

        public Control GetEditControl()
        {
            if (_definition.Type == "complex") return RegionEditor();
            var present = _attributes.Values.TryGetValue(_path, out var value);
            if (_definition.Values.Count > 0 || _definition.Type == "boolean")
            {
                var combo = new ComboBox { SelectedValuePath = "Tag", ToolTip = _definition.Description };
                combo.Items.Add(new ComboBoxItem { Content = "(nezadáno)", Tag = null });
                var choices = _definition.Type == "boolean"
                    ? new System.Collections.Generic.Dictionary<string, string> { ["false"] = "Ne", ["true"] = "Ano" }
                    : _definition.Values;
                foreach (var choice in choices)
                    combo.Items.Add(new ComboBoxItem { Content = choice.Value + " [" + choice.Key + "]", Tag = choice.Key });
                if (_definition.Type == "boolean" && present) value = value == "1" || value == "true" ? "true" : "false";
                var selected = combo.Items.OfType<ComboBoxItem>().FirstOrDefault(i => (string)i.Tag == (present ? value : null));
                if (selected == null && present)
                {
                    selected = new ComboBoxItem { Content = "Neznámá hodnota [" + value + "]", Tag = value };
                    combo.Items.Add(selected);
                }
                combo.SelectedItem = selected;
                combo.SelectionChanged += (_, _) =>
                {
                    _attributes.SetText(_path, (string)(combo.SelectedItem as ComboBoxItem)?.Tag);
                    combo.BorderBrush = _required && combo.SelectedIndex == 0 ? Brushes.Firebrick : SystemColors.ControlDarkBrush;
                    PropPage?.InvalidateCanvas();
                };
                return combo;
            }

            var panel = new DockPanel { LastChildFill = true };
            var check = new CheckBox { ToolTip = "Údaj je vyplněn. Zrušením zaškrtnutí jej vynecháte z exportu; zaškrtnuté prázdné pole znamená prázdný text.", IsChecked = present, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0, 0, 0) };
            DockPanel.SetDock(check, Dock.Right);
            panel.Children.Add(check);
            var text = new TextBox
            {
                Text = value ?? "", IsEnabled = present, MinWidth = 50,
                AcceptsReturn = _definition.Type == "complex", TextWrapping = TextWrapping.Wrap,
                MinHeight = _definition.Type == "complex" ? 64 : 20,
                ToolTip = _definition.Description + (_definition.Type == "date" ? " (RRRR-MM-DD)" : _definition.Type == "dateTime" ? " (ISO 8601)" : "")
            };
            panel.Children.Add(text);
            void Update()
            {
                text.IsEnabled = check.IsChecked == true;
                _attributes.SetText(_path, check.IsChecked == true ? text.Text : null);
                var error = check.IsChecked == true ? DtmSchemaAttributes.ValidateValue(_definition, text.Text) : _required ? "Povinný údaj není vyplněn." : null;
                text.BorderBrush = error != null ? Brushes.Firebrick : SystemColors.ControlDarkBrush;
                text.ToolTip = error ?? _definition.Description;
                PropPage?.InvalidateCanvas();
            }
            check.Checked += (_, _) => Update();
            check.Unchecked += (_, _) => Update();
            text.TextChanged += (_, _) => { if (check.IsChecked == true) Update(); };
            return new ContentControl { Content = panel };
        }

        Control RegionEditor()
        {
            var panel = new DockPanel();
            var edit = new Button { Content = "Vymezit / upravit oblast…", ToolTip = _definition.Description };
            var remove = new Button { Content = "Vymazat", Margin = new Thickness(4, 0, 0, 0), IsEnabled = _attributes.Values.ContainsKey(_path) };
            DockPanel.SetDock(remove, Dock.Right);
            panel.Children.Add(remove); panel.Children.Add(edit);
            remove.Click += (_, _) =>
            {
                _attributes.SetText(_path, null);
                remove.IsEnabled = false;
                PropPage?.InvalidateCanvas();
            };
            edit.Click += (_, _) =>
            {
                var geometry = new DtmSurfaceGeometry { Id = "KI" + Guid.NewGuid().ToString("N"), SrsDimension = 2, Points = new List<DtmPoint>() };
                if (_attributes.Values.TryGetValue(_path, out var raw))
                {
                    var xml = new XmlDocument(); xml.LoadXml(raw);
                    geometry = (DtmSurfaceGeometry)DtmGeometryXml.Read(xml.DocumentElement.ChildNodes.OfType<XmlElement>().Single());
                }
                var target = new List<IDtmGeometry> { geometry };
                var dialog = new DtmGeometryEditorDialog(_attributes.Owner, target, new[] { "surfaceProperty" })
                {
                    Owner = Application.Current?.MainWindow, Title = "Oblast kritické infrastruktury"
                };
                if (dialog.ShowDialog() != true) return;
                using var exporter = new DtmXmlFragmentWriter();
                target[0].ExportToDtm(exporter);
                var element = new XElement(XName.Get(_definition.Name, "atr"), XElement.Parse(exporter.ToString()));
                _attributes.SetText(_path, element.ToString(SaveOptions.DisableFormatting));
                remove.IsEnabled = true;
                PropPage?.InvalidateCanvas();
            };
            return new ContentControl { Content = panel };
        }
    }
}
