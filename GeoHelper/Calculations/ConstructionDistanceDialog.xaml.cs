using System;
using System.Windows;
using System.Windows.Controls;
using GeoBase.Localization;
using GeoCalculations.CalculationContexts;
using GeoCalculations.CalculationResultContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoHelper.ExtensionMethods;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;

namespace GeoHelper.Calculations
{
    public partial class ConstructionDistanceDialog
    {
        public ConstructionDistanceDialog(string dialogName, IMainWindow aMainWindow)
            : base(dialogName, aMainWindow)
        {
            InitializeComponent();
            DataContext = this;
        }

        public ConstructionDistanceContex IdenticallyPoints { get; set; }
        public ConstructionDistanceResultContext CalculatedPoints { get; set; }
        public bool RemoveEnabled
        {
            get
            {
                return IdenticallyPoints != null && IdenticallyPoints.TableNodes.Count > 0 &&
                       _dataGridIdenticallyPoints.SelectedItem is ConstructionDistancePoint;
            }
        }

        protected override void Calculate()
        {
            ConstructionDistanceMethod.Calculate(IdenticallyPoints, CalculatedPoints);
        }

        protected override void ResetCalculation(object sender, RoutedEventArgs e)
        {
            IdenticallyPoints = new ConstructionDistanceContex();
            CalculatedPoints = new ConstructionDistanceResultContext();
            NotifyPropertiesChanged();
        }


        void NotifyPropertiesChanged()
        {
            OnPropertyChanged("IdenticallyPoints");
            OnPropertyChanged("CalculatedPoints");
            OnPropertyChanged("RemoveEnabled");
        }

        void OnKey(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void OnAdd(object sender, RoutedEventArgs e)
        {
            var selIndex = _dataGridIdenticallyPoints.SelectedIndex;
            var newBod = (ConstructionDistancePoint)IdenticallyPoints.Node.Clone();
            if (double.IsNaN(newBod.Distance) && IdenticallyPoints.TableNodes.Count > 0)
            {
                var dictionary = LanguageConverter.ResolveDictionary();
                dictionary.ShowMessageBox("ConstuctionDistanceDialog.16", null, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selIndex == -1)
            {
                IdenticallyPoints.TableNodes.Add(newBod);
                selIndex = 0;
            }
            else
                IdenticallyPoints.TableNodes.Insert(selIndex, newBod);
            _dataGridIdenticallyPoints.SelectedIndex = selIndex + 1;
            var point = IdenticallyPoints.Node.Number;
            IdenticallyPoints.Node = new ConstructionDistancePoint { Number = point.Icrement() };
            NotifyPropertiesChanged();
        }

        void OnRemove(object sender, RoutedEventArgs e)
        {
            var selIndex = _dataGridIdenticallyPoints.SelectedIndex;
            IdenticallyPoints.TableNodes.Remove((ConstructionDistancePoint)_dataGridIdenticallyPoints.SelectedItem);
            if (selIndex >= IdenticallyPoints.TableNodes.Count) _dataGridIdenticallyPoints.SelectedIndex = selIndex - 1;
            else _dataGridIdenticallyPoints.SelectedIndex = selIndex;
            CalculatedPoints = new ConstructionDistanceResultContext();
            NotifyPropertiesChanged();
        }

        void OnIdenticallyPointsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataGridIdenticallyPoints.SelectedItem == null) return;
            var constructionDistancePoint = _dataGridIdenticallyPoints.SelectedItem as ConstructionDistancePoint;
            if (constructionDistancePoint != null)
                IdenticallyPoints.Node = (ConstructionDistancePoint)constructionDistancePoint.Clone();
            NotifyPropertiesChanged();
        }

        void OnIdenticallyPointsDragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("SeznamSouradnicTabulka")) return;
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        void OnIdenticallyPointsDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("SeznamSouradnicTabulka")) return;
            var table = (DragDropDataObjectBase)e.Data.GetData("SeznamSouradnicTabulka");
            IdenticallyPoints.Node.LoadValues(new CoordinatePointAdapter(table.Data as TableCoordinateListNode));
            NotifyPropertiesChanged();
        }
    }
}
