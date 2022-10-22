using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using WSGP.OutputObject;

namespace WSGP.SOAP
{
    abstract class ISOAPResponseBase
        : ISOAPResponse
    {
        readonly string _xmlContent;
        public static XNamespace ns0 = "http://katastr.cuzk.cz/geows/types/v2.2";
        public static XNamespace ns1 = "http://katastr.cuzk.cz/commonTypes/v2.2";
        public static XNamespace nsS = "http://schemas.xmlsoap.org/soap/envelope/";

        protected ISOAPResponseBase(string xmlContent)
        {
            _xmlContent = xmlContent;
        }

        protected IEnumerable<XElement> ParseBody(string bodyElementName)
        {
            XDocument xmlDocument = XDocument.Parse(_xmlContent);

            var responseData = xmlDocument
                .Elements()
                .Elements(nsS + "Body")
                .Elements(ns0 + bodyElementName)
                .First()
                .Elements();
            return responseData;
        }

        protected Vysledek CreateVysledek(IEnumerable<XElement> xmlBodyElement)
        {
            var vysledekXmlElement = xmlBodyElement.ElementAt(0);
            var zpravaXmlElement = vysledekXmlElement.Element(ns1 + "zprava");
            var vysledek = new Vysledek
            {
                Kod = zpravaXmlElement.Attribute("kod").Value,
                Uroven = zpravaXmlElement.Attribute("uroven").Value,
                Zprava = zpravaXmlElement.Value
            };
            return vysledek;
        }
    }
}
