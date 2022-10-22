using System.Collections.Generic;

namespace GeoBase.Utils
{
    public class ResourceParams
    {
        public ResourceParams()
        {
            Params=new Dictionary<string, string>();
        }
        public Dictionary<string, string> Params
        {
            get; private set;
        }
        public void Add(string key, string value)
        {
            Params[key] = value;
        }
    }
}
