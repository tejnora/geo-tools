using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GeoBase.Localization;
using GeoBase.Utils;
using GeoCalculations.CalculationContexts;
using GeoCalculations.CalculationResultContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoHelper.Protocols;
using GeoHelper.Tables;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;

namespace GeoHelper.Calculations
{
    public partial class TransformCoordinatesDialog
    {
        public TransformCoordinatesDialog(string dialogName, IMainWindow aMainWindow)
            : base(dialogName, aMainWindow)
        {
            InitializeComponent();
            CalculationContext = new CoordinatesTransformationContex();
            ProtocolContext = new TransformCoordinatesProtocolContex(CalculationContext);
            DataContext = this;
            CalculationContext.PropertyChanged += OnCalculationContextChanged;
        }

        public CoordinatesTransformationContex CalculationContext { get; set; }
        protected override CalculationContextBase CalculationContextBase { get { return CalculationContext; } }
        TableBase _outputTable;
        TableBase _inputTalbe;

        protected override void ResetCalculation(object sender, RoutedEventArgs e)
        {
            CalculationContext.SetDefaultValues();
            _outputTable = null;
            _inputTalbe = null;
        }

        protected override void AddCalculatedPointIntoTable(CalculatedPointBase point)
        {
            if (_outputTable == null || point == null) return;
            _outputTable.AddNewNode(point, ProtocolContext);
        }

        protected override void Calculate()
        {
            CordinateTransformationMethod.Calculate(CalculationContext);
        }

        protected override void CalculatePoint()
        {
            if (_inputTalbe == null) return;
            var result = new CoordinateTransformationResultContext
                {
                    Nodes = new ObservableCollection<CordinateTransformationCalculatedPoint>(_inputTalbe.Nodes.Select(n =>
                            {
                                var node = new CordinateTransformationCalculatedPoint();
                                node.LoadValues(new CoordinatePointAdapter(n as TableCoordinateListNode));
                                node.XLocal = node.X;
                                node.X = double.NaN;
                                node.YLocal = node.Y;
                                node.Y = double.NaN;
                                return node;
                            }))
                };
            CordinateTransformationMethod.CalculatePoint(CalculationContext, result);
            foreach (var node in result.Nodes)
            {
                ProtocolContext.AddCalculatedPoint(node);
                AddCalculatedPointIntoTable(node);
            }
        }

        public void OnTableSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            CalculationContext.SelectionChanged(_dataGrid.SelectedItem);
        }

        void OnAddIntoTable(object sender, RoutedEventArgs e)
        {
            var point = (CordinateTransformationPoint)CalculationContext.Node.Clone();
            if (point.Number.Length == 0 || double.IsNaN(point.X) || double.IsNaN(point.Y) || double.IsNaN(point.XLocal) || double.IsNaN(point.YLocal))
            {
                var dictionary = LanguageConverter.ResolveDictionary();
                dictionary.ShowMessageBox("TransfromCoordinatesDialog.12", new ResourceParams(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CalculationContext.AddNode();
            TryToCalculate(true);
        }

        void OnRemoveFromTable(object sender, RoutedEventArgs e)
        {
            CalculationContext.RemoveNode((CordinateTransformationPoint)_dataGrid.SelectedItem);
            TryToCalculate(true);
        }

        void OnDropOverCoordinate(object sender, DragEventArgs e)
        {
            if (e.IsCoordinateListTable())
                e.Accept();
        }

        void OnDropCoordinate(object sender, DragEventArgs args)
        {
            if (!args.IsCoordinateListTable()) return;
            args.Accept();
            CalculationContext.Node.LoadValues(new CoordinatePointAdapter(args.TableNode<TableCoordinateListNode>()));
            CalculationContext.Node.XLocal = double.NaN;
            CalculationContext.Node.YLocal = double.NaN;
        }

        void OnDropLocalCoordinate(object sender, DragEventArgs args)
        {
            if (!args.IsCoordinateListTable()) return;
            args.Accept();
            var nodes = args.TableNode<TableCoordinateListNode>();
            CalculationContext.Node.XLocal = nodes.X;
            CalculationContext.Node.YLocal = nodes.Y;
        }

        void OnFileOver(object sender, DragEventArgs e)
        {
            if (e.IsCoordinateListTable())
                e.Accept();
        }

        void OnInputFile(object sender, DragEventArgs e)
        {
            if (!e.IsCoordinateListTable()) return;
            e.Accept();
            _inputTalbe = e.Table<CoordinateListTable>();
            if (e.Table<CoordinateListTable>().FileName == null)
                CalculationContext.InputFileName = LanguageDictionary.Current.Translate<string>("TransfromCoordinatesDialog.20", "Text");
            else
                CalculationContext.InputFileName = e.Table<CoordinateListTable>().FileName.FullName;
        }

        void OnOutputFile(object sender, DragEventArgs e)
        {
            if (!e.IsCoordinateListTable()) return;
            e.Accept();
            var table = (DragDropDataObjectBase)e.Data.GetData(CoordinateListTable.TableName);
            _outputTable = (TableBase)table.MdiWindow;
            if (e.Table<CoordinateListTable>().FileName == null)
                CalculationContext.OutputFileName = LanguageDictionary.Current.TranslateText("TransfromCoordinatesDialog.20");
            else
                CalculationContext.OutputFileName = e.Table<CoordinateListTable>().FileName.FullName;
        }

        void OnCalculationContextChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "TransformationType")
                TryToCalculate(true);
        }
    }
}