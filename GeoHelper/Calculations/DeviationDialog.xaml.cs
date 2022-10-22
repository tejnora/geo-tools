using System.Linq;
using System.Windows.Documents;
using GeoCalculations.Deviations;

namespace GeoHelper.Calculations
{
    public partial class DeviationDialog
    {
        public DeviationDialog(CalculationDeviation deviations)
            : base("VypoctyOdchylkyDialog")
        {
            InitializeComponent();
            int counter = 1;
            var par = new Paragraph();
            _textBlock.Document.Blocks.Clear();
            foreach (DeviationBase o in deviations.Deviations.Where(o => o.Exceeded))
            {
                par.Inlines.Add(string.Format("{0}. - {1}\n", counter++, o));
            }
            _textBlock.Document.Blocks.Add(par);
        }
    }
}