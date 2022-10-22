using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GeoBase.Gui;

namespace GeoHelper.Controls
{
    public partial class ProgressBarDialog : DialogBase
    {
        public ProgressBarDialog(Action<ProgressBarDialog, CancellationToken> task)
            : base("ProgessBar", true)
        {
            InitializeComponent();
            DataContext = this;
            _task = task;
        }

        readonly object _progessBarValueLock = new object();
        readonly Action<ProgressBarDialog, CancellationToken> _task;
        CancellationTokenSource _canceler;
        double _progessBarValue;

        public double ProgressBarValue
        {
            get
            {
                lock (_progessBarValueLock)
                {
                    return _progessBarValue;
                }
            }
            set
            {
                lock (_progessBarValueLock)
                {
                    _progessBarValue = value;
                    if (value == 100)
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(Close));
                        return;
                    }
                }
                OnPropertyChanged("ProgressBarValue");
            }
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            _canceler.Cancel();
            Close();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            _canceler = new CancellationTokenSource();
            CancellationToken token = _canceler.Token;
            Task.Factory.StartNew(() => _task(this, token));
        }
    }
}