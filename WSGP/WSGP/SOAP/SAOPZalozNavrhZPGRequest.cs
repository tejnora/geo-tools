using System.IO;
using WSGP.Extenstions;

namespace WSGP.SOAP
{
   class SAOPZalozNavrhZPGRequest
      : ISOAPRqeuest
   {

      Stream _xmlNavrhu;
      Stream[] _prilohyNavrhu;

      public SAOPZalozNavrhZPGRequest(Stream xmlNavrhu, Stream[] prilohyNavrhu)
      {
         _xmlNavrhu = xmlNavrhu;
         _prilohyNavrhu = prilohyNavrhu;
      }

      public string SoapAction => "zalozNavrhZPG";

      public string RawBody
      {
         get
         {
            var res = "<v1:ZalozNavrhZPGRequest>";
            res += $"<v1:XMLNavrhu>{_xmlNavrhu.ToBase64()}</v1:XMLNavrhu>";
            res += "<v1:hashNavrhu>11</v1:hashNavrhu>";
            for (var i = 0; i < _prilohyNavrhu.Length; i++)
            {
               res += $"<v1:prilohaNavrhu idPrilohy=\"{i + 1}\">";
               res += "<v1:prilohaNavrhu>";
               res += _prilohyNavrhu[i].ToBase64();
               res += "</v1:prilohaNavrhu>";
               res += "</v1:prilohaNavrhu>";
            }
            res += "</v1:ZalozNavrhZPGRequest>";
            return res;
         }

      }

      public ISOAPResponse CreateResponse(string content)
      {
         return new SAOPZalozNavrhZPGResponse(content);
      }
   }
}
