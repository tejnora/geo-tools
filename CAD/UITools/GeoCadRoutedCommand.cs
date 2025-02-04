using System;
using System.Windows.Input;

namespace CAD.UITools
{
    public class GeoCadRoutedCommand : RoutedCommand
    {
                public enum CommandTypes
        {
            None,
            DrawTool,
            EditTool,
            Select,
            Pan,
            Move,
            InfoTool
        }
                        public GeoCadRoutedCommand(string name, Type ownerType, CommandTypes commandType)
            :base(name, ownerType)
        {
            HasState = true;
            CommandType = commandType;
        }
        public GeoCadRoutedCommand(string name, Type ownerType, bool hasState)
            : base(name, ownerType)
        {
            HasState = hasState;
            CommandType = CommandTypes.None;
        }
                        public bool HasState
        {
            get; private set;
        }
        public CommandTypes CommandType
        {
            get; private set;
        }
            }
}
