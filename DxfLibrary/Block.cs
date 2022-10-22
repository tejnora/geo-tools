namespace DxfLibrary
{
    public class Block : Element
    {
        public Block()
        {
            DataAcceptanceList.AddRange(new int[] { 0, 5, 102, 330, 100, 8, 70, 10, 20, 30, 3, 1, 4, 2 });
            StartTag = new Data(0, "BLOCK");
        }
        public Block(string s)
            : base(s)
        {
        }

        public void SetEndBlk(EndBlk eb)
        {
            if (Elements.Count > 0 && ((Element)Elements[Elements.Count - 1]).Name == "ENDBLK")
            {
                Elements.RemoveAt(Data.Count - 1);
            }
            Elements.Add(eb);
        }

        public void AddEntity(Entity e)
        {
            if (Data.Count == 0) Elements.Add(e);
            else Elements.Insert(Elements.Count - 1, e);
        }

        public void SetLayer(string l)
        {
            int ind = GetIndexFor(8);
            if (ind > -1)
            {
                Data.RemoveAt(ind);
                Data.Insert(ind, new Data(8, l));
            }
            else
                Data.Add(new Data(8, l));
        }
        
        public void SetPosition(double x, double y, double z)
        {
            Data dx, dy, dz;
            bool swx = false, swy = false, swz = false;
            foreach (Data d in Data)
            {
                if (d.Code == 10)
                {
                    dx = d;
                    swx = true;
                }
                if (d.Code == 20)
                {
                    dy = d;
                    swy = true;
                }
                if (d.Code == 30)
                {
                    dz = d;
                    swz = true;
                }
            }
            if (swx) dx._data = x;
            else
            {
                dx.Code = 10;
                dx._data = x;
                Data.Add(dx);
            }
            if (swy) dy._data = y;
            else
            {
                dy.Code = 20;
                dy._data = y;
                Data.Add(dy);
            }
            if (swz) dz._data = z;
            else
            {
                dz.Code = 30;
                dz._data = z;
                Data.Add(dz);
            }
        }
        public void SetName(string name)
        {
            AddReplace(2, name);
        }

        public void SetHandle(string handle)
        {
            AddReplace(5, handle);
        }

        public void SetFlag(short flag)
        {
            AddReplace(70, flag);
        }
    }
}
