﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using GeoBase.Compiler.Parser;

namespace GeoBase.Compiler.Grammars
{
    public class Grammar
    {
        public static Rule Node(Rule rule)
        {
            return new NodeRule(rule);
        }
        public static Rule Recursive(Func<Rule> ruleGen)
        {
            return new RecursiveRule(ruleGen);
        }

        public static Rule At(Rule rule)
        {
            return new AtRule(rule);
        }

        public static Rule Seq(params Rule[] rs)
        {
            return new SeqRule(rs);
        }

        public static Rule Choice(params Rule[] rs)
        {
            return new ChoiceRule(rs);
        }

        public static Rule End = new EndRule();

        public static Rule Not(Rule r)
        {
            return new NotRule(r);
        }

        public static Rule ZeroOrMore(Rule r)
        {
            return new ZeroOrMoreRule(r);
        }

        public static Rule OneOrMore(Rule r)
        {
            return new PlusRule(r);
        }

        public static Rule Opt(Rule r)
        {
            return new OptRule(r);
        }

        public static Rule MatchString(Func<string, int, int> f)
        {
            return new StringRule(f);
        }

        public static Rule MatchString(string s)
        {
            return new StringRule(s);
        }

        public static Rule MatchChar(Predicate<char> f)
        {
            return new CharRule(f);
        }

        public static Rule MatchChar(char c)
        {
            return MatchChar(x => x == c).SetName(c.ToString());
        }

        public static Rule MatchRegex(Regex re)
        {
            return new RegexRule(re);
        }

        public static Rule CharSet(string s)
        {
            if (String.IsNullOrEmpty(s)) throw new ArgumentException();
            return MatchChar(c => s.Contains(c)).SetName(String.Format("[{0}]", s));
        }

        public static Rule CharRange(char a, char b)
        {
            return MatchChar(c => (c >= a) && (c <= b)).SetName(String.Format("[{0}..{1}]", a, b));
        }

        public static Rule ExceptCharSet(string s)
        {
            if (String.IsNullOrEmpty(s)) throw new ArgumentException();
            return MatchChar(c => !s.Contains(c)).SetName(String.Format("[{0}]", s)); ;
        }

        public static Rule ExceptStringSet(string s)
        {
            if (String.IsNullOrEmpty(s)) throw new ArgumentException();
            var exceptStrings = s.Split(' ');
            return MatchString(
                (text, startIdx) =>
                {
                    var endIdx = text.Length;
                    foreach (var exceptString in exceptStrings)
                    {
                        var idx = text.IndexOf(exceptString, startIdx);
                        if (idx == -1) continue;
                        endIdx = Math.Min(idx, endIdx);
                    }
                    return endIdx;
                }).SetName(String.Format("[{0}]", s)); ;
        }

        public static Rule AnyChar = MatchChar(c => true).SetName(".");

        public static Rule AdvanceWhileNot(Rule r)
        {
            if (r == null) throw new ArgumentNullException();
            return ZeroOrMore(Seq(Not(r), AnyChar));
        }

        public static Rule Pattern(string s)
        {
            if (String.IsNullOrEmpty(s)) throw new ArgumentException();
            return MatchRegex(new Regex(s));
        }

        public static IEnumerable<Rule> GetRules(Type type)
        {
            foreach (var fi in type.GetFields())
                if (fi.FieldType.Equals(typeof(Rule)))
                    yield return (Rule)fi.GetValue(null);
        }

        public static void InitGrammar(Type type)
        {
            foreach (var fi in type.GetFields())
            {
                if (fi.FieldType.Equals(typeof(Rule)))
                {
                    var rule = fi.GetValue(null) as Rule;
                    if (rule == null)
                        throw new Exception("Unexpected null rule");
                    rule.Name = fi.Name;
                }
            }
        }

        public static void OutputGrammar(Type type, TextWriter tw)
        {
            foreach (var r in GetRules(type))
                tw.WriteLine("{0} <- {1}", r.Name, r.Definition);
        }

        public static void OutputGrammar(Type type)
        {
            OutputGrammar(type, Console.Out);
        }
    }
}
