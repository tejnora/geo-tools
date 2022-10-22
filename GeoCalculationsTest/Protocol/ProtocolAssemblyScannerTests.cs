using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using GeoCalculations.Protocol;
using NUnit.Framework;

namespace GeoCalculationsTest.Protocol
{
    [ProtocolRootData("ProtocolAttribute")]
    public class ProtocolAttributeTestClass
    {
        [ProtocolPropertyDataAttribute("StringProp")]
        public string StringProp { get; set; }

        [ProtocolPropertyDataAttribute("StringListProp")]
        public List<string> StringListProp { get; set; }

        [ProtocolPropertyDataAttribute("ListClassProp")]
        public List<ListClass> ListClassProp { get; set; }

    };

    public class ListClass
    {
        [ProtocolPropertyDataAttribute("StringProp")]
        public string StringProp { get; set; }
    }

    [TestFixture]
    class ProtocolAssemblyScannerTests
    {
        [Test]
        public void ProtocolRootDataAttribute()
        {
            var types = ScanProtocolResultData.GetCalculationResults(Assembly.GetAssembly(typeof(ProtocolAssemblyScannerTests)));
            Assert.Contains(typeof(ProtocolAttributeTestClass), new Collection<Type>(types.ToList()));
        }

        [Ignore]
        public void ProtocolPropertyDataAttribute()
        {
            var types = ScanProtocolResultData.GetCalculationResults(Assembly.GetAssembly(typeof(ProtocolAssemblyScannerTests)));
            var properties = ScanProtocolResultData.GetProperties(types.First());
            Assert.AreEqual(3, properties.Children.Count);
            Assert.AreEqual(0, properties.Children[0].Children.Count);
            Assert.AreEqual(2, properties.Children[1].Children.Count);
            Assert.AreEqual(2, properties.Children[2].Children.Count);
        }
    }
}
