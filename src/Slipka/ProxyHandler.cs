using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Slipka
{
    public class ProxyHandler : HttpMessageHandler
    {
        public ProxyHandler(Session session, IFileRepository fileRepository)
        {
            From = new HostString("proxy",session.ProxyPort);
            Too = new HostString(session.TargetHost,session.TargetPort);
            handler = new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false };
            client = new HttpClient(handler);
            Session = session;
            FileRepository = fileRepository;
        }
        private Session Session;
        private HostString From;
        private HostString Too;

        private HttpClientHandler handler;
        private HttpClient client;
        private IFileRepository FileRepository;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpRequestMessage msg = new HttpRequestMessage();
            foreach(var p in typeof(HttpRequestMessage).GetProperties().Where(x=>x.CanWrite))
                p.SetValue(msg, p.GetValue(request));
            foreach (var h in request.Headers)
                msg.Headers.Add(h.Key, h.Value);
            var start = DateTime.Now;
            var call = new Call {
                Uri = msg.RequestUri,
                Method = msg.Method.ToString(),
                Request = new Message
                {
                     Headers = msg.Headers.Select(x=> new KeyValuePair<string, List<string>>(x.Key, x.Value.ToList())).ToList(),
                }
            };
            if(msg.Content != null)
                call.Request.Headers.AddRange(
                            msg.Content.Headers.Select(y => new KeyValuePair<string, List<string>>(y.Key, y.Value.ToList())));

            Session.Calls.Add(call);

            if(Intercepting(call))
            {
                call.Intercepted = true;
                var responseTemplate = Matches(Session.InterceptedCalls, call);
                var response = new HttpResponseMessage(Enum.Parse<HttpStatusCode>(responseTemplate.StatusCode));
                if(responseTemplate.Response!=null)
                {
                    if (responseTemplate.Response.Content != null)
                        response.Content = new StringContent(responseTemplate.Response.Content);
                    else
                        response.Content = new StringContent(string.Empty);

                    foreach (var h in response.Headers)
                        foreach (var i in h.Value)
                            response.Headers.Add(h.Key, i);
                }
                
                //TOOD: there has to be a more elegant way to do this w/ tasks
                var task = new Task<HttpResponseMessage>(() => {
                    Thread.Sleep(responseTemplate.Duration ?? 1);
                    return response;
                });
                task.Start();
                return task;
            }

            return client.SendAsync(msg, cancellationToken)
                .ContinueWith(x=> 
                {
                    call.StatusCode = (int)x.Result.StatusCode;
                    call.Duration = (DateTime.Now - start).TotalMilliseconds;
                    call.Response = new Message
                    {
                        Headers = x.Result.Headers.Select(y => new KeyValuePair<string, List<string>>(y.Key, y.Value.ToList())).ToList()
                    };

                    if (x.Result.Content != null)
                        call.Response.Headers.AddRange(
                                    x.Result.Content.Headers.Select(y => new KeyValuePair<string, List<string>>(y.Key, y.Value.ToList())));

                    if (Recording(call))
                    {
                        call.Recorded = true;
                        if(msg.Content!=null)
                            call.Request.Content = FileRepository.Upload(msg.Content.ReadAsByteArrayAsync().Result, Session);
                        if (x.Result.Content != null)
                            call.Response.Content = FileRepository.Upload(x.Result.Content.ReadAsByteArrayAsync().Result, Session);
                    }

                    Trace.WriteLine($"duration: {(DateTime.Now - start).TotalMilliseconds} ms code:{x.Result.StatusCode} url:{msg.RequestUri}");
                    return x.Result;
                });
        }

        bool Recording(Call call) => Matches(Session.RecordedCalls, call) != null;

        bool Intercepting(Call call) => Matches(Session.InterceptedCalls, call) != null;


        CallTemplate Matches(List<CallTemplate> options, Call target)
        {
            return options.FirstOrDefault(
                c =>
                ((c.Uri == null) || (new Regex(c.Uri, RegexOptions.IgnoreCase).IsMatch(target.Uri.AbsolutePath))) &&
                (c.Request == null || c.Request.Headers == null || c.Request.Headers.Count == 0 || c.Request.Headers.Any(h =>
                    target.Request.Headers.Any(t => t.Key == h.Key && t.Value.Intersect(h.Value).Any()))
                ) &&
                (c.Response == null || c.Response.Headers == null || c.Response.Headers.Count == 0 || c.Response.Headers.Any(h =>
                    target.Response.Headers.Any(t => t.Key == h.Key && t.Value.Intersect(h.Value).Any()))
                )
                );
        }
    }
}
