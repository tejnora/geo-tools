using System.ComponentModel;
using System.Windows;

namespace GeoHelper.Tables.AdditionalGui
{
    public partial class MassChangeLockGroupBox : INotifyPropertyChanged
    {
        public enum LockTypes
        {
            NotChange,
            Unlock,
            Lock
        }

        public MassChangeLockGroupBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static readonly DependencyProperty GroupBoxHeaderProperty = DependencyProperty.Register("GroupBoxHeader", typeof(string), typeof(MassChangeLockGroupBox), new FrameworkPropertyMetadata(string.Empty));

        LockTypes _lockType = LockTypes.NotChange;

        public string GroupBoxHeader
        {
            get { return (string)GetValue(GroupBoxHeaderProperty); }
            set { SetValue(GroupBoxHeaderProperty, value); }
        }

        public LockTypes LockType
        {
            get { return _lockType; }
            set
            {
                _lockType = value;
                OnPropertyChanged("LockType");
            }
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        protected override void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}