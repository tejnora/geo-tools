using WSGP;
using Xunit;

namespace WSGPTests
{
    public class ZalozRizeniPMTests
    {
        WSGPCaller _caller;
        public ZalozRizeniPMTests()
        {
            _caller = new WSGPCaller(new DemoLogin());
        }

        [Fact]
        public void ZalozRizeniPM()
        {
            var res = _caller.Rizeni.Zalozit("1/2017", new[] { "628301" }, new[] { "752228203", "752230203", "991254203" });
            Assert.True(res.IsValid());
            Assert.Equal("50623895010", res.IdRizeni);
            Assert.Equal("PM-1073/2017-203", res.CisloRizeni);
            Assert.Equal("203", res.PraresKod);
            Assert.Equal("PM", res.RizeniTyp);
            Assert.Equal("1073", res.PoradoveCislo);
            Assert.Equal("2017", res.Rok);
        }

        [Fact]
        public void ZalozRizeniPM_neexistujeKU()
        {
            var res = _caller.Rizeni.Zalozit("Test ITO PM pro ZPG/PGP 2", new[] { "733859" }, new[] { "2757502305" });
            Assert.False(res.IsValid());
            Assert.Equal("302", res.Vysledek.Kod);
            Assert.Equal("CHYBA", res.Vysledek.Uroven);
        }

        [Fact]
        public void ZalozRizeniPM_parcela_nelezi_v_zadanem_KU()
        {
            var res = _caller.Rizeni.Zalozit("Test ITO PM pro ZPG/PGP 2", new[] { "733857" }, new[] { "275750235" });
            Assert.False(res.IsValid());
            Assert.Equal("429", res.Vysledek.Kod);
            Assert.Equal("CHYBA", res.Vysledek.Uroven);
        }

        [Fact]
        public void ZalozRizeniPM_KU_nejsou_ve_sprave_stejneho_KP()
        {
            var res = _caller.Rizeni.Zalozit("Test ITO PM pro ZPG/PGP X", new[] { "733857", "622222" }, new[] { "2757502305" });
            Assert.False(res.IsValid());
            Assert.Equal("428", res.Vysledek.Kod);
            Assert.Equal("CHYBA", res.Vysledek.Uroven);
        }

    }
}
