using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace CAD.DTM.Gui
{
    internal partial class DtmExportProgressDialog : Window
    {
        readonly DtmExportProgress _progress;
        bool _started;
        bool _finished;

        internal DtmExportProgressDialog(DtmExportProgress progress)
        {
            _progress = progress;
            InitializeComponent();
            DataContext = progress;
        }

        async void OnContentRendered(object sender, EventArgs e)
        {
            if (_started) return;
            _started = true;
            try
            {
                await _progress.RunAsync(async () => await Dispatcher.Yield(DispatcherPriority.ApplicationIdle));
                _summary.Text = _progress.HasFailures ? "Některé akce selhaly. Podrobnosti najdete v nápovědě u jednotlivých akcí."
                    : "Export DTM byl dokončen.";
            }
            finally
            {
                _finished = true;
                _close.IsEnabled = true;
                _close.Focus();
            }
        }

        void OnClosing(object sender, CancelEventArgs e) => e.Cancel = !_finished;
        void OnClose(object sender, RoutedEventArgs e) => Close();
    }
}
