using System;
using GeoBase.Localization;
using GeoBase.Utils;
using GeoCalculations.Protocol;

namespace GeoCalculations.Deviations
{
    [Serializable]
    public abstract class DeviationBase
    {
        protected DeviationBase(object owner)
        {
            Owner = owner;
        }
        public double LimitDeviation
        { get; protected set; }
        public double CalculationDeviation
        { get; protected set; }
        public object Owner
        { get; private set; }
        public virtual bool Exceeded
        {
            get { return Math.Abs(CalculationDeviation) > Math.Abs(LimitDeviation); }
        }
        protected string GetString(string deviationId)
        {
            var dictionary = LanguageConverter.ResolveDictionary();
            return dictionary.Translate<string>(deviationId, "DeviationText");
        }
        protected string GetString(int deviationId, ResourceParams par)
        {
            var dictionary = LanguageConverter.ResolveDictionary();
            return dictionary.Translate("Deviations." + deviationId, "DeviationText", par);
        }
        protected string GetStringFromId(int aId)
        {
            var par = new ResourceParams();
            par.Add("vypoctena", ProtocolUnitsPlugin.DoubleToString(4,CalculationDeviation));
            par.Add("mezni", ProtocolUnitsPlugin.DoubleToString(4,LimitDeviation));
            return GetString(aId, par);
        }
    }
}
