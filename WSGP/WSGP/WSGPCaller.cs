using System;
using System.Text;
using System.Net.Http;
using WSGP.SOAP;
using WSGP.OutputObject;
using System.Collections.Generic;
using System.IO;
using WSGP.Base;
using WSGP.InputObject;

namespace WSGP
{
   public class WSGPCaller
   {

      HttpClient _client;
      ILogin _login;

      public WSGPCaller(ILogin login)
      {
         _login = login;
         _client = new HttpClient();
         _client.BaseAddress = new Uri("https://katastr.cuzk.cz/");
      }

      static string EnvelopBegin =
          "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
              "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:v1=\"http://katastr.cuzk.cz/geows/types/v2.2\" xmlns:v2=\"http://katastr.cuzk.cz/commonTypes/v2.2\" xmlns:v3=\"http://www.opengis.net/gml/3.2\">";
      static string EnvelopEnd = "</soapenv:Envelope>";
      static string HeaderBegin =
              "<soapenv:Header>" +
                  "<wsse:Security xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">";
      static string HeaderEnd =
                  "</wsse:Security>" +
              "</soapenv:Header>";


      string CreateHeader()
      {
         return HeaderBegin +
                 $"<wsse:UsernameToken wsu:Id=\"UsernameToken-{Guid.NewGuid()}\">" +
                     $"<wsse:Username>{_login.UserName}</wsse:Username>" +
                     $"<wsse:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText\">{_login.Password}</wsse:Password>" +
                 "</wsse:UsernameToken>" +
                 HeaderEnd;
      }

      string CreateBody(ISOAPRqeuest rqeuest)
      {
         return "<soapenv:Body>" + rqeuest.RawBody + "</soapenv:Body>";
      }

      string CreateEvelope(ISOAPRqeuest reqeuest)
      {
         return EnvelopBegin + CreateHeader() + CreateBody(reqeuest) + EnvelopEnd;
      }

      T CallSOAPService<T>(ISOAPRqeuest request)
      {
         var soapContent = CreateEvelope(request);
         //File.WriteAllText(@"c:\Temp\test1.xml",soapContent);
         //soapContent = File.ReadAllText(@"c:\Temp\test.xml");
         var httpRequest = new HttpRequestMessage(HttpMethod.Post, "trial/ws/geo/2.2/geo");
         httpRequest.Content = new StringContent(soapContent, Encoding.UTF8, "text/xml");
         httpRequest.Headers.Add("SOAPAction", request.SoapAction);
         var response = _client.SendAsync(httpRequest);
         var content = response.Result.Content;
         var responseContent = content.ReadAsStringAsync().Result;
         return (T)request.CreateResponse(responseContent);
      }

      static Vysledek CreateVysledekFromException(Exception exception)
      {
         return new Vysledek() { Kod = "-1", Uroven = "EXCEPTION", Zprava = exception.Message };
      }

      public class RizeniNamespace
      {
         readonly WSGPCaller _caller;
         public RizeniNamespace(WSGPCaller caller)
         {
            _caller = caller;
         }

         public SeznamRizeniPM Seznam()
         {
            try
            {
               var request = new SOAPSeznamRizeniPMRequest();
               var response = _caller.CallSOAPService<SOAPSeznamRizeniPMResponse>(request);
               return response.CreateDataObject();
            }
            catch (Exception ex)
            {
               return new SeznamRizeniPM { Vysledek = CreateVysledekFromException(ex) };
            }
         }
         public DetailRizeni Detail(string idRizeni)
         {
            try
            {
               var request = new ISOAPDetailRizeniPMRequest(idRizeni, true);
               var response = _caller.CallSOAPService<ISOAPDetailRizeniPMResponse>(request);
               return response.CreateDataObject();
            }
            catch (Exception ex)
            {
               return new DetailRizeni { Vysledek = CreateVysledekFromException(ex) };
            }
         }

         public RizeniPM Zalozit(string cisloZakazky, IEnumerable<string> katuzeKodList, IEnumerable<string> dotceneParcely)
         {
            try
            {
               var request = new SOAPZalozRizeniPMRequest(cisloZakazky, katuzeKodList, dotceneParcely);
               var response = _caller.CallSOAPService<SOAPZalozRizeniPMResponse>(request);
               return response.CreateDataObject();
            }
            catch (Exception ex)
            {
               return new RizeniPM { Vysledek = CreateVysledekFromException(ex) };
            }
         }

         public Success MylneRizeni(string idRizeniPM, string popis)
         {
            try
            {
               var request = new SOAPMylneRizeniPMRequest(idRizeniPM, popis);
               var response = _caller.CallSOAPService<SOAPMylneRizeniPMResponse>(request);
               return response.CreateDataObject();
            }
            catch (Exception ex)
            {
               return new Success { Vysledek = CreateVysledekFromException(ex) };
            }
         }

      }

      RizeniNamespace _rizeni;
      public RizeniNamespace Rizeni
      {
         get
         {
            if (_rizeni != null) return _rizeni;
            lock (this)
            {
               if (_rizeni != null) return _rizeni;
               _rizeni = new RizeniNamespace(this);
               return _rizeni;
            }

         }
      }

      public class ZPMZNamespace
      {
         readonly WSGPCaller _caller;

         public ZPMZNamespace(WSGPCaller caller)
         {
            _caller = caller;
         }


