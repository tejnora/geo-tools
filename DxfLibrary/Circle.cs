namespace DxfLibrary
{
    public class Circle : Entity
    {
        public Circle(double x, double y, double radius, string layer)
            : base("CIRCLE", layer)
        {
            DataAcceptanceList.AddRange(new int[] { 39, 10, 20, 30, 40, 210, 220, 230 });
            AddReplace(10, x);
            AddReplace(20, y);
            AddReplace(40, radius);
        }
    }
}
