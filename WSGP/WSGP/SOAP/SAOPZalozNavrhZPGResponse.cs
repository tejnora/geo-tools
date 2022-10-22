using System;
using System.Collections.Generic;
using System.Text;
using WSGP.OutputObject;

namespace WSGP.SOAP
{
   class SAOPZalozNavrhZPGResponse
      : ISOAPResponseBase
   {
      public SAOPZalozNavrhZPGResponse(string xmlContent) : base(xmlContent)
      {
      }

      public Success CreateDataObject()
      {
         var res = new Success();
         var detailRizeniPMXmlElement = ParseBody("ZalozNavrhZPGResponse");
         res.Vysledek = CreateVysledek(detailRizeniPMXmlElement);
         //if (!res.IsValid())
         return res;
      }
   }
}
