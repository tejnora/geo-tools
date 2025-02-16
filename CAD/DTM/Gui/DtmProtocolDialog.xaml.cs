using System;
using GeoBase.Gui;
using System.Windows;

namespace CAD.DTM.Gui
{
    public partial class DtmProtocolDialog : DialogBase
    {
        public DtmProtocolDialog(string text)
        : base("DtmProtocolDialog", false, true)
        {
            InitializeComponent();
            Text = text;
            DataContext = this;
        }

        void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public string Text { get; set; }
    }
}
