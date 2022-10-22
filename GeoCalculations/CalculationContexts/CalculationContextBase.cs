using System;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.Deviations;
using GeoCalculations.Protocol;

namespace GeoCalculations.CalculationContexts
{
    [Serializable]
    public class CalculationContextBase : DataObjectBase<CalculationContextBase>
    {
        public CalculationContextBase()
            : base(null, new StreamingContext())
        {

        }

        public CalculationContextBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        readonly PropertyData _deviationsProperty = RegisterProperty("Deviations", typeof(CalculationDeviation), new CalculationDeviation());
        [ProtocolPropertyData("Deviations")]
        public CalculationDeviation Deviations
        {
            get { return GetValue<CalculationDeviation>(_deviationsProperty); }
            set { SetValue(_deviationsProperty, value); }
        }

        public virtual void ResetBeforeCalculation()
        {
            Deviations = new CalculationDeviation();
        }
    }
}
