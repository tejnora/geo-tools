using System;
using System.Collections.Generic;
using System.Windows;
using System.Globalization;
using GeoBase.Utils;

namespace GeoBase.Localization
{
    public abstract class LanguageDictionary
    {
        private static readonly Dictionary<CultureInfo, LanguageDictionary> RegisteredDictionaries = new Dictionary<CultureInfo, LanguageDictionary>();
        public CultureInfo Culture { get { return CultureInfo.GetCultureInfo(CultureName); } }
        public static LanguageDictionary Current { get { return GetDictionary(LanguageContext.Instance.Culture); } }

        public void Load()
        {
            OnLoad();
        }

        public void Unload()
        {
            OnUnload();
        }

        public TValue Translate<TValue>(string uid, string vid)
        {
            return (TValue)Translate(uid, vid, null, typeof(TValue));
        }

        public string TranslateText(string uid)
        {
            return Translate<string>(uid, "Text");
        }

        public string TranslateText(string uid, ResourceParams @params)
        {
            return Translate(uid, "Text", @params);
        }

        public string Translate(string uid, string vid, ResourceParams @params)
        {
            var message = Translate<string>(uid, vid);
            if (message == null) return string.Empty;
            if (@params != null)
            {
                foreach (var param in @params.Params)
                {
                    message = message.Replace("#" + param.Key + "#", param.Value);
                }
            }
            return message;
        }

        public object Translate(string uid, string vid, object defaultValue, Type type)
        {
            return OnTranslate(uid, vid, defaultValue, type);
        }

        public MessageBoxResult ShowMessageBox(string uid, ResourceParams param, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage)
        {
            return MessageBox.Show(Application.Current.MainWindow, Translate(uid, "Text", param),
                            Translate<string>(uid, "Caption"), messageBoxButton, messageBoxImage);
        }

        public static void RegisterDictionary(CultureInfo cultureInfo, LanguageDictionary dictionary)
        {
            if (!RegisteredDictionaries.ContainsKey(cultureInfo))
            {
                RegisteredDictionaries.Add(cultureInfo, dictionary);
            }
        }

        public static void UnregisterDictionary(CultureInfo cultureInfo)
        {
            if (RegisteredDictionaries.ContainsKey(cultureInfo))
            {
                RegisteredDictionaries.Remove(cultureInfo);
            }
        }

        public static LanguageDictionary GetDictionary(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException("cultureInfo");
            }
            if (RegisteredDictionaries.ContainsKey(cultureInfo))
            {
                var dictionary = RegisteredDictionaries[cultureInfo];
                return dictionary;
            }
            return new NullLanguageDictionary();
        }

        public virtual string CultureName { get; protected set; }
        public virtual string EnglishName { get; protected set; }
        protected abstract void OnLoad();
        protected abstract void OnUnload();
        protected abstract object OnTranslate(string uid, string vid, object defaultValue, Type type);

        private sealed class NullLanguageDictionary : LanguageDictionary
        {
            protected override void OnLoad() { }
            protected override void OnUnload() { }
            protected override object OnTranslate(string uid, string vid, object defaultValue, Type type) { return defaultValue; }

            public override string CultureName
            {
                get { return CultureInfo.InstalledUICulture.Name; }
                protected set { }
            }

            public override string EnglishName
            {
                get { return CultureInfo.InstalledUICulture.EnglishName; }
                protected set { }
            }
        };

    }
}
