using System;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CAD.UITools
{
    public abstract class GeoCadToolBar : System.Windows.Controls.ToolBar, INotifyPropertyChanged
    {
        protected GeoCadToolBar()
        {
            AllowDrop = true;
        }
        public GeoCadToolBarManager ToolBarManager { get; set; }
        public virtual void Notify(NotificationType type, object additionData)
        {
            switch (type)
            {
                case NotificationType.ToolChanged:
                    {
                        var currentCommnad = additionData as GeoCadRoutedCommand;
                        foreach (var item in Items)
                        {
                            if (item is ToggleButton)
                            {
                                ButtonBase buttonBase = item as ButtonBase;
                                var buttonCommand = buttonBase.Command as GeoCadRoutedCommand;
                                if (buttonCommand == null)
                                    continue;
                                ((ToggleButton)buttonBase).IsChecked = buttonBase.Command == currentCommnad;
                            }
                        }
                    }
                    break;
                case NotificationType.MergeCommandBindings:
                    {
                        var commandBindingCollection = (CommandBindingCollection)additionData;
                        foreach (CommandBinding command in CommandBindings)
                        {
                            if (!commandBindingCollection.Contains(command))
                                commandBindingCollection.Add(command);
                        }
                    }
                    break;
                case NotificationType.MergeInputBindings:
                    {
                        var inputBindingCollection = (InputBindingCollection)additionData;
                        foreach (InputBinding input in InputBindings)
                        {
                            if (!inputBindingCollection.Contains(input))
                                inputBindingCollection.Add(input);
                        }
                    }
                    break;
                case NotificationType.DocumentChanged:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        protected void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsEnabled;
        }

        protected virtual void OnExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == null) return;
            var command = e.Command as GeoCadRoutedCommand;
            if (ToolBarManager.Command == command)
            {
                if (e.OriginalSource is ToggleButton tb)
                    tb.IsChecked = true;
            }
            else
                ToolBarManager.Command = e.Command as GeoCadRoutedCommand;
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}