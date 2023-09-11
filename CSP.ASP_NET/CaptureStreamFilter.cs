using System;
using System.IO;
using System.Text;

namespace CSP.ASP_NET
{
    public class CaptureStreamFilter : Stream
    {
        private Stream _originalStream;
        private string _nonce;
        private ContentLengthContainer _contentLengthContainer;

        public CaptureStreamFilter(Stream originalStream, string nonce, ContentLengthContainer contentLengthContainer)
        {
            _originalStream = originalStream;
            _nonce = nonce;
            _contentLengthContainer = contentLengthContainer;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            string inputstring = Encoding.UTF8.GetString(data);
            data = Encoding.UTF8.GetBytes(inputstring.Replace("{nonce}", _nonce));
            _originalStream.Write(data, 0, count);

            _contentLengthContainer.TotalContentLength += count;
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            _originalStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
    }

    public class ContentLengthContainer
    {
        public long TotalContentLength { get; set; }
    }
}