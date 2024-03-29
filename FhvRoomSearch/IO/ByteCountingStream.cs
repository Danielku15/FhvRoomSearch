﻿using System.IO;

namespace FhvRoomSearch.IO
{
    /// <summary>
    /// This stream counts the amount of bytes read and written to track 
    /// the uploaded/downloaded bytes
    /// </summary>
    class ByteCountingStream : Stream
    {
        private readonly Stream _stream;

        public long ReadByteCount
        {
            get;
            private set;
        }
        public long WrittenByteCount
        {
            get;
            private set;
        }

        public ByteCountingStream(Stream stream)
        {
            _stream = stream;
        }


        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = _stream.Read(buffer, offset, count);
            ReadByteCount += read;
            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WrittenByteCount += count;
            _stream.Write(buffer, offset, count);
        }

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }
    }
}
