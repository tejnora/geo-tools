namespace DxfLibrary
{
    public class Document
    {
        public Document()
        {
            Entities = new Entities();
        }
        internal Header Header;
        internal Entities Entities;
        internal Blocks Blocks;
        internal Tables Tables;

        public void SetHeader(Header h)
        {
            Header = h;
        }

        public void SetEntities(Entities e)
        {
            Entities = e;
        }

        public void SetBlocks(Blocks b)
        {
            Blocks = b;
        }

        public void SetTables(Tables t)
        {
            Tables = t;
        }

        public void Add(Entity e)
        {
            Entities.AddEntity(e);
        }

        public void Add(Block b)
        {
            Blocks.AddBlock(b);
        }
    }
}
