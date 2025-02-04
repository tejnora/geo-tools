namespace DxfLibrary
{
    public class Variable : Element
    {
                public Variable(string nume, int dataType, object val)
        {
            StartTag = new Data(0, 0);
            EndTag = new Data(0, 0);

            Data.Add(new Data(9, nume));
            Data.Add(new Data(dataType, val));
        }
                        public string VarName
        {
            get
            {
                return (string)((Data)Data[0])._data;
            }
        }
        public object Value
        {
            get
            {
                return ((Data)Data[1])._data;
            }
        }
            }
}