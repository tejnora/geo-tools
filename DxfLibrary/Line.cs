namespace DxfLibrary
{
    public class Line : Entity
    {
        public Line(string layer, double xi, double yi, double xf, double yf)
            : base("LINE", layer)
        {
            DataAcceptanceList.AddRange(new int[] { 39, 10, 20, 30, 11, 21, 31, 210, 220, 230 });
            AddReplace(10, xi);
            AddReplace(20, yi);
            AddReplace(11, xf);
            AddReplace(21, yf);
        }
        public void SetInitialPoint(double x, double y)
        {
            AddReplace(10, x);
            AddReplace(20, x);
        }
        public void SetFinalPoint(double x, double y)
        {
            AddReplace(11, x);
            AddReplace(21, x);
        }
    }
}
