using System;
using System.Windows.Data;
using System.Globalization;
using System.Diagnostics;

namespace GeoBase.Localization
{
    public class LanguageConverter : IValueConverter, IMultiValueConverter
    {
        string _uid;
        readonly string _vid;
        readonly object _defaultValue;
        bool _isStaticUid;

        public LanguageConverter(string uid, string vid, object defaultValue, string nameSpace)
        {
            _uid = string.IsNullOrEmpty(nameSpace) ? uid : nameSpace + "." + uid;
            _vid = vid;
            _defaultValue = defaultValue;
            _isStaticUid = true;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dictionary = ResolveDictionary();
            var translation = dictionary.Translate(_uid, _vid, _defaultValue, targetType);
            return translation;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var parametersCount = _isStaticUid ? values.Length - 1 : values.Length - 2;
                if (string.IsNullOrEmpty(_uid))
                {
                    if (values[1] == null)
                    {
                        throw new ArgumentNullException("Uid must be provided as the first Binding element, and must not be null");
                    }
                    _isStaticUid = false;
                    _uid = values[1].ToString();
                    --parametersCount;
                }
                var dictionary = ResolveDictionary();
                var translatedObject = dictionary.Translate(_uid, _vid, _defaultValue, targetType);
                if (translatedObject != null && parametersCount != 0)
                {
                    var parameters = new object[parametersCount];
                    Array.Copy(values, values.Length - parametersCount, parameters, 0, parameters.Length);
                    try
                    {
                        translatedObject = string.Format(translatedObject.ToString(), parameters);
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine(string.Format("LanguageConverter failed to format text {0}", translatedObject));
                    }
                }
                return translatedObject;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("LanguageConverter failed to convert text: {0}", ex.Message));
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[0];
        }

        public static LanguageDictionary ResolveDictionary()
        {
            var dictionary = LanguageDictionary.GetDictionary(LanguageContext.Instance.Culture);
            if (dictionary == null)
                throw new InvalidOperationException(string.Format("Dictionary for language {0} was not found", LanguageContext.Instance.Culture));
            return dictionary;
        }
    }
}
