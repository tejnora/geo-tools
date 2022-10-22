using System;
using System.IO;
using GeoHelper.Utils;

namespace Utils.PartialStream
{
    public interface IStreamWithParent
    {
        Stream GetParentStream();
    }


    public class PartialStreamFactory
    {
        public static Stream CreateReadPartialStream(Stream stream)
        {
            var size = (uint) stream.ReadInt32();
            return new PartialStream(stream, stream.Position, size);
        }

        public static Stream CreateWritePartialStream(Stream stream)
        {
            stream.WriteInt32(0);
            return new UpdateSizeStream(stream);
        }

        class PartialStream : Stream, IStreamWithParent
        {
            readonly uint _size;
            readonly long _startPosition;
            Stream _s;

            // Methods
            public PartialStream(Stream stream, long startPosition, uint size)
            {
                if (stream == null)
                {
                    throw new ArgumentNullException("stream");
                }
                if (!stream.CanSeek)
                {
                    throw new InvalidDataException();
                }
                _s = stream;
                _startPosition = startPosition;
                _size = size;
            }

            public override bool CanRead
            {
                get { return (_s != null && _s.CanRead); }
            }

            public override bool CanSeek
            {
                get { return ((_s != null) && _s.CanSeek); }
            }

            public override bool CanWrite
            {
                get { return ((_s != null) && _s.CanWrite); }
            }

            public override long Length
            {
                get { return _size; }
            }

            public override long Position
            {
                get { return (_s.Position - _startPosition); }
                set
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    if (value > _startPosition + _size)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    _s.Seek(_startPosition + value, SeekOrigin.Begin);
                }
            }

            public Stream GetParentStream()
            {
                return _s;
            }

            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (disposing && (_s != null))
                    {
                        _s.Seek(_startPosition + _size, SeekOrigin.Begin);
                    }
                }
                finally
                {
                    _s = null;
                }
            }

            public override void Flush()
            {
                _s.Flush();
            }

            public override int Read(byte[] array, int offset, int count)
            {
                return _s.Read(array, offset, count);
            }

            public override int ReadByte()
            {
                return _s.ReadByte();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                if (origin == SeekOrigin.Begin)
                    return _s.Seek(_startPosition + offset, origin);
                if (origin == SeekOrigin.Current)
                    return _s.Seek(offset, origin);
                if (origin == SeekOrigin.End)
                    return _s.Seek(_startPosition + _size - offset, SeekOrigin.Begin);
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                _s.SetLength(value);
            }

            public override void Write(byte[] array, int offset, int count)
            {
                _s.Write(array, offset, count);
            }

            public override void WriteByte(byte value)
            {
                _s.WriteByte(value);
            }

            // Properties
        }

        class UpdateSizeStream : Stream, IStreamWithParent
        {
            readonly long _updatePosition;
            Stream _s;

            // Methods
            public UpdateSizeStream(Stream stream)
            {
                if (stream == null)
                {
                    throw new ArgumentNullException("stream");
                }
                if (!stream.CanSeek)
                {
                    throw new InvalidDataException();
                }
                _s = stream;
                _updatePosition = _s.Position;
            }

            public override bool CanRead
            {
                get { return (_s != null && _s.CanRead); }
            }

            public override bool CanSeek
            {
                get { return ((_s != null) && _s.CanSeek); }
            }

            public override bool CanWrite
            {
                get { return ((_s != null) && _s.CanWrite); }
            }

            public override long Length
            {
                get { return _s.Length; }
            }

            public override long Position
            {
                get { return (_s.Position); }
                set
                {
                    if (value < _updatePosition)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    _s.Seek(value, SeekOrigin.Begin);
                }
            }

            public Stream GetParentStream()
            {
                return _s;
            }

            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (disposing && (_s != null))
                    {
                        long position = _s.Position;
                        _s.Seek(_updatePosition - 4, SeekOrigin.Begin);
                        _s.WriteInt32((int) (position - _updatePosition));
                        _s.Position = position;
                        _s.Flush();
                    }
                }
                finally
                {
                    _s = null;
                }
            }

            public override void Flush()
            {
                _s.Flush();
            }

            public override int Read(byte[] array, int offset, int count)
            {
                return _s.Read(array, offset, count);
            }

            public override int ReadByte()
            {
                return _s.ReadByte();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                if (origin == SeekOrigin.Begin)
                    return _s.Seek(_updatePosition + offset, origin);
                return _s.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _s.SetLength(value);
            }

            public override void Write(byte[] array, int offset, int count)
            {
                _s.Write(array, offset, count);
            }

            public override void WriteByte(byte value)
            {
                _s.WriteByte(value);
            }

            // Properties
        }
    }
}