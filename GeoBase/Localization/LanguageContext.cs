using System;
using System.Globalization;
using System.Threading;
using System.ComponentModel;

namespace GeoBase.Localization
{
    public sealed class LanguageContext : INotifyPropertyChanged
    {
        public static readonly LanguageContext Instance = new LanguageContext();
        private CultureInfo _cultureInfo;
        private LanguageDictionary _dictionary;
        public CultureInfo Culture
        {
            get { return _cultureInfo ?? (_cultureInfo = CultureInfo.CurrentUICulture); }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Culture must not be null");
                }
                if (value == _cultureInfo)
                {
                    return;
                }
                if (_cultureInfo != null)
                {
                    var currentDictionary = LanguageDictionary.GetDictionary(_cultureInfo);
                    currentDictionary.Unload();
                }
                _cultureInfo = value;
                var newDictionary = LanguageDictionary.GetDictionary(_cultureInfo);
                Thread.CurrentThread.CurrentUICulture = _cultureInfo;
                newDictionary.Load();
                Dictionary = newDictionary;
                OnPropertyChanged("Culture");
            }
        }

        public LanguageDictionary Dictionary
        {
            get { return _dictionary; }
            set
            {
                if (value == null || value == _dictionary) return;
                _dictionary = value;
                OnPropertyChanged("Dictionary");
            }
        }

        private LanguageContext() { }
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
