using System;

namespace CAD.VFK
{
    class UnExpectException:Exception
    {
        public UnExpectException()
            :base("Unexpect exception, please contact software produce.")
        {
            
        }
    }
    class NotImplemented:Exception
    {
        public NotImplemented()
            :base("This future is not implemented.")
        {
            
        }
    }

    class ExportExcetiption : Exception
    {
        public ExportExcetiption(string value)
            : base(value)
        {

        }
    }

    class VfkImportException:Exception
    {
        public VfkImportException(string description)
            : base(description)
        {}
    }

}
