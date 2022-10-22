using System;
using System.Linq;
using GeoBase.Compiler.Parser;
using GeoCalculations.Protocol;
using NUnit.Framework;

namespace GeoCalculationsTest.Protocol
{
    [TestFixture]
    class ProtocolParserTest
    {
        string Eval(Node node)
        {
            switch (node.Label)
            {
                case "Terms":
                case "TableRows":
                case "TableColumns":
                case "CellContent":
                case "Attribute":
                case "IfElse":
                    return node.Nodes.Aggregate(string.Empty, (current, curNode) => current + Eval(curNode));
                case "Table":
                    return "<table" + Eval(node.Nodes[0]) + ">" + Eval(node.Nodes[1]) + "</table>";
                case "TableRow":
                    return "<tr" + Eval(node.Nodes[0]) + ">" + Eval(node.Nodes[1]) + "</tr>";
                case "ParseAttributes":
                    return node.Nodes.Aggregate(string.Empty, (current, curNode) => " " + current + Eval(curNode));
                case "ParseAttribute":
                    return node.Nodes[0].Text + "=\"" + Eval(node.Nodes[1]) + "\"";
                case "AttributeValue":
                    return node.Text;
                case "TableHeaderColumn":
                    return "<th" + Eval(node.Nodes[0]) + ">" + Eval(node.Nodes[1]) + "</th>";
                case "TableDataColumn":
                    return "<td" + Eval(node.Nodes[0]) + ">" + Eval(node.Nodes[1]) + "</td>";
                case "Expression":
                case "Text":
                case "EndOfLine":
                    return node.Text;
                case "Else":
                    return "<else> " + node.Nodes.Aggregate(string.Empty, (current, curNode) => current + Eval(curNode)) + "</else>";
                case "If":
                    return "<if " + Eval(node.Nodes[0]) + " >" + Eval(node.Nodes[1]) + "</if>";
                case "Line":
                    return node.Nodes.Aggregate(string.Empty, (current, curNode) => current + Eval(curNode));
                case "AttributeBoolValueExpressin":
                    {
                        if (node.Count == 1)
                            return Eval(node.Nodes[0]);
                        return Eval(node.Nodes[0]) + Eval(node.Nodes[1]);
                    }
                case "ExpressionValue":
                    return "{{" + node.Text + "}}";
                case "Return":
                    return "<return/>";
                case "Space":
                    return "<space/>";
                default:
                    throw new NotImplementedException();
            }
        }

        [Test]
        public void Text()
        {
            var node = ProtocolGrammar.Terms.Parse("ahoj");
            var evaluated = Eval(node[0]);
            Assert.AreEqual("ahoj", evaluated);
        }

        [Test]
        public void Expression()
        {
            var node = ProtocolGrammar.Terms.Parse("{{ahoj}}");
            var evaluated = Eval(node[0]);
            Assert.AreEqual("{{ahoj}}", evaluated);
        }

        [Test]
        public void ExpressionInsideText()
        {
            var node = ProtocolGrammar.Terms.Parse("prefix {{ahoj}} postfix");
            var evaluated = Eval(node[0]);
            Assert.AreEqual("prefix {{ahoj}} postfix", evaluated);
        }

        [Test]
        public void TwoLine()
        {
            var node = ProtocolGrammar.Terms.Parse("prefix {{ahoj}} postfix<EOL/> nextLine");
            var evaluated = Eval(node[0]);
            Assert.AreEqual("prefix {{ahoj}} postfix<EOL/> nextLine", evaluated);
        }

        [Test]
        public void Table()
        {
            var node = ProtocolGrammar.Terms.Parse("<table></table>");
            var evaluated = Eval(node[0]);
            Assert.AreEqual("<table></table>", evaluated);
        }

        [Test]
        public void TableWithContent()
        {
            var node = ProtocolGrammar.Terms.Parse("<table variable=\"data\"><tr><td>prefix {{ahoj}} postfix</td></tr></table>");
            var evaluated = Eval(node[0]);
            Assert.AreEqual("<table variable=\"data\"><tr><td>prefix {{ahoj}} postfix</td></tr></table>", evaluated);
        }

        [Test]
        public void MultiLineTable()
        {
            var node = ProtocolGrammar.Terms.Parse("<table variable=\"data\">" +
                                                        "<tr>" +
                                                            "<td>prefix {{ahoj}} postfix</td>" +
                                                        "</tr>" +
                                                    "</table>");
            var evaluated = Eval(node[0]);
            Assert.AreEqual("<table variable=\"data\"><tr><td>prefix {{ahoj}} postfix</td></tr></table>", evaluated);
        }

        [Test]
        public void MultiLineTableWithHeader()
        {
            var node = ProtocolGrammar.Terms.Parse("<table variable=\"data\">" +
                                                        "<tr><th>Data</th></tr>" +
                                                        "<tr>" +
                                                            "<td>prefix {{ahoj}} postfix</td>" +
                                                        "</tr>" +
                                                    "</table>");
            var evaluated = Eval(node[0]);
            Assert.AreEqual("<table variable=\"data\"><tr><th>Data</th></tr><tr><td>prefix {{ahoj}} postfix</td></tr></table>", evaluated);
        }

        [Test]
        public void TableWithEmptyColumn()
        {
            var node = ProtocolGrammar.Terms.Parse("<table><tr><th><SPACE/></th></tr></table>");
            var evaluated = Eval(node[0]);
            Assert.AreEqual("<table><tr><th><space/></th></tr></table>", evaluated);
        }

        [Test]
        public void Return()
        {
            var node = ProtocolGrammar.Terms.Parse("<return/>");
            var evaluated = Eval(node[0]);
            Assert.AreEqual("<return/>", evaluated);
        }

        [Test]
        public void IfWithReturn()
        {
            var node = ProtocolGrammar.Terms.Parse("<if Variable=\"true\"><return/></if>");
            var evaluated = Eval(node[0]);
            Assert.AreEqual("<if  Variable=\"true\" ><return/></if>", evaluated);
        }

        [Test]
        public void IfElseWithTable()
        {
            var node = ProtocolGrammar.Terms.Parse("<if Variable=\"false\"><table></table></if><else><table></table></else>");
            var evaluated = Eval(node[0]);
            Assert.AreEqual("<if  Variable=\"false\" ><table></table></if><else> <table></table></else>", evaluated);
        }

        [Test]
        public void IfElseWithTableAndText()
        {
            var node = ProtocolGrammar.Terms.Parse("<if>d</if><else>text<table></table>text</else>");
            var evaluated = Eval(node[0]);
            Assert.AreEqual("<if  >d</if><else> text<table></table>text</else>", evaluated);
        }

    }
}
