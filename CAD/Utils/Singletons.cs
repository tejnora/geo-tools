using GeoBase;
using GeoBase.Utils;
using VFK;
using VFK.GUI;

namespace CAD.Utils
{
    public class Singletons:SingletonsBase
    {
        public static VfkElements VFKElements
        {
            get
            {
                return Singleton<VfkElements>.Instance;
            }
        }
        public static ScaleList ScaleList
        {
            get
            {
                return Singleton<ScaleList>.Instance;
            }
        }
        public static LandTypes LandTypes
        {
            get
            {
                return Singleton<LandTypes>.Instance;
            }
        }
        public static DeterminateAreaTypes DeterminateeAreaTypes
        {
            get
            {
                return Singleton<DeterminateAreaTypes>.Instance;
            }
        }
        public static CadasterNamesTreeNodeReader CadasterNamesTreeNodeReader
        {
            get
            {
                return Singleton<CadasterNamesTreeNodeReader>.Instance;
            }
        }
    }
}
