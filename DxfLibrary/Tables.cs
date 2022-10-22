namespace DxfLibrary
{
    public class Tables : Section
    {
        #region Constructor
        public Tables()
            : base("TABLES")
        {
        }
        #endregion
        #region Methods
        public void AddTable(Table t)
        {
            AddElement(t);
        }
        #endregion
    }
}
