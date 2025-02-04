namespace DxfLibrary
{
    public class EndBlk : Element
    {
                public EndBlk()
        {
            DataAcceptanceList.AddRange(new int[] { 0, 5, 8, 102, 330, 100 });
            StartTag = new Data(0, "ENDBLK");
        }
        public EndBlk(Block b)
            : this()
        {
            if (b.GetIndexFor(5) != -1) AddData(b.GetDataFor(5));
            if (b.GetIndexFor(8) != -1) AddData(b.GetDataFor(8));
        }
            }
}
