namespace DxfLibrary
{
    public class Insert : Entity
    {
        #region Constructor
        public Insert(string block, double x, double y, string layer)
            : base("INSERT", layer)
        {
            DataAcceptanceList.AddRange(new int[] { 66, 2, 10, 20, 30, 41, 42, 43, 50, 70, 71, 44, 45, 210, 220, 230 });
            AddData(new Data(2, block));
            AddData(new Data(10, x));
            AddData(new Data(20, y));
        }
        #endregion
    }
}
