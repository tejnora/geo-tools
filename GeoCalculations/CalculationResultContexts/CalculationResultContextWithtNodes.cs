using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoCalculations.CalculationResultContexts
{
    public class CalculationResultContextBase<TNode, TContext>
        : DataObjectBase<TContext> where TNode : CalculatedPointBase, new()
    {
        public CalculationResultContextBase()
            : base(null, new StreamingContext())
        {

        }

        public CalculationResultContextBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        readonly PropertyData _nodesProperty = RegisterProperty("Nodes", typeof(ObservableCollection<TNode>), new ObservableCollection<TNode>());
        [ProtocolPropertyData("Nodes"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public ObservableCollection<TNode> Nodes
        {
            get { return GetValue<ObservableCollection<TNode>>(_nodesProperty); }
            set { SetValue(_nodesProperty, value); }
        }

        readonly PropertyData _nodeProperty = RegisterProperty("Node", typeof(TNode), new TNode());
        public TNode Node
        {
            get { return GetValue<TNode>(_nodeProperty); }
            set { SetValue(_nodeProperty, value); }
        }
    }
}
