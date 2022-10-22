namespace VFK
{
    public interface IVfkRegister
    {
        bool RegisterSegment(IVFKMain aOwner);
        void DeleteSegment(IVFKMain aOwner);
        void InitFromElement(VfkElement aElement);
    }
}
