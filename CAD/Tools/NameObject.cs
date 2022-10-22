using System.Collections.Generic;

namespace CAD.Tools
{
    public class NameObject<T>
    {
        public NameObject(string name, T obj)
        {
            Name = name;
            Object = obj;
        }
        public string Name
        {
            get;
            private set;
        }
        public T Object
        {
            get;
            private set;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class NameObjectTwo<T,T2>
    {
        public NameObjectTwo(string name, T obj, T2 obj2)
        {
            Name = name;
            Object = obj;
            Object2 = obj2;
        }
        public string Name
        {
            get; 
            private set;
        }
        public T Object
        {
            get;
            private set;
        }
        public T2 Object2
        {
            get; 
            private set;
        }

        public override string ToString()
        {
            return Name;
        }

    }
    public class NameObjectCollection<T> : List<NameObject<T>>
    {
        public void Add(string name, T value)
        {
            Add(new NameObject<T>(name, value));
        }
        public NameObject<T> FindValue(T value)
        {
            foreach (NameObject<T> item in this)
            {
                if (item.Object.Equals(value))
                    return item;
            }
            return null;
        }
    }
}
