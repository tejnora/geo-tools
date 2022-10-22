using System;
using GeoCalculations.BasicTypes;
using GeoCalculations.Methods;
using NUnit.Framework;

namespace GeoCalculationsTest.Methods
{
    [TestFixture]
    class BasicCalculationTests
    {
        [Test]
        public void DirectionAzimut()
        {
            var calculateSmernik = SimpleCalculation.CalculateDirectionAzimut(new Point(10, 10), new Point(20, 20));
            Assert.AreEqual(calculateSmernik, 50);
            calculateSmernik = SimpleCalculation.CalculateDirectionAzimut(new Point(-10, 10), new Point(-20, 20));
            Assert.AreEqual(calculateSmernik, 150);
            calculateSmernik = SimpleCalculation.CalculateDirectionAzimut(new Point(10, -10), new Point(20, -20));
            Assert.AreEqual(calculateSmernik, 350);
            calculateSmernik = SimpleCalculation.CalculateDirectionAzimut(new Point(-10, -10), new Point(-20, -20));
            Assert.AreEqual(calculateSmernik, 250);
        }

        [Test]
        public void Length()
        {
            var delkaStrany = SimpleCalculation.CalculateDistance(new Point(10, 10), new Point(20, 20));
            Assert.AreEqual(Math.Round(delkaStrany, 4), 14.1421);
            delkaStrany = SimpleCalculation.CalculateDistance(new Point(0, 3), new Point(4, 0));
            Assert.AreEqual(Math.Round(delkaStrany, 4), 5);
        }

        [Test]
        public void HeightDistance()
        {
            var prevyseni = SimpleCalculation.CalculateHeightDistance(new Point { Height = 10 }, new Point { Height = 20 });
            Assert.AreEqual(prevyseni, 10);
            prevyseni = SimpleCalculation.CalculateHeightDistance(new Point { Height = 30 }, new Point { Height = 20 });
            Assert.AreEqual(prevyseni, -10);
            prevyseni = SimpleCalculation.CalculateHeightDistance(new Point { Height = -20 }, new Point { Height = 20 });
            Assert.AreEqual(prevyseni, 40);
            prevyseni = SimpleCalculation.CalculateHeightDistance(new Point { Height = -20 }, new Point { Height = -30 });
            Assert.AreEqual(prevyseni, -10);
            prevyseni = SimpleCalculation.CalculateHeightDistance(new Point { Height = -30 }, new Point { Height = -20 });
            Assert.AreEqual(prevyseni, 10);
            prevyseni = SimpleCalculation.CalculateHeightDistance(new Point { Height = 20 }, new Point { Height = -20 });
            Assert.AreEqual(prevyseni, -40);
        }

        [Test]
        public void Lenght1()
        {
            var delka = SimpleCalculation.CalculateDistance(new Point { X = 10, Y = 10, Height = 10 }, new Point { X = 20, Y = 20, Height = 20 });
            Assert.AreEqual(Math.Round(delka, 4), 17.3205);
            delka = SimpleCalculation.CalculateDistance(new Point { X = 10, Y = -10, Height = 10 }, new Point { X = 20, Y = -20, Height = 20 });
            Assert.AreEqual(Math.Round(delka, 4), 17.3205);
            delka = SimpleCalculation.CalculateDistance(new Point { X = 10, Y = -10, Height = 10 }, new Point { X = 20, Y = -20, Height = -100 });
            Assert.AreEqual(Math.Round(delka, 4), 110.9054);
        }

        [Test]
        public void Slope()
        {
            var sklon = SimpleCalculation.CalculateSlope(new Point { X = 10, Y = 10, Height = 10 }, new Point { X = 20, Y = 20, Height = 20 });
            Assert.AreEqual(Math.Round(sklon, 2), 39.18);
            sklon = SimpleCalculation.CalculateSlope(new Point { X = 10, Y = 10, Height = 10 }, new Point { X = -20, Y = -20, Height = -20 });
            Assert.AreEqual(Math.Round(sklon, 2), -39.18);
        }

        [Test]
        public void Gradient()
        {
            var spad = SimpleCalculation.CalculateGradient(new Point { X = 10, Y = 10, Height = 10 }, new Point { X = 20, Y = 20, Height = 20 });
            Assert.AreEqual(Math.Round(spad, 3), 70.711);
            spad = SimpleCalculation.CalculateGradient(new Point { X = 10, Y = 10, Height = 10 }, new Point { X = -20, Y = -20, Height = -20 });
            Assert.AreEqual(Math.Round(spad, 3), -70.711);
        }
    }
}
