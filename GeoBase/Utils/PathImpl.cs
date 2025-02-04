using System;
using System.Collections;
using System.Collections.Generic;
namespace GeoBase.Utils
{
    public abstract class PathSegment
    {
        public enum SegmentTypes
        {
            Line,
            Arc
        }
        public SegmentTypes SegmentType
        { get; protected set; }
    }
    public class LineSegment:PathSegment
    {
        public LineSegment(UnitPoint p1, UnitPoint p2)
        {
            SegmentType = SegmentTypes.Line;
            P1 = p1;
            P2 = p2;
        }
                public UnitPoint P1
        { get; set; }
        public UnitPoint P2
        { get; private set; }
            }
    public class ArcSegment:PathSegment
    {
        public ArcSegment(UnitPoint p, float radius, float begin, float angle)
        {
            SegmentType = SegmentTypes.Arc;
            Center = p;
            Radius = radius;
            StartAngle = begin;
            Angle = angle;
        }
                public UnitPoint Center
        { get; private set; }
        public float Radius
        { get; private set; }
        public float StartAngle
        { get; private set; }
        public float Angle
        { get; private set; }
            }

    public class PathImpl: IEnumerable
    {
                private List<PathSegment> _pathSegments = new List<PathSegment>();
                        public int ItmesCount
        {
            get { return _pathSegments.Count; }
        }
        public PathSegment this[int index]
        {
            get { return _pathSegments[index]; }
            set { _pathSegments[index] = value; }
        }
        public void AddLine(UnitPoint p1, UnitPoint p2)
        {
            _pathSegments.Add(new LineSegment(p1,p2));
        }
        public void AddArc(UnitPoint p, float radius, float begin, float angle)
        {
            _pathSegments.Add(new ArcSegment(p, radius, begin, angle));
        }
        public PathImpl GetTransformPath(UnitPoint offset)
        {
            var path = new PathImpl();
            foreach (var segment in _pathSegments)
            {
                switch (segment.SegmentType)
                {
                    case PathSegment.SegmentTypes.Line:
                        {
                            var ls = (LineSegment) segment;
                            path.AddLine(ls.P1+offset,ls.P2+offset);
                        }break;
                    case PathSegment.SegmentTypes.Arc:
                        {
                            var a = (ArcSegment) segment;
                            path.AddArc(a.Center + offset, a.Radius, a.StartAngle, a.Angle);
                        }break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return path;
        }
                        public IEnumerator GetEnumerator()
        {
            foreach (var pathSegment in _pathSegments)
                yield return pathSegment;
        }
            }
}
