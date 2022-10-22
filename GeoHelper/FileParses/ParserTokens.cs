using System;
using System.Collections.Generic;
using System.Globalization;

namespace GeoHelper.FileParses
{
    /*
    P - predcisli
    N - cislo
    Num - cele cislo
    SpolX
    SpolY
    SpolPrec
    Des - popis
    SobrX
    SobrY
    SobrZ
    SobrPrec
    Ha - vodorovný úhel
    Hl - vodorovná délka
    Za - zenitový úhel
    Su - převýšení
    Hp - Výška hranolu stanoviska
    W - váha
    */

    public class TokenFormatStringException : Exception
    {
        public TokenFormatStringException(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }

    public abstract class Token
    {
        public int Begin { get; set; }
        public int End { get; set; }

        public static List<Token> ParsePatter(string parsePattern)
        {
            var tokens = new List<Token>();
            bool parseSymbol = false;
            int begin = 0;
            for (int i = 0; i < parsePattern.Length; i++)
            {
                char c = parsePattern[i];
                if (c == '<')
                {
                    if (parseSymbol)
                        throw new ParserPatternUnexpectItem(0, i);
                    if (i != begin)
                    {
                        tokens.Add(new TextToken(begin, i - begin, parsePattern.Substring(begin, i - begin)));
                        begin = i;
                    }
                    parseSymbol = true;
                }
                else if (c == '>')
                {
                    if (!parseSymbol || i == begin)
                        throw new ParserPatternUnexpectItem(0, i);
                    begin++; //remove <
                    tokens.Add(new SymbolToken(begin, i - begin, parsePattern.Substring(begin, i - begin)));
                    begin = i + 1;
                    parseSymbol = false;
                }
            }
            if (parseSymbol)
                throw new ParserPatternUnexpectItem(0, parsePattern.Length);
            if (begin != parsePattern.Length)
            {
                tokens.Add(new TextToken(begin, parsePattern.Length - begin,
                                         parsePattern.Substring(begin, parsePattern.Length - begin)));
            }
            return tokens;
        }
    }

    internal class TextToken : Token
    {
        public TextToken(int begin, int end, string text)
        {
            Begin = begin;
            End = end;
            Text = text;
        }

        public string Text { get; set; }
    }

    public class SymbolToken : Token
    {
        public SymbolToken(int begin, int end, string symbol)
        {
            Begin = begin;
            End = end;
            int step = 0;
            string subString = string.Empty;
            LengthBeforeDot = 0;
            LengthAfterDot = 0;
            for (int i = 0; i < symbol.Length; i++)
            {
                char c = symbol[i];
                if (step == 0)
                {
                    if (char.IsLetter(c))
                    {
                        Symbol += c;
                        continue;
                    }
                    if (i == 0 || c != ':')
                        throw new ParserPatternUnexpectItem(0, Begin);
                    step++;
                    continue;
                }
                if (step == 1)
                {
                    if (char.IsDigit(c))
                    {
                        subString += c;
                        continue;
                    }
                    if (c != '.')
                    {
                        throw new ParserPatternUnexpectItem(0, Begin + i);
                    }
                    uint result;
                    if (!uint.TryParse(subString, out result))
                        throw new ParserPatternUnexpectItem(0, Begin + i);
                    LengthBeforeDot = result;
                    subString = string.Empty;
                    step++;
                    continue;
                }
                if (char.IsDigit(c))
                {
                    subString += c;
                    continue;
                }
                throw new ParserPatternUnexpectItem(0, Begin + i);
            }
            if (subString.Length > 0)
            {
                uint result;
                if (!uint.TryParse(subString, out result))
                    throw new ParserPatternUnexpectItem(0, Begin);
                if (step == 1)
                    LengthBeforeDot = result;
                else
                    LengthAfterDot = result;
            }
        }

        public string Symbol { get; set; }
        public uint LengthBeforeDot { get; set; }
        public uint LengthAfterDot { get; set; }

        public override string ToString()
        {
            return Symbol + LengthBeforeDot.ToString() + ":" +
                   LengthAfterDot.ToString();
        }

        public string ToString(double value)
        {
            string format = "{0:0." + string.Empty.PadRight((int) LengthAfterDot, '0') + "}";
            string returnValue = string.Format(CultureInfo.InvariantCulture, format, value);
            if (returnValue.Length > LengthBeforeDot + LengthAfterDot + 1)
                throw new TokenFormatStringException(returnValue);
            return returnValue.PadLeft((int) (LengthBeforeDot + LengthAfterDot + 1), '0');
        }

        public string ToString(uint value)
        {
            string returnValue = string.Format("{0}", value);
            if (returnValue.Length > LengthBeforeDot)
                throw new TokenFormatStringException(returnValue);
            return returnValue.PadLeft((int) (LengthBeforeDot), '0');
        }

        public string ToString(string value)
        {
            if (value.Length > LengthBeforeDot)
                throw new TokenFormatStringException(value);
            return value.PadLeft((int) (LengthBeforeDot), ' ');
        }
    }
}