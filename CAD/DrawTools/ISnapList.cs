using System;
namespace CAD.DrawTools
{
    interface ISnapList
    {
        Type[] RunningSnaps { get; }
    }
}
