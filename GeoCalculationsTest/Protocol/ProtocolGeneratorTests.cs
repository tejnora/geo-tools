using System.Collections.Generic;
using System.Globalization;
using GeoCalculations.Deviations;
using GeoCalculations.Protocol;
using NUnit.Framework;

namespace GeoCalculationsTest.Protocol
{
    public class Tuple
    {
        [ProtocolPropertyDataAttribute("Key")]
        public string Key { get; set; }
        [ProtocolPropertyDataAttribute("Value")]
        public string Value { get; set; }
    }

    [ProtocolRootData("ProtocolAttribute")]
    public class GeneratorDataContext
    {
        [ProtocolPropertyDataAttribute("FirstName"), ProtocolPropertyValueTypeAttribute(ProtocolPropertyValueTypeAttribute.Types.Heigth)]
        public string FirstName { get; set; }

        [ProtocolPropertyDataAttribute("ConditionValue")]
        public bool ConditionValue { get; set; }


        [ProtocolPropertyDataAttribute("DoubleConditionValue")]
        public double DoubleConditionValue { get; set; }

        [ProtocolPropertyDataAttribute("Surname")]
        public string Surname { get; set; }

        [ProtocolPropertyDataAttribute("ListItems")]
        public List<string> ListItems { get; set; }

        [ProtocolPropertyDataAttribute("ListObjectItems")]
        public List<Tuple> ListObjectItems { get; set; }

        [ProtocolPropertyDataAttribute("Deviations")]
        public CalculationDeviation Deviations { get; set; }

        [ProtocolMethodAttribute("AdditionalMessage")]
        public string getAdditionalMessage(string arrayName, int rowId)
        {
            return arrayName + "[" + rowId.ToString(CultureInfo.InvariantCulture) + "]";
        }
    };


    [TestFixture]
    class ProtocolGeneratorTests
    {
        [Test]
        public void SimpleTest()
        {
            var node = ProtocolGrammar.Terms.Parse("Hello {{FirstName}} {{Surname}} :-)");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var data = new GeneratorDataContext { FirstName = "David", Surname = "Tejnora" };
            var result = generator.Eval(node[0], dataProperties, data);
            Assert.AreEqual("Hello David Tejnora :-)", result);
        }

        [Test]
        public void ArrayCountTest()
        {
            var node = ProtocolGrammar.Terms.Parse("List count:{{ListItems.Count}}.");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var data = new GeneratorDataContext { ListItems = new List<string> { "Test" } };
            var result = generator.Eval(node[0], dataProperties, data);
            Assert.AreEqual("List count:1.", result);
        }


