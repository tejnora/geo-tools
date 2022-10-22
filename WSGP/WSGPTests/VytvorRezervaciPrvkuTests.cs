using WSGP;
using WSGP.InputObject;
using Xunit;

namespace WSGPTests
{
    public class VytvorRezervaciPrvkuTests
    {
        WSGPCaller _caller;

        public VytvorRezervaciPrvkuTests()
        {
            _caller = new WSGPCaller(new DemoLogin());
        }

        [Fact]
        public void VytvorRezervaciPrvku_parcelni_cislo()
        {
            var res = _caller.Prvky.VytvorRezervaciParcely("22285597010", "733857",
                new RezervaceParcela { CisloZPMZ = "1605", DruhCislovaniPar = "2", PocetRezParcel = "1" });
            Assert.True(res.IsValid());
            Assert.Equal("733857", res.KatuzeKod);
            Assert.Equal("1605", res.CisloZPMZ);
            Assert.Equal("2", res.DruhCislovaniPar);
            Assert.Equal("3106", res.KmenoveCislo);
        }

        [Fact]
        public void VytvorRezervaciPrvku_poddeleni()
        {
            var res = _caller.Prvky.VytvorPoddeleni("22285597010", "733857",
                new RezervacePoddeleni
                {
                    CisloZPMZ = "1605",
                    DruhCislovaniPar = "2",
                    KmenoveCislo = "1531",
                    PocetRezPoddeleni = "1"
                });
            Assert.True(res.IsValid());
            Assert.Equal("733857", res.KatuzeKod);
            Assert.Equal("1605", res.CisloZPMZ);
            Assert.Equal("2", res.DruhCislovaniPar);
            Assert.Equal("1531", res.KmenoveCislo);
            Assert.Equal("31", res.PoddeleniCisla[0]);
        }

        [Fact]
        public void VytvorRezervaciPrvku_poddeleni2()
        {
            var res = _caller.Prvky.VytvorPoddeleni("50623895010", "628301",
                new RezervacePoddeleni
                {
                    CisloZPMZ = "328",
                    DruhCislovaniPar = "2",
                    KmenoveCislo = "198",
                    PocetRezPoddeleni = "2"
                });
            Assert.True(res.IsValid());
            Assert.Equal("628301", res.KatuzeKod);
            Assert.Equal("328", res.CisloZPMZ);
            Assert.Equal("2", res.DruhCislovaniPar);
            Assert.Equal("198", res.KmenoveCislo);
            Assert.Equal("23", res.PoddeleniCisla[0]);
            Assert.Equal("24", res.PoddeleniCisla[1]);
        }

        [Fact]
        public void VytvorRezervaciPrvku_BPPB()
        {
            var res = _caller.Prvky.VytvorPBPP("22285597010", "733857", new RezervacePBPP { PocetRezPBPP = "2" });
            Assert.True(res.IsValid());
            Assert.Equal("733857", res.KatuzeKod);
            Assert.Equal("502", res.CisloPBPP[0]);
            Assert.Equal("503", res.CisloPBPP[1]);
        }
    }
}
