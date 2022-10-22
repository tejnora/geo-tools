using GeoBase.Localization;

namespace GeoBase.Gui.Buttons
{
    public class ProtocolButton : ButtonBase
    {
        public ProtocolButton()
        {
            Content = LanguageDictionary.Current.Translate<string>("Buttons.3", "Content");
        }
    }
}
