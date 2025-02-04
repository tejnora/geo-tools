namespace DxfLibrary
{
    public class Entities : Section
    {
                public Entities()
            : base("ENTITIES")
        {
            // 
            // TODO: Add constructor logic here
            //
        }
                        public void AddEntity(Entity e)
        {
            AddElement(e);
        }
            }
}
