using System.Collections.Generic;
using GeoBase.Utils;

namespace GeoCalculations.CalculationContexts
{
    public class PolarMethodBatchContext : CalculationContextBase
    {
        readonly PropertyData _inputFileNameProperty = RegisterProperty("InputFileName", typeof(string), string.Empty);
        public string InputFileName
        {
            get { return GetValue<string>(_inputFileNameProperty); }
            set { SetValue(_inputFileNameProperty, value); }
        }

        readonly PropertyData _outputFileNameProperty = RegisterProperty("OutputFileName", typeof(string), string.Empty);
        public string OutputFileName
        {
            get { return GetValue<string>(_outputFileNameProperty); }
            set { SetValue(_outputFileNameProperty, value); }
        }

        readonly PropertyData _useOnlySelectedNodesProperty = RegisterProperty("UseOnlySelectedNodes", typeof(bool), false);
        public bool UseOnlySelectedNodes
        {
            get { return GetValue<bool>(_useOnlySelectedNodesProperty); }
            set { SetValue(_useOnlySelectedNodesProperty, value); }
        }

        readonly PropertyData _editOrientationProperty = RegisterProperty("EditOrientation", typeof(bool), true);
        public bool EditOrientation
        {
            get { return GetValue<bool>(_editOrientationProperty); }
            set { SetValue(_editOrientationProperty, value); }
        }

        readonly PropertyData _calculateFreePointOfViewProperty = RegisterProperty("CalculateFreePointOfView", typeof(bool), true);
        public bool CalculateFreePointOfView
        {
            get { return GetValue<bool>(_calculateFreePointOfViewProperty); }
            set { SetValue(_calculateFreePointOfViewProperty, value); }
        }

        readonly PropertyData _numberOfPointOfViewsProperty = RegisterProperty("NumberOfPointOfViews", typeof(int), 0);
        public int NumberOfPointOfViews
        {
            get { return GetValue<int>(_numberOfPointOfViewsProperty); }
            set { SetValue(_numberOfPointOfViewsProperty, value); }
        }
        public void IncrementNumberOfPointOfViews()
        {
            NumberOfPointOfViews = NumberOfPointOfViews + 1;
        }

        readonly PropertyData _numberOfDetailedPointsProperty = RegisterProperty("NumberOfDetailedPoints", typeof(int), 0);
        public int NumberOfDetailedPoints
        {
            get { return GetValue<int>(_numberOfDetailedPointsProperty); }
            set { SetValue(_numberOfDetailedPointsProperty, value); }
        }

        public void IncrementNumberOfDetailedPoints()
        {
            NumberOfDetailedPoints = NumberOfDetailedPoints + 1;
        }

        readonly PropertyData _numberOfUnusedMesuringPointsProperty = RegisterProperty("NumberOfUnusedMeasuringPoints", typeof(int), 0);
        public int NumberOfUnusedMeasuringPoints
        {
            get { return GetValue<int>(_numberOfUnusedMesuringPointsProperty); }
            set { SetValue(_numberOfUnusedMesuringPointsProperty, value); }
        }
        public void IncrementNumberOfUnusedMeasuringPoints()
        {
            NumberOfUnusedMeasuringPoints = NumberOfUnusedMeasuringPoints + 1;
        }

        readonly PropertyData _polarMethodsResultsProperty = RegisterProperty("PolarMethodResults", typeof(List<string>), new List<string>());
        public List<string> PolarMethodResults
        {
            get { return GetValue<List<string>>(_polarMethodsResultsProperty); }
            set { SetValue(_polarMethodsResultsProperty, value); }
        }

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();
            PolarMethodResults.Clear();
        }

    }
}
