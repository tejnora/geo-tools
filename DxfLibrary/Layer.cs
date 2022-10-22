namespace DxfLibrary
{
    public class Layer : TableEntry
    {
        public Layer(string name, short color, string linetype)
            : base("LAYER")
        {
            DataAcceptanceList.AddRange(new[] { 2, 70, 62, 6, 290, 370, 390 });
            AddReplace(70, (short)0);
            AddReplace(2, name);
            AddReplace(62, color);
            AddReplace(6, linetype);
        }
    }
}
