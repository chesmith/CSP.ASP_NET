using System;
using System.IO;
using System.Web;

namespace CSP.ASP_NET
{
    public class CspHttpModule : IHttpModule
    {
        private string _nonce;
        private ContentLengthContainer _contentLengthContainer = new ContentLengthContainer();

        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
            context.PostReleaseRequestState += OnPostReleaseRequestState;

            _nonce = GenerateNonce();
        }

        public void Dispose()
        {
            // Cleanup code here, if needed
        }

        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;

            // Set your CSP policy with the nonce
            string cspPolicy = $"default-src 'self' 'nonce-{_nonce}'; script-src-attr 'unsafe-inline'; style-src 'self' 'unsafe-inline'";

            // Add CSP header to the response
            context.Response.Headers.Add("Content-Security-Policy", cspPolicy);
        }

        private string GenerateNonce()
        {
            using (var rngCryptoServiceProvider = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var nonceBytes = new byte[32];
                rngCryptoServiceProvider.GetBytes(nonceBytes);
                return Convert.ToBase64String(nonceBytes);
            }
        }

        private void OnPostReleaseRequestState(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;

            // Capture and modify the response content
            context.Response.Filter = new CaptureStreamFilter(context.Response.Filter, _nonce, _contentLengthContainer);
            //context.Response.Filter = CaptureResponseStream(context.Response, context.Response.Filter);
            System.Diagnostics.Debug.WriteLine(_contentLengthContainer.TotalContentLength);
            //context.Response.Headers["Content-Length"] = _contentLengthContainer.TotalContentLength.ToString();
        }

        //private Stream CaptureResponseStream(HttpResponse response, Stream originalStream)
        //{
        //    Stream captureStream = new MemoryStream();
        //    //response.Filter = new CaptureStreamFilter(originalStream, captureStream, _nonce);
        //    response.Filter = new SimpleStreamFilter(originalStream);
        //    return captureStream;
        //}
    }
}