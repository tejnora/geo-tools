using GeoBase.Localization;

namespace GeoBase.Gui.Buttons
{
    public class CancelButton : ButtonBase
    {
        public CancelButton()
        {
            Content = LanguageDictionary.Current.Translate<string>("Buttons.Cancel", "Content");
            IsCancel = true;
        }
    }
}
