using System;
using System.Windows;
using System.Windows.Threading;
using GeoBase.Localization;

namespace GeoHelper.Tools
{
    public partial class FileTransferDownloadDialog
    {
        public FileTransferDownloadDialog(FileTransferDialog owner)
            : base("PrenosSouboruStahovaniDialog")
        {
            InitializeComponent();
        }

        bool _isLabelUpdated;

        protected void OnLoaded(object sender, RoutedEventArgs args)
        {
            var dictionary = LanguageConverter.ResolveDictionary();
            _text.Content = dictionary.Translate<string>("FileTransferDownloadDialog.1", "Content");
            _cancleButton.Content = dictionary.Translate<string>("FileTransferDownloadDialog.1", "Button");
        }

        protected void OnCancleClick(object sender, EventArgs args)
        {
            Close();
        }

        public void BeginDownload()
        {
            if (_isLabelUpdated)
                return;
            Dispatcher.Invoke(DispatcherPriority.Normal, new TimeSpan(0, 0, 0, 0, 500), new Action(UpdateLabel));
        }

        void UpdateLabel()
        {
            var dictionary = LanguageConverter.ResolveDictionary();
            _text.Content = dictionary.Translate<string>("FileTransferDownloadDialog.2", "Content");
            _cancleButton.Content = dictionary.Translate<string>("FileTransferDownloadDialog.2", "Button");
            _isLabelUpdated = true;
        }
    }
}