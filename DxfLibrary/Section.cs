using System.Collections;

namespace DxfLibrary
{
    public class Section : Element
    {
        #region Constructor
        public Section(string s)
        {
            StartTag = new Data(0, "SECTION");
            EndTag = new Data(0, "ENDSEC");

            Data = new ArrayList();
            Elements = new ArrayList();
            Data.Add(new Data(2, s));
        }
        #endregion
    }
}
