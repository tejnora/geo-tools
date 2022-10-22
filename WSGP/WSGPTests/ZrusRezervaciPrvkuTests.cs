using System;
using System.Collections.Generic;
using System.Text;
using WSGP;

namespace WSGPTests
{
    class ZrusRezervaciPrvkuTests
    {
        WSGPCaller _caller;

        public ZrusRezervaciPrvkuTests()
        {
            _caller = new WSGPCaller(new DemoLogin());
        }


    }
}
