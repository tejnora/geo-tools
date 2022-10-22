namespace DxfLibrary
{
    public struct Data
    {
        public Data(int code, object data)
            :this()
        {
            Code = code;
            _data = data;
        }
        public int Code;
        public object _data;
    }
}