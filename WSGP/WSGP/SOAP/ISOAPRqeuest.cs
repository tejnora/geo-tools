namespace WSGP.SOAP
{
    interface ISOAPRqeuest
    {
        string SoapAction { get; }
        string RawBody { get; }
        ISOAPResponse CreateResponse(string content);
    }
}
