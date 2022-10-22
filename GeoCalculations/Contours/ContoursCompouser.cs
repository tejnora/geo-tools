using System;
using System.Collections.Generic;
using System.Diagnostics;
using GeoCalculations.BasicTypes;

namespace GeoCalculations.Contours
{
    public class ContoursCompouser
    {
        public class Path
        {
            public readonly List<Point> Points = new List<Point>();
            public bool IsClosed { get; set; }
        }

        class EndPointKeyComparer : IEqualityComparer<Point>
        {
            public bool Equals(Point x, Point y)
            {
                return Math.Abs(x.X - y.X) < 1e-15 && Math.Abs(x.Y - y.Y) < 1e-15;
            }

            public int GetHashCode(Point obj)
            {
                return obj.GetHashCode();
            }
        }

        class ContourData
        {
            enum EndPoint
            {
                Begin,
                End
            };

            readonly List<Path> _subPaths = new List<Path>();
            readonly Dictionary<Point, Tuple<Path, EndPoint>> _endPoints = new Dictionary<Point, Tuple<Path, EndPoint>>(new EndPointKeyComparer());
            readonly EndPointKeyComparer _comparer = new EndPointKeyComparer();
            public double Elevation { get; private set; }
            public ContourData(double elevationLevel)
            {
                Elevation = elevationLevel;
            }

            public void AddLine(Point startPoint, Point endPoint)
            {
                if (_comparer.Equals(startPoint, endPoint)) return;
                if (TryAppendLineSegmentAnywhere(startPoint, endPoint))
                    return;
                var path = new Path();
                path.Points.Add(startPoint);
                path.Points.Add(endPoint);
                _subPaths.Add(path);
                _endPoints.Add(startPoint, new Tuple<Path, EndPoint>(path, EndPoint.Begin));
                _endPoints.Add(endPoint, new Tuple<Path, EndPoint>(path, EndPoint.End));
            }

            bool TryAppendLineSegmentAnywhere(Point startPoint, Point endPoint)
            {
                Tuple<Path, EndPoint> sPath, ePath;
                var sContains = _endPoints.TryGetValue(startPoint, out sPath);
                var eContains = _endPoints.TryGetValue(endPoint, out ePath);
                if (eContains && sContains)
                {
                    ConnectPaths(sPath, ePath);
                    return true;
                }
                if (sContains)
                {
                    _endPoints.Remove(startPoint);
                    ExpandPath(sPath, endPoint);
                    _endPoints.Add(endPoint, sPath);
                    return true;
                }
                if (eContains)
                {
                    _endPoints.Remove(endPoint);
                    ExpandPath(ePath, startPoint);
                    _endPoints.Add(startPoint, ePath);
                    return true;
                }
                return false;
            }

            void ConnectPaths(Tuple<Path, EndPoint> path1, Tuple<Path, EndPoint> path2)
            {
                _endPoints.Remove(path1.Item1.Points[0]);
                _endPoints.Remove(path1.Item1.Points[path1.Item1.Points.Count - 1]);
                if (path1.Item1 == path2.Item1)
                {
                    path1.Item1.Points.Add(path1.Item1.Points[0]);
                    path1.Item1.IsClosed = true;
                    return;
                }
                Debug.Assert(!path1.Item1.IsClosed);
                _endPoints.Remove(path2.Item1.Points[0]);
                _endPoints.Remove(path2.Item1.Points[path2.Item1.Points.Count - 1]);
                Path newPath;
                if (path1.Item2 == EndPoint.Begin)
                {
                    if (path2.Item2 == EndPoint.Begin)
                    {
                        foreach (var point in path2.Item1.Points)
                        {
                            path1.Item1.Points.Insert(0, point);
                        }
                        _subPaths.Remove(path2.Item1);
                        newPath = path1.Item1;
                    }
                    else
                    {
                        path2.Item1.Points.AddRange(path1.Item1.Points);
                        _subPaths.Remove(path1.Item1);
                        newPath = path2.Item1;
                    }
                }
                else
                {
                    if (path2.Item2 == EndPoint.Begin)
                        path1.Item1.Points.AddRange(path2.Item1.Points);
                    else
                    {
                        for (var i = path2.Item1.Points.Count - 1; i >= 0; --i)
                        {
                            path1.Item1.Points.Add(path2.Item1.Points[i]);
                        }
                    }
                    _subPaths.Remove(path2.Item1);
                    newPath = path1.Item1;
                }
                _endPoints.Add(newPath.Points[0], new Tuple<Path, EndPoint>(newPath, EndPoint.Begin));
                _endPoints.Add(newPath.Points[newPath.Points.Count - 1], new Tuple<Path, EndPoint>(newPath, EndPoint.End));
            }

            void ExpandPath(Tuple<Path, EndPoint> path, Point newPoint)
            {
                if (path.Item2 == EndPoint.Begin)
                    path.Item1.Points.Insert(0, newPoint);
                else
                    path.Item1.Points.Add(newPoint);
            }

            public Path<Point> GetPath()
            {
                var path = new Path<Point>();
                foreach (var subPath in _subPaths)
                {
                    if (subPath.Points.Count < 2) continue;
                    path.MoveTo(subPath.Points[0]);
                    for (var i = 1; i < subPath.Points.Count; i++)
                    {
                        path.LineTo(subPath.Points[i]);
                    }
                }
                return path;
            }
        }

        readonly ContourData[] _contourDatas;

        public ContoursCompouser(double[] elevationLevels)
        {
            _contourDatas = new ContourData[elevationLevels.Length];
            for (var i = 0; i < elevationLevels.Length; i++)
            {
                _contourDatas[i] = new ContourData(elevationLevels[i]);
            }
        }

        public void AddLine(int contourLevel, Point startPoint, Point endPoint)
        {
            _contourDatas[contourLevel].AddLine(startPoint, endPoint);
        }

        public class ContourAtElevation
        {
            public Path<Point> Path { get; set; }
            public double Elevation { get; set; }
        }

        public List<ContourAtElevation> GetPaths()
        {
            var paths = new List<ContourAtElevation>(_contourDatas.Length);
            foreach (var contourData in _contourDatas)
            {
                paths.Add(new ContourAtElevation { Elevation = contourData.Elevation, Path = contourData.GetPath() });
            }
            return paths;
        }
    }
}
