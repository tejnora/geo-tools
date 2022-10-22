using GeoBase.Utils;

namespace GeoHelper
{
    public class Singletons
    {
        public static Registry MyRegistry
        {
            get { return Singleton<Registry>.Instance; }
        }
    }
}