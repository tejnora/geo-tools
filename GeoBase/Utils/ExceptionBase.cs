using System;
using GeoBase.Localization;

namespace GeoBase.Utils
{
    public class ExceptionBase : Exception
    {
        
    }

    public class LanguageDictionaryException : ExceptionBase
    {
        public LanguageDictionaryException(string messageId)
        {
            Dictionary = LanguageConverter.ResolveDictionary();
            MessageId = messageId;
        }

        public LanguageDictionaryException(string messageId, ResourceParams par)
        {
            Dictionary = LanguageConverter.ResolveDictionary();
            MessageId = messageId;
            Params = par;
        }

        private LanguageDictionary Dictionary
        { get; set; }

        private string MessageId
        {
            get; set;
        }

        private ResourceParams Params
        { 
            get; set;
        }
        public override string ToString()
        {
            return Dictionary.Translate(MessageId,"Text",Params);
        }
    }
}
