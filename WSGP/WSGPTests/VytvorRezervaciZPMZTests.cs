using WSGP;
using Xunit;

namespace WSGPTests
{
    public class VytvorRezervaciZPMZTests
    {
        WSGPCaller _caller;
        public VytvorRezervaciZPMZTests()
        {
            _caller = new WSGPCaller(new DemoLogin());
        }

        [Fact]
        public void VytvorRezervaciZPMZ()
        {
            var res = _caller.ZPMZ.VytvorRezervaci("50623895010", "628301");
            Assert.True(res.IsValid());
            Assert.Equal("628301", res.KatuzeKod);
            Assert.Equal("328", res.CisloZPMZ);
        }

        [Fact]
        public void VytvorRezervaciZPMZ_KU_neni_soucasti_rizeni()
        {
            var res = _caller.ZPMZ.VytvorRezervaci("22285597010", "624161");
            Assert.False(res.IsValid());
            Assert.Equal("430", res.Vysledek.Kod);
            Assert.Equal("CHYBA", res.Vysledek.Uroven);
        }

        [Fact]
        public void VytvorRezervaciZPMZ_rizeni_neexistuje()
        {
            var res = _caller.ZPMZ.VytvorRezervaci("22285597010", "628506");
            Assert.False(res.IsValid());
            Assert.Equal("426", res.Vysledek.Kod);
            Assert.Equal("CHYBA", res.Vysledek.Uroven);
        }

    }
}