        [Test]
        public void ArrayTableValueTest()
        {
            var node = ProtocolGrammar.Terms.Parse("<table>" +
                                                   "<tr Variable=\"ListItems\">" +
                                                   "<td>{{ListItems.Value}}</td>" +
                                                   "</tr>" +
                                                   "</table>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var data = new GeneratorDataContext { ListItems = new List<string> { "Test" } };
            var result = generator.Eval(node[0], dataProperties, data);
            Assert.AreEqual("Test\n", result);
        }

        [Test]
        public void TableWithAdditionalMessageAfterVariableRow()
        {
            var node = ProtocolGrammar.Terms.Parse("<table>" +
                                       "<tr Variable=\"ListItems\" AdditionalMessageAfterRow=\"getAdditionalMessage\">" +
                                       "<td>{{ListItems.Value}}</td>" +
                                       "</tr>" +
                                       "</table>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var data = new GeneratorDataContext { ListItems = new List<string> { "Test" } };
            var result = generator.Eval(node[0], dataProperties, data);
            Assert.AreEqual("Test\nListItems[0]\n", result);
        }

        [Test]
        public void TableWithAdditionalMessageAfterRow()
        {
            var node = ProtocolGrammar.Terms.Parse("<table>" +
                                       "<tr AdditionalMessageAfterRow=\"getAdditionalMessage\">" +
                                       "<td>{{FirstName}}</td>" +
                                       "</tr>" +
                                       "</table>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var data = new GeneratorDataContext { FirstName = "Test" };
            var result = generator.Eval(node[0], dataProperties, data);
            Assert.AreEqual("Test[-1]\n\n", result);
        }


        [Test]
        public void ArrayTableValueWithHeaderTest()
        {
            var node = ProtocolGrammar.Terms.Parse("<table>" +
                                                   "<tr><th>Value</th></tr>" +
                                                   "<tr Variable=\"ListItems\"><td>{{ListItems.Value}}</td></tr>" +
                                                   "</table>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var data = new GeneratorDataContext { ListItems = new List<string> { "Test" } };
            var result = generator.Eval(node[0], dataProperties, data);
            Assert.AreEqual("Value\n Test\n", result);
        }

        [Test]
        public void MulitiLineTableWithTwoCell()
        {
            var node = ProtocolGrammar.Terms.Parse("<table>" +
                                                   "<tr><th>Key</th><th>Value</th></tr>" +
                                                   "<tr Variable=\"ListObjectItems\"><td>{{ListObjectItems.Key}}</td><td>{{ListObjectItems.Value}}</td></tr>" +
                                                   "</table>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var data = new GeneratorDataContext
                           {
                               ListObjectItems = new List<Tuple>
                                                     {
                                                         new Tuple {Key = "key1", Value = "value1"},
                                                         new Tuple {Key = "key2", Value = "value2"}
                                                     }
                           };
            var result = generator.Eval(node[0], dataProperties, data);
            Assert.AreEqual(" Key Value\nkey1value1\nkey2value2\n", result);
        }

        [Test]
        public void MulitiLineTableWithCellSpace()
        {
            var node = ProtocolGrammar.Terms.Parse("<table CellSpace=\"2\">" +
                                                   "<tr><th>Key</th><th>Value</th></tr>" +
                                                   "<tr Variable=\"ListObjectItems\"><td>{{ListObjectItems.Key}}</td><td>{{ListObjectItems.Value}}</td></tr>" +
                                                   "</table>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var data = new GeneratorDataContext
            {
                ListObjectItems = new List<Tuple>
                                                         {
                                                         new Tuple {Key = "key1", Value = "value1"},
                                                         new Tuple {Key = "key2", Value = "value2"}
                                                     }
            };
            var result = generator.Eval(node[0], dataProperties, data);
            Assert.AreEqual(" Key   Value\nkey1  value1\nkey2  value2\n", result);
        }

        [Test]
        public void TableInsideText()
        {
            var node = ProtocolGrammar.Terms.Parse("Front<table>" +
                                                  "<tr Variable=\"ListItems\">" +
                                                  "<td>{{ListItems.Value}}</td>" +
                                                  "</tr>" +
                                                  "</table>Back");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var data = new GeneratorDataContext { ListItems = new List<string> { "Test" } };
            var result = generator.Eval(node[0], dataProperties, data);
            Assert.AreEqual("FrontTest\nBack", result);
        }

        [Test]
        public void TableRowWithLine()
        {
            var node = ProtocolGrammar.Terms.Parse("<table><tr Line=\"-\"><td>Line</td></tr></table>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var data = new GeneratorDataContext { ListItems = new List<string> { "Test" } };
            var result = generator.Eval(node[0], dataProperties, data);
            Assert.AreEqual("Line\n----\n", result);
        }

        [Test]
        public void IfStatement()
        {
            var node = ProtocolGrammar.Terms.Parse("<if Variable=\"ConditionValue\">True</if>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var data = new GeneratorDataContext { ConditionValue = true };
            var result = new ProtocolGenerator().Eval(node[0], dataProperties, data);
            Assert.AreEqual("True", result);
        }

        [Test]
        public void IfStatementWithTable()
        {
            var node = ProtocolGrammar.Terms.Parse("<if Variable=\"ConditionValue\"><table><tr><th>{{ConditionValue}}</th></tr></table></if>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var data = new GeneratorDataContext { ConditionValue = true };
            var result = new ProtocolGenerator().Eval(node[0], dataProperties, data);
            Assert.AreEqual("True\n", result);
        }

        [Test]
        public void IfElseStatement()
        {
            var node = ProtocolGrammar.Terms.Parse("<if Variable=\"ConditionValue\">If</if><else>else</else>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var data = new GeneratorDataContext { ConditionValue = false };
            var result = new ProtocolGenerator().Eval(node[0], dataProperties, data);
            Assert.AreEqual("else", result);
        }

        [Test]
        public void ToTables()
        {
            var node = ProtocolGrammar.Terms.Parse(
                "<table LastLine=\"-\"><tr Line=\"-\"><td>Line</td></tr></table><table>" +
                "<tr Line=\"-\"><td>Line</td></tr></table>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var result = generator.Eval(node[0], dataProperties, null);
            Assert.AreEqual("Line\n----\n----\nLine\n----\n", result);

        }


        [Test]
        public void ToWithSpaceBeforTables()
        {
            var node = ProtocolGrammar.Terms.Parse(
                "         <table LastLine=\"-\"><tr Line=\"-\"><td>Line</td></tr></table><table>" +
                "<tr Line=\"-\"><td>Line</td></tr></table>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var result = generator.Eval(node[0], dataProperties, null);
            Assert.AreEqual("Line\n----\n----\nLine\n----\n", result);

        }

        [Test]
        public void EndOfLine()
        {
            var node = ProtocolGrammar.Terms.Parse("Ahoj<EOL/>David");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var result = generator.Eval(node[0], dataProperties, null);
            Assert.AreEqual("Ahoj\nDavid", result);
        }

        [Test]
        public void RemoveSpaceBeforAfterText()
        {
            var node = ProtocolGrammar.Terms.Parse("   Ahoj   <EOL/>   David   ");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var result = generator.Eval(node[0], dataProperties, null);
            Assert.AreEqual("Ahoj   \n   David", result);
        }

        [Test]
        public void OneSpace()
        {
            var node = ProtocolGrammar.Terms.Parse("<SPACE/>Ahoj");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var generator = new ProtocolGenerator();
            var result = generator.Eval(node[0], dataProperties, null);
            Assert.AreEqual(" Ahoj", result);
        }

        [Test]
        public void IfBoolCondition()
        {
            var node = ProtocolGrammar.Terms.Parse("<if Variable=\"{{ConditionValue}}\">True<EOL/></if>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var data = new GeneratorDataContext { ConditionValue = true };
            var result = new ProtocolGenerator().Eval(node[0], dataProperties, data);
            Assert.AreEqual("True\n", result);
        }

        [Test]
        public void IfDoubleCondition()
        {
            var node = ProtocolGrammar.Terms.Parse("<if Variable=\"{{DoubleConditionValue==10}}\">True</if>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var data = new GeneratorDataContext { DoubleConditionValue = 10 };
            var result = new ProtocolGenerator().Eval(node[0], dataProperties, data);
            Assert.AreEqual("True", result);
        }

        [Test]
        [Ignore]
        public void Deviations()
        {
            var node = ProtocolGrammar.Terms.Parse("<deviations Variable=\"Deviations\" Name=\"PolarMethodDeviation\"/>");
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var data = new GeneratorDataContext { DoubleConditionValue = 10, Deviations = new CalculationDeviation() };
            data.Deviations.Deviations.Add(new PolarMethodDeviation(null, PolarMethodDeviation.Types.MaxVOrientace, 0.05));
            var result = new ProtocolGenerator().Eval(node[0], dataProperties, data);
        }

        [Test]
        public void Return()
        {
            var node = ProtocolGrammar.Terms.Parse("<if Variable=\"{{true}}\"><return/></if>david");
            var result = new ProtocolGenerator().Eval(node[0], null, null);
            Assert.AreEqual("", result);
        }

        [Test]
        public void ReturnFalse()
        {
            var node = ProtocolGrammar.Terms.Parse("<if Variable=\"{{false}}\"><return/></if>david");
            var result = new ProtocolGenerator().Eval(node[0], null, null);
            Assert.AreEqual("david", result);
        }
    }
}
