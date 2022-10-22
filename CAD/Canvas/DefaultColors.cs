using System;

namespace CAD.Canvas
{
    static class DefaultColors
    {
        public struct Color
        {
            public Byte R;
            public Byte G;
            public Byte B;
        }

        private static Color[] _items;
        public static Color[] Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new Color[256];
                    _items[0] = new Color { R = 255, G = 255, B = 255 };//firts line
                    _items[1] = new Color { R = 0, G = 0, B = 255 };
                    _items[2] = new Color { R = 0, G = 255, B = 0 };
                    _items[3] = new Color { R = 255, G = 0, B = 0 };
                    _items[4] = new Color { R = 255, G = 255, B = 0 };
                    _items[5] = new Color { R = 255, G = 0, B = 255 };
                    _items[6] = new Color { R = 255, G = 127, B = 0 };
                    _items[7] = new Color { R = 0, G = 255, B = 255 };
                    _items[8] = new Color { R = 64, G = 64, B = 64 };
                    _items[9] = new Color { R = 192, G = 192, B = 192 };
                    _items[10] = new Color { R = 254, G = 0, B = 96 };
                    _items[11] = new Color { R = 160, G = 224, B = 102 };
                    _items[12] = new Color { R = 0, G = 254, B = 160 };
                    _items[13] = new Color { R = 128, G = 0, B = 160 };
                    _items[14] = new Color { R = 176, G = 176, B = 176 };
                    _items[15] = new Color { R = 0, G = 240, B = 240 };
                    GenerateColor(16, 240, 240, 240, 15, 15, 15);//0
                    GenerateColor(17, 0, 0, 240, 0, 0, 15);//1
                    GenerateColor(18, 0, 240, 0, 0, 15, 0);//2
                    GenerateColor(19, 240, 0, 0, 15, 0, 0);//3
                    GenerateColor(20, 240, 240, 0, 15, 15, 0);//4
                    GenerateColor(21, 240, 0, 240, 15, 0, 15);//5
                    GenerateColor(22, 240, 122, 0, 15, 5, 0);//6
                    GenerateColor(23, 0, 240, 240, 0, 15, 15);//7
                    GenerateColor(24, 240, 240, 240, 15, 15, 15);//8
                    GenerateColor(25, 0, 0, 240, 0, 0, 15);//9
                    GenerateColor(26, 0, 240, 0, 0, 15, 0);//10
                    GenerateColor(27, 240, 0, 0, 15, 0, 0);//11
                    GenerateColor(28, 240, 240, 0, 15, 15, 0);//12
                    GenerateColor(29, 240, 0, 240, 15, 0, 15);//13
                    GenerateColor(30, 240, 122, 0, 15, 5, 0);//14
                    GenerateColor(31, 0, 225, 225, 0, 5, 5);//15
                }
                return _items;
            }
        }
        static private void GenerateColor(int columnIndex, byte r, byte g, byte b, byte dr, byte dg, byte db)
        {
            while (columnIndex < 256)
            {
                _items[columnIndex] = new Color { R = r, G = g, B = b };
                columnIndex += 16;
                if (r >= dr) r -= dr;
                if (g >= dg) g -= dg;
                if (b >= dg) b -= db;
            }
        }

        static public System.Drawing.Color GetColor(int index)
        {
            if(index>Items.Length)
                return System.Drawing.Color.White;
            var c = Items[index];
            return System.Drawing.Color.FromArgb(c.R, c.G, c.B);
        }
    }
}
