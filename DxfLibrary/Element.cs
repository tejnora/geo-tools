using System.Collections;

namespace DxfLibrary
{
    public abstract class Element
    {
        #region Constructor
        protected Element()
        {
            Data = new ArrayList();
            Elements = new ArrayList();
            DataAcceptanceList = new ArrayList();
            ElementAcceptanceList = new ArrayList();
        }
        public Element(string s)  // Used for createing an element with user dxf code
        {
            S = s;
        }
        #endregion
        #region Fields
        internal Data StartTag = new Data(-10, 0);
        internal Data EndTag = new Data(-10, 0);
        protected ArrayList Data;
        protected ArrayList Elements;
        protected ArrayList DataAcceptanceList;
        protected ArrayList ElementAcceptanceList;
        internal string S;
        #endregion
        #region Methods
        internal int AddElement(Element e)
        {
            if (IsAccepted(e))
                return Elements.Add(e);
            throw new UnexpectedElement();
        }
        internal void InsertElement(int index, Element e)
        {
            if (IsAccepted(e)) Elements.Insert(index, e);
            else throw new UnexpectedElement();
        }
        internal void RemoveElementAt(int index)
        {
            Elements.RemoveAt(index);
        }
        internal Element GetElement(int index)
        {
            return (Element)Elements[index];
        }
        internal int ElementCount()
        {
            return Elements.Count;
        }

        internal int AddData(Data e)
        {
            if (IsAccepted(e)) 
                return Data.Add(e);
            throw new UnexpectedElement();
        }
        internal void InsertData(int index, Data e)
        {
            if (IsAccepted(e)) Data.Insert(index, e);
            else throw new UnexpectedElement();
        }
        internal void RemoveDataAt(int index)
        {
            Data.RemoveAt(index);
        }
        internal Data GetData(int index)
        {
            return (Data)Data[index];
        }
        internal int GetIndexFor(int code)
        {
            foreach (Data d in Data)
            {
                if (d.Code == code) return Data.IndexOf(d);
            }
            return -1;
        }
        internal Data GetDataFor(int code)
        {
            foreach (Data d in Data)
            {
                if (d.Code == code) return d;
            }
            return new Data(-10, 0);
        }
        internal int DataCount()
        {
            return Data.Count;
        }

        protected bool IsAccepted(Data d) // verifica daca tipul de date ( rep prin cod dxf ) este acceptat de element
        {

            return (DataAcceptanceList.Contains(d.Code) && IsCorectData(d));
        }
        protected bool IsAccepted(Element e)
        {
            return true; // DEBUG : trebuie modificat
            //return elementAcceptanceList.Contains(e);
        }
        protected bool IsCorectData(Data d) // Verifica daca tipul datei corespunde cu codul acesteia
        {
            if (d.Code >= 290 && d.Code <= 299)
                if (d._data.GetType().ToString() == "System.Boolean") return true;
                else return false;
            if ((d.Code >= 60 && d.Code <= 79) || (d.Code >= 270 && d.Code <= 289) || (d.Code >= 370 && d.Code <= 389) || (d.Code >= 170 && d.Code <= 179))
                if (d._data.GetType().ToString() == "System.Int16") return true;
                else return false;
            if ((d.Code >= 90 && d.Code <= 99) || (d.Code == 1071))
                if (d._data.GetType().ToString() == "System.Int32") return true;
                else return false;
            if ((d.Code >= 10 && d.Code <= 59) || (d.Code >= 110 && d.Code <= 149) || (d.Code >= 210 && d.Code <= 239) || (d.Code >= 1010 && d.Code <= 1059))
                if (d._data.GetType().ToString() == "System.Double") return true;
                else return false;
            if (d.Code == 100 || d.Code == 102 || d.Code == 105 || d.Code == 999 || (d.Code >= 300 && d.Code <= 369) || (d.Code >= 390 && d.Code <= 399) || (d.Code >= 410 && d.Code <= 419))
                if ((d._data.GetType().ToString() == "System.String") && ((string)d._data).Length <= 255) return true;
                else return false;
            if ((d.Code >= 0 && d.Code <= 9) || (d.Code >= 1000 && d.Code <= 1009))
                if (d._data.GetType().ToString() == "System.String") return true;
                else return false;
            return false;
        }

        public string Name
        {
            get
            {
                return ((Data)Data[0])._data.ToString();
            }
        }
        public void AddReplace(int cod, object o)
        {
            int ind = GetIndexFor(cod);
            if (ind == -1) AddData(new Data(cod, o));
            else
            {
                Data.RemoveAt(ind);
                InsertData(ind, new Data(cod, o));
            }
        }
        public void AddData(int cod, object o)
        {
            AddData(new Data(cod, o));
        }
        #endregion
    }
}
