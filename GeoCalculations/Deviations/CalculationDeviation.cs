using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeoCalculations.Deviations
{
    [Serializable]
    public class CalculationDeviation : IEnumerable<DeviationBase>
    {
        public CalculationDeviation()
        {
            Deviations = new List<DeviationBase>();
        }
        public List<DeviationBase> Deviations
        { get; private set; }

        public bool HasError
        {
            get { return Deviations.Any(deviation => deviation.Exceeded); }
        }
        public void ShowErrorDialog()
        {
            //todo
            //new VypoctyOdchylkyDialog(this).ShowDialog();
        }

        public IEnumerable<DeviationBase> GetDeviations<T>()
        {
            var result = new List<DeviationBase>();
            foreach (var deviationBase in Deviations)
            {
                if (deviationBase is T)
                    result.Add(deviationBase);
            }
            return result;
        }

        public Dictionary<object, BalancedDirectionAzimutDeviation> GetPrekroceneOpravyOrientace()
        {
            var result = new Dictionary<object, BalancedDirectionAzimutDeviation>();
            foreach (var odchylka in Deviations)
            {
                if (!odchylka.Exceeded || !(odchylka is BalancedDirectionAzimutDeviation)) continue;
                result[odchylka.Owner] = odchylka as BalancedDirectionAzimutDeviation;
            }
            return result;
        }

        public IEnumerator<DeviationBase> GetEnumerator()
        {
            return Deviations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Deviations.GetEnumerator();
        }
    }
}
