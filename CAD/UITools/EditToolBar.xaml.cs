using System.Windows.Input;
using CAD.Canvas;

namespace CAD.UITools
{
    public partial class EditToolBar : GeoCadToolBar
    {
                public static GeoCadRoutedCommand Select = new GeoCadRoutedCommand("Select", typeof(EditToolBar), GeoCadRoutedCommand.CommandTypes.Select);
        public static GeoCadRoutedCommand Pan = new GeoCadRoutedCommand("Pan", typeof(EditToolBar), GeoCadRoutedCommand.CommandTypes.Pan);
        public static GeoCadRoutedCommand Move = new GeoCadRoutedCommand("Move", typeof(EditToolBar), GeoCadRoutedCommand.CommandTypes.Move);
        public static GeoCadRoutedCommand Undo = new GeoCadRoutedCommand("Undo", typeof(EditToolBar), false);
        public static GeoCadRoutedCommand Redo = new GeoCadRoutedCommand("Redo", typeof(EditToolBar), false);
        public static GeoCadRoutedCommand FitView = new GeoCadRoutedCommand("FitView", typeof(EditToolBar),false);
                        public EditToolBar()
        {
            InitializeComponent();
        }
                        public override void Notify(NotificationType type, object additionData)
        {
            base.Notify(type, additionData);
            switch (type)
            {
                case NotificationType.DocumentChanged:
                    {
                        bool enable = ToolBarManager.Document != null;
                        IsEnabled = enable;
                    } break;
            }
        }
                        private void OnCanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ToolBarManager!=null && ToolBarManager.Document != null && ToolBarManager.Document.DataModel.CanUndo();
        }
        private void OnExecuteUndo(object sender, ExecutedRoutedEventArgs e)
        {
            ToolBarManager.Document.DataModel.DoUndo();
            ToolBarManager.Document.CanvasCommand.InvalidateAll();
        }
        private void OnCanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ToolBarManager != null && ToolBarManager.Document != null && ToolBarManager.Document.DataModel.CanRedo();
        }
        private void OnExecuteRedo(object sender, ExecutedRoutedEventArgs e)
        {
            ToolBarManager.Document.DataModel.DoRedo();
            ToolBarManager.Document.CanvasCommand.InvalidateAll();
        }
        private void OnExecuteFitView(object sender, ExecutedRoutedEventArgs e)
        {
            ICanvasCommand cc = ToolBarManager.Document.CanvasCommand;
            cc.CommandFitView();
        }
            }
}
