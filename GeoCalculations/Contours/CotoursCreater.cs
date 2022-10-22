using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoCalculations.BasicTypes;
using GeoCalculations.Methods;

namespace GeoCalculations.Contours
{
    public class CotoursCreater
    {
        class TriangleIsoline
        {
            public enum Positions
            {
                Above,
                Below,
            }

            public Positions[] Points { get; set; }
            //http://en.wikipedia.org/wiki/Marching_squares
            public int GetTriangleId()
            {
                if (Points[0] == Positions.Above && Points[1] == Positions.Above && Points[2] == Positions.Above)
                    return 0;
                if (Points[0] == Positions.Below && Points[1] == Positions.Below && Points[2] == Positions.Below)
                    return 1;
                if (Points[0] == Positions.Below && Points[1] == Positions.Above && Points[2] == Positions.Above)
                    return 2;
                if (Points[0] == Positions.Above && Points[1] == Positions.Below && Points[2] == Positions.Above)
                    return 3;
                if (Points[0] == Positions.Above && Points[1] == Positions.Above && Points[2] == Positions.Below)
                    return 4;
                if (Points[0] == Positions.Above && Points[1] == Positions.Below && Points[2] == Positions.Below)
                    return 5;
                if (Points[0] == Positions.Below && Points[1] == Positions.Above && Points[2] == Positions.Below)
                    return 6;
                if (Points[0] == Positions.Below && Points[1] == Positions.Below && Points[2] == Positions.Above)
                    return 7;
                throw new ArgumentOutOfRangeException();
            }
        }

        struct IsolineOfTriangles
        {
            public TriangleIsoline[] Isolines;
            public double Elevation;
            public int LevelId;
        }

        readonly double _contoursDelta;
        readonly double _minHeight;
        readonly double _maxHeight;
        readonly int _countOfCoutourLevels;

        IsolineOfTriangles[] _isolineOfLevels;
        ContourNode[][] _contoursNormalizedTriangles;
        ContoursCompouser _contoursCompouser;

        public ContoursCompouser ContoursCompouser { get { return _contoursCompouser; } }

        public CotoursCreater(double contoursDelta, double minHeight, double maxHeight)
        {
            _contoursDelta = contoursDelta;
            _minHeight = minHeight;
            _maxHeight = maxHeight;
            _countOfCoutourLevels = (int)((_maxHeight - _minHeight) / _contoursDelta + 1);
        }

        public void CreateContours(IEnumerable<ContourTriangulationCell<ContourNode>> vertices)
        {
            var contourTriangulationCells = vertices as ContourTriangulationCell<ContourNode>[] ?? vertices.ToArray();
            if (!contourTriangulationCells.Any()) return;
            NormalizeTriangles(contourTriangulationCells);
            CreateAllIsolines();
            _contoursCompouser = new ContoursCompouser(_isolineOfLevels.Select(n => n.Elevation).ToArray());
            CreateContours();
        }

        void NormalizeTriangles(IList<ContourTriangulationCell<ContourNode>> vertices)
        {
            _contoursNormalizedTriangles = new ContourNode[(vertices.Count)][];
            for (var i = 0; i < vertices.Count; i++)
            {
                _contoursNormalizedTriangles[i] = NormalizeTriangle(vertices[i].Vertices);
            }
        }

