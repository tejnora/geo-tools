using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VFK
{
    [Serializable]
    public class DeterminateAreaTypes //zpusob urceni vymery
    {
        public List<DeterminateeAreaTypeNode> DeterminateeAreaNodes
        {
            get; set;
        }
        private DeterminateAreaTypes()
        {
            DeterminateeAreaNodes=new List<DeterminateeAreaTypeNode>();
            DeterminateeAreaNodes.Add(new DeterminateeAreaTypeNode() { KOD = 0, NAZEV = "Graficky nebo v digitalizované mapě" });
            DeterminateeAreaNodes.Add(new DeterminateeAreaTypeNode() { KOD = 1, NAZEV = "Jiným číselným způsobem" });
            DeterminateeAreaNodes.Add(new DeterminateeAreaTypeNode() { KOD = 2, NAZEV = "Ze souřadnic v S-JTSK" });
            DeterminateeAreaNodes.Add(new DeterminateeAreaTypeNode() {KOD = UInt32.MaxValue, NAZEV = "-------------"});
        }

    }
    [Serializable]
    public class DeterminateeAreaTypeNode
    {
        public UInt32 KOD
        {
            get; set;
        }
        public string NAZEV
        {
            get; set;
        }
    }
}
