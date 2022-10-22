namespace GeoHelper.Options
{
    public interface IOptionItem
    {
        IOptionContextItem Context { get; set; }
    }

    public interface IOptionContextItem
    {
        void LoadFromRegistry();
        void SaveToRegistry();
    }
}