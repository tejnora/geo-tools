using System;
using System.Collections.Generic;
using System.Linq;

namespace CAD.DTM.Validation
{
    /// <summary>
    /// Areas of JVF DTM are derived objects: they are formed by closed cycles of ZPS construction lines,
    /// the definition point only marks which area it is. This builds the bounded faces of the planar graph
    /// of the lines (lines are expected to be vertex-identical where they meet, see the topology rules).
    /// </summary>
    public static class DtmDerivedAreas
    {
        public static List<List<DtmPoint>> Faces(IEnumerable<DtmValidationItem> lines)
        {
            var nodes = new Dictionary<string, DtmPoint>();
            var adjacency = new Dictionary<string, HashSet<string>>();
            foreach (var line in lines)
                foreach (var curve in Curves(line))
                    for (var i = 1; i < curve.Count; i++)
                    {
                        var a = Node(curve[i - 1]);
                        var b = Node(curve[i]);
                        if (a == b) continue;
                        adjacency[a].Add(b);
                        adjacency[b].Add(a);
                    }

            // Neighbours of every node ordered counter-clockwise by angle.
            var ordered = adjacency.ToDictionary(n => n.Key, n => n.Value
                .OrderBy(m => Math.Atan2(nodes[m].Y - nodes[n.Key].Y, nodes[m].X - nodes[n.Key].X)).ToList());

            var faces = new List<List<DtmPoint>>();
            var visited = new HashSet<(string, string)>();
            var halfEdges = ordered.Sum(n => n.Value.Count);
            foreach (var start in ordered.SelectMany(n => n.Value.Select(m => (n.Key, m))))
            {
                if (visited.Contains(start)) continue;
                var ring = new List<DtmPoint>();
                var edge = start;
                for (var steps = 0; steps <= halfEdges && visited.Add(edge); steps++)
                {
                    ring.Add(nodes[edge.Item1]);
                    // At the end node continue with the neighbour clockwise next to the edge we came from: the face stays on the left.
                    var around = ordered[edge.Item2];
                    var back = around.IndexOf(edge.Item1);
                    edge = (edge.Item2, around[(back - 1 + around.Count) % around.Count]);
                }
                if (SignedArea(ring) > 1e-9) faces.Add(ring);
            }
            return faces;

            string Node(DtmPoint p)
            {
                var key = DtmGeometryMath.Key2D(p);
                if (nodes.TryAdd(key, p)) adjacency[key] = new HashSet<string>();
                return key;
            }
        }

        static IEnumerable<IList<DtmPoint>> Curves(DtmValidationItem line)
        {
            foreach (var geometry in line.Geometries)
                switch (geometry)
                {
                    case DtmMultiCurveGeometry multi:
                        foreach (var member in multi.Curves)
                            if (member.Points != null) yield return member.Points;
                        break;
                    case DtmCurveGeometry curve when curve.Points != null:
                        yield return curve.Points;
                        break;
                }
        }

        static double SignedArea(IList<DtmPoint> ring)
        {
            double sum = 0;
            for (int i = 0, j = ring.Count - 1; i < ring.Count; j = i++)
                sum += ring[j].X * ring[i].Y - ring[i].X * ring[j].Y;
            return sum / 2;
        }
    }
}
