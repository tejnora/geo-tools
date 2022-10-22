using Avalonia;
using Avalonia.Markup.Xaml;

namespace WSGPApp
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