         public RezervCisloZPMZ VytvorRezervaci(string idRizeniPM, string katuzeKod)
         {
            try
            {
               var request = new SOAPRezervaceZPMZRequest(idRizeniPM, katuzeKod);
               var response = _caller.CallSOAPService<SOAPRezervaceZPMZResponse>(request);
               return response.CreateDataObject();
            }
            catch (Exception ex)
            {
               return new RezervCisloZPMZ { Vysledek = CreateVysledekFromException(ex) };
            }
         }

         public Success ZrusRezervaci(string idRizeniPM, string cisloZPMZ, string katuzeKod)
         {
            try
            {
               var request = new SOAPZrusRezervaciZPMZRequest(idRizeniPM, cisloZPMZ, katuzeKod);
               var response = _caller.CallSOAPService<SOAPZrusRezervaciZPMZResponse>(request);
               return response.CreateDataObject();
            }
            catch (Exception ex)
            {
               return new Success { Vysledek = CreateVysledekFromException(ex) };
            }
         }
      }

      ZPMZNamespace _zpmz;
      public ZPMZNamespace ZPMZ
      {
         get
         {
            if (_zpmz != null) return _zpmz;
            lock (this)
            {
               if (_zpmz != null) return _zpmz;
               _zpmz = new ZPMZNamespace(this);
               return _zpmz;
            }
         }
      }


      public class PrvkyNamespace
      {
         readonly WSGPCaller _caller;

         public PrvkyNamespace(WSGPCaller caller)
         {
            _caller = caller;
         }

         public RezervParcely VytvorRezervaciParcely(string idRizeniPM, string katuzeKod, RezervaceParcela rezervaceParcela)
         {
            try
            {
               var request = new SOAPVytvorRezervaciPrvkuRequest(idRizeniPM, katuzeKod, rezervaceParcela);
               var response = _caller.CallSOAPService<SOAPVytvorRezervaciPrvkuResponse>(request);
               return (RezervParcely)response.CreateDataObject();
            }
            catch (Exception ex)
            {
               return new RezervParcely { Vysledek = CreateVysledekFromException(ex) };
            }
         }

         public RezervPoddeleni VytvorPoddeleni(string idRizeniPM, string katuzeKod, RezervacePoddeleni rezervacePoddeleni)
         {
            try
            {
               var request = new SOAPVytvorRezervaciPrvkuRequest(idRizeniPM, katuzeKod, rezervacePoddeleni);
               var response = _caller.CallSOAPService<SOAPVytvorRezervaciPrvkuResponse>(request);
               return (RezervPoddeleni)response.CreateDataObject();
            }
            catch (Exception ex)
            {
               return new RezervPoddeleni { Vysledek = CreateVysledekFromException(ex) };
            }
         }

         public RezervBodyPBPP VytvorPBPP(string idRizeniPM, string katuzeKod, RezervacePBPP rezervacePBPP)
         {
            try
            {
               var request = new SOAPVytvorRezervaciPrvkuRequest(idRizeniPM, katuzeKod, rezervacePBPP);
               var response = _caller.CallSOAPService<SOAPVytvorRezervaciPrvkuResponse>(request);
               return (RezervBodyPBPP)response.CreateDataObject();
            }
            catch (Exception ex)
            {
               return new RezervBodyPBPP { Vysledek = CreateVysledekFromException(ex) };
            }
         }
      }

      PrvkyNamespace _prvky;
      public PrvkyNamespace Prvky
      {
         get
         {
            if (_prvky != null) return _prvky;
            lock (this)
            {
               if (_prvky != null) return _prvky;
               _prvky = new PrvkyNamespace(this);
               return _prvky;
            }
         }
      }

      public class ExportVFNamespace
      {
         readonly WSGPCaller _caller;

         public ExportVFNamespace(WSGPCaller caller)
         {
            _caller = caller;
         }

         public ExportVF Export(string idRizeni, string email, Path2D<Point2D> path, VfExportType exportTypes)
         {
            try
            {
               var request = new SOAPExportVFRequest(idRizeni, email, path, exportTypes);
               var response = _caller.CallSOAPService<SOAPExportVFResponse>(request);
               return (ExportVF)response.CreateDataObject();
            }
            catch (Exception ex)
            {
               return new ExportVF { Vysledek = CreateVysledekFromException(ex) };
            }
         }
      }

      ExportVFNamespace __exportVF;
      public ExportVFNamespace ExportVF
      {
         get
         {
            if (__exportVF != null) return __exportVF;
            lock (this)
            {
               if (__exportVF != null) return __exportVF;
               __exportVF = new ExportVFNamespace(this);
               return __exportVF;
            }
         }
      }

      public class ZPGNamespace
      {
         readonly WSGPCaller _caller;

         public ZPGNamespace(WSGPCaller caller)
         {
            _caller = caller;
         }

         public Success ZalozNavrh(Stream xmlNavrhu, Stream[] prilohyNavrhu)
         {
            try
            {
               var request = new SAOPZalozNavrhZPGRequest(xmlNavrhu, prilohyNavrhu);
               var response = _caller.CallSOAPService<SAOPZalozNavrhZPGResponse>(request);
               return (Success)response.CreateDataObject();
            }
            catch (Exception ex)
            {
               return new Success { Vysledek = CreateVysledekFromException(ex) };
            }
         }
      }

      ZPGNamespace __zpg;
      public ZPGNamespace ZPG
      {
         get
         {
            if (__zpg != null) return __zpg;
            lock (this)
            {
               if (__zpg != null) return __zpg;
               __zpg = new ZPGNamespace(this);
               return __zpg;
            }
         }
      }

   }

}

