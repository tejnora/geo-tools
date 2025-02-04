namespace DxfLibrary
{
    public class Table : Element
    {
                public Table(string type)
        {
            DataAcceptanceList.AddRange(new int[] { 0, 2, 5, 102, 360, 102, 330, 100, 70 });
            AddReplace(2, type);
            StartTag = new Data(0, "TABLE");
            EndTag = new Data(0, "ENDTAB");
        }
                        public void AddTableEntry(TableEntry te)
        {
            AddElement(te);
        }
            }
}
