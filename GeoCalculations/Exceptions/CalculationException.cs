using System;
using GeoBase.Localization;
using GeoBase.Utils;

namespace GeoCalculations.Exceptions
{
    public class CalculationException : Exception
    {
        protected string ResolveException(string exceptionId, ResourceParams resParams)
        {
            exceptionId = "Exception." + exceptionId;
            var dictionary = LanguageConverter.ResolveDictionary();
            return resParams != null ? dictionary.Translate(exceptionId, "Description", resParams) : dictionary.Translate<string>(exceptionId, "Description");
        }
        public string Description
        {
            get;
            protected set;
        }
    }

}
