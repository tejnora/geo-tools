namespace DxfLibrary
{
    public class Blocks : Section
    {
        public Blocks()
            : base("BLOCKS")
        {
            // 
            // TODO: Add constructor logic here
            //
        }

        public Blocks(string s)
            : base(s)
        {
        }

        public void AddBlock(Block b)
        {
            Elements.Add(b);
        }
    }
}
