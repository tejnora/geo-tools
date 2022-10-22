using System.Collections.Generic;
using System.IO;
using System.Text;
using GeoHelper.Tables;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Tabulky;

namespace GeoHelper.FileParses
{
    public abstract class UserFormatParser : IUserPattern
    {
        public string ParserPatter { get; set; }
        public virtual List<TableNodesBase> Nodes { get; set; }

        public virtual TableNodesBase GetNewNode()
        {
            return null;
        }

        public void ParseFile(FileInfo location)
        {
            Nodes = new List<TableNodesBase>();
            List<Token> tokens = Token.ParsePatter(ParserPatter);
            TextReader reader = new StreamReader(new FileStream(location.FullName, FileMode.Open), Encoding.UTF8);
            string line;
            int lineIdx = 0;
            while ((line = reader.ReadLine()) != null)
            {
                TableNodesBase node = GetNewNode();
                int tokenIdx = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    if (tokenIdx >= tokens.Count)
                        break;
                    if (tokens[tokenIdx] is TextToken)
                    {
                        var token = (TextToken) tokens[tokenIdx];
                        string text = token.Text;
                        string subString = line.Substring(i, text.Length);
                        if (text != subString)
                        {
                            for (int j = 0; j < text.Length; j++)
                            {
                                if (subString.Length >= j)
                                    throw new ParserUnexpectLine(lineIdx, i + j);
                                if (subString[j] != text[j])
                                    throw new ParserUnexpectLine(lineIdx, i + j);
                            }
                        }
                        i += text.Length - 1;
                        tokenIdx++;
                        continue;
                    }
                    if (tokens[tokenIdx] is SymbolToken)
                    {
                        var token = (SymbolToken) tokens[tokenIdx];
                        //parse length before dot and after dot
                        uint numberLength = token.LengthBeforeDot;
                        if (token.LengthAfterDot != 0)
                            numberLength += token.LengthAfterDot + 1 /*dot*/;
                        string subString;
                        if (i + numberLength < line.Length)
                            subString = line.Substring(i, (int) numberLength);
                        else if (i < line.Length)
                            subString = line.Substring(i);
                        else
                            break;
                        subString = subString.Trim(' ');
                        if (!node.SetValue(token, subString))
                            throw new ParserUnexpectLine(lineIdx, i + subString.Length);
                        i += (int) numberLength - 1;
                        tokenIdx++;
                        continue;
                    }
                }
                Nodes.Add(node);
                lineIdx++;
            }
        }
    }

    internal class SSUserFormatParser : UserFormatParser
    {
        public override TableNodesBase GetNewNode()
        {
            return new TableCoordinateListNode();
        }
    }

    internal class SSDUserFormatParser : UserFormatParser
    {
        public override TableNodesBase GetNewNode()
        {
            return new TableDoubleCoordinateListNode();
        }
    }

    internal class MLPMUserFormatParser : UserFormatParser
    {
        public override TableNodesBase GetNewNode()
        {
            return new TableMeasureListNode();
        }
    }
}