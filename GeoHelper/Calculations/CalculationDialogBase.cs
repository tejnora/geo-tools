using System;
using System.Windows;
using GeoBase.Gui;
using GeoBase.Gui.Buttons;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;
using GeoHelper.Protocols;
using GeoHelper.Protocols.Gui;
using GeoHelper.Tables;

namespace GeoHelper.Calculations
{
    public class CalculationDialogBase : DialogBase
    {
        protected IMainWindow MainWindow;

        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("NewButtomContent", typeof(object), typeof(CalculationDialogBase), new UIPropertyMetadata(null));

        public object NewButtomContent
        {
            get { return GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        public CalculationDialogBase()
        {
        }

        public CalculationDialogBase(string registerName, IMainWindow mainWindow)
            : base(registerName, true)
        {
            WindowStyle = WindowStyle.ToolWindow;
            ResizeMode = ResizeMode.NoResize;
            MainWindow = mainWindow;
            Closed += OnClosed;
        }

        public override void EndInit()
        {
            base.EndInit();
            var protocolButton = new ProtocolButton();
            protocolButton.Click += ShowProtocol;
            _buttonsStackPanel.Children.Add(protocolButton);
            if (NewButtomContent != null)
            {
                var newButtom = new ButtonBase { Content = NewButtomContent };
                newButtom.Click += ResetCalculation;
                _buttonsStackPanel.Children.Add(newButtom);
            }
            var calculateButton = new CalculateButton();
            calculateButton.Click += Calculate;
            _buttonsStackPanel.Children.Add(calculateButton);
        }

        protected virtual void Calculate()
        {
            throw new NotImplementedException();
        }

        protected virtual void CalculatePoint()
        {
            throw new NotImplementedException();
        }

        protected virtual void ResetCalculation(object sender, RoutedEventArgs e)
        {
            CalculationContextBase.SetDefaultValues();
            CalculatedPoint.SetDefaultValues();
        }

        protected virtual void AddCalculatedPointIntoTable(CalculatedPointBase point)
        {
            MainWindow.AddNewNodeIntoTable(typeof(CoordinateListTable), point, ProtocolContext);
        }

        protected IProtocolContext ProtocolContext { get; set; }
        protected virtual CalculationContextBase CalculationContextBase
        {
            get { throw new NotImplementedException(); }
        }

        private ProtocolDialog _protocolDialog;

        public CalculatedPointBase CalculatedPoint { get; set; }

        protected void Calculate(object sender, RoutedEventArgs e)
        {
            if (!TryToCalculate(false)) return;
            TryCalculatePoints(false);
        }

        public bool TryToCalculate(bool silent)
        {
            try
            {
                CalculationContextBase.ResetBeforeCalculation();
                Calculate();
                return true;
            }
            catch (SilentAbortCalculation)
            {
                return false;
            }
            catch (CalculationException ex)
            {
                if (!silent)
                    MessageBox.Show(ex.Description);
            }
            catch (Exception ex)
            {
                if (!silent)
                    MessageBox.Show(ex.Message);
            }
            ProtocolContext.Clear();
            return false;
        }

        public bool TryCalculatePoints(bool silent)
        {
            try
            {
                CalculatePoint();
                ProtocolContext.AddCalculatedPoint(CalculatedPoint);
                AddCalculatedPointIntoTable(CalculatedPoint);
                updateProtocolData();
                return true;
            }
            catch (SilentAbortCalculation)
            {
                return false;
            }
            catch (CalculationException ex)
            {
                if (!silent)
                    MessageBox.Show(ex.Description);
            }
            catch (Exception ex)
            {
                if (!silent)
                    MessageBox.Show(ex.Message);
            }
            return false;
        }

        public string GetProtocolSimpleText()
        {
            var protocolDialog = new ProtocolDialog(ProtocolContext, MainWindow);
            protocolDialog.updateContent();
            return protocolDialog.Text;
        }

        void ShowProtocol(object sender, RoutedEventArgs e)
        {
            if (!TryToCalculate(false)) return;
            if (_protocolDialog != null) return;
            _protocolDialog = new ProtocolDialog(ProtocolContext, MainWindow);
            if (!_protocolDialog.updateContent())
            {
                _protocolDialog = null;
                return;
            }
            _protocolDialog.Closed += onProtocolDialogColsed;
            _protocolDialog.Show();
        }

        void updateProtocolData()
        {
            if (_protocolDialog == null) return;
            _protocolDialog.updateContent();
        }

        void onProtocolDialogColsed(object sender, EventArgs e)
        {
            _protocolDialog = null;
        }

        protected new void OnClosed(object sender, EventArgs args)
        {
            base.OnClosed(sender, args);
            MainWindow.ReleaseCalculationMethod(_registerName);
        }
    }
}