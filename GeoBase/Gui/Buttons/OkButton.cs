using GeoBase.Localization;

namespace GeoBase.Gui.Buttons
{
    public class OkButton : ButtonBase
    {
        public OkButton()
        {
            Content = LanguageDictionary.Current.Translate<string>("Buttons.0", "Content");
        }
    }
}
