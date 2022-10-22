using System.IO;
using WSGP;
using Xunit;

namespace WSGPTests
{
   public class ZPGTests
   {
      WSGPCaller _caller;
      public ZPGTests()
      {
         _caller = new WSGPCaller(new DemoLogin());
      }

      [Fact]
      public void Zaloz()
      {
         var startupPath = System.IO.Directory.GetCurrentDirectory();
         var dir = Path.Combine(Directory.GetParent(startupPath).Parent.Parent.FullName, "Data");
         var xmlNavrhu = File.OpenRead(Path.Combine(dir, "ZPGgen1438.xml"));
         var prilohy = new Stream[3];
         prilohy[0] = File.OpenRead(Path.Combine(dir, "700321_GP.pdf"));
         prilohy[1] = File.OpenRead(Path.Combine(dir, "700321_ZPMZ.zip"));
         prilohy[2] = File.OpenRead(Path.Combine(dir, "700321_Zadost.pdf"));
         var res = _caller.ZPG.ZalozNavrh(xmlNavrhu, prilohy);
         Assert.False(res.IsValid());
         Assert.Equal("436", res.Vysledek.Kod);

      }

   }
}
