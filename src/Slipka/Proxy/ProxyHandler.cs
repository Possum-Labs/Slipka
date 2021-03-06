﻿using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using Slipka.DomainObjects;
using Slipka.Repositories;
using Slipka.ValueObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Slipka.Proxy
{
    public class ProxyHandler : HttpMessageHandler
    {
        public ProxyHandler(Session session, IFileRepository fileRepository, IMessageRepository messageRepository)
        {
            From = new HostString("proxy", session.ProxyPort);
            Too = new HostString(session.TargetHost, session.TargetPort.Value);
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

        public event EventHandler<SessionEventArgs> ImportantDataAddedEvent;

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage incommingRequest, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = await CreateForwardableRequest(incommingRequest);
            var call = new Call
            {
                Uri = request.RequestUri,
                Method = request.Method.ToString()
            };

            lock (Session.Calls)
            {
                Session.Calls.Add(call);
                Session.Active = true;
            }

            var requestMessage = BuildMessage(request.Headers, request.Content);

            if (Injecting(call, requestMessage))
            {
                var responseTemplate = Matches(Session.InjectedCalls, call, requestMessage, Message.Empty, ignoreResponseAttributes:true).First();

                call.Injected = true;
                if (int.TryParse(responseTemplate.StatusCode, out var code))
                    call.StatusCode = code;
                call.Duration = responseTemplate.Duration;

                var response = new HttpResponseMessage(Enum.Parse<HttpStatusCode>(responseTemplate.StatusCode));

                if (responseTemplate.Response != null)
                {
                    var contentHeaders = responseTemplate.Response.Headers.Where(h => h.Key.ToUpper() == "Content-Type".ToUpper());
                    ParseContentHeaders(contentHeaders, out var encoding, out var mediaType);
                    if (responseTemplate.Response.Content != null)
                        response.Content = new StringContent(responseTemplate.Response.Content, encoding, mediaType);
                    else
                        response.Content = new StringContent(string.Empty, encoding, mediaType);

                    foreach (var h in responseTemplate.Response.Headers.Except(contentHeaders))
                        foreach (var i in h.Values)
                            response.Headers.Add(h.Key, i);
                }
                Tag(call, requestMessage);
                return await Task.Delay(responseTemplate.Duration ?? 1).ContinueWith((result) => response);
            }

            Decorate(request);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            return await Client.SendAsync(request, cancellationToken)
                .ContinueWith(responseTask =>
                {
                    stopwatch.Stop();
                    var response = responseTask.Result;
                    call.StatusCode = (int)response.StatusCode;
                    call.Duration = stopwatch.ElapsedMilliseconds;
                    var responseMessage = BuildMessage(response.Headers, response.Content);
                    if (Recording(call, requestMessage, responseMessage))
                    {
                        Record(request.Content, call, requestMessage, (id) => call.RequestId = id);
                        Record(response.Content, call, responseMessage, (id) => call.ResponseId = id);
                        call.Recorded = true;
                    }
                    Tag(call, requestMessage, responseMessage);
                    return response;
                });
        }

        private void ParseContentHeaders(IEnumerable<Header> contentHeaders, out Encoding encoding, out string mediaType)
        {
            encoding = Encoding.Default;
            mediaType = MediaTypeNames.Text.Plain;
            if (contentHeaders.None())
                return;
            var header = contentHeaders.First();
            var charsetRegex = new Regex("charset=(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var mediaTypeRegex = new Regex("[a-z]+/.*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            foreach (var v in header.Values)
            {
                if (charsetRegex.IsMatch(v))
                {
                    var e = charsetRegex.Match(v).Groups[1].Value.ToUpper().Replace("-","");
                    switch (e)
                    {
                        case "UTF8":
                            encoding = Encoding.UTF8;
                            break;
                        case "UTF7":
                            encoding = Encoding.UTF7;
                            break;
                        case "UTF32":
                            encoding = Encoding.UTF32;
                            break;
                        case "UNICODE":
                            encoding = Encoding.Unicode;
                            break;
                        case "BIGENDIANUNICODE":
                            encoding = Encoding.BigEndianUnicode;
                            break;
                        case "ASCII":
                            encoding = Encoding.ASCII;
                            break;
                    }
                }
                if (mediaTypeRegex.IsMatch(v))
                    mediaType = v;
            }
        }

        private void Tag(Call call, Message requestMessage, Message responseMessage = null)
        {
            var tags = Matches(Session.TaggedCalls, call, requestMessage, responseMessage ?? Message.Empty);
            if (tags.None())
                return;
            tags.ForEach(callTemplate => call.Tags = call.Tags.Union(callTemplate.Tags).ToList());

            lock (Session.Tags)
            {
                Session.Tags = Session.Tags.Union(call.Tags).ToList();
            }
              
            RaiseImportantDataAddedEvent();
        }

        private async Task<HttpRequestMessage> CreateForwardableRequest(HttpRequestMessage incommingRequest)
        {
            HttpRequestMessage request = new HttpRequestMessage();
 
            foreach (var h in incommingRequest.Headers)
                request.Headers.Add(h.Key, h.Value);

            if (incommingRequest.Content != null)
            {
                var ms = new MemoryStream();
                var text = await incommingRequest.Content.ReadAsStringAsync();
                await incommingRequest.Content.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;
                request.Content = new StreamContent(ms);

                if (incommingRequest.Content.Headers != null)
                    foreach (var h in incommingRequest.Content.Headers)
                        request.Content.Headers.Add(h.Key, h.Value);
            }

            request.Version = incommingRequest.Version;
            request.Method = incommingRequest.Method;
            request.RequestUri = incommingRequest.RequestUri;

            foreach (KeyValuePair<string, object> prop in incommingRequest.Properties)
                request.Properties.Add(prop);

            return request;
        }

        private void Decorate(HttpRequestMessage request)
            => Session.Decorations.ForEach(h => h.Values.ForEach(v => request.Headers.Add(h.Key, v)));

        private void Record(HttpContent content, Call call, Message message, Action<ObjectId> record)
        {
            if (content != null)
                content.ReadAsByteArrayAsync()
                    .ContinueWith(data => SaveMessage(message, record, data.Result));
            else
                SaveMessage(message, record);
        }

        private Message BuildMessage(HttpHeaders headers, HttpContent content)
        {
            var message = new Message
            {
                Headers = headers.Select(h => new Header(h.Key, h.Value)).ToList(),
                RetainDataUntil = Session.RetainDataUntil
            };
            if (content != null)
            {
                message.Headers.AddRange(
                            content.Headers.Select(h => new Header(h.Key, h.Value)));
            }
            return message;
        }

        protected void RaiseImportantDataAddedEvent()
            => ImportantDataAddedEvent(this, new SessionEventArgs(Session));

        private void SaveMessage(Message message, Action<ObjectId> record, byte[] content = null)
        {
            if(content!= null)
                FileRepository.Upload(content, Session)
                    .ContinueWith(uploadTask=> {
                        message.ContentId = uploadTask.Result;
                        MessageRepository.AddMessage(message)
                            .ContinueWith(messageTask =>
                            {
                                record(message.InternalId);
                                RaiseImportantDataAddedEvent();
                            }, TaskContinuationOptions.OnlyOnRanToCompletion)
                            .ContinueWith(t => { Console.WriteLine($"SaveMessage.1.{t.Exception}"); },
                                TaskContinuationOptions.OnlyOnFaulted);
                    }, TaskContinuationOptions.OnlyOnRanToCompletion)
                    .ContinueWith(t => { Console.WriteLine($"SaveMessage.2.{t.Exception}"); },
                        TaskContinuationOptions.OnlyOnFaulted);
            else
                MessageRepository.AddMessage(message)
                    .ContinueWith(task=> record(message.InternalId), TaskContinuationOptions.OnlyOnRanToCompletion)
                    .ContinueWith(t => { Console.WriteLine($"SaveMessage.3.{t.Exception}"); },
                        TaskContinuationOptions.OnlyOnFaulted);
        }

        private bool Recording(Call call, Message request, Message response = null) 
            => Matches(Session.RecordedCalls, call, request, response ?? Message.Empty).Any();

        private bool Injecting(Call call, Message request, Message response = null) 
            => Matches(Session.InjectedCalls, call, request, response ?? Message.Empty, ignoreResponseAttributes:true).Any();

        private IEnumerable<CallTemplate> Matches(List<CallTemplate> options, Call target, Message request, Message response, bool ignoreResponseAttributes = false)
        {
            return options.Where(
                c =>
                (c.Method == null || c.Method == target.Method) &&
                (c.Uri == null || new Regex(c.Uri, RegexOptions.IgnoreCase).IsMatch(target.Uri.AbsolutePath)) &&
                (ignoreResponseAttributes || c.Duration == null || (target.Duration != null && c.Duration<=target.Duration)) &&
                (ignoreResponseAttributes || c.StatusCode == null || new Regex(c.StatusCode, RegexOptions.IgnoreCase).IsMatch(target.StatusCode?.ToString())) &&
                (c.Request == null || c.Request.Headers == null || c.Request.Headers.Count == 0 || c.Request.Headers.Any(h =>
                    request.Headers.Any(t => t.Key == h.Key && t.Values.Intersect(h.Values).Any()))
                ) &&
                (ignoreResponseAttributes || c.Response == null || c.Response.Headers == null || c.Response.Headers.Count == 0 || c.Response.Headers.Any(h =>
                    response.Headers.Any(t => t.Key == h.Key && t.Values.Intersect(h.Values).Any()))
                )
                );
        }
    }
}
