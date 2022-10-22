using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CAD.UITools
{
    public abstract class GeoCadToolBar : System.Windows.Controls.ToolBar, INotifyPropertyChanged
    {
        #region Constructor
        public GeoCadToolBar()
        {
            AllowDrop = true;
        }
        #endregion
        #region Property
        public GeoCadToolBarManager ToolBarManager
        { get; set; }
        #endregion
        #region Methods
        public virtual void Notify(NotificationType type, object additionData)
        {
            switch (type)
            {
                case NotificationType.ToolChanged:
                    {
                        GeoCadRoutedCommand currentCommnad = additionData as GeoCadRoutedCommand;
                        foreach (var item in Items)
                        {
                            if (item is ToggleButton)
                            {
                                ButtonBase buttonBase = item as ButtonBase;
                                var buttonCommand = buttonBase.Command as GeoCadRoutedCommand;
                                if(buttonCommand==null)
                                    continue;
                                ((ToggleButton)buttonBase).IsChecked = buttonBase.Command == currentCommnad;
                            }
                        }
                    } break;
                case NotificationType.MergeCommandBindings:
                    {
                        CommandBindingCollection commandBindingCollection = (CommandBindingCollection)additionData;
                        foreach (CommandBinding command in CommandBindings)
                        {
                            if(!commandBindingCollection.Contains(command))
                                commandBindingCollection.Add(command);
                        }
                    } break;
                case NotificationType.MergeInputBindings:
                    {
                        InputBindingCollection inputBindingCollection = (InputBindingCollection) additionData;
                        foreach(InputBinding input in InputBindings)
                        {
                            if(!inputBindingCollection.Contains(input))
                                inputBindingCollection.Add(input);
                        }
                    } break;
            }
        }
        #endregion
        #region ICommands
        protected void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsEnabled;
        }
        protected virtual void OnExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command != null)
            {
                GeoCadRoutedCommand command = e.Command as GeoCadRoutedCommand;
                if (ToolBarManager.Command==command)
                {
                    ToggleButton tb = e.OriginalSource as ToggleButton;
                    if (tb != null)
                        tb.IsChecked = true;
                }
                else
                    ToolBarManager.Command = e.Command as GeoCadRoutedCommand;
            }
        }
        #endregion
        #region INotifyPropertyChanged
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion INotifyPropertyChanged
    }
}
