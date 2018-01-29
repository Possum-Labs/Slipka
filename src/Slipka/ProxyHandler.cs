using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Slipka
{
    public class ProxyHandler : HttpMessageHandler
    {
        public ProxyHandler(Session session)
        {
            From = new HostString("proxy",session.ProxyPort);
            Too = new HostString(session.TargetHost,session.TargetPort);
            handler = new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false };
            client = new HttpClient(handler);
            Session = session;
        }
        private Session Session;
        private HostString From;
        private HostString Too;

        private HttpClientHandler handler;
        private HttpClient client;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpRequestMessage msg = new HttpRequestMessage();
            foreach(var p in typeof(HttpRequestMessage).GetProperties().Where(x=>x.CanWrite))
                p.SetValue(msg, p.GetValue(request));
            foreach (var h in request.Headers)
                msg.Headers.Add(h.Key, h.Value);
            var start = DateTime.Now;
            var call = new Call { RequestUri = msg.RequestUri, Method = msg.Method.ToString() };
            Session.Calls.Add(call);
            return client.SendAsync(msg, cancellationToken)
                // TODO: make this optional
                .ContinueWith(x=> 
                {
                    //    if (x.Result.Content.Headers.ContentType != null &&
                    //        x.Result.Content.Headers.ContentType.MediaType != null &&
                    //        x.Result.Content.Headers.ContentType.MediaType.ToLower().Contains("text/html"))
                    //    {
                    //        Stream responseStream = responseStream = x.Result.Content.ReadAsStreamAsync().Result;
                    //        if (x.Result.Content.Headers.ContentEncoding.Any(y => y.ToLower().Contains("gzip")))
                    //            responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                    //        else if (x.Result.Content.Headers.ContentEncoding.Any(y => y.ToLower().Contains("deflate")))
                    //            responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

                    //        StreamReader Reader = new StreamReader(responseStream, Encoding.Default);

                    //        string html = Reader.ReadToEnd();

                    //        x.Result.Content = new StringContent(
                    //            html.Replace($"{Too.Host}:{Too.Port}", $"{From.Host}:{From.Port}"),
                    //            System.Text.Encoding.UTF8,
                    //            "text/html");
                    //    }
                    call.StatusCode = x.Result.StatusCode.ToString();
                    call.Duration = (DateTime.Now - start).TotalMilliseconds;

                    Trace.WriteLine($"duration: {(DateTime.Now - start).TotalMilliseconds} ms code:{x.Result.StatusCode} url:{msg.RequestUri}");
                    return x.Result;
                });
        }
    }
}
