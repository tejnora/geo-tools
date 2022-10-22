using CAD.Canvas.Layers;

namespace CAD.UITools
{
    public partial class DrawToolBar : GeoCadToolBar
    {
        #region RoutedCommands
        public static GeoCadRoutedCommand MultiLine = new GeoCadRoutedCommand("MultiLine", typeof(DrawToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public static GeoCadRoutedCommand Line = new GeoCadRoutedCommand("Line", typeof(DrawToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public static GeoCadRoutedCommand Circle2Point = new GeoCadRoutedCommand("Circle2Point", typeof(DrawToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public static GeoCadRoutedCommand CircleCentrePoint = new GeoCadRoutedCommand("CircleCentrePoint", typeof(DrawToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public static GeoCadRoutedCommand TextBox = new GeoCadRoutedCommand("TextBox", typeof(DrawToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public static GeoCadRoutedCommand ActivePoint = new GeoCadRoutedCommand("ActivePoint", typeof(DrawToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public static GeoCadRoutedCommand Arc3Points = new GeoCadRoutedCommand("Arc3Points", typeof(DrawToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public static GeoCadRoutedCommand Arc = new GeoCadRoutedCommand("Arc", typeof(DrawToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        #endregion
        #region Constructor
        public DrawToolBar()
        {
            InitializeComponent();
        }
        #endregion
        #region GeoCadToolBar
        public override void Notify(NotificationType type, object additionData)
        {
            base.Notify(type, additionData);
            switch (type)
            {
                case NotificationType.DocumentChanged:
                    {
                        bool enable = ToolBarManager.Document != null &&
                                      !(ToolBarManager.Document.DataModel.ActiveLayer is VFKDrawingLayerMain);
                        IsEnabled = enable;
                    } break;
            }
        }
        #endregion
    }
}
