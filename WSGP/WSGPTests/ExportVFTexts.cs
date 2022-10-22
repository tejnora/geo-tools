using System;
using System.Collections.Generic;
using System.Text;
using WSGP;
using WSGP.Base;
using WSGP.InputObject;
using Xunit;

namespace WSGPTests
{
   public class ExportVFTexts
   {
      public ExportVFTexts()
      {
      }

      [Fact]
      public void Ok_ID_CHARSET()
      {
         var caller = new WSGPCaller(new DemoLogin());
         var points = new Path2D<Point2D>
         {
            Points = new List<Point2D>
            {
               new Point2D() {Y = -764828.6, X = -1033878.23},
               new Point2D() {Y = -764928.6, X = -1033878.23},
               new Point2D() {Y = -764928.6, X = -1033778.23},
               new Point2D() {Y = -764828.6, X = -1033778.23},
               new Point2D() {Y = -764828.6, X = -1033878.23}
            }
         };
         var response = caller.ExportVF.Export("50623878010", "demo3@demo.dem", points, VfExportType.Nemo);
         Assert.True(response.IsValid());
         Assert.Equal("134003906010", response.BehId);
      }

      [Fact]
      public void Ok_ID_CHARSET1()
      {
         var caller = new WSGPCaller(new Demo1Login());
         var points = new Path2D<Point2D>
         {
            Points = new List<Point2D>
            {
               new Point2D() {Y = -764828.6, X = -1033878.23},
               new Point2D() {Y = -764928.6, X = -1033878.23},
               new Point2D() {Y = -764928.6, X = -1033778.23},
               new Point2D() {Y = -764828.6, X = -1033778.23},
               new Point2D() {Y = -764828.6, X = -1033878.23}
            }
         };
         var response = caller.ExportVF.Export("50623878010", "cervena@cuzk.cz", points, VfExportType.Nemo);
         Assert.True(response.IsValid());
         Assert.Equal("134003906010", response.BehId);
      }

   }
}
