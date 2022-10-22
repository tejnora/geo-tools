using GeoBase.Utils;

namespace GeoBase
{
    public class SingletonsBase
    {
        public static Registry Registry
        {
            get
            {
                return Singleton<Registry>.Instance;
            }
        }
    }
}
