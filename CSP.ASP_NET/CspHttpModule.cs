using System;
using System.IO;
using System.Web;

namespace CSP.ASP_NET
{
    public class CspHttpModule : IHttpModule
    {
        private string _nonce;

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
            //string cspPolicy = $"default-src 'self' 'nonce-{_nonce}'; script-src-attr 'unsafe-inline'; style-src 'self' 'unsafe-inline'";
            string cspPolicy = $"default-src 'self'; " +
				$"script-src 'self' 'unsafe-eval'; " +
				$"script-src-elem 'self' 'nonce-{_nonce}' www.scripthost.com; " +
				$"script-src-attr 'unsafe-inline'; " +
				$"style-src 'self'; " +
                $"style-src-elem 'self' 'nonce-{_nonce}' www.stylehost.com;" +
				$"style-src-attr 'unsafe-inline'; " +
				$"object-src 'none'; " +
                $"frame-ancestors 'self'; " +
                $"frame-src 'self';";

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
            context.Response.Filter = new CaptureStreamFilter(context.Response.Filter, _nonce);
        }
    }
}