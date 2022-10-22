using WSGP;
using Xunit;

namespace WSGPTests
{
    public class ZrusRezervaciZPMZTests
    {
        WSGPCaller _caller;
        public ZrusRezervaciZPMZTests()
        {
            _caller = new WSGPCaller(new DemoLogin());
        }

        [Fact]
        public void ZrusRezervaciZPMZ()
        {
            var res = _caller.ZPMZ.ZrusRezervaci("22285597010", "1605", "733857");
            Assert.True(res.IsValid());
        }
    }
}
