namespace DxfLibrary
{
    public class Table : Element
    {
        #region Constructor
        public Table(string type)
        {
            DataAcceptanceList.AddRange(new int[] { 0, 2, 5, 102, 360, 102, 330, 100, 70 });
            AddReplace(2, type);
            StartTag = new Data(0, "TABLE");
            EndTag = new Data(0, "ENDTAB");
        }
        #endregion
        #region Methods
        public void AddTableEntry(TableEntry te)
        {
            AddElement(te);
        }
        #endregion
    }
}
