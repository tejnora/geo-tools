using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoCalculations.ConvexHull
{
    internal class ConvexHullInternal
    {
        bool _computed;
        readonly int _dimension;
        List<VertexWrap> _inputVertices;
        readonly List<VertexWrap> _originalInputVertices;
        List<VertexWrap> _convexHull;
        FaceList _unprocessedFaces;
        List<ConvexFaceInternal> _convexFaces;
        VertexWrap _currentVertex;
        double _maxDistance;
        VertexWrap _furthestVertex;
        double[] _center;
        double[] _ntX, _ntY, _ntZ;
        double[] _nDNormalSolveVector;
        double[,] _nDMatrix;
        double[][] _jaggedNdMatrix;
        ConvexFaceInternal[] _updateBuffer;
        int[] _updateIndices;
        Stack<ConvexFaceInternal> _traverseStack;
        Stack<ConvexFaceInternal> _recycledFaceStack;
        Stack<FaceConnector> _connectorStack;
        Stack<VertexBuffer> _emptyBufferStack;
        VertexBuffer _emptyBuffer;
        VertexBuffer _beyondBuffer;
        List<ConvexFaceInternal> _affectedFaceBuffer;
        const int ConnectorTableSize = 2017;
        ConnectorList[] _connectorTable;

        void Initialize()
        {
            _convexHull = new List<VertexWrap>();
            _unprocessedFaces = new FaceList(); // new LinkedList<ConvexFaceInternal>();
            _convexFaces = new List<ConvexFaceInternal>();

            _center = new double[_dimension];
            _ntX = new double[_dimension];
            _ntY = new double[_dimension];
            _ntZ = new double[_dimension];
            _traverseStack = new Stack<ConvexFaceInternal>();
            _updateBuffer = new ConvexFaceInternal[_dimension];
            _updateIndices = new int[_dimension];
            _recycledFaceStack = new Stack<ConvexFaceInternal>();
            _connectorStack = new Stack<FaceConnector>();
            _emptyBufferStack = new Stack<VertexBuffer>();
            _emptyBuffer = new VertexBuffer();
            _affectedFaceBuffer = new List<ConvexFaceInternal>();
            _beyondBuffer = new VertexBuffer();

            _connectorTable = Enumerable.Range(0, ConnectorTableSize).Select(_ => new ConnectorList()).ToArray();

            _nDNormalSolveVector = new double[_dimension];
            _jaggedNdMatrix = new double[_dimension][];
            for (var i = 0; i < _dimension; i++)
            {
                _nDNormalSolveVector[i] = 1.0;
                _jaggedNdMatrix[i] = new double[_dimension];
            }
            _nDMatrix = new double[_dimension, _dimension];
        }

        int DetermineDimension()
        {
            var r = new Random();
            var vCount = _originalInputVertices.Count;
            var dimensions = new List<int>();
            for (var i = 0; i < 10; i++)
                dimensions.Add(_originalInputVertices[r.Next(vCount)].Vertex.Position.Length);
            var dimension = dimensions.Min();
            if (dimension != dimensions.Max()) throw new ArgumentException("Invalid input data (non-uniform dimension).");
            return dimension;
        }

        IEnumerable<ConvexFaceInternal> InitiateFaceDatabase()
        {
            var faces = new ConvexFaceInternal[_dimension + 1];

            for (var i = 0; i < _dimension + 1; i++)
            {
                var vertices = _convexHull.Where((_, j) => i != j).ToArray(); // Skips the i-th vertex
                var newFace = new ConvexFaceInternal(_dimension, new VertexBuffer()) {Vertices = vertices};
                Array.Sort(vertices, VertexWrapComparer.Instance);
                CalculateFacePlane(newFace);
                faces[i] = newFace;
            }

            // update the adjacency (check all pairs of faces)
            for (var i = 0; i < _dimension; i++)
            {
                for (var j = i + 1; j < _dimension + 1; j++) UpdateAdjacency(faces[i], faces[j]);
            }

            return faces;
        }

        private void CalculateFacePlane(ConvexFaceInternal face)
        {
            var vertices = face.Vertices;
            var normal = face.Normal;
            FindNormalVector(vertices, normal);

            if (double.IsNaN(normal[0])) ThrowSingular();

            double offset = 0.0;
            double centerDistance = 0.0;
            var fi = vertices[0].PositionData;
            for (int i = 0; i < _dimension; i++)
            {
                double n = normal[i];
                offset += n * fi[i];
                centerDistance += n * _center[i];
            }
            face.Offset = -offset;
            centerDistance -= offset;

            if (centerDistance > 0)
            {
                for (int i = 0; i < _dimension; i++) normal[i] = -normal[i];
                face.Offset = offset;
                face.IsNormalFlipped = true;
            }
            else face.IsNormalFlipped = false;
        }

        double GetVertexDistance(VertexWrap v, ConvexFaceInternal f)
        {
            double[] normal = f.Normal;
            double[] p = v.PositionData;
            double distance = f.Offset;
            for (int i = 0; i < _dimension; i++) distance += normal[i] * p[i];
            return distance;
        }

        void TagAffectedFaces(ConvexFaceInternal currentFace)
        {
            _affectedFaceBuffer.Clear();
            _affectedFaceBuffer.Add(currentFace);
            TraverseAffectedFaces(currentFace);
        }

        void TraverseAffectedFaces(ConvexFaceInternal currentFace)
        {
            _traverseStack.Clear();
            _traverseStack.Push(currentFace);
            currentFace.Tag = 1;

            while (_traverseStack.Count > 0)
            {
                var top = _traverseStack.Pop();
                for (var i = 0; i < _dimension; i++)
                {
                    var adjFace = top.AdjacentFaces[i];

                    if (adjFace.Tag == 0 && GetVertexDistance(_currentVertex, adjFace) >= 0)
                    {
                        _affectedFaceBuffer.Add(adjFace);
                        //TraverseAffectedFaces(adjFace);
                        adjFace.Tag = 1;
                        _traverseStack.Push(adjFace);
                    }
                }
            }

        }

        void UpdateAdjacency(ConvexFaceInternal l, ConvexFaceInternal r)
        {
            var lv = l.Vertices;
            var rv = r.Vertices;
            int i;

            // reset marks on the 1st face
            for (i = 0; i < _dimension; i++) lv[i].Marked = false;

            // mark all vertices on the 2nd face
            for (i = 0; i < _dimension; i++) rv[i].Marked = true;

            // find the 1st false index
            for (i = 0; i < _dimension; i++) if (!lv[i].Marked) break;

            // no vertex was marked
            if (i == _dimension) return;

            // check if only 1 vertex wasn't marked
            for (int j = i + 1; j < _dimension; j++) if (!lv[j].Marked) return;

            // if we are here, the two faces share an edge
            l.AdjacentFaces[i] = r;

            // update the adj. face on the other face - find the vertex that remains marked
            for (i = 0; i < _dimension; i++) lv[i].Marked = false;
            for (i = 0; i < _dimension; i++)
            {
                if (rv[i].Marked) break;
            }
            r.AdjacentFaces[i] = l;
        }

        void RecycleFace(ConvexFaceInternal face)
        {
            for (var i = 0; i < _dimension; i++)
            {
                face.AdjacentFaces[i] = null;
            }
        }

        ConvexFaceInternal GetNewFace()
        {
            return _recycledFaceStack.Count != 0
                    ? _recycledFaceStack.Pop()
                    : new ConvexFaceInternal(_dimension, _emptyBufferStack.Count != 0 ? _emptyBufferStack.Pop() : new VertexBuffer());
        }

        FaceConnector GetNewConnector()
        {
            return _connectorStack.Count != 0
                    ? _connectorStack.Pop()
                    : new FaceConnector(_dimension);
        }

        void ConnectFace(FaceConnector connector)
        {
            var index = connector.HashCode % ConnectorTableSize;
            var list = _connectorTable[index];

            for (var current = list.First; current != null; current = current.Next)
            {
                if (FaceConnector.AreConnectable(connector, current, _dimension))
                {
                    list.Remove(current);
                    FaceConnector.Connect(current, connector);
                    current.Face = null;
                    connector.Face = null;
                    _connectorStack.Push(current);
                    _connectorStack.Push(connector);
                    return;
                }
            }

            list.Add(connector);
        }

        private void CreateCone()
        {
            var oldFaces = _affectedFaceBuffer;

            var currentVertexIndex = _currentVertex.Index;

            foreach (var oldFace in oldFaces)
            {
                // Find the faces that need to be updated
                int updateCount = 0;
                for (int i = 0; i < _dimension; i++)
                {
                    var af = oldFace.AdjacentFaces[i];
                    if (af.Tag == 0) // Tag == 0 when oldFaces does not contain af
                    {
                        _updateBuffer[updateCount] = af;
                        _updateIndices[updateCount] = i;
                        ++updateCount;
                    }
                }

                // Recycle the face for future use
                if (updateCount == 0)
                {
                    // If the face is present in the unprocessed list, remove it 
                    _unprocessedFaces.Remove(oldFace);

                    RecycleFace(oldFace);
                    _recycledFaceStack.Push(oldFace);
                }

                for (int i = 0; i < updateCount; i++)
                {
                    var adjacentFace = _updateBuffer[i];

                    int oldFaceAdjacentIndex = 0;
                    var adjFaceAdjacency = adjacentFace.AdjacentFaces;
                    for (int j = 0; j < _dimension; j++)
                    {
                        if (!ReferenceEquals(oldFace, adjFaceAdjacency[j])) continue;
                        oldFaceAdjacentIndex = j;
                        break;
                    }

                    var forbidden = _updateIndices[i]; // Index of the face that corresponds to this adjacent face

                    ConvexFaceInternal newFace;

                    int oldVertexIndex;
                    VertexWrap[] vertices;

                    // Recycle the oldFace
                    if (i == updateCount - 1)
                    {
                        RecycleFace(oldFace);
                        newFace = oldFace;
                        vertices = newFace.Vertices;
                        oldVertexIndex = vertices[forbidden].Index;
                    }
                    else // Pop a face from the recycled stack or create a new one
                    {
                        newFace = GetNewFace();
                        vertices = newFace.Vertices;
                        for (int j = 0; j < _dimension; j++) vertices[j] = oldFace.Vertices[j];
                        oldVertexIndex = vertices[forbidden].Index;
                    }

                    int orderedPivotIndex;

                    // correct the ordering
                    if (currentVertexIndex < oldVertexIndex)
                    {
                        orderedPivotIndex = 0;
                        for (int j = forbidden - 1; j >= 0; j--)
                        {
                            if (vertices[j].Index > currentVertexIndex) vertices[j + 1] = vertices[j];
                            else
                            {
                                orderedPivotIndex = j + 1;
                                break;
                            }
                        }
                    }
                    else
                    {
                        orderedPivotIndex = _dimension - 1;
                        for (int j = forbidden + 1; j < _dimension; j++)
                        {
                            if (vertices[j].Index < currentVertexIndex) vertices[j - 1] = vertices[j];
                            else
                            {
                                orderedPivotIndex = j - 1;
                                break;
                            }
                        }
                    }

                    vertices[orderedPivotIndex] = _currentVertex;

                    CalculateFacePlane(newFace);
                    newFace.AdjacentFaces[orderedPivotIndex] = adjacentFace;
                    adjacentFace.AdjacentFaces[oldFaceAdjacentIndex] = newFace;

                    // let there be a connection.
                    for (int j = 0; j < _dimension; j++)
                    {
                        if (j == orderedPivotIndex) continue;
                        var connector = GetNewConnector();
                        connector.Update(newFace, j, _dimension);
                        ConnectFace(connector);
                    }

                    // This could slightly help...
                    if (adjacentFace.VerticesBeyond.Count < oldFace.VerticesBeyond.Count)
                    {
                        FindBeyondVertices(newFace, adjacentFace.VerticesBeyond, oldFace.VerticesBeyond);
                    }
                    else
                    {
                        FindBeyondVertices(newFace, oldFace.VerticesBeyond, adjacentFace.VerticesBeyond);
                    }

                    // This face will definitely lie on the hull
                    if (newFace.VerticesBeyond.Count == 0)
                    {
                        _convexFaces.Add(newFace);
                        _unprocessedFaces.Remove(newFace);
                        _emptyBufferStack.Push(newFace.VerticesBeyond);
                        newFace.VerticesBeyond = _emptyBuffer;
                    }
                    else // Add the face to the list
                    {
                        _unprocessedFaces.Add(newFace);
                    }
                }
            }
        }

        void SubtractFast(double[] x, double[] y, double[] target)
        {
            for (int i = 0; i < _dimension; i++)
            {
                target[i] = x[i] - y[i];
            }
        }

        void FindNormalVector4D(VertexWrap[] vertices, double[] normal)
        {
            SubtractFast(vertices[1].PositionData, vertices[0].PositionData, _ntX);
            SubtractFast(vertices[2].PositionData, vertices[1].PositionData, _ntY);
            SubtractFast(vertices[3].PositionData, vertices[2].PositionData, _ntZ);

            var x = _ntX;
            var y = _ntY;
            var z = _ntZ;

            // This was generated using Mathematica
            var nx = x[3] * (y[2] * z[1] - y[1] * z[2])
                   + x[2] * (y[1] * z[3] - y[3] * z[1])
                   + x[1] * (y[3] * z[2] - y[2] * z[3]);
            var ny = x[3] * (y[0] * z[2] - y[2] * z[0])
                   + x[2] * (y[3] * z[0] - y[0] * z[3])
                   + x[0] * (y[2] * z[3] - y[3] * z[2]);
            var nz = x[3] * (y[1] * z[0] - y[0] * z[1])
                   + x[1] * (y[0] * z[3] - y[3] * z[0])
                   + x[0] * (y[3] * z[1] - y[1] * z[3]);
            var nw = x[2] * (y[0] * z[1] - y[1] * z[0])
                   + x[1] * (y[2] * z[0] - y[0] * z[2])
                   + x[0] * (y[1] * z[2] - y[2] * z[1]);

            double norm = Math.Sqrt(nx * nx + ny * ny + nz * nz + nw * nw);

            double f = 1.0 / norm;
            normal[0] = f * nx;
            normal[1] = f * ny;
            normal[2] = f * nz;
            normal[3] = f * nw;
        }

        void FindNormalVector3D(VertexWrap[] vertices, double[] normal)
        {
            SubtractFast(vertices[1].PositionData, vertices[0].PositionData, _ntX);
            SubtractFast(vertices[2].PositionData, vertices[1].PositionData, _ntY);

            var x = _ntX;
            var y = _ntY;

            var nx = x[1] * y[2] - x[2] * y[1];
            var ny = x[2] * y[0] - x[0] * y[2];
            var nz = x[0] * y[1] - x[1] * y[0];

            double norm = Math.Sqrt(nx * nx + ny * ny + nz * nz);

            double f = 1.0 / norm;
            normal[0] = f * nx;
            normal[1] = f * ny;
            normal[2] = f * nz;
        }

        void FindNormalVector2D(VertexWrap[] vertices, double[] normal)
        {
            SubtractFast(vertices[1].PositionData, vertices[0].PositionData, _ntX);

            var x = _ntX;

            var nx = -x[1];
            var ny = x[0];

            double norm = Math.Sqrt(nx * nx + ny * ny);

            double f = 1.0 / norm;
            normal[0] = f * nx;
            normal[1] = f * ny;
        }

        private void FindNormalVector(VertexWrap[] vertices, double[] normalData)
        {
            switch (_dimension)
            {
                case 2: FindNormalVector2D(vertices, normalData); break;
                case 3: FindNormalVector3D(vertices, normalData); break;
                case 4: FindNormalVector4D(vertices, normalData); break;
                default:
                    {
                        for (var i = 0; i < _dimension; i++) _nDNormalSolveVector[i] = 1.0;
                        for (var i = 0; i < _dimension; i++)
                        {
                            var row = _jaggedNdMatrix[i];
                            var pos = vertices[i].Vertex.Position;
                            for (int j = 0; j < _dimension; j++) row[j] = pos[j];
                        }
                        StarMath.StarMath.GaussElimination(_dimension, _jaggedNdMatrix, _nDNormalSolveVector, normalData);
                        StarMath.StarMath.NormalizeInPlace(normalData, _dimension);
                        break;
                    }
            }
        }

        void IsBeyond(ConvexFaceInternal face, VertexBuffer beyondVertices, VertexWrap v)
        {
            double distance = GetVertexDistance(v, face);
            if (distance >= 0)
            {
                if (distance > _maxDistance)
                {
                    _maxDistance = distance;
                    _furthestVertex = v;
                }
                beyondVertices.Add(v);
            }
        }

        void FindBeyondVertices(ConvexFaceInternal face)
        {
            var beyondVertices = face.VerticesBeyond;

            _maxDistance = double.NegativeInfinity;
            _furthestVertex = null;

            int count = _inputVertices.Count;
            for (int i = 0; i < count; i++) IsBeyond(face, beyondVertices, _inputVertices[i]);

            face.FurthestVertex = _furthestVertex;
            //face.FurthestDistance = MaxDistance;
        }

        void FindBeyondVertices(ConvexFaceInternal face, VertexBuffer beyond, VertexBuffer beyond1)
        {
            var beyondVertices = _beyondBuffer;

            _maxDistance = double.NegativeInfinity;
            _furthestVertex = null;
            VertexWrap v;

            int count = beyond1.Count;
            for (int i = 0; i < count; i++) beyond1[i].Marked = true;
            _currentVertex.Marked = false;
            count = beyond.Count;
            for (int i = 0; i < count; i++)
            {
                v = beyond[i];
                if (ReferenceEquals(v, _currentVertex)) continue;
                v.Marked = false;
                IsBeyond(face, beyondVertices, v);
            }

            count = beyond1.Count;
            for (int i = 0; i < count; i++)
            {
                v = beyond1[i];
                if (v.Marked) IsBeyond(face, beyondVertices, v);
            }

            face.FurthestVertex = _furthestVertex;
            //face.FurthestDistance = MaxDistance;

            // Pull the old switch a roo
            var temp = face.VerticesBeyond;
            face.VerticesBeyond = beyondVertices;
            if (temp.Count > 0) temp.Clear();
            _beyondBuffer = temp;
        }

        void UpdateCenter()
        {
            for (int i = 0; i < _dimension; i++) _center[i] *= (_convexHull.Count - 1);
            double f = 1.0 / _convexHull.Count;
            for (int i = 0; i < _dimension; i++) _center[i] = f * (_center[i] + _currentVertex.PositionData[i]);
        }

        void InitConvexHull(Boolean removeDuplicates)
        {
            if (removeDuplicates) SortAndRemoveRepeats();
            var extremes = FindExtremes();
            var initialPoints = FindInitialPoints(extremes);

            // Add the initial points to the convex hull.
            foreach (var vertex in initialPoints)
            {
                _currentVertex = vertex;
                _convexHull.Add(_currentVertex);
                UpdateCenter();
                _inputVertices.Remove(vertex);

                // Because of the AklTou heuristic.
                extremes.Remove(vertex);
            }

            // Create the initial simplexes.
            var faces = InitiateFaceDatabase();

            // Init the vertex beyond buffers.
            foreach (var face in faces)
            {
                FindBeyondVertices(face);
                if (face.VerticesBeyond.Count == 0) _convexFaces.Add(face); // The face is on the hull
                else _unprocessedFaces.Add(face);
            }
        }

        private IEnumerable<VertexWrap> FindInitialPoints(List<VertexWrap> extremes)
        {
            var initialPoints = new List<VertexWrap>();// { extremes[0], extremes[1] };

            VertexWrap first = null, second = null;
            double maxDist = 0;
            for (var i = 0; i < extremes.Count - 1; i++)
            {
                var a = extremes[i];
                for (var j = i + 1; j < extremes.Count; j++)
                {
                    var b = extremes[j];
                    var dist = StarMath.StarMath.Norm2(StarMath.StarMath.Subtract(a.PositionData, b.PositionData, _dimension), _dimension, true);
                    if (dist > maxDist)
                    {
                        first = a;
                        second = b;
                        maxDist = dist;
                    }
                }
            }

            initialPoints.Add(first);
            initialPoints.Add(second);

            for (var i = 2; i <= _dimension; i++)
            {
                var maximum = Constants.Epsilon;
                VertexWrap maxPoint = null;
                foreach (var extreme in extremes)
                {
                    if (initialPoints.Contains(extreme)) continue;

                    var val = GetSimplexVolume(extreme, initialPoints);

                    if (val > maximum)
                    {
                        maximum = val;
                        maxPoint = extreme;
                    }
                }
                if (maxPoint != null) initialPoints.Add(maxPoint);
                else
                {
                    int vCount = _inputVertices.Count;
                    for (int j = 0; j < vCount; j++)
                    {
                        var point = _inputVertices[j];
                        if (initialPoints.Contains(point)) continue;

                        var val = GetSimplexVolume(point, initialPoints);

                        if (val > maximum)
                        {
                            maximum = val;
                            maxPoint = point;
                        }
                    }

                    if (maxPoint != null) initialPoints.Add(maxPoint);
                    else ThrowSingular();
                }
            }
            return initialPoints;
        }

        double GetSimplexVolume(VertexWrap pivot, List<VertexWrap> initialPoints)
        {
            var dim = initialPoints.Count;
            var m = _nDMatrix;

            for (int i = 0; i < dim; i++)
            {
                var pts = initialPoints[i];
                for (int j = 0; j < dim; j++) m[i, j] = pts.PositionData[j] - pivot.PositionData[j];
            }

            return Math.Abs(StarMath.StarMath.DeterminantDestructive(m, dim));
        }

        List<VertexWrap> FindExtremes()
        {
            var extremes = new List<VertexWrap>(2 * _dimension);

            int vCount = _inputVertices.Count;
            for (int i = 0; i < _dimension; i++)
            {
                double min = double.MaxValue, max = double.MinValue;
                int minInd = 0, maxInd = 0;
                for (int j = 0; j < vCount; j++)
                {
                    var v = _inputVertices[j].PositionData[i];
                    if (v < min)
                    {
                        min = v;
                        minInd = j;
                    }
                    if (v > max)
                    {
                        max = v;
                        maxInd = j;
                    }
                }

                if (minInd != maxInd)
                {
                    extremes.Add(_inputVertices[minInd]);
                    extremes.Add(_inputVertices[maxInd]);
                }
                else extremes.Add(_inputVertices[minInd]);
            }
            return extremes;
        }

        void ThrowSingular()
        {
            throw new InvalidOperationException(
                    "ConvexHull: Singular input data (i.e. trying to triangulate a data that contain a regular lattice of points).\n"
                    + "Introducing some noise to the data might resolve the issue.");
        }

        void FindConvexHull(Boolean removeDuplicates)
        {
            // Find the (dimension+1) initial points and create the simplexes.
            InitConvexHull(removeDuplicates);

            // Expand the convex hull and faces.
            while (_unprocessedFaces.First != null)
            {
                var currentFace = _unprocessedFaces.First;
                _currentVertex = currentFace.FurthestVertex;
                _convexHull.Add(_currentVertex);
                UpdateCenter();

                // The affected faces get tagged
                TagAffectedFaces(currentFace);

                // Create the cone from the currentVertex and the affected faces horizon.
                CreateCone();

                // Need to reset the tags
                int count = _affectedFaceBuffer.Count;
                for (int i = 0; i < count; i++) _affectedFaceBuffer[i].Tag = 0;
            }
        }

        private void SortAndRemoveRepeats()
        {
            var vertexSort = new VertexSort(_dimension);
            /* the reason for sorting is that it is a tried and true technique which  likely has a lower time
             * complexity than the brute force approach to detecting duplicates (nested for-loops...clearly O(n^2)). 
             * So, first we sort and then we can quickly remove the duplicates. During the compare function, 
             * we make note of any duplicates that exist. */
            _inputVertices.Sort(vertexSort);
            var dupes = vertexSort.Duplicates;
            dupes.Sort(vertexSort);
            /* now the list of InputVertices has been sorted and the duplicates as well. All that is left is
             * to go through InputVerties and remove all duplicates. This could actually be done in a single
             * line of LINQ code, but it is believed that this verbose method, which takes advantage of the 
             * fact that both lists are now sorted is much quick and should have a time complexity much less
             * than O(n). */
            var lowerBound = 0;
            var upperBound = _inputVertices.Count - 1;
            while (dupes.Count > 0)
            {
                var dupe = dupes[0];
                var index = (upperBound - lowerBound) / dupes.Count;
                /* the idea here is that the best guess for the index is the fraction through the list. For example,
                 * if InputVertices is 1000 and there are 5 duplicates - and since both lists are sorted - the first
                 * duplicate is around 200. It doesn't matter if this is too high, the binary search can go in either 
                 * direction. */
                while (upperBound > lowerBound)
                {
                    if (index == 0) upperBound = 0;
                    else if (vertexSort.Compare(dupe, _inputVertices[index]) == 1)
                    {
                        lowerBound = index;
                        index = (lowerBound + upperBound) / 2;
                        if (index == lowerBound) index++;
                    }
                    else if (vertexSort.Compare(dupe, _inputVertices[index]) == 0)
                    {
                        if (vertexSort.Compare(dupe, _inputVertices[index - 1]) == 1)
                        {
                            lowerBound = upperBound = index;
                        }
                        else
                        {
                            upperBound = index;
                            index = (lowerBound + upperBound) / 2;
                        }
                    }

                    else if (vertexSort.Compare(dupe, _inputVertices[index]) == -1)
                    {
                        if (vertexSort.Compare(dupe, _inputVertices[index + 1]) == 0)
                        {
                            lowerBound = upperBound = index = index + 1;
                        }
                        else
                        {
                            upperBound = index;
                            index = (lowerBound + upperBound) / 2;
                        }
                    }
                }
                while (index < _inputVertices.Count - 1 && vertexSort.Compare(_inputVertices[index], _inputVertices[index + 1]) == 0)
                    _inputVertices.RemoveAt(index);
                dupes.RemoveAt(0);
                lowerBound = index;
                upperBound = _inputVertices.Count - 1;
            }
            /* now, need to reset the InputVertices. */
            for (int i = 0; i < _inputVertices.Count; i++)
            {
                _inputVertices[i].Index = i;
                _inputVertices[i].Marked = false;
            }
        }

        private ConvexHullInternal(IEnumerable<IVertex> vertices)
        {
            _originalInputVertices = new List<VertexWrap>(vertices.Select((v, i) => new VertexWrap { Vertex = v, PositionData = v.Position, Index = i }));
            _dimension = DetermineDimension();
            Initialize();
        }

        IEnumerable<TVertex> GetConvexHullInternal<TVertex>(bool onlyCompute = false) where TVertex : IVertex
        {
            if (_computed) return onlyCompute ? null : _convexHull.Select(v => (TVertex)v.Vertex).ToArray();

            if (_dimension < 2) throw new ArgumentException("Dimension of the input must be 2 or greater.");
            try
            {
                _inputVertices = new List<VertexWrap>(_originalInputVertices);
                FindConvexHull(false);
                /* first attempt assumes that the input does not have duplicate vertices. */
            }
            catch (InvalidOperationException)
            {
                /* if the code throws this error then it resulted from a singularity in the data. This
                 * may have been caused by duplicate vertices. So, now we re-run the FindConvexHull routine
                 * by first removing duplicates. */
                _inputVertices = new List<VertexWrap>(_originalInputVertices);
                Initialize();
                FindConvexHull(true);
            }
            _computed = true;
            return onlyCompute ? null : _convexHull.Select(v => (TVertex)v.Vertex).ToArray();
        }

        IEnumerable<TFace> GetConvexFacesInternal<TVertex, TFace>()
            where TFace : ConvexFace<TVertex, TFace>, new()
            where TVertex : IVertex
        {
            if (!_computed) GetConvexHullInternal<TVertex>(true);

            var faces = _convexFaces;
            int cellCount = faces.Count;
            var cells = new TFace[cellCount];

            for (int i = 0; i < cellCount; i++)
            {
                var face = faces[i];
                var vertices = new TVertex[_dimension];
                for (int j = 0; j < _dimension; j++) vertices[j] = (TVertex)face.Vertices[j].Vertex;
                cells[i] = new TFace
                {
                    Vertices = vertices,
                    Adjacency = new TFace[_dimension],
                    Normal = face.Normal
                };
                face.Tag = i;
            }

            for (int i = 0; i < cellCount; i++)
            {
                var face = faces[i];
                var cell = cells[i];
                for (int j = 0; j < _dimension; j++)
                {
                    if (face.AdjacentFaces[j] == null) continue;
                    cell.Adjacency[j] = cells[face.AdjacentFaces[j].Tag];
                }

                // Fix the vertex orientation.
                if (face.IsNormalFlipped)
                {
                    var tempVert = cell.Vertices[0];
                    cell.Vertices[0] = cell.Vertices[_dimension - 1];
                    cell.Vertices[_dimension - 1] = tempVert;

                    var tempAdj = cell.Adjacency[0];
                    cell.Adjacency[0] = cell.Adjacency[_dimension - 1];
                    cell.Adjacency[_dimension - 1] = tempAdj;
                }
            }

            return cells;
        }

        internal static List<ConvexFaceInternal> GetConvexFacesInternal<TVertex, TFace>(IEnumerable<TVertex> data)
            where TFace : ConvexFace<TVertex, TFace>, new()
            where TVertex : IVertex
        {
            var ch = new ConvexHullInternal(data.Cast<IVertex>());
            ch.GetConvexHullInternal<TVertex>(true);
            return ch._convexFaces;
        }

        internal static Tuple<IEnumerable<TVertex>, IEnumerable<TFace>> GetConvexHullAndFaces<TVertex, TFace>(IEnumerable<IVertex> data)
            where TFace : ConvexFace<TVertex, TFace>, new()
            where TVertex : IVertex
        {
            var ch = new ConvexHullInternal(data);
            return Tuple.Create(
                ch.GetConvexHullInternal<TVertex>(),
                ch.GetConvexFacesInternal<TVertex, TFace>());
        }
    }
}
