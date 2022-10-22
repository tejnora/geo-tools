namespace DxfLibrary
{
    public class Entities : Section
    {
        #region Constructor
        public Entities()
            : base("ENTITIES")
        {
            // 
            // TODO: Add constructor logic here
            //
        }
        #endregion
        #region Methods
        public void AddEntity(Entity e)
        {
            AddElement(e);
        }
        #endregion
    }
}
