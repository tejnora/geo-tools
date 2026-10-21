using System.Windows;
using System.Windows.Controls;
using CAD.DTM.Gui;
using CAD.GUI;

namespace CAD.DTM.Elements.GUI
{
    public sealed class DtmGeometryCustomProperty : IDtmCustomProperty
    {
        readonly DtmElement _element;
        public DtmGeometryCustomProperty(DtmElement element) => _element = element;
        public string Name => "Geometrie";
        public IPropPage PropPage { get; set; }
        public Control GetEditControl()
        {
            var button = new Button { Content = "Upravit vrcholy, části a otvory…" };
            button.Click += (_, _) =>
            {
                var dialog = new DtmGeometryEditorDialog(_element) { Owner = Application.Current?.MainWindow };
                if (dialog.ShowDialog() == true) PropPage?.InvalidateCanvas();
            };
            return button;
        }
    }
}
