namespace DxfLibrary
{
    public class Style : TableEntry
    {
                public Style(string name, bool shape, double height, double width, double obliqueAngle, bool backward, bool upsidedown, double lastHeightUsed, string primaryFontFile)
            : base("STYLE")
        {
            DataAcceptanceList.AddRange(new int[] { 2, 70, 40, 41, 50, 71, 42, 3, 4, 1000, 1001});
            AddData(2, name);
            short c70 = 0;
            if (shape) c70 += 1;
            AddData(70, c70);
            AddData(40, height);
            AddData(41, width);
            AddData(50, obliqueAngle);
            short c71 = 0;
            if (backward) c71 += 2;
            if (upsidedown) c71 += 4;
            AddData(71, c71);
            if (height == 0) AddData(42, lastHeightUsed);
            AddData(3, primaryFontFile);
        }
                        public void AddMicrostationExtendetData(string fontName)
        {
            AddReplace(1001, "ACAD");
            AddReplace(1000, fontName);
        }
            }
}
