using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoCalculations.Protocol;

namespace GeoCalculations.CalculationContexts
{
    public class CalculationContextWithSimpleTable<TTableNode> : CalculationContextBase
        where TTableNode : PointBaseEx, new()
    {
        public CalculationContextWithSimpleTable()
        {

        }

        public CalculationContextWithSimpleTable(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        readonly PropertyData _tableNodesProperty = RegisterProperty("TableNodes", typeof(ObservableCollection<TTableNode>), new ObservableCollection<TTableNode>());
        [ProtocolPropertyData("Items")]
        public ObservableCollection<TTableNode> TableNodes
        {
            get { return GetValue<ObservableCollection<TTableNode>>(_tableNodesProperty); }
            set { SetValue(_tableNodesProperty, value); }
        }

        readonly PropertyData _tableNodeProperty = RegisterProperty("Node", typeof(TTableNode), new TTableNode());
        public TTableNode Node
        {
            get { return GetValue<TTableNode>(_tableNodeProperty); }
            set
            {
                SetValue(_tableNodeProperty, value);

            }
        }

        public bool CanAddNodeIntoTable
        {
            get { return Node != null; }
        }

        public bool CanRemoveItemFromTable
        {
            get { return TableNodes.Count > 0; }
        }

        public virtual bool AddNode()
        {
            Debug.Assert(Node != null);
            Node.Validate();
            if (Node.HasErrors) return false;
            TableNodes.Add((TTableNode)Node.Clone());
            UpdateTableAdditionalProperties();
            return true;
        }

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();
            TableNodes.Clear();
        }

        public void RemoveNode(TTableNode node)
        {
            TableNodes.Remove(node);
            UpdateTableAdditionalProperties();
        }

        public void SelectionChanged(object node)
        {
            var tableNode = node as TTableNode;
            if (tableNode != null)
                Node = (TTableNode)tableNode.Clone();
            UpdateTableAdditionalProperties();
        }

        void UpdateTableAdditionalProperties()
        {
            OnPropertyChanged("CanAddNodeIntoTable");
            OnPropertyChanged("CanRemoveItemFromTable");
        }

        [ProtocolPropertyData("ResiduesMiddleDeviationX"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double ResiduesMiddleDeviationX
        {
            get
            {
                var sum = TableNodes.Sum(residuesPoint => Math.Pow(residuesPoint.dX, 2));
                return Math.Sqrt(sum / (TableNodes.Count - 1));
            }
        }

        [ProtocolPropertyData("ResiduesMiddleDeviationY"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double ResiduesMiddleDeviationY
        {
            get
            {
                var sum = TableNodes.Sum(residuesPoint => Math.Pow(residuesPoint.dY, 2));
                return Math.Sqrt(sum / (TableNodes.Count - 1));
            }
        }

        [ProtocolPropertyData("ResiduesMiddleDeviation"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double MiddleDeviation
        {
            get { return SimpleCalculation.CalculateM0Red(ResiduesMiddleDeviationX, ResiduesMiddleDeviationY); }
        }

    }
}
