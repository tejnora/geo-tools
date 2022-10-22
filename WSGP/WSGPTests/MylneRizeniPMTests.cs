using WSGP;
using Xunit;

namespace WSGPTests
{
    public class MylneRizeniPMTests
    {
        WSGPCaller _caller;
        public MylneRizeniPMTests()
        {
            _caller = new WSGPCaller(new DemoLogin());
        }

        [Fact]
        public void MylneRizeni()
        {
            var res = _caller.Rizeni.MylneRizeni("50623878010", "storno zakázky");
            Assert.True(res.IsValid());
        }

        [Fact]
        public void MylneRizeni_Existuje_Rezervace_ZPMZ()
        {
            var res = _caller.Rizeni.MylneRizeni("50623878020", "storno zakázky");
            Assert.False(res.IsValid());
            Assert.Equal("431", res.Vysledek.Kod);
            Assert.Equal("CHYBA", res.Vysledek.Uroven);
        }

    }
}
