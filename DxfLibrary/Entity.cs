using System;

namespace DxfLibrary
{
    public abstract class Entity : Element
    {
        #region Constructor
        public Entity(string name, string layer)
        {
            DataAcceptanceList.AddRange(new int[] { 0, 5, 102, 330, 360, 100, 67, 410, 8, 6, 62, 370, 48, 60, 92, 310 });
            StartTag = new Data(0, name);
            AddData(new Data(8, layer));
        }
        #endregion
        #region Properties
        public string Layer
        {
            get
            {
                return (string)(GetDataFor(8))._data;
            }
            set
            {
                AddReplace(8, S);
            }
        }
        public Int16 Color
        {
            get
            {
                return (Int16)GetDataFor(62)._data;
            }
            set
            {
                AddReplace(62, value);
            }
        }
        #endregion
    }
}
