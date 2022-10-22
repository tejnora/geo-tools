namespace DxfLibrary
{
    public class LineType : TableEntry
    {
        #region Constructor
        public LineType(string name, string description, double patternLength)
            : base("LTYPE")
        {
            DataAcceptanceList.AddRange(new int[] { 2, 70, 3, 72, 73, 40, 49, 74, 75, 340, 46, 50, 44, 45, 9 });
            AddReplace(2, name);
            AddData(70, (short)0);
            AddData(3, description);
            AddData(72, (short)65);
            _nrElements = 0;
            AddReplace(73, _nrElements);
            AddReplace(40, patternLength);
        }
        public LineType(string s)
            : base(s)
        {
        }
        #endregion
        #region Fields
        short _nrElements;
        #endregion
        #region Methods
        /// <summary>
        /// Add a simple section to the pattern. If positive, represents continuous line, if negative, represents space.
        /// </summary>
        /// <param name="length">The length of the section added</param>
        public void AddElement(double length)
        {
            AddData(49, length);
            _nrElements++;
            AddReplace(73, _nrElements);
        }

        /// <summary>
        /// Add a text element to the pattern.
        /// </summary>
        /// <param name="length">Dash, dot or space length</param>
        /// <param name="text">Text string</param>
        /// <param name="absoluteRotation"></param>
        /// <param name="rotation">R = (relative) or A = (absolute) rotation value in radians of embedded text. -1000 value represents no rotation</param>
        public void AddElement(double length, string text, bool absoluteRotation, double rotation)
        {
            AddData(49, length);
            if (absoluteRotation)
                AddData(74, (short)3);
            else
                AddData(74, (short)2);
            AddData(75, (short)0);
            AddData(50, rotation);
            AddData(9, text);
            _nrElements++;
            AddReplace(73, _nrElements);
        }

        /// <summary>
        /// Add a shape element to the pattern.
        /// </summary>
        /// <param name="length">The length of the added section to the pattern</param>
        /// <param name="shapeNumber">The SHAPE element number</param>
        /// <param name="absoluteRotation"></param>
        /// <param name="rotation">R = (relative) or A = (absolute) rotation value in radians of embedded text. -1000 value represents no rotation</param>
        public void AddElement(double length, short shapeNumber, bool absoluteRotation, double rotation)
        {
            AddData(49, length);
            if (absoluteRotation)
                AddData(74, (short)5);
            else
                AddData(74, (short)4);
            AddData(75, shapeNumber);
            if (rotation != -1000)
                AddData(50, rotation);
            _nrElements++;
            AddReplace(73, _nrElements);
        }
        #endregion
    }
}
