using GeoBase.Localization;

namespace GeoBase.Gui.Buttons
{
    public class CloseButton : ButtonBase
    {
        public CloseButton()
        {
            Content = LanguageDictionary.Current.Translate<string>("Buttons.1", "Content");
            IsCancel = true;
        }
    }
}
