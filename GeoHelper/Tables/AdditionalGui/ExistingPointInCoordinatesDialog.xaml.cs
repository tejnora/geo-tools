using System;
using System.Runtime.Serialization;
using System.Windows;
using GeoBase.Localization;
using GeoBase.Utils;
using GeoCalculations.Protocol;
using GeoHelper.Calculations;
using GeoHelper.Protocols;
using GeoHelper.Tables.TableNodes;

namespace GeoHelper.Tables.AdditionalGui
{
    partial class ExistingPointInCoordinatesDialog
    {
        private readonly TableBase _tableBase;
        private readonly IProtocolContext _protocol;

        public ExistingPointInCoordinatesDialog(TableCoordinateListNode oldCoordinates, TableCoordinateListNode newCoordinates, TableBase tableBase, IProtocolContext protocol)
            : base("ExistingPointInCoordinatesDialog", false)
        {
            _tableBase = tableBase;
            _protocol = protocol;
            InitializeComponent();
            CoordinatesContext = new ExistingPointInCoordinatesContext
                                     {
                                         NewCoordinates = newCoordinates,
                                         OldCoordinates = oldCoordinates
                                     };
            DataContext = CoordinatesContext;
        }

        ExistingPointInCoordinatesContext CoordinatesContext { get; set; }

        protected override void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            base.OnOkButtonClick(sender, e);
            TableCoordinateListNode selectedNode;
            switch (CoordinatesContext.CoordinatePrefer)
            {
                case ExistingPointInCoordinatesContext.CoordinatePreferTypes.New:
                    selectedNode = CoordinatesContext.NewCoordinates.Clone() as TableCoordinateListNode;
                    break;
                case ExistingPointInCoordinatesContext.CoordinatePreferTypes.Old:
                    selectedNode = CoordinatesContext.OldCoordinates.Clone() as TableCoordinateListNode;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (selectedNode == null) return;
            var pointRenamed = false;
            if (!string.IsNullOrEmpty(CoordinatesContext.NewNumber))
            {
                selectedNode.Number = CoordinatesContext.NewNumber;
                pointRenamed = true;
            }
            if (!string.IsNullOrEmpty(CoordinatesContext.NewPrefix))
            {
                selectedNode.Prefix = CoordinatesContext.NewPrefix;
                pointRenamed = true;
            }

            if (CoordinatesContext.IsHeightEnabled)
            {
                switch (CoordinatesContext.HeightPrefer)
                {
                    case ExistingPointInCoordinatesContext.HeightPreferTypes.New:
                        selectedNode.Z = CoordinatesContext.NewCoordinates.Z;
                        break;
                    case ExistingPointInCoordinatesContext.HeightPreferTypes.Old:
                        selectedNode.Z = CoordinatesContext.OldCoordinates.Z;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            var resultContext = new ProtocolCoordinateReplaceContext();
            if (!pointRenamed)
            {
                if (!_tableBase.DeleteNodeRaw(CoordinatesContext.OldCoordinates, true, true))
                    return;
            }
            resultContext.WasPointRenamed = pointRenamed;
            _tableBase.InsertItem(selectedNode, -1, _protocol);
            resultContext.NewCoordinates.LoadValues(new CoordinatePointAdapter(CoordinatesContext.NewCoordinates));
            resultContext.OldCoordinates.LoadValues(new CoordinatePointAdapter(CoordinatesContext.OldCoordinates));
            resultContext.SavedCoordinates.LoadValues(new CoordinatePointAdapter(selectedNode));
            if (_protocol != null)
                _protocol.AddPointConversion(CoordinatesContext.NewCoordinates.NumberWithPrefix, resultContext);
        }
    }

    class ExistingPointInCoordinatesContext : DataObjectBase<ExistingPointInCoordinatesContext>
    {

        public enum CoordinatePreferTypes
        {
            New,
            Old
        }

        public enum HeightPreferTypes
        {
            New,
            Old
        }

        public ExistingPointInCoordinatesContext()
            : base(null, new StreamingContext())
        {
        }

        public ExistingPointInCoordinatesContext(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        readonly PropertyData _oldCoordinatesProperty = RegisterProperty("OldCoordinates", typeof(TableCoordinateListNode), null);
        public TableCoordinateListNode OldCoordinates
        {
            get { return GetValue<TableCoordinateListNode>(_oldCoordinatesProperty); }
            set { SetValue(_oldCoordinatesProperty, value); }
        }

        readonly PropertyData _newCoordinatesProperty = RegisterProperty("NewCoordinates", typeof(TableCoordinateListNode), null);
        public TableCoordinateListNode NewCoordinates
        {
            get { return GetValue<TableCoordinateListNode>(_newCoordinatesProperty); }
            set { SetValue(_newCoordinatesProperty, value); }
        }

        public string GroupLable
        {
            get
            {
                var par = new ResourceParams();
                par.Add("point", OldCoordinates.NumberWithPrefix);
                return LanguageDictionary.Current.TranslateText("ExistingPointInCoordinatesDialog.13", par);
            }
        }

        public double CoordinateDifferenceY
        {
            get { return OldCoordinates.Y - NewCoordinates.Y; }
        }

        public double CoordinateDifferenceX
        {
            get { return OldCoordinates.X - NewCoordinates.X; }
        }

        public double CoordinateDifferenceZ
        {
            get { return OldCoordinates.Z - NewCoordinates.Z; }
        }

        readonly PropertyData _newPrefixProperty = RegisterProperty("NewPrefix", typeof(string), string.Empty);
        public string NewPrefix
        {
            get { return GetValue<string>(_newPrefixProperty); }
            set { SetValue(_newPrefixProperty, value); }
        }

        readonly PropertyData _newNumberProperty = RegisterProperty("NewNumber", typeof(string), string.Empty);
        public string NewNumber
        {
            get { return GetValue<string>(_newNumberProperty); }
            set { SetValue(_newNumberProperty, value); }
        }

        readonly PropertyData _coordinateTypeProperty = RegisterProperty("CoordinatePrefer", typeof(CoordinatePreferTypes), CoordinatePreferTypes.New);
        public CoordinatePreferTypes CoordinatePrefer
        {
            get { return GetValue<CoordinatePreferTypes>(_coordinateTypeProperty); }
            set { SetValue(_coordinateTypeProperty, value); }
        }

        readonly PropertyData _heightPreferProperty = RegisterProperty("HeightPrefer", typeof(HeightPreferTypes), HeightPreferTypes.New);
        public HeightPreferTypes HeightPrefer
        {
            get { return GetValue<HeightPreferTypes>(_heightPreferProperty); }
            set { SetValue(_heightPreferProperty, value); }
        }

        public bool IsHeightEnabled
        {
            get { return !double.IsNaN(OldCoordinates.Z) || !double.IsNaN(NewCoordinates.Z); }
        }

    }
}