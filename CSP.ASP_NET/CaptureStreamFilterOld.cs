using System;
using System.IO;
using System.Text;

namespace CSP.ASP_NET
{
    public class CaptureStreamFilterOld : Stream
    {
        private Stream _originalStream;
        private Stream _captureStream;
        private string _nonce;

        public CaptureStreamFilterOld(Stream originalStream, Stream captureStream, string nonce)
        {
            _originalStream = originalStream;
            _captureStream = captureStream;
            _nonce = nonce;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // Capture the response content into a buffer
            byte[] captureBuffer = new byte[count];
            Array.Copy(buffer, offset, captureBuffer, 0, count);

            // Optionally, you can modify the response content here
            // For example, you can replace '{nonce}' with the actual nonce
            string content = Encoding.UTF8.GetString(captureBuffer);
            string modifiedContent = content.Replace("{nonce}", _nonce);

            // Convert the modified content back to bytes
            byte[] modifiedBuffer = Encoding.UTF8.GetBytes(modifiedContent);

            // Write the modified content to the capture stream
            _captureStream.Write(modifiedBuffer, 0, modifiedBuffer.Length);

            // Write the original content to the original stream
            _originalStream.Write(buffer, offset, count);
        }

        public override bool CanRead { get { return /*false*/ true; } }

        public override bool CanSeek { get { return /*false*/ true; } }

        public override bool CanWrite { get { return true; } }

        public override long Length { get { return 0; } } // => throw new NotSupportedException();

        public override long Position
        {
            get { return 0; }// => throw new NotSupportedException();
            set { int i = 0; } // => throw new NotSupportedException();
        }

        public override void Flush()
        {
            _captureStream.Flush();
            _originalStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _originalStream.Read(buffer, offset, count);
            //throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _originalStream.Seek(offset, origin);
            //throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            _originalStream.SetLength(Length);
            //throw new NotSupportedException();
        }
    }
}