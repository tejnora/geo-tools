using System;
using CAD.Canvas.DrawTools;


namespace CAD.GUI
{
    public partial class ActivePointPropPage 
        :PpWindow
    {
        ActivePointEdit _owner;
        public ActivePointPropPage(ActivePointEdit aOwner)
            : base("PPActivePoint")
        {
            _owner = aOwner;
            InitializeComponent();
            loadValues();
        }

        private void loadValues()
        {
            _radius.Text = _owner.Radius.ToString();
        }

        private void onChanged(object aSender, EventArgs e)
        {
            if (_radius.Text.Length == 0)
                _owner.Radius = 0;
            else
                _owner.Radius = (float)System.Convert.ToDouble(_radius.Text);
        }
    }
}
