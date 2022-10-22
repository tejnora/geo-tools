using System;
using System.Linq;
using GeoHelper.Tables.TableNodes;

namespace GeoHelper.Tables.AdditionalGui
{
    public partial class MassChangeMeasureDialog
    {
        public MassChangeMeasureDialog(TableBase table)
            : base("MassChangeMeasureDialog", true)
        {
            InitializeComponent();
            DisableDialogResize();
            TableBase = table;
            DataContext = this;
        }

        TableBase TableBase { get; set; }

        public string FileName
        {
            get { return TableBase.FileName.FullName; }
        }

        public void OnChange(object sender, EventArgs arts)
        {
            using (new Action(TableBase))
            {
                if (_zmenit.OperationType == MassChangeChangeGroupBox.OperationTypes.Selected)
                {
                    var nodes = (from n in TableBase.Nodes where n.Selected select n).ToList();
                    foreach (var node in nodes)
                        ChangeNode(node);
                }
                else
                {
                    foreach (var node in TableBase.Nodes)
                        ChangeNode(node);
                }
            }
            TableBase.MarkModification();
        }

        void ChangeNode(TableNodesBase node)
        {
            using (var action = new Action(TableBase))
            {
                if (node.Locked) return;
                action.AddEditedNode(node);
                var measuringNode = (TableMeasureListNode)node;
                if (_prefix.IsEnabledCheckBox)
                    measuringNode.Prefix = _prefix.StringValue;
                measuringNode.Hz = MassChangeCoordinateDialog.Change(_Hz, measuringNode.Hz);
                measuringNode.ZenitAngle = MassChangeCoordinateDialog.Change(_zenitAgnle, measuringNode.ZenitAngle);
                if (_description.IsEnabledCheckBox)
                    measuringNode.Description = _description._textBox.Text;
                measuringNode.HorizontalDistance = MassChangeCoordinateDialog.Change(_horizontalDistance, measuringNode.HorizontalDistance);
                measuringNode.ElevationDefference = MassChangeCoordinateDialog.Change(_elevationDefference, measuringNode.ElevationDefference);
                measuringNode.Signal = MassChangeCoordinateDialog.Change(_heightOfTarget, measuringNode.Signal);
            }
        }
    }
}