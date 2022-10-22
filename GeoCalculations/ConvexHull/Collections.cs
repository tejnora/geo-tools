using System;

namespace GeoCalculations.ConvexHull
{
    internal sealed class VertexBuffer
    {
        int _capacity;
        int _count;
        VertexWrap[] _items;

        public int Count
        {
            get { return _count; }
        }

        public VertexWrap this[int i]
        {
            get { return _items[i]; }
        }

        void EnsureCapacity()
        {
            if (_count + 1 <= _capacity) return;
            if (_capacity == 0) _capacity = 4;
            else _capacity = 2 * _capacity;
            Array.Resize(ref _items, _capacity);
        }

        public void Add(VertexWrap item)
        {
            EnsureCapacity();
            _items[_count++] = item;
        }

        public void Clear()
        {
            _count = 0;
        }
    }

    internal sealed class FaceList
    {
        ConvexFaceInternal _first, _last;

        public ConvexFaceInternal First
        {
            get { return _first; }
        }

        void AddFirst(ConvexFaceInternal face)
        {
            face.InList = true;
            _first.Previous = face;
            face.Next = _first;
            _first = face;
        }

        public void Add(ConvexFaceInternal face)
        {
            if (face.InList)
            {
                //if (this.first.FurthestDistance < face.FurthestDistance)
                if (_first.VerticesBeyond.Count < face.VerticesBeyond.Count)
                {
                    Remove(face);
                    AddFirst(face);
                }
                return;
            }

            face.InList = true;

            //if (first != null && first.FurthestDistance < face.FurthestDistance)
            if (_first != null && _first.VerticesBeyond.Count < face.VerticesBeyond.Count)
            {
                _first.Previous = face;
                face.Next = _first;
                _first = face;
            }
            else
            {
                if (_last != null)
                {
                    _last.Next = face;
                }
                face.Previous = _last;
                _last = face;
                if (_first == null)
                {
                    _first = face;
                }
            }
        }

        public void Remove(ConvexFaceInternal face)
        {
            if (!face.InList) return;

            face.InList = false;

            if (face.Previous != null)
            {
                face.Previous.Next = face.Next;
            }
            else if ( /*first == face*/ face.Previous == null)
            {
                _first = face.Next;
            }

            if (face.Next != null)
            {
                face.Next.Previous = face.Previous;
            }
            else if ( /*last == face*/ face.Next == null)
            {
                _last = face.Previous;
            }

            face.Next = null;
            face.Previous = null;
        }
    }

    internal sealed class ConnectorList
    {
        FaceConnector _first, _last;
        public FaceConnector First
        {
            get { return _first; }
        }

        void AddFirst(FaceConnector connector)
        {
            _first.Previous = connector;
            connector.Next = _first;
            _first = connector;
        }

        public void Add(FaceConnector element)
        {
            if (_last != null)
            {
                _last.Next = element;
            }
            element.Previous = _last;
            _last = element;
            if (_first == null)
            {
                _first = element;
            }
        }

        public void Remove(FaceConnector connector)
        {
            if (connector.Previous != null)
            {
                connector.Previous.Next = connector.Next;
            }
            else if ( /*first == face*/ connector.Previous == null)
            {
                _first = connector.Next;
            }

            if (connector.Next != null)
            {
                connector.Next.Previous = connector.Previous;
            }
            else if ( /*last == face*/ connector.Next == null)
            {
                _last = connector.Previous;
            }

            connector.Next = null;
            connector.Previous = null;
        }
    }
}
