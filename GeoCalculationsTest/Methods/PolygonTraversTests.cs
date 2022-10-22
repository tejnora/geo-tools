using System;
using System.Collections.ObjectModel;
using GeoCalculations.CalculationContexts;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoCalculations.Points;
using NUnit.Framework;

namespace GeoCalculationsTest.Methods
{
    [TestFixture]
    class PolygonTraversTests
    {
        [Test]
        public void VolnyPolygonovyPorad()
        {
            var vychoziBod = new PointBaseEx();
            var vychoziBodOrientace = new OrientationContext();
            var merenaData = new ObservableCollection<MeasuredPoint>();
            vychoziBod.Number = "4007";
            vychoziBod.X = 1070517.76;
            vychoziBod.Y = 818095.44;
            vychoziBodOrientace.TableNodes.Add(new OrientationPoint { Number = "4004", Y = 818277.21, X = 1070443.97, Hz = 100 });
            var bod = new MeasuredPoint
                          {
                              PointOfView = { Number = "4007" },
                              MeasuringForward = new OrientationPoint { Hz = 295.418, Distance = 130.392 }
                          };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4010" },
                          MeasuringForward = new OrientationPoint { Hz = 236.8006, Distance = 151.040 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 130.392 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4005" },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 151.040 }
                      };
            merenaData.Add(bod);
            var result = new PolygonCalculatedPoints();
            var calculationContext = new PolygonTraverseContext
                {
                    BeginPoint = vychoziBod,
                    BeginOrientationContext = vychoziBodOrientace,
                    EndPoint = new PointBaseEx(),
                    EndOrientationContext = new OrientationContext(),
                    MeasuredContext = new MeasuredPointsContext { TableNodes = merenaData }
                };
            PolygonTraverseMethod.PolygonTraverse(calculationContext, ref result);
            Assert.AreEqual(result.PolygonTraverseType, PolygonTraverseTypes.Free);
            Assert.AreEqual(Math.Round(result.Nodes[1].X, 2), Math.Round(1070557.99, 2));
            Assert.AreEqual(Math.Round(result.Nodes[1].Y, 2), Math.Round(817971.41, 2));
            Assert.AreEqual(Math.Round(result.Nodes[2].X, 2), Math.Round(1070675.52, 2));
            Assert.AreEqual(Math.Round(result.Nodes[2].Y, 2), Math.Round(817876.54, 2));
        }
        [Test]
        public void PolygonovyPoradVetknuty()
        {
            var vychoziBod = new PointBaseEx();
            var koncovyBod = new PointBaseEx();
            var merenaData = new ObservableCollection<MeasuredPoint>();
            vychoziBod.Number = "4007";
            vychoziBod.X = 1070517.76;
            vychoziBod.Y = 818095.44;
            koncovyBod.Number = "4011";
            koncovyBod.X = 1070693.61;
            koncovyBod.Y = 817740.62;
            var bod = new MeasuredPoint
                          {
                              PointOfView = { Number = "4007" },
                              MeasuringForward = new OrientationPoint { Hz = 0, Distance = 130.392 }
                          };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4010" },
                          MeasuringForward = new OrientationPoint { Hz = 236.8006, Distance = 151.040 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 130.392 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4005" },
                          MeasuringForward = new OrientationPoint { Hz = 151.6539, Distance = 137.122 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 151.040 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4011" },
                          MeasuringForward = new OrientationPoint(),
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 137.122 }
                      };
            merenaData.Add(bod);
            var result = new PolygonCalculatedPoints();
            var calculationContext = new PolygonTraverseContext
            {
                BeginPoint = vychoziBod,
                BeginOrientationContext = new OrientationContext(),
                EndPoint = koncovyBod,
                EndOrientationContext = new OrientationContext(),
                MeasuredContext = new MeasuredPointsContext { TableNodes = merenaData }
            };

            PolygonTraverseMethod.PolygonTraverse(calculationContext, ref result);
            Assert.AreEqual(result.PolygonTraverseType, PolygonTraverseTypes.Fixed);
            Assert.AreEqual(Math.Round(result.Nodes[0].X, 2), Math.Round(1070517.76, 2));
            Assert.AreEqual(Math.Round(result.Nodes[0].Y, 2), Math.Round(818095.44, 2));
            Assert.AreEqual(Math.Round(result.Nodes[1].X, 2), Math.Round(1070557.99, 2));
            Assert.AreEqual(Math.Round(result.Nodes[1].Y, 2), Math.Round(817971.41, 2));
            Assert.AreEqual(Math.Round(result.Nodes[2].X, 2), Math.Round(1070675.52, 2));
            Assert.AreEqual(Math.Round(result.Nodes[2].Y, 2), Math.Round(817876.54, 2));
            Assert.AreEqual(Math.Round(result.Nodes[3].X, 2), Math.Round(1070693.61, 2));
            Assert.AreEqual(Math.Round(result.Nodes[3].Y, 2), Math.Round(817740.62, 2));
            Assert.AreEqual(Math.Round(result.LocationDeviation, 4), Math.Round(0.0006, 4));
        }
        [Test]
        public void UzavrenyPolygonovyPorad()
        {
            var vychoziBod = new PointBaseEx();
            var koncovyBod = new PointBaseEx();
            var vychoziBodOrientace = new OrientationContext();
            var koncovyBodOrientace = new OrientationContext();
            var merenaData = new ObservableCollection<MeasuredPoint>();
            vychoziBod.Number = "4013";
            vychoziBod.X = 1070673.68;
            vychoziBod.Y = 818122.04;
            vychoziBodOrientace.TableNodes.Add(new OrientationPoint { Number = "4003", Y = 818318.08, X = 1070650.87, Hz = 214.3251 });
            vychoziBodOrientace.TableNodes.Add(new OrientationPoint { Number = "4007", Y = 818095.44, X = 1070517.76, Hz = 317.7088 });
            koncovyBod.Number = "4013";
            koncovyBod.X = 1070673.68;
            koncovyBod.Y = 818122.04;
            koncovyBodOrientace.TableNodes.Add(new OrientationPoint { Number = "4003", Y = 818318.08, X = 1070650.87, Hz = 214.3251 });
            koncovyBodOrientace.TableNodes.Add(new OrientationPoint { Number = "4007", Y = 818095.44, X = 1070517.76, Hz = 317.7088 });
            var bod = new MeasuredPoint
                          {
                              PointOfView = { Number = "4013" },
                              MeasuringForward = new OrientationPoint { Hz = 379.017, Distance = 151.601 }
                          };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4006" },
                          MeasuringForward = new OrientationPoint { Hz = 144.1966, Distance = 52.99 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 151.601 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4010" },
                          MeasuringForward = new OrientationPoint { Hz = 232.1796, Distance = 60.270 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 52.99 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4001" },
                          MeasuringForward = new OrientationPoint { Hz = 262.7230, Distance = 99.722 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 60.270 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4009" },
                          MeasuringForward = new OrientationPoint { Hz = 256.2208, Distance = 185.724 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 99.722 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4011" },
                          MeasuringForward = new OrientationPoint { Hz = 341.0385, Distance = 137.122 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 185.724 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4005" },
                          MeasuringForward = new OrientationPoint { Hz = 192.0574, Distance = 245.502 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 137.122 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4013" },
                          MeasuringForward = new OrientationPoint(),
                          MeasuringBack = new OrientationPoint { Hz = 7.4329, Distance = 245.502 }
                      };
            merenaData.Add(bod);
            var result = new PolygonCalculatedPoints();
            var calculationContext = new PolygonTraverseContext
            {
                BeginPoint = vychoziBod,
                BeginOrientationContext = vychoziBodOrientace,
                EndPoint = koncovyBod,
                EndOrientationContext = koncovyBodOrientace,
                MeasuredContext = new MeasuredPointsContext { TableNodes = merenaData }
            };

            PolygonTraverseMethod.PolygonTraverse(calculationContext, ref result);
            Assert.AreEqual(Math.Round(result.Nodes[0].X, 2), Math.Round(result.Nodes[result.Nodes.Count - 1].X, 2));
            Assert.AreEqual(Math.Round(result.Nodes[0].Y, 2), Math.Round(result.Nodes[result.Nodes.Count - 1].Y, 2));
            Assert.AreEqual(Math.Round(result.LocationDeviation, 5), Math.Round(0.00681, 5));
            Assert.AreEqual(result.PolygonTraverseType, PolygonTraverseTypes.Closure);
        }
        [Test]
        public void OboustrannePripojenyJednostranneOrientovany()
        {
            var vychoziBod = new PointBaseEx();
            var koncovyBod = new PointBaseEx();
            var vychoziBodOrientace = new OrientationContext();
            var merenaData = new ObservableCollection<MeasuredPoint>();
            vychoziBod.Number = "4007";
            vychoziBod.X = 1070517.76;
            vychoziBod.Y = 818095.44;
            vychoziBodOrientace.TableNodes.Add(new OrientationPoint { Number = "4004", Y = 818277.21, X = 1070443.97, Hz = 100.00 });
            koncovyBod.Number = "4011";
            koncovyBod.X = 1070693.61;
            koncovyBod.Y = 817740.62;
            var bod = new MeasuredPoint
                          {
                              PointOfView = { Number = "4007" },
                              MeasuringForward = new OrientationPoint { Hz = 295.418, Distance = 130.392 }
                          };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4010" },
                          MeasuringForward = new OrientationPoint { Hz = 128.4723, Distance = 60.270 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 130.392 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4001" },
                          MeasuringForward = new OrientationPoint { Hz = 262.723, Distance = 99.722 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 60.270 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4009" },
                          MeasuringForward = new OrientationPoint { Hz = 256.2208, Distance = 185.724 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 99.722 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4011" },
                          MeasuringForward = new OrientationPoint(),
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 185.724 }
                      };
            merenaData.Add(bod);
            var result = new PolygonCalculatedPoints();
            var calculationContext = new PolygonTraverseContext
            {
                BeginPoint = vychoziBod,
                BeginOrientationContext = vychoziBodOrientace,
                EndPoint = koncovyBod,
                EndOrientationContext = new OrientationContext(),
                MeasuredContext = new MeasuredPointsContext { TableNodes = merenaData }
            };

            PolygonTraverseMethod.PolygonTraverse(calculationContext, ref result);
            Assert.AreEqual(result.PolygonTraverseType, PolygonTraverseTypes.MutuallyConnectedAndOneSideFixed);
            Assert.AreEqual(Math.Round(result.Nodes[0].X, 2), Math.Round(1070517.76, 2));
            Assert.AreEqual(Math.Round(result.Nodes[0].Y, 2), Math.Round(818095.44, 2));
            Assert.AreEqual(Math.Round(result.Nodes[1].X, 2), Math.Round(1070557.99, 2));
            Assert.AreEqual(Math.Round(result.Nodes[1].Y, 2), Math.Round(817971.41, 2));
            Assert.AreEqual(Math.Round(result.Nodes[2].X, 2), Math.Round(1070514.34, 2));
            Assert.AreEqual(Math.Round(result.Nodes[2].Y, 2), Math.Round(817929.85, 2));
            Assert.AreEqual(Math.Round(result.Nodes[3].X, 2), Math.Round(1070531.74, 2));
            Assert.AreEqual(Math.Round(result.Nodes[3].Y, 2), Math.Round(817831.66, 2));
            Assert.AreEqual(Math.Round(result.Nodes[4].X, 2), Math.Round(1070693.61, 2));
            Assert.AreEqual(Math.Round(result.Nodes[4].Y, 2), Math.Round(817740.62, 2));
            Assert.AreEqual(Math.Round(result.LocationDeviation, 5), Math.Round(0.010063217177258759, 5));
        }
        [Test]
        public void OboustrannePripojenyOboustranneOrientovany()
        {
            var vychoziBod = new PointBaseEx();
            var koncovyBod = new PointBaseEx();
            var vychoziBodOrientace = new OrientationContext();
            var koncovyBodOrientace = new OrientationContext();
            var merenaData = new ObservableCollection<MeasuredPoint>();
            vychoziBod.Number = "4013";
            vychoziBod.X = 1070673.68;
            vychoziBod.Y = 818122.04;
            vychoziBodOrientace.TableNodes.Add(new OrientationPoint { Number = "4003", Y = 818318.08, X = 1070650.87, Hz = 300.00 });
            koncovyBod.Number = "4011";
            koncovyBod.X = 1070693.61;
            koncovyBod.Y = 817740.62;
            koncovyBodOrientace.TableNodes.Add(new OrientationPoint { Number = "4012", Y = 817648.60, X = 1070776.00, Hz = 0 });
            var bod = new MeasuredPoint
                          {
                              PointOfView = { Number = "4013" },
                              MeasuringForward = new OrientationPoint { Hz = 64.6920, Distance = 151.601 }
                          };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4006" },
                          MeasuringForward = new OrientationPoint { Hz = 144.19660, Distance = 52.99 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 151.601 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4010" },
                          MeasuringForward = new OrientationPoint { Hz = 232.17960, Distance = 60.270 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 52.99 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4001" },
                          MeasuringForward = new OrientationPoint { Hz = 262.7230, Distance = 99.722 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 60.270 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4009" },
                          MeasuringForward = new OrientationPoint { Hz = 256.22080, Distance = 185.724 },
                          MeasuringBack = new OrientationPoint { Hz = 0.0, Distance = 99.722 }
                      };
            merenaData.Add(bod);
            bod = new MeasuredPoint
                      {
                          PointOfView = { Number = "4009" },
                          MeasuringForward = new OrientationPoint(),
                          MeasuringBack = new OrientationPoint { Hz = 220.894, Distance = 185.724 }
                      };
            merenaData.Add(bod);
            var result = new PolygonCalculatedPoints();
            var calculationContext = new PolygonTraverseContext
            {
                BeginPoint = vychoziBod,
                BeginOrientationContext = vychoziBodOrientace,
                EndPoint = koncovyBod,
                EndOrientationContext = koncovyBodOrientace,
                MeasuredContext = new MeasuredPointsContext { TableNodes = merenaData }
            };

            PolygonTraverseMethod.PolygonTraverse(calculationContext, ref result);
            Assert.AreEqual(result.PolygonTraverseType, PolygonTraverseTypes.MutuallyConnectedAndMutuallyFixed);
            Assert.AreEqual(Math.Round(result.Nodes[0].X, 2), Math.Round(1070673.68, 2));
            Assert.AreEqual(Math.Round(result.Nodes[0].Y, 2), Math.Round(818122.04, 2));
            Assert.AreEqual(Math.Round(result.Nodes[1].X, 2), Math.Round(1070609.27, 2));
            Assert.AreEqual(Math.Round(result.Nodes[1].Y, 2), Math.Round(817984.80, 2));
            Assert.AreEqual(Math.Round(result.Nodes[2].X, 2), Math.Round(1070558.00, 2));
            Assert.AreEqual(Math.Round(result.Nodes[2].Y, 2), Math.Round(817971.42, 2));
            Assert.AreEqual(Math.Round(result.Nodes[3].X, 2), Math.Round(1070514.35, 2));
            Assert.AreEqual(Math.Round(result.Nodes[3].Y, 2), Math.Round(817929.86, 2));
            Assert.AreEqual(Math.Round(result.Nodes[4].X, 2), Math.Round(1070531.74, 2));
            Assert.AreEqual(Math.Round(result.Nodes[4].Y, 2), Math.Round(817831.66, 2));
            Assert.AreEqual(Math.Round(result.Nodes[5].X, 2), Math.Round(1070693.61, 2));
            Assert.AreEqual(Math.Round(result.Nodes[5].Y, 2), Math.Round(817740.62, 2));
        }
        private bool _vyskovyVypocet;
        public void SettingCallback(string paramName, string value)
        {
            switch (paramName)
            {
                case "Type":
                    {
                        if (value == "3D")
                            _vyskovyVypocet = true;
                    } break;
            }
        }
        [Test]
        public void NativeTests()
        {
            return;//todo
            /*Singletons.MyRegistry.SuppressRegistry = true;
            const string direcotryPath = @"c:\geo\GeoHelperUnitTests\TestFiles";
            var polFiles = Directory.GetFiles(direcotryPath, "*.pol");
            foreach (var file in polFiles)
            {
                try
                {
                    var beginPoint = new PointOfView();
                    var vychoziBodOrientace = new OrientationPoint(null);
                    var koncovyBod = new PointOfView();
                    var koncovyBodOrientace = new OrientationPoint(null);
                    var merenaData = new ObservableCollection<MeasuredPoint>();
                    var importer = new VypoctyImport();
                    importer.SettingCallback = SettingCallback;
                    importer.ImportFromFile(file, beginPoint, vychoziBodOrientace, koncovyBod, koncovyBodOrientace, merenaData);
                    var result = VypoctyEngine.PolygonTraverse(beginPoint, vychoziBodOrientace, koncovyBod, koncovyBodOrientace, merenaData);
                    if (_vyskovyVypocet)
                        result = VypoctyEngine.PolygonTraversHeight(result, beginPoint, koncovyBod, merenaData);
                    var context = new PolygonovyPoradProtokolContext(result, beginPoint, vychoziBodOrientace, koncovyBod,
                                                       koncovyBodOrientace, merenaData, _vyskovyVypocet);
                    var lines = context.GetLines();
                    string refFile = direcotryPath + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(file) + ".prot";
                    var refLines = File.ReadAllLines(refFile);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        var l = lines[i];
                        var rl = refLines[i];
                        if (l.StartsWith("Typ pořadu"))
                            continue;//net resources here
                        if (l != rl)
                        {
                            Assert.AreEqual(l, rl);
                        }
                    }
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }

            }*/
        }
        [Test]
        public void ValidateVyska()
        {
            /* object obj = (double)UnitTestUtilities.Helper.RunStaticMethod(typeof(VypoctyEngine), "ValidateVyska", new object[] { 0.0 });
             Assert.AreEqual(obj, 0.0);
             obj = (double)UnitTestUtilities.Helper.RunStaticMethod(typeof(VypoctyEngine), "ValidateVyska", new object[] { 300.0 });
             Assert.AreEqual(obj, 100.0);
             obj = (double)UnitTestUtilities.Helper.RunStaticMethod(typeof(VypoctyEngine), "ValidateVyska", new object[] { 250.0 });
             Assert.AreEqual(obj, 150.0);
             obj = (double)UnitTestUtilities.Helper.RunStaticMethod(typeof(VypoctyEngine), "ValidateVyska", new object[] { 790.0 });
             Assert.AreEqual(obj, 10);*/
        }
        [Test]
        public void CalckVyska()
        {
            /*object obj = (double)UnitTestUtilities.Helper.RunStaticMethod(typeof(VypoctyEngine), "CalckVyska", new object[] { 20.0, 100.0 });
            Assert.AreEqual(Math.Round((double)obj, 5), 307.76835);
            obj = (double)UnitTestUtilities.Helper.RunStaticMethod(typeof(VypoctyEngine), "CalckVyska", new object[] { 180.0, 100.0 });
            Assert.AreEqual(Math.Round((double)obj, 5), -307.76835);
            obj = (double)UnitTestUtilities.Helper.RunStaticMethod(typeof(VypoctyEngine), "CalckVyska", new object[] { 220.0, 100.0 });
            Assert.AreEqual(Math.Round((double)obj, 5), -307.76835);
            obj = (double)UnitTestUtilities.Helper.RunStaticMethod(typeof(VypoctyEngine), "CalckVyska", new object[] { 380.0, 100.0 });
            Assert.AreEqual(Math.Round((double)obj, 5), 307.76835);*/
        }

    }
}
