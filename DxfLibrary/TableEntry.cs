namespace DxfLibrary
{
    public class TableEntry : Element
    {
        #region Constructor
        internal TableEntry(string type)
        {
            DataAcceptanceList.AddRange(new int[] { 0, 5, 105, 102, 330, 360, 100 });
            StartTag = new Data(0, type);
        }
        #endregion
    }
}
