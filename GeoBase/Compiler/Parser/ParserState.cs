﻿using System;
using System.Collections.Generic;

namespace GeoBase.Compiler.Parser
{
    public class ParserState
    {
        public string input;
        public int pos;
        public List<Node> nodes = new List<Node>();
        private Dictionary<int, Dictionary<Rule, Node>> cache = new Dictionary<int,Dictionary<Rule, Node>>();

        public string Current
        {
            get { return input.Substring(pos); }
        }
        public ParserState Clone()
        {
            return new ParserState { 
                input = input, 
                pos = pos, 
                nodes = new List<Node>(nodes) };
        }
        public void Assign(ParserState state)
        {
            input = state.input; 
            pos = state.pos; 
            nodes = state.nodes;
        }
        public void RestoreAfter(Action action)
        {
            var state = Clone(); 
            action(); 
            Assign(state);
        }
        public override string ToString()
        {
            return String.Format("{0}/{1}", pos, input.Length);
        }
        public void CacheResult(Rule rule, int pos, Node node)
        {
            if (!cache.ContainsKey(pos)) 
                cache.Add(pos, new Dictionary<Rule, Node>());
            var tmp = cache[pos];
            if (!tmp.ContainsKey(rule))
                tmp.Add(rule, node);
        }
        public bool GetCachedResult(NodeRule rule, out Node node)
        {
            node = null;
            if (!cache.ContainsKey(pos))
                return false;
            if (cache[pos].ContainsKey(rule))
            {
                node = cache[pos][rule];
                return true;
            }
            return false;
        }
    }
}
