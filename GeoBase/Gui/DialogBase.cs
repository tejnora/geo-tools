using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GeoBase.Gui.Buttons;
using GeoBase.Localization;
using GeoBase.Utils;

namespace GeoBase.Gui
{
    [Flags]
    public enum DialogButtons
    {
        Custom = 0,
        Ok = 1,
        Cancel = 2
    }

    public class DialogBase : WindowBase, INotifyPropertyChanged
    {
        bool _newVersion = false;
        Grid _grid;
        protected StackPanel _buttonsStackPanel;
        UIElement _dialogContent = new UIElement();
        protected string _registerName;
        protected bool SaveOnlyPosition;
        public event PropertyChangedEventHandler PropertyChanged;

        public DialogBase()
        {
            Init();
        }

        public DialogBase(string registerName)
        {
            Init();
            _registerName = registerName;
        }

        public DialogBase(string registerName, bool saveOnlyPosition, bool newVersion = false)
        {
            _newVersion = newVersion;
            Init();
            _registerName = registerName;
            SaveOnlyPosition = saveOnlyPosition;
        }

        private void Init()
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName();
            _newVersion = _newVersion || assemblyName.Name == "GeoHelper";//hack
            if (_newVersion)
            {
                Background = Brushes.LightGray;
                Owner = Application.Current.MainWindow;
                _grid = new Grid();
                _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                Content = _grid;
                _buttonsStackPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
                Grid.SetRow(_buttonsStackPanel, 1);
                _grid.Children.Add(_buttonsStackPanel);
            }
            else
            {
                Background = Brushes.LightGray;
                Owner = Application.Current.MainWindow;
                Closed += OnClosed;
                KeyUp += OnKeyUp;
                Initialized += OnLoaded;
            }
            ShowInTaskbar = false;
        }

        public override void EndInit()
        {
            base.EndInit();
            if (!_newVersion) return;

            if ((Buttons & DialogButtons.Cancel) == DialogButtons.Cancel)
                _buttonsStackPanel.Children.Insert(0, new CloseButton());
            else
                KeyUp += OnKeyUp;
            if ((Buttons & DialogButtons.Ok) == DialogButtons.Ok)
                _buttonsStackPanel.Children.Insert(0, new OkButton());
            foreach (UIElement ui in _buttonsStackPanel.Children)
            {
                if (ui is CloseButton)
                {
                    (ui as CloseButton).Click += OnCancelButtonClick;
                    if (DefaultButton == DialogButtons.Cancel)
                        (ui as CloseButton).IsDefault = true;
                }
                if (ui is OkButton)
                {
                    (ui as OkButton).Click += OnOkButtonClick;
                    if (DefaultButton == DialogButtons.Ok)
                        (ui as OkButton).IsDefault = true;
                }
            }
            LoadPosAndSize();
        }

        public UIElement DialogContent
        {
            get { return _dialogContent; }
            set
            {
                if (_dialogContent == value)
                    return;
                if (_dialogContent != null)
                    _grid.Children.Remove(_dialogContent);
                _dialogContent = value;
                Grid.SetRow(_dialogContent, 0);
                _grid.Children.Add(_dialogContent);
            }
        }

        public UIElementCollection CustomButtons
        {
            get { return _buttonsStackPanel.Children; }
            set
            {
                _buttonsStackPanel.Children.Clear();
                foreach (UIElement ui in value)
                    _buttonsStackPanel.Children.Add(ui);
            }
        }

        public DialogButtons DefaultButton { get; set; }

        public DialogButtons Buttons { get; set; }

        protected virtual void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            if (!IsVisible) return;
            DialogResult = true;
            SavePosAndSize();
            Close();
        }
        protected virtual void OnKeyUp(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
                OnCancelButtonClick(this, null);
        }
        protected void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            if (!IsVisible) return;
            if (DialogResult.HasValue)
                DialogResult = false;
            Close();
        }
        protected virtual void OnLoaded(object sender, EventArgs args)
        {
            LoadPosAndSize();
        }
        public void OnClosed(object sender, EventArgs args)
        {
            SavePosAndSize();
        }

        protected void LoadPosAndSize()
        {
            var po = SingletonsBase.Registry.getEntry(Registry.SubKey.kCurrentUser, _registerName + "/WindowPos");
            if (po.isPoint())
            {
                Left = po.getPoint().X;
                Top = po.getPoint().Y;
            }
            if (SaveOnlyPosition) return;
            po = SingletonsBase.Registry.getEntry(Registry.SubKey.kCurrentUser, _registerName + "/WindowState");
            if (po.isBool())
            {
                if (po.getBool())
                {
                    WindowState = WindowState.Maximized;
                }
            }
            po = SingletonsBase.Registry.getEntry(Registry.SubKey.kCurrentUser, _registerName + "WindowSize");
            if (!po.isPoint()) return;
            Width = po.getPoint().X;
            Height = po.getPoint().Y;
        }

        protected void SavePosAndSize()
        {
            SingletonsBase.Registry.setEntry(Registry.SubKey.kCurrentUser, _registerName + "/WindowPos", new ProgramOption(new Point(Left, Top)));
            if (!SaveOnlyPosition)
            {
                if (WindowState != WindowState.Maximized)
                {
                    SingletonsBase.Registry.setEntry(Registry.SubKey.kCurrentUser, _registerName + "WindowSize", new ProgramOption(new Point(ActualWidth, ActualHeight)));
                }
                SingletonsBase.Registry.setEntry(Registry.SubKey.kCurrentUser, _registerName + "WindowState", new ProgramOption(WindowState == WindowState.Maximized));
            }
        }
        public void DisableDialogResize()
        {
            return;
            MaxHeight = Height;
            MinHeight = Height;
            MinWidth = Width;
            MaxWidth = Width;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected ButtonBase AddAnyButton(string buttomId)
        {
            var eraseButton = new ButtonBase();
            eraseButton.Content = LanguageDictionary.Current.Translate<string>("Buttons." + buttomId, "Content");
            _buttonsStackPanel.Children.Add(eraseButton);
            return eraseButton;
        }
    }
}