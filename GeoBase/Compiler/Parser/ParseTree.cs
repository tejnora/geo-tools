using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace GeoBase.Compiler.Parser
{
    public class Node
    {
        public Node()
        {
        }
        public Node(int begin, string label, string input)
        {
            if (label == null)
                throw new ArgumentNullException();
            Begin = begin;
            Label = label;
            Input = input;
        }
        public Node(string label, IEnumerable<Node> content)
        {
            Label = label;
            Nodes = content.ToList();
        }
        public Node(string label, params Node[] content)
            : this(label, content as IEnumerable<Node>)
        {
        }
        public Node(string label, string content)
        {
            Label = label;
            Input = content;
            Begin = 0;
            End = Input.Length;
        }
        public Node(XElement e)
        {
            Label = e.Name.LocalName;
            if (e.HasElements)
            {
                var tmp = from x in e.Elements() select new Node(x);
                Nodes = tmp.ToList();
            }
            else
            {
                Input = e.Value;
                Begin = 0;
                End = Input.Length;
            }
        }
        public string Input;
        public int Begin;
        public int End;
        public string Label;
        public List<Node> Nodes = new List<Node>();
        public int Length { get { return End > Begin ? End - Begin : 0; } }
        public string Text { get { return Input.Substring(Begin, Length); } }
        public bool IsLeaf { get { return Nodes.Count == 0; } }
        public XElement ToXml
        {
            get
            {
                return IsLeaf
                    ? new XElement(Label, Text)
                    : new XElement(Label, from n in Nodes select n.ToXml);
            }
        }
        public IEnumerable<Node> GetNodes(string label)
        {
            return Nodes.Where(n => n.Label == label);
        }
        public Node GetNode(string label)
        {
            return GetNodes(label).First();
        }
        public Node this[string label]
        {
            get { return GetNode(label); }
        }
        public Node this[int n]
        {
            get { return Nodes[n]; }
        }
        public int Count
        {
            get { return Nodes.Count; }
        }
        public override string ToString()
        {
            return String.Format("{0}:{1}", Label, Text);
        }
        public IEnumerable<Node> Descendants
        {
            get
            {
                foreach (var n in Nodes)
                {
                    foreach (var d in n.Descendants)
                        yield return d;
                    yield return n;
                }
            }
        }
    }
}

