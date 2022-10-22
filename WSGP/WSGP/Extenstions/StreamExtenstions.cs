using System;
using System.IO;

namespace WSGP.Extenstions
{
   public static class StreamExtenstions
   {
      public static string ToBase64(this Stream stream)
      {
         stream.Seek(0, SeekOrigin.Begin);
         byte[] bytes;
         using (var memoryStream = new MemoryStream())
         {
            stream.CopyTo(memoryStream);
            bytes = memoryStream.ToArray();
         }
         var base64 = Convert.ToBase64String(bytes);
         return base64;
      }
   }
}
