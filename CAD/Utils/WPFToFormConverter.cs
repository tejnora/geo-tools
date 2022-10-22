using System.Windows;

namespace CAD.Utils
{
    public class WPFToFormConverter
    {
        public static System.Windows.Media.Color getWPFColor(System.Drawing.Color aColor)
        {
            return System.Windows.Media.Color.FromRgb(aColor.R, aColor.G, aColor.B);
        }
        public static System.Drawing.Color getFormColor(System.Windows.Media.Color aColor)
        {
            return System.Drawing.Color.FromArgb(aColor.R, aColor.G, aColor.B);
        }
        public static bool comapareColor(System.Drawing.Color aC1, System.Windows.Media.Color aC2)
        {
            return aC1.A == aC2.A && aC1.G == aC2.G && aC1.B == aC2.B && aC1.R==aC2.R;
        }

        public static Rect unionRect(Rect aA1, Rect aA2)
        {
            if (aA1.IsEmpty)
                return aA2;
            if (aA2.IsEmpty)
                return aA1;
            return Rect.Union(aA2, aA1);
        }

    }
}
