using CAD.Canvas;
using CAD.VFK.DrawTools;

namespace CAD.GUI
{
    public partial class VfkNumberPropPage
        : PpWindow
    {
                public VfkNumberPropPage(IDrawObject owner)
            :base("GUI\\VFKNumberPropPage")
        {
            InitializeComponent();
            SetOwner(owner);
            DataContext = this;
        }
                        private VfkTextsEdit _numbers;
        private string _textBoxContent;
        public string TextBoxContent
        {
            get { return _textBoxContent; }
            set 
            { 
                _textBoxContent = value;
                OnPropertyChanged("TextBoxContent");
                OnSaveValues();
            }
        }
        private bool _loading;
        private bool _setAngle;
        public bool SetAngle
        {
            get { return _setAngle; }
            set
            {
                _setAngle = value;
                OnPropertyChanged("SetAngle");
                OnSaveValues();
            }
        }

        private double _textSize;
        public double TextSize
        {
            get { return _textSize; }
            set
            {
                _textSize = value;
                OnPropertyChanged("TextSize");
                OnSaveValues();
            }
        }
                        public override sealed void SetOwner(IDrawObject aOwner)
        {
            _numbers = (VfkTextsEdit)aOwner;
            OnLoadValues();
        }
        private void OnSaveValues()
        {
            if (_loading) return;
            _numbers.Text = TextBoxContent;
            _numbers.SetAngle = SetAngle;
            _numbers.VyskaTextu = TextSize;
        }
        private void OnLoadValues()
        {
            _loading = true;
            TextBoxContent = _numbers.Text;
            SetAngle = _numbers.SetAngle;
            TextSize = _numbers.VyskaTextu;
            _loading = false;
        }
            }
}
