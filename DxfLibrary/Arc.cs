namespace DxfLibrary
{
    public class Arc : Entity
    {
        public Arc(double x, double y, double radius, double startAngle, double endAngle, string layer)
            : base("ARC", layer)
        {
            DataAcceptanceList.AddRange(new int[] { 39, 10, 20, 30, 40, 100, 50, 51, 210, 220, 230 });
            AddReplace(10, x);
            AddReplace(20, y);
            AddReplace(40, radius);
            AddReplace(50, startAngle);
            AddReplace(51, endAngle);
        }
    }
}