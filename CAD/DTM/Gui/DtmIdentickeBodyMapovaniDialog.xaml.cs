using System;
using System.Windows;
using GeoBase.Gui;
namespace CAD.DTM.Gui
{
    public partial class DtmIdentickeBodyMapovaniDialog : DialogBase
    {
        readonly DtmIdentickeBodyMapovaniCtx _ctx;

        public DtmIdentickeBodyMapovaniDialog(DtmIdentickeBodyMapovaniCtx ctx)
            : base("DtmIdentickeBodyMapovaniDialog", false, true)
        {
            _ctx = ctx;
            DataContext = ctx;
            InitializeComponent();
        }

        public bool? DoModal()
        {
            return ShowDialog();
        }

        void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void OnConfirm(object sender, RoutedEventArgs e)
        {
            _ctx.ApplyChanges();
            DialogResult = true;
            SavePosAndSize();
            Close();
        }

        void OnAutoMap(object sender, RoutedEventArgs e)
        {
            _ctx.AutoMap();
        }

        void OnShowProtocol(object sender, RoutedEventArgs e)
        {
            var pointMappingBakcup = _ctx.DtmMain.IndeticalPointsMapping;
            try
            {
                _ctx.ApplyChanges();
                var identickeBodyReport = new DtmIdentickeBodyProtocolBuilder(_ctx.DtmMain);
                var text = identickeBodyReport.CreateProtocol();
                var dialog = new DtmProtocolDialog(text);
                dialog.ShowDialog();
            }
            finally
            {
                _ctx.DtmMain.IndeticalPointsMapping = pointMappingBakcup;
            }
        }
    }
}
