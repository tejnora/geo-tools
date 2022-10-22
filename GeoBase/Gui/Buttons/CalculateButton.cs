using GeoBase.Localization;

namespace GeoBase.Gui.Buttons
{
    public class CalculateButton : ButtonBase
    {
        public CalculateButton()
        {
            Content = LanguageDictionary.Current.Translate<string>("Buttons.4", "Content");
        }

    }
}
