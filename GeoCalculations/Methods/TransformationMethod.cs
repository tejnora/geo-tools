using System;
using DotNetMatrix;
using GeoBase.Exceptions;
using GeoBase.Utils;
using GeoCalculations.CalculationContexts;
using GeoCalculations.CalculationResultContexts;
using GeoCalculations.Exceptions;
using GeoCalculations.TransformationWrappers;

namespace GeoCalculations.Methods
{
    public static class CordinateTransformationMethod
    {
        public static void Calculate(CoordinatesTransformationContex calculationContex)
        {
            CalculatePoint(calculationContex, null);
        }
        public static void CalculatePoint(CoordinatesTransformationContex calculationcontex, CoordinateTransformationResultContext calculatedPoints)
        {
            CheckInput(calculationcontex);
            switch (calculationcontex.TransformationType)
            {
                case TransformationTypes.Similar:
                    TransformWithExternalDll(calculationcontex, calculatedPoints, (globalPoints, localPoints) => new Similarity2DTransformationWrapper(globalPoints, localPoints));
                    break;
                case TransformationTypes.HelmertSimilar:
                    HelmertSimilar(calculationcontex, calculatedPoints);
                    break;
                case TransformationTypes.Identity:
                    TransformWithExternalDll(calculationcontex, calculatedPoints, (globalPoints, localPoints) => new Identity2DTransformationWrapper(globalPoints, localPoints));
                    break;
                case TransformationTypes.Affine:
                    TransformWithExternalDll(calculationcontex, calculatedPoints, (globalPoints, localPoints) => new Affine2DTransformationWrapper(globalPoints, localPoints));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            for (var i = 0; i < calculationcontex.TableNodes.Count; i++)
            {
                calculationcontex.TableNodes[i].dX = calculationcontex.TableNodes[i].vX;
                calculationcontex.TableNodes[i].dY = calculationcontex.TableNodes[i].vY;
            }
        }

        public static void CheckInput(CoordinatesTransformationContex points)
        {
            foreach (var node in points.TableNodes)
            {
                if (double.IsNaN(node.X) || double.IsNaN(node.Y) || double.IsNaN(node.XLocal) || double.IsNaN(node.YLocal))
                {
                    var par = new ResourceParams();
                    par.Add("number", node.Number);
                    throw new CoordinateTransformationCalculationException("E31", par);
                }
            }
        }

        public static void HelmertSimilar(CoordinatesTransformationContex calculationContex, CoordinateTransformationResultContext calculatedPoints)
        {
            var a = new GeneralMatrix(calculationContex.TableNodes.Count * 2, 4);
            var x = new GeneralMatrix(calculationContex.TableNodes.Count * 2, 1);
            for (var i = 0; i < calculationContex.TableNodes.Count; i++)
            {
                var node = calculationContex.TableNodes[i];
                a.SetElement(i, 0, 1);
                a.SetElement(i, 1, 0);
                a.SetElement(i, 2, node.XLocal);
                a.SetElement(i, 3, -node.YLocal);

                a.SetElement(i + calculationContex.TableNodes.Count, 0, 0);
                a.SetElement(i + calculationContex.TableNodes.Count, 1, 1);
                a.SetElement(i + calculationContex.TableNodes.Count, 2, node.YLocal);
                a.SetElement(i + calculationContex.TableNodes.Count, 3, node.XLocal);

                x.SetElement(i, 0, node.X);
                x.SetElement(i + calculationContex.TableNodes.Count, 0, node.Y);
            }
            GeneralMatrix h;
            try
            {
                var at = a.Transpose();
                h = (at * a).Inverse() * at * x;
                calculationContex.Rotation = -SimpleCalculation.CalculateRotation(h.GetElement(2, 0), h.GetElement(3, 0));
                calculationContex.Scale = SimpleCalculation.CalculateScale(h.GetElement(2, 0), h.GetElement(3, 0));
                //residues
                var rezidua = a * h - x;
                for (var i = 0; i < calculationContex.TableNodes.Count; i++)
                {

                    calculationContex.TableNodes[i].vX = -rezidua.GetElement(i, 0);
                    calculationContex.TableNodes[i].vY = -rezidua.GetElement(i + calculationContex.TableNodes.Count, 0);
                }
            }
            catch (Exception)
            {
                throw new CoordinateTransformationCalculationException("E32");
            }
            if (calculatedPoints == null) return;
            var us = new GeneralMatrix(calculatedPoints.Nodes.Count * 2, 4);
            for (var i = 0; i < calculatedPoints.Nodes.Count; i++)
            {
                var node = calculatedPoints.Nodes[i];
                us.SetElement(i, 0, 1);
                us.SetElement(i, 1, 0);
                us.SetElement(i, 2, node.XLocal);
                us.SetElement(i, 3, -node.YLocal);

                us.SetElement(i + calculatedPoints.Nodes.Count, 0, 0);
                us.SetElement(i + calculatedPoints.Nodes.Count, 1, 1);
                us.SetElement(i + calculatedPoints.Nodes.Count, 2, node.YLocal);
                us.SetElement(i + calculatedPoints.Nodes.Count, 3, node.XLocal);
            }
            try
            {
                var vypocteneUrcovaneSouradnice = us * h;
                for (var i = 0; i < calculatedPoints.Nodes.Count; i++)
                {
                    calculatedPoints.Nodes[i].X = vypocteneUrcovaneSouradnice.GetElement(i, 0);
                    calculatedPoints.Nodes[i].Y = vypocteneUrcovaneSouradnice.GetElement(i + calculatedPoints.Nodes.Count, 0);
                }
            }
            catch (Exception)
            {
                throw new CoordinateTransformationCalculationException("E33");
            }
        }

        public static GeneralMatrix CreateGlobalPointsArray(CoordinatesTransformationContex points)
        {
            var globalPointsArray = new double[points.TableNodes.Count][];
            for (var i = 0; i < points.TableNodes.Count; i++)
            {
                globalPointsArray[i] = new[] { points.TableNodes[i].Y, points.TableNodes[i].X, 0 };
            }
            return new GeneralMatrix(globalPointsArray);
        }

        public static GeneralMatrix CreateLocalPointsArray(CoordinatesTransformationContex points)
        {
            var globalPointsArray = new double[points.TableNodes.Count][];
            for (var i = 0; i < points.TableNodes.Count; i++)
            {
                globalPointsArray[i] = new[] { points.TableNodes[i].YLocal, points.TableNodes[i].XLocal, 0 };
            }
            return new GeneralMatrix(globalPointsArray);
        }

        public static GeneralMatrix CreateTransformPointsArray(CoordinateTransformationResultContext points)
        {
            var globalPointsArray = new double[points.Nodes.Count][];
            for (var i = 0; i < points.Nodes.Count; i++)
            {
                globalPointsArray[i] = new[] { points.Nodes[i].YLocal, points.Nodes[i].XLocal, 0 };
            }
            return new GeneralMatrix(globalPointsArray);
        }

        public static void AssignCalculatedPointsIntoResult(GeneralMatrix transformedPoints, CoordinateTransformationResultContext calculatedPoints)
        {
            if (transformedPoints.RowDimension != calculatedPoints.Nodes.Count)
                throw new InternalException();
            for (var i = 0; i < calculatedPoints.Nodes.Count; i++)
            {
                calculatedPoints.Nodes[i].Y = transformedPoints.GetElement(i, 0);
                calculatedPoints.Nodes[i].X = transformedPoints.GetElement(i, 1);
            }
        }

        public static void AssignCalculatedResidualsIntoResult(double[][] residualsPoints, CoordinatesTransformationContex calculatedPoints)
        {
            for (var i = 0; i < residualsPoints.Length; i++)
            {
                calculatedPoints.TableNodes[i].vY = residualsPoints[i][0];
                calculatedPoints.TableNodes[i].vX = residualsPoints[i][1];
            }
        }

        public static void AddRotationAndScaleIntoResult(double[] trMatrix, CoordinatesTransformationContex calculatedPoints)
        {
            calculatedPoints.Rotation = SimpleCalculation.CalculateRotation(trMatrix[4], trMatrix[5]);
            calculatedPoints.Scale = SimpleCalculation.CalculateScale(trMatrix[4], trMatrix[5]);
        }

        public static void TransformWithExternalDll(CoordinatesTransformationContex points, CoordinateTransformationResultContext calculatedPoints, Func<MatrixWrapper, MatrixWrapper, TransformationBaseWrapper> createTransformation)
        {
            var globalPoints = new MatrixWrapper(CreateGlobalPointsArray(points));
            var localPoints = new MatrixWrapper(CreateLocalPointsArray(points));
            var tr = createTransformation(globalPoints, localPoints);
            tr.Solve();
            if (calculatedPoints.Nodes.Count > 0)
            {
                var localTransformPoints = new MatrixWrapper(CreateTransformPointsArray(calculatedPoints));
                var transformedPoints = tr.TransformLocalPoints(localTransformPoints).ConvertToGeneralMatrix();
                AssignCalculatedPointsIntoResult(transformedPoints, calculatedPoints);
            }
            var report = tr.GetReport();
            AssignCalculatedResidualsIntoResult(report.Residuals, points);
            AddRotationAndScaleIntoResult(report.Keys, points);
        }
    }
}
