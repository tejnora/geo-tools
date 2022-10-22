using System;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoCalculations.Points;
using NUnit.Framework;

namespace GeoCalculationsTest.Methods
{
    [TestFixture]
    class LinesIntersectionTests
    {
        [Test]
        public void Test1()
        {
            var a = new Line { StartPoint = new SimplePoint { X = 0, Y = 0 }, EndPoint = new SimplePoint { X = 5, Y = 5 } };
            var b = new Line { StartPoint = new SimplePoint { X = 0, Y = 5 }, EndPoint = new SimplePoint { X = 5, Y = 0 } };
            var calculatedPoint = new CalculatedPointBase();
            LinesIntersectionMethod.Calculated(new LinesIntersectionContext { FirstLine = a, SecondLine = b }, calculatedPoint);
            Assert.AreEqual(Math.Round(calculatedPoint.X, 2), 2.5);
            Assert.AreEqual(Math.Round(calculatedPoint.Y, 2), 2.5);
        }
        [Test]
        public void Test2()
        {
            var a = new Line { StartPoint = new SimplePoint { X = 0, Y = 0 }, EndPoint = new SimplePoint { X = 0, Y = 0 } };
            var b = new Line { StartPoint = new SimplePoint { X = 0, Y = 5 }, EndPoint = new SimplePoint { X = 0, Y = 0 } };
            var calculatedPoint = new CalculatedPointBase();
            LinesIntersectionMethod.Calculated(new LinesIntersectionContext { FirstLine = a, SecondLine = b }, calculatedPoint);
            Assert.AreEqual(Math.Round(calculatedPoint.X, 2), double.NaN);
            Assert.AreEqual(Math.Round(calculatedPoint.Y, 2), double.NaN);
        }
        [Test]
        [ExpectedException(typeof(LinesIntersectionException))]
        public void Paralel()
        {
            var a = new Line { StartPoint = new SimplePoint { X = 1, Y = 5 }, EndPoint = new SimplePoint { X = 1, Y = 0 } };
            var b = new Line { StartPoint = new SimplePoint { X = 0, Y = 5 }, EndPoint = new SimplePoint { X = 0, Y = 0 } };
            var calculatedPoint = new CalculatedPointBase();
            LinesIntersectionMethod.Calculated(new LinesIntersectionContext { FirstLine = a, SecondLine = b }, calculatedPoint);
        }
        [Test]
        [ExpectedException(typeof(LinesIntersectionException))]
        public void Confluent()
        {
            var a = new Line { StartPoint = new SimplePoint { X = 0, Y = 0 }, EndPoint = new SimplePoint { X = 5, Y = 5 } };
            var b = new Line { StartPoint = new SimplePoint { X = 0, Y = 0 }, EndPoint = new SimplePoint { X = 5, Y = 5 } };
            var calculatedPoint = new CalculatedPointBase();

            LinesIntersectionMethod.Calculated(new LinesIntersectionContext { FirstLine = a, SecondLine = b }, calculatedPoint);
        }
    }
}
