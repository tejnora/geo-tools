using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace CAD.Canvas
{
    /// <summary>
    /// DTM catalogue symbols are designed for a white map; many are black (e.g. the definition point ZPS_db.png)
    /// and disappear on the dark canvas background. On a dark background the dark pixels are drawn with inverted
    /// lightness, light and coloured pixels are kept.
    /// </summary>
    public static class SymbolContrast
    {
        static readonly ConditionalWeakTable<Image, Image> DarkBackgroundVariants = new();

        public static bool IsDark(Color color) => color.GetBrightness() < 0.5f;

        public static Image ForBackground(Image symbol, Color background) =>
            symbol == null || !IsDark(background) ? symbol : DarkBackgroundVariants.GetValue(symbol, CreateDarkBackgroundVariant);

        internal static Image CreateDarkBackgroundVariant(Image symbol)
        {
            var bitmap = new Bitmap(symbol.Width, symbol.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
                graphics.DrawImage(symbol, 0, 0, symbol.Width, symbol.Height);
            for (var y = 0; y < bitmap.Height; y++)
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    if (pixel.A == 0 || pixel.GetBrightness() >= 0.35f || pixel.GetSaturation() > 0.25f) continue;
                    bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, 255 - pixel.R, 255 - pixel.G, 255 - pixel.B));
                }
            return bitmap;
        }
    }
}
