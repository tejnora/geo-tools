using GeoBase.Compiler.Grammars;
using GeoBase.Compiler.Parser;

namespace GeoCalculations.Protocol
{
    public class ProtocolGrammar : SharedGrammar
    {
        public static Rule UnicodeChar = MatchString("\\u") + HexDigit + HexDigit + HexDigit + HexDigit;
        public static Rule Text = Node(UnicodeChar | ExceptStringSet("\" \\ \n {{ <"));
        public static Rule ExpressionValue = Node(ZeroOrMore(ExceptCharSet("}")));
        public static Rule Expression = MatchString("{{") + ExpressionValue + MatchString("}}");
        public static Rule EndOfLine = Node(MatchString("<EOL/>") | MatchString("<EOL />"));
        public static Rule Space = Node(MatchString("<SPACE/>") | MatchString("<SPACE />"));
        public static Rule Line = Node(OneOrMore(Expression | Text | Space | EndOfLine));
        public static Rule CellContent = Node(Line);
        public static Rule AttributeValue = Node(ZeroOrMore(ExceptCharSet("\"")));
        public static Rule Attribute = Node(Identifier);
        public static Rule ParseAttribute = Node(WS + Attribute + WS + Eq + WS + MatchChar('\"') + AttributeValue + MatchChar('\"'));
        public static Rule ParseAttributes = Node(ZeroOrMore(ParseAttribute));
        public static Rule StartTableTag = MatchString("<table") + ParseAttributes + WS + MatchAnyString(">");
        public static Rule TableHeaderColumnStartTag = MatchString("<th") + ParseAttributes + WS + MatchAnyString(">");
        public static Rule TableHeaderColumn = Node(TableHeaderColumnStartTag + CellContent + MatchString("</th>"));
        public static Rule TableDataColumnStartTag = MatchString("<td") + ParseAttributes + WS + MatchAnyString(">");
        public static Rule TableDataColumn = Node(TableDataColumnStartTag + CellContent + MatchString("</td>"));
        public static Rule TableColumns = Node(ZeroOrMore(TableHeaderColumn | TableDataColumn | EndOfLine));
        public static Rule StartRowTag = MatchString("<tr") + ParseAttributes + WS + MatchAnyString(">");
        public static Rule TableRow = Node(StartRowTag + TableColumns + MatchString("</tr>"));
        public static Rule TableRows = Node(ZeroOrMore(TableRow | EndOfLine));
        public static Rule Table = Node(StartTableTag + TableRows + MatchString("</table>"));
        public static Rule IfStartTag = MatchString("<if") + ParseAttributes + WS + MatchAnyString(">");
        public static Rule Deviations = Node(MatchString("<deviations") + WS + ParseAttributes + WS + MatchAnyString("/>"));
        public static Rule Return = Node(MatchString("<return") + WS + MatchString("/>"));
        public static Rule Statement = Return | Deviations | Line | Table | Recursive(() => IfElse);
        public static Rule If = Node(IfStartTag + ZeroOrMore(Statement) + MatchString("</if>"));
        public static Rule Else = Node(MatchString("<else>") + ZeroOrMore(Statement) + MatchString("</else>"));
        public static Rule IfElse = Node(If + ZeroOrMore(Else));
        public static Rule Term = Recursive(() => Statement);
        public static Rule Terms = Node(ZeroOrMore(Term) + End);
        static ProtocolGrammar() { InitGrammar(typeof(ProtocolGrammar)); }
    }
}
