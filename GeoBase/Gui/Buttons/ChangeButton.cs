using GeoBase.Localization;

namespace GeoBase.Gui.Buttons
{
    public class ChangeButton : ButtonBase
    {
        public ChangeButton()
        {
            Content = LanguageDictionary.Current.Translate<string>("Buttons.2", "Content");
            IsDefault = true;
        }
    }
}
