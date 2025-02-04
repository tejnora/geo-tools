namespace DxfLibrary
{
    public class Header : Section
    {
                internal Header()
            : base("HEADER")
        {
        }
                        public int VariableCount()
        {
            return Elements.Count;
        }
        public Variable GetVariable(int index)
        {
            return (Variable)Elements[index];
        }
        public object ValueOf(string varName)
        {
            foreach (Variable v in Elements)
            {
                if (v.VarName == varName) return v.Value;
            }
            return null;
        }
        public void AddVariable(Variable v)
        {
            Elements.Add(v);
        }
            }
}
