using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CAD.Utils;

namespace GeoHelper.Tables.AdditionalGui
{
    public partial class MassChangeGroupBox : INotifyPropertyChanged
    {
        public enum OperationTypes
        {
            Set,
            Multiply,
            Add
        }

        public MassChangeGroupBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected bool _isEnabledCheckBox;
        public bool IsEnabledCheckBox
        {
            get { return _isEnabledCheckBox; }
            set
            {
                _isEnabledCheckBox = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        OperationTypes _operationType = OperationTypes.Set;
        public OperationTypes OperationType
        {
            get { return _operationType; }
            set
            {
                _operationType = value;
                OnPropertyChanged("OperationType");
            }
        }

        public double DoubleValue
        {
            get
            {
                double value;
                return double.TryParse(_textBox.Text, out value) ? value : double.NaN;
            }
        }

        public uint UIntValue
        {
            get
            {
                uint value;
                return uint.TryParse(_textBox.Text, out value) ? value : 0;
            }
        }

        public string StringValue
        {
            get { return _textBox.Text; }
        }


        public static readonly DependencyProperty GroupBoxHeaderProperty =
            DependencyProperty.Register("GroupBoxHeader", typeof(string), typeof(MassChangeGroupBox), new FrameworkPropertyMetadata(string.Empty));

        public static readonly DependencyProperty _valueConvertProperty =
            DependencyProperty.Register("ValueConvert", typeof(IValueConverter), typeof(MassChangeGroupBox), new FrameworkPropertyMetadata(null, OnConvertPropertyChanged));

        public static readonly DependencyProperty GroupBoxVisibilityProperty =
            DependencyProperty.Register("GroupBoxVisibility", typeof(Visibility), typeof(MassChangeGroupBox), new FrameworkPropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty TextBoxMaskProperty =
            DependencyProperty.Register("TextBoxMask", typeof(MaskType), typeof(MassChangeGroupBox), new FrameworkPropertyMetadata(MaskType.Decimal, OnChangedTextBoxMaskProperty));

        public string GroupBoxHeader
        {
            get { return (string)GetValue(GroupBoxHeaderProperty); }
            set { SetValue(GroupBoxHeaderProperty, value); }
        }

        public IValueConverter ValueConvert
        {
            get { return (IValueConverter)GetValue(_valueConvertProperty); }
            set
            {
                SetValue(_valueConvertProperty, value);
                UdateTextBoxBinding();
            }
        }

        public Visibility GroupBoxVisibility
        {
            get { return (Visibility)GetValue(GroupBoxVisibilityProperty); }
            set { SetValue(GroupBoxVisibilityProperty, value); }
        }

        public MaskType TextBoxMask
        {
            get { return (MaskType)GetValue(TextBoxMaskProperty); }
            set { SetValue(TextBoxMaskProperty, value); }
        }

        static void OnConvertPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var hzgb = (MassChangeGroupBox)source;
            hzgb.UdateTextBoxBinding();
        }

        static void OnChangedTextBoxMaskProperty(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var groupBox = (MassChangeGroupBox)source;
            TextBoxMaskBehavior.SetMask(groupBox._textBox, groupBox.TextBoxMask);
        }

        void UdateTextBoxBinding()
        {
            var textProp = TextBox.TextProperty;
            if (BindingOperations.IsDataBound(_textBox, textProp)) return;
            var b = new Binding("Value") { Converter = ValueConvert };
            BindingOperations.SetBinding(_textBox, textProp, b);
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        protected override void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}