namespace DxfLibrary
{
    public class Point : Entity
    {
        public Point(string layer, double x, double y)
            : base("POINT", layer)
        {
            DataAcceptanceList.AddRange(new int[] { 39, 10, 20, 30, 210, 220, 230, 50 });
            AddReplace(10, x);
            AddReplace(20, y);
        }
    }
}
