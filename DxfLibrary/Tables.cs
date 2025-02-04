namespace DxfLibrary
{
    public class Tables : Section
    {
                public Tables()
            : base("TABLES")
        {
        }
                        public void AddTable(Table t)
        {
            AddElement(t);
        }
            }
}
