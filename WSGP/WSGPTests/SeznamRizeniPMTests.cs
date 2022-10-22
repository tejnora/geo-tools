using System.Linq;
using WSGP;
using Xunit;

namespace WSGPTests
{
    public class SeznamRizeniPMTests
    {
        WSGPCaller _caller;
        public SeznamRizeniPMTests()
        {
            _caller = new WSGPCaller(new DemoLogin());
        }

        [Fact]
        public void SeznamRizeniPM()
        {
            var res = _caller.Rizeni.Seznam();
            Assert.Equal(37, res.RizeniPMList.Count());
        }

    }
}
