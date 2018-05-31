using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Slipka
{
    public class ProxyHandler : HttpMessageHandler
    {
        public ProxyHandler(Session session, IFileRepository fileRepository, IMessageRepository messageRepository)
        {
            From = new HostString("proxy", session.ProxyPort);
            Too = new HostString(session.TargetHost, session.TargetPort);
            Handler = new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false };
            Client = new HttpClient(Handler);
            Session = session;
            FileRepository = fileRepository;
            MessageRepository = messageRepository;
        }

        private Session Session { get; }
        private HostString From { get; }
        private HostString Too { get; }
        private HttpClientHandler Handler { get; }
        private HttpClient Client { get; }
        private IFileRepository FileRepository { get; }
        private IMessageRepository MessageRepository { get; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpRequestMessage msg = new HttpRequestMessage();
            foreach (var p in typeof(HttpRequestMessage).GetProperties().Where(x => x.CanWrite))
                p.SetValue(msg, p.GetValue(request));
            foreach (var h in request.Headers)
                msg.Headers.Add(h.Key, h.Value);
            var start = DateTime.Now;
            var call = new Call
            {
                Uri = msg.RequestUri,
                Method = msg.Method.ToString()
            };

            Session.Calls.Add(call);

            var requestMessage = BuildMessage(msg.Headers, msg.Content);

            if (Intercepting(call, requestMessage))
            {
                call.Intercepted = true;
                var responseTemplate = Matches(Session.InterceptedCalls, call, requestMessage, Message.Empty);
                var response = new HttpResponseMessage(Enum.Parse<HttpStatusCode>(responseTemplate.StatusCode));
                if (responseTemplate.Response != null)
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
                var task = new Task<HttpResponseMessage>(() =>
                {
                    Thread.Sleep(responseTemplate.Duration ?? 1);
                    return response;
                });
                task.Start();
                return task;
            }

            return Client.SendAsync(msg, cancellationToken)
                .ContinueWith(responseTask =>
                {
                    var response = responseTask.Result;
                    call.StatusCode = (int)response.StatusCode;
                    call.Duration = (DateTime.Now - start).TotalMilliseconds;
                    var responseMessage = BuildMessage(response.Headers, response.Content);
                    if (Recording(call, requestMessage, responseMessage))
                    {
                        Record(msg.Content, call, requestMessage, (id) => call.RequestId = id);
                        Record(response.Content, call, responseMessage, (id) => call.ResponseId = id);
                        call.Recorded = true;
                    }

                    Trace.WriteLine($"duration: {(DateTime.Now - start).TotalMilliseconds} ms code:{response.StatusCode} url:{msg.RequestUri}");
                    return response;
                });
        }

        private void Record(HttpContent content, Call call, Message message, Action<ObjectId> record)
        {
            if (content != null)
                content.ReadAsByteArrayAsync().ContinueWith(data => SaveMessage(message, record, data.Result));
            else
                SaveMessage(message, record);
        }

        private Message BuildMessage(HttpHeaders headers, HttpContent content)
        {
            var message = new Message
            {
                Headers = headers.Select(h => new Header(h.Key, h.Value)).ToList(),
            };
            if (content != null)
            {
                message.Headers.AddRange(
                            content.Headers.Select(h => new Header(h.Key, h.Value)));
            }
            return message;
        }

        private void SaveMessage(Message message, Action<ObjectId> record, byte[] content = null)
        {
            if(content!= null)
                FileRepository.Upload(content, Session)
                    .ContinueWith(uploadTask=> {
                        message.Content = uploadTask.Result;
                        MessageRepository.AddMessage(message)
                            .ContinueWith(messageTask => record(message.InternalId));
                    });
            else
            MessageRepository.AddMessage(message)
                .ContinueWith(task=> record(message.InternalId));
        }


        private bool Recording(Call call, Message request, Message response = null) 
            => Matches(Session.RecordedCalls, call, request, response ?? Message.Empty) != null;

        private bool Intercepting(Call call, Message request, Message response = null) 
            => Matches(Session.InterceptedCalls, call, request, response ?? Message.Empty) != null;

        private CallTemplate Matches(List<CallTemplate> options, Call target, Message request, Message response)
        {
            return options.FirstOrDefault(
                c =>
                ((c.Uri == null) || (new Regex(c.Uri, RegexOptions.IgnoreCase).IsMatch(target.Uri.AbsolutePath))) &&
                (c.Request == null || c.Request.Headers == null || c.Request.Headers.Count == 0 || c.Request.Headers.Any(h =>
                    request.Headers.Any(t => t.Key == h.Key && t.Values.Intersect(h.Values).Any()))
                ) &&
                (c.Response == null || c.Response.Headers == null || c.Response.Headers.Count == 0 || c.Response.Headers.Any(h =>
                    response.Headers.Any(t => t.Key == h.Key && t.Values.Intersect(h.Values).Any()))
                )
                );
        }
    }
}
