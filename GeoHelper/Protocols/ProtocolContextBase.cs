using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoBase.Localization;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols
{
    public class ProtocolContextBase<TCalculatedPoint> : IProtocolContext
        where TCalculatedPoint : CalculatedPointBase
    {
        [ProtocolPropertyData("CalculatedPoints")]
        public List<TCalculatedPoint> CalculatedPoints { get { return _calculatedPoints; } }

        protected readonly List<TCalculatedPoint> _calculatedPoints = new List<TCalculatedPoint>();
        protected readonly Dictionary<int, ProtocolCoordinateReplaceContext> _replacedNodes = new Dictionary<int, ProtocolCoordinateReplaceContext>();

        public void Clear()
        {
            _calculatedPoints.Clear();
            _replacedNodes.Clear();
        }

        public virtual void AddCalculatedPoint(CalculatedPointBase point)
        {
            if (point == null) return;
            _calculatedPoints.Add((TCalculatedPoint)point.Clone());
        }

        public virtual void AddPointConversion(string numberWithPrefix, ProtocolCoordinateReplaceContext coordinateReplaceContext)
        {
            Debug.Assert((_calculatedPoints.Last().NumberWithPrefix == numberWithPrefix));
            _replacedNodes[_calculatedPoints.Count - 1] = coordinateReplaceContext;
        }

        [ProtocolMethod("GetConversionString")]
        public virtual string GetConversionString(string array, int arrayIndex)
        {
            ProtocolCoordinateReplaceContext conversionContext;
            if (!_replacedNodes.TryGetValue(arrayIndex, out conversionContext)) return string.Empty;
            var converstionString = EvaluateProtocolTemplate("CoordinateCorrection", conversionContext);
            return converstionString;
        }

        protected string EvaluateProtocolTemplate(string templateName, object data)
        {
            var template = LanguageDictionary.Current.Translate<string>("Protocols.ProtocolTemplate." + templateName, "Text");
            if (string.IsNullOrEmpty(template))
                throw new ArgumentException("Template can not be empty:.");
            var dataProperties = ScanProtocolResultData.GetProperties(data.GetType());
            var parsedTemplate = ProtocolGrammar.Terms.Parse(template.Replace("\r\n", ""));
            var generator = new ProtocolGenerator();
            generator.RegisterPlugin(ProtocolPluginTypes.Units, new ProtocolUnitsPlugin(Singletons.MyRegistry));
            var evaluatedString = generator.Eval(parsedTemplate[0], dataProperties, data);
            return evaluatedString;
        }
    }
}
