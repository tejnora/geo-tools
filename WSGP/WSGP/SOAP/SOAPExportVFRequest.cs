using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WSGP.Base;
using WSGP.InputObject;

namespace WSGP.SOAP
{
   class SOAPExportVFRequest
      : ISOAPRqeuest
   {
      string _idRizeni;
      string _email;
      Path2D<Point2D> _path;
      VfExportType _exportTypes;

      public SOAPExportVFRequest(string idRizeni, string email, Path2D<Point2D> path, VfExportType exportTypes)
      {
         _idRizeni = idRizeni;
         _email = email;
         _path = path;
         _exportTypes = exportTypes;
      }
      public string SoapAction => "http://katastr.cuzk.cz/geows/exportVF";

      public string RawBody
      {
         get
         {
            var result = "<v1:ExportVFRequest>";
            result += $"<v1:idRizeni>{_idRizeni}</v1:idRizeni>";
            result += $"<v1:charset>ISO8859-2</v1:charset>";
            result += $"<v1:email>{_email}</v1:email>";
            result += "<v3:Polygon v3:id=\"_1950-10-04_10-00\" srsName=\"urn:ogc:def:crs:EPSG::5514\" srsDimension=\"2\">";
            result += "<v3:exterior>";
            result += "<v3:LinearRing>";
            result += "<v3:posList srsName=\"urn:ogc:def:crs:EPSG::5514\" srsDimension=\"2\">";
            foreach (var point in _path.Points)
            {
               result += string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} ", point.Y, point.X);
            }
            result += "</v3:posList>";
            result += "</v3:LinearRing>";
            result += "</v3:exterior>";
            result += "</v3:Polygon>";
            if ((_exportTypes & VfExportType.Nemo) == VfExportType.Nemo)
            {
               result += "<v1:skNemo>true</v1:skNemo>";
            }
            if ((_exportTypes & VfExportType.Bdpa) == VfExportType.Bdpa)
            {
               result += "<v1:skBdpa>true</v1:skBdpa>";
            }
            if ((_exportTypes & VfExportType.Vlst) == VfExportType.Vlst)
            {
               result += "<v1:skVlst>true</v1:skVlst>";
            }
            if ((_exportTypes & VfExportType.Jpvz) == VfExportType.Jpvz)
            {
               result += "<v1:skJpvz>true</v1:skJpvz>";
            }
            if ((_exportTypes & VfExportType.Pkmp) == VfExportType.Pkmp)
            {
               result += "<v1:skPkmp>true</v1:skPkmp>";
            }
            if ((_exportTypes & VfExportType.Bpej) == VfExportType.Bpej)
            {
               result += "<v1:skBpej>true</v1:skBpej>";
            }
            if ((_exportTypes & VfExportType.Gmpl) == VfExportType.Gmpl)
            {
               result += "<v1:skGmpl>true</v1:skGmpl>";
            }
            if ((_exportTypes & VfExportType.Reze) == VfExportType.Reze)
            {
               result += "<v1:skReze>true</v1:skReze>";
            }
            if ((_exportTypes & VfExportType.Debo) == VfExportType.Debo)
            {
               result += "<v1:skDebo>true</v1:skDebo>";
            }
            result += "</v1:ExportVFRequest>";
            return result;
         }
      }
      public ISOAPResponse CreateResponse(string content)
      {
         return new SOAPExportVFResponse(content);
      }
   }
}
/*
      <v1:idRizeni>22285603010</v1:idRizeni>
      <v1:charset>CP1250</v1:charset>
      <v1:email>demo5 @demo.dem</v1:email>
      <v2:Polygon v2:id= "_1950-10-04_10-00" srsName= "urn:ogc:def:crs:EPSG::5514" srsDimension= "2" >
        < v2:exterior>
          <v2:LinearRing>
            <v2:posList srsName = "urn:ogc:def:crs:EPSG::5514" srsDimension= "2" > -772322.55 - 1137163.14 - 770867.1 - 1137105.08 - 770898.2 - 1138083.68 - 772206.45 - 1137903.3 - 772322.55 - 1137163.14 </ v2:posList>
          </v2:LinearRing>
        </v2:exterior>
      </v2:Polygon>
      <v1:skNemo>true</v1:skNemo>
    </v1:ExportVFRequest>
    */
