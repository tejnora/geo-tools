using GeoCalculations;
using GeoCalculations.Protocol;
using NUnit.Framework;

namespace GeoCalculationsTest.Protocol
{
    [TestFixture]
    class ProtocolExpressionEvaluatorTest
    {
        [Test]
        public void PlusTest()
        {
            var eval = new ProtocolExpressionEvaluator("1+2", null);
            Assert.AreEqual(3, eval.Evaluate().IntValue);
        }

        [Test]
        public void MinusTest()
        {
            var eval = new ProtocolExpressionEvaluator("1-2", null);
            Assert.AreEqual(-1, eval.Evaluate().IntValue);
        }

        [Test]
        public void MultiplyTest()
        {
            var eval = new ProtocolExpressionEvaluator("2*2", null);
            Assert.AreEqual(4, eval.Evaluate().IntValue);
        }


        [Test]
        public void DivideTest()
        {
            var eval = new ProtocolExpressionEvaluator("2/2", null);
            Assert.AreEqual(1, eval.Evaluate().IntValue);
        }

        [Test]
        public void PlusRationTest()
        {
            var eval = new ProtocolExpressionEvaluator("1+2.5", null);
            Assert.AreEqual(3.5, eval.Evaluate().DoubleValue);
        }

        [Test]
        public void MinusRationTest()
        {
            var eval = new ProtocolExpressionEvaluator("1-1.5", null);
            Assert.AreEqual(-0.5, eval.Evaluate().DoubleValue);
        }

        [Test]
        public void MultiplyRationTest()
        {
            var eval = new ProtocolExpressionEvaluator("2.5*2", null);
            Assert.AreEqual(5, eval.Evaluate().DoubleValue);
        }

        [Test]
        public void DivideRationTest()
        {
            var eval = new ProtocolExpressionEvaluator("3/1.5", null);
            Assert.AreEqual(2, eval.Evaluate().DoubleValue);
        }

        [Test]
        public void ParenthesisTest()
        {
            var eval = new ProtocolExpressionEvaluator("(2+3)*5", null);
            Assert.AreEqual(25, eval.Evaluate().IntValue);
        }

        [Test]
        public void SimpleConditionTest()
        {
            var eval = new ProtocolExpressionEvaluator("(2+3)*5<10", null);
            Assert.False(eval.Evaluate().BoolValue);
        }


        [Test]
        public void SimpleConditionTest2()
        {
            var eval = new ProtocolExpressionEvaluator("(2+3)*5>10", null);
            Assert.True(eval.Evaluate().BoolValue);
        }

        [Test]
        public void SimpleConditionTest3()
        {
            var eval = new ProtocolExpressionEvaluator("(2+3)*5==10", null);
            Assert.False(eval.Evaluate().BoolValue);
        }

        [Test]
        public void SimpleConditionTest4()
        {
            var eval = new ProtocolExpressionEvaluator("10==10", null);
            Assert.True(eval.Evaluate().BoolValue);
        }

        [ProtocolRootData("ProtocolAttribute")]
        public class GeneratorDataContext
        {
            [ProtocolPropertyData("DoubleTest")]
            public double DoubleTest { get; set; }
            [ProtocolPropertyData("Int32Test")]
            public double Int32Test { get; set; }
        }

        class FakeScope : ProtocolGenerator.Scope
        {
            public FakeScope(ScanProtocolResultData.PropertyContext properties, object data)
                : base(null, properties, data)
            {
            }
        }

        [Test]
        public void EvaluateDoubleVariable()
        {
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var eval = new ProtocolExpressionEvaluator("DoubleTest==10", new FakeScope(dataProperties, new GeneratorDataContext { DoubleTest = 10 }));
            Assert.True(eval.Evaluate().BoolValue);
        }

        [Test]
        public void EvaluateInt32Variable()
        {
            var dataProperties = ScanProtocolResultData.GetProperties(typeof(GeneratorDataContext));
            var eval = new ProtocolExpressionEvaluator("Int32Test==10", new FakeScope(dataProperties, new GeneratorDataContext { Int32Test = 10 }));
            Assert.True(eval.Evaluate().BoolValue);
        }

    }
}
