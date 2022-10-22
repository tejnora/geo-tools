using WSGP;
using Xunit;

namespace WSGPTests
{
    public class DetailRizeniPMTests
    {
        WSGPCaller _caller;
        public DetailRizeniPMTests()
        {
            _caller = new WSGPCaller(new DemoLogin());
        }

        [Fact]
        public void DetailRizeniPM_Ok()
        {
            var res = _caller.Rizeni.Detail("22286597010");
            Assert.True(res.IsValid());
            Assert.Equal("22286597010", res.IdRizeniPM);
            Assert.Equal("PM-7/2017-305", res.CisloRizeni);
            Assert.Equal("Test ITO PM pro ZPG/PGP 2", res.CisloZakazky);
            Assert.Equal("733857", res.KatuzeKodList[0]);
            Assert.Equal("733857", res.RezervCislaZPMZList[0].KatuzeKod);
            Assert.Equal("1606", res.RezervCislaZPMZList[0].CisloZPMZ);
            Assert.Equal("733857", res.RezervPoddeleniList[0].KatuzeKod);
            Assert.Equal("1606", res.RezervPoddeleniList[0].CisloZPMZ);
            Assert.Equal("2", res.RezervPoddeleniList[0].DruhCislovaniPar);
            Assert.Equal("1531", res.RezervPoddeleniList[0].KmenoveCislo);
            Assert.Equal("32", res.RezervPoddeleniList[0].PoddeleniCisla[0]);
        }

        [Fact]
        public void DetailRizeniPM_Neexistuje()
        {
            var res = _caller.Rizeni.Detail("22286597019");
            Assert.False(res.IsValid());
            Assert.Equal("426", res.Vysledek.Kod);
            Assert.Equal("CHYBA", res.Vysledek.Uroven);
        }
    }
}
