namespace DxfLibrary
{
    public class Vertex : Entity
    {
                public Vertex(double x, double y, string layer)
            : base("VERTEX", layer)
        {
            DataAcceptanceList.AddRange(new int[] { 10, 20, 30, 70, 40, 41, 42, 50, 71, 72, 73, 74, });
            AddReplace(10, x);
            AddReplace(20, y);
        }
            }
}
