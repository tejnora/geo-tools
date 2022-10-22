using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VFK
{
    public interface IVFKRegisterIface
    {
        void RegisterSegment(IVFKMain aOwner);
        void DeleteSegment(IVFKMain aOwner);
    }
}