        ContourNode[] NormalizeTriangle(IList<ContourNode> vertices)
        {
            Debug.Assert(vertices.Count == 3);
            var topNodeIdx = 0;
            if (!IsFirstNodeHigherThenSecond(vertices[topNodeIdx], vertices[1]))
                topNodeIdx = 1;
            if (!IsFirstNodeHigherThenSecond(vertices[topNodeIdx], vertices[2]))
                topNodeIdx = 2;
            int leftNodeIdx;
            int rightNodeIdx;
            switch (topNodeIdx)
            {
                case 0:
                    leftNodeIdx = 1;
                    rightNodeIdx = 2;
                    break;
                case 1:
                    leftNodeIdx = 0;
                    rightNodeIdx = 2;
                    break;
                case 2:
                    leftNodeIdx = 0;
                    rightNodeIdx = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (vertices[rightNodeIdx].CordinateX < vertices[leftNodeIdx].CordinateX)
            {
                var temp = rightNodeIdx;
                rightNodeIdx = leftNodeIdx;
                leftNodeIdx = temp;
            }
            var normalized = new ContourNode[3];
            normalized[0] = vertices[topNodeIdx];
            normalized[1] = vertices[rightNodeIdx];
            normalized[2] = vertices[leftNodeIdx];
            return normalized;
        }

        static bool IsFirstNodeHigherThenSecond(ContourNode node1, ContourNode node2)
        {
            if (node1.CordinateY > node2.CordinateY) return true;
            if (node1.CordinateY < node2.CordinateY) return false;
            if (node1.CordinateX > node2.CordinateX)
                return false;
            return true;
        }

        void CreateContours()
        {
            for (var i = 0; i < _countOfCoutourLevels; i++)
            {
                CreateContoursDataAtSameLevel(_isolineOfLevels[i]);
            }
        }

        void CreateContoursDataAtSameLevel(IsolineOfTriangles isolineOfTriangles)
        {
            for (var i = 0; i < _contoursNormalizedTriangles.Length; i++)
            {
                var triangleNodes = _contoursNormalizedTriangles[i];
                CreateContoursDataForTriangleAtHeight(triangleNodes[0].PositionWithHeight, triangleNodes[1].PositionWithHeight, triangleNodes[2].PositionWithHeight, isolineOfTriangles.Isolines[i], isolineOfTriangles.Elevation, isolineOfTriangles.LevelId);
            }
        }

        void CreateContoursDataForTriangleAtHeight(Point pos0, Point pos1, Point pos2, TriangleIsoline triangleIsoline, double height, int levelId)
        {
            var line1Pos = new Point[2];
            var line2Pos = new Point[2];
            switch (triangleIsoline.GetTriangleId())
            {
                case 0:
                case 1:
                    return;
                case 2:
                    line1Pos[0] = pos0;
                    line1Pos[1] = pos1;
                    line2Pos[0] = pos0;
                    line2Pos[1] = pos2;
                    break;
                case 3:
                    line1Pos[0] = pos1;
                    line1Pos[1] = pos0;
                    line2Pos[0] = pos1;
                    line2Pos[1] = pos2;
                    break;
                case 4:
                    line1Pos[0] = pos2;
                    line1Pos[1] = pos1;
                    line2Pos[0] = pos2;
                    line2Pos[1] = pos0;
                    break;
                case 5:
                    line1Pos[0] = pos1;
                    line1Pos[1] = pos0;
                    line2Pos[0] = pos2;
                    line2Pos[1] = pos0;
                    break;
                case 6:
                    line1Pos[0] = pos0;
                    line1Pos[1] = pos1;
                    line2Pos[0] = pos2;
                    line2Pos[1] = pos1;
                    break;
                case 7:
                    line1Pos[0] = pos0;
                    line1Pos[1] = pos2;
                    line2Pos[0] = pos1;
                    line2Pos[1] = pos2;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            _contoursCompouser.AddLine(levelId, CalculatePoint(line1Pos, height), CalculatePoint(line2Pos, height));
        }

        static Point CalculatePoint(Point[] line, double currentHeigh)
        {
            currentHeigh -= Math.Min(line[0].Height, line[1].Height);
            var relativeHight = Math.Abs(line[0].Height - line[1].Height);
            var distance = SimpleCalculation.CalculateDistance(line[0], line[1]);
            var currentHeightDistance = currentHeigh / relativeHight * distance;
            var point = SimpleCalculation.CalculatePointOnLineAtSpecificDistance(line[0], line[1], currentHeightDistance);
            point.Height = currentHeigh;
            return point;
        }

        void CreateAllIsolines()
        {
            _isolineOfLevels = new IsolineOfTriangles[_countOfCoutourLevels];
            for (var i = 0; i < _countOfCoutourLevels; i++)
            {
                _isolineOfLevels[i] = CreateIsolinesFroSpecificHeight(_minHeight + i * _contoursDelta);
                _isolineOfLevels[i].LevelId = i;
            }
        }

        IsolineOfTriangles CreateIsolinesFroSpecificHeight(double height)
        {
            var count = _contoursNormalizedTriangles.Count();
            var isolines = new TriangleIsoline[count];
            for (var i = 0; i < count; i++)
            {
                isolines[i] = CreateIsolaniesForTriangle(height, _contoursNormalizedTriangles[i]);
            }
            return new IsolineOfTriangles { Elevation = height, Isolines = isolines };
        }

        static TriangleIsoline CreateIsolaniesForTriangle(double height, ContourNode[] trianglePoints)
        {
            var isolineInfo = new TriangleIsoline { Points = new TriangleIsoline.Positions[3] };
            isolineInfo.Points[0] = (trianglePoints[0].Height > height) ? TriangleIsoline.Positions.Above : TriangleIsoline.Positions.Below;
            isolineInfo.Points[1] = (trianglePoints[1].Height > height) ? TriangleIsoline.Positions.Above : TriangleIsoline.Positions.Below;
            isolineInfo.Points[2] = (trianglePoints[2].Height > height) ? TriangleIsoline.Positions.Above : TriangleIsoline.Positions.Below;
            return isolineInfo;
        }
    }
}
