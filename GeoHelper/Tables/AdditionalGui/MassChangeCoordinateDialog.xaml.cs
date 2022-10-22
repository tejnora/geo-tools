using System;
using System.Linq;
using GeoHelper.Tables.TableNodes;

namespace GeoHelper.Tables.AdditionalGui
{
    public partial class MassChangeCoordinateDialog
    {
        public MassChangeCoordinateDialog(TableBase table)
            : base("MassChangeCoordinateDialog", true)
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
                if (_change.OperationType == MassChangeChangeGroupBox.OperationTypes.Selected)
                {
                    var nodes = (from n in TableBase.Nodes where n.Selected select n).ToList();
                    foreach (var node in nodes)
                    {
                        ChangeNode(node);
                    }
                }
                else
                {
                    foreach (var node in TableBase.Nodes)
                    {
                        ChangeNode(node);
                    }
                }
                TableBase.MarkModification();
            }
        }

        void ChangeNode(TableNodesBase node)
        {
            using (var action = new Action(TableBase))
            {
                if (node.Locked) return;
                action.AddEditedNode(node);
                var coordinateNode = (TableCoordinateListNode)node;
                coordinateNode.X = Change(_coordinateX, coordinateNode.X);
                coordinateNode.Y = Change(_coordinateY, coordinateNode.Y);
                coordinateNode.Z = Change(_coordinateZ, coordinateNode.Z);
                coordinateNode.Description = _popis._textBox.Text;
                coordinateNode.Quality = Change(_quality, coordinateNode.Quality);
                ChangeLock(_lock, node);
                if (_popis.IsEnabledCheckBox)
                    coordinateNode.Description = _popis._textBox.Text;
            }
        }

        public static double Change(MassChangeGroupBox gb, double value)
        {
            if (!gb.IsEnabledCheckBox) return value;
            switch (gb.OperationType)
            {
                case MassChangeGroupBox.OperationTypes.Set:
                    return gb.DoubleValue;
                case MassChangeGroupBox.OperationTypes.Multiply:
                    return value * gb.DoubleValue;
                case MassChangeGroupBox.OperationTypes.Add:
                    return value + gb.DoubleValue;
                default:
                    return value;
            }
        }

        public static uint Change(MassChangeGroupBox gb, uint value)
        {
            if (!gb.IsEnabledCheckBox) return value;
            switch (gb.OperationType)
            {
                case MassChangeGroupBox.OperationTypes.Set:
                    return gb.UIntValue;
                case MassChangeGroupBox.OperationTypes.Multiply:
                    return value * gb.UIntValue;
                case MassChangeGroupBox.OperationTypes.Add:
                    return value + gb.UIntValue;
                default:
                    return value;
            }
        }


        public static void ChangeLock(MassChangeLockGroupBox @lock, TableNodesBase node)
        {
            switch (@lock.LockType)
            {
                case MassChangeLockGroupBox.LockTypes.NotChange:
                    break;
                case MassChangeLockGroupBox.LockTypes.Unlock:
                    node.Locked = false;
                    break;
                case MassChangeLockGroupBox.LockTypes.Lock:
                    node.Locked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}