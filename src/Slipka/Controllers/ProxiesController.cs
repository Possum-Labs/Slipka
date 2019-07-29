using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Slipka.ApiArguments;
using Slipka.Configuration;
using Slipka.DomainObjects;
using Slipka.Proxy;
using Slipka.Repositories;
using Slipka.ValueObjects;

namespace Slipka.Controllers
{
    [Produces("application/json")]
    [Route("api/Proxies")]
    public class ProxiesController : Controller
    {
        public ProxiesController(ProxySettings settings, ProxyStore store, IFileRepository fileRepository, IMessageRepository messageRepository)
        {
            Settings = settings;
            Store = store;
            FileRepository = fileRepository;
            MessageRepository = messageRepository;
            Random = new Random(Guid.NewGuid().GetHashCode());
            ReservedPorts = Enumerable.Range(Settings.FirstPort, Settings.LastPort - Settings.FirstPort).ToArray();
        }

        private ProxySettings Settings { get; }
        private ProxyStore Store { get; }
        private IFileRepository FileRepository { get; }
        private IMessageRepository MessageRepository { get; }
        private int[] ReservedPorts { get; }
        private Random Random { get; }

        // POST: api/Proxies
        [HttpPost]
        public ActionResult<Session> Post([FromBody] CreateProxyMessage value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var open = value.OpenFor == null ? Settings.DefaultOpenFor : TimeSpan.Parse(value.OpenFor);
            var retained = value.RetainedFor == null ? Settings.DefaultRetainedFor : TimeSpan.Parse(value.RetainedFor);

            var session = new Session
            {
                TargetHost = value.TargetHost,
                TargetPort = value.TargetPort ?? 80,
                LeaveProxyOpenUntil = DateTime.UtcNow.Add(open > Settings.MaxOpenFor ? Settings.MaxOpenFor : open),
                RetainDataUntil = DateTime.UtcNow.Add(retained > Settings.MaxRetainedFor ? Settings.MaxRetainedFor : retained)
            };

            if (value.TaggedCalls != null)
                session.TaggedCalls.AddRange(value.TaggedCalls.Select(x => x.AsCallTemplate));
            if (value.RecordedCalls != null)
                session.RecordedCalls.AddRange(value.RecordedCalls.Select(x => x.AsCallTemplate));
            if (value.InjectedCalls != null)
                session.InjectedCalls.AddRange(value.InjectedCalls.Select(x => x.AsCallTemplate));
            if (value.Decorations != null)
                session.Decorations.AddRange(value.Decorations.Select(x => x.AsHeader));

            session.InternalId = new MongoDB.Bson.ObjectId();
            session.Calls = new List<Call>();

            Slipka.Proxy.Proxy proxy;
            lock (ReservedPorts)
            {
                var retries = 0;
                while (retries < 10)
                {
                    retries++;

                    session.ProxyPort = GetNewPort(session);
                    proxy = new Slipka.Proxy.Proxy(session, FileRepository, MessageRepository, Store.SaveSession);
                    try
                    {
                        Store.Add(proxy);
                        proxy.Init(); // possibly not thread safe
                        return session;
                    }
                    catch (Exception e)
                    {
                        Store.Remove(proxy.Session.Id);
                        Console.WriteLine($"try {retries}, Failed to start proxy {e.Message}");
                    }
                }
                throw new Exception($"Failed to find a port in {retries} tries");
            }
        }

        private int GetNewPort(Session value)
        {
            var available = ReservedPorts.Except(Store.All.Select(x => x.ProxyPort)).ToList();

            var retries = 0;
            while (retries < 1000)
            {
                retries++;
                using (TcpClient tcpClient = new TcpClient())
                {
                    // check if the port happens to be in use
                    try
                    {
                        var port = available.ElementAt(Random.Next(available.Count()));
                        var result = tcpClient.BeginConnect("127.0.0.1", port, null, null);

                        var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(1000));

                        if (success)
                        {
                            available.RemoveAll(x => x == port);
                            tcpClient.EndConnect(result);
                            throw new Exception("in use");
                        }
                        return port;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"try {retries}, Port in use; available {available.Count} options message {e.Message}");
                    }
                }
            }
            throw new Exception($"Failed to start proxyt in {retries} tries");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            Store.Remove(id);
        }

        [HttpPut("{id}/record")]
        public ActionResult<Session> PutRecord(string id, [FromBody] CallTemplate call)
        {
            if (!SessionAvailableForModification(id, out var error, out Session session))
                return error;
            lock (session.RecordedCalls)
            {
                session.RecordedCalls.Add(call);
            }
            return Ok(session);
        }

        [HttpPut("{id}/inject")]
        public ActionResult<Session> PutInject(string id, [FromBody] CallTemplate call)
        {
            if (!SessionAvailableForModification(id, out var error, out Session session))
                return error;
            if (string.IsNullOrWhiteSpace(call.StatusCode))
                call.StatusCode = HttpStatusCode.OK.ToString();
            lock (session.InjectedCalls)
            {
                session.InjectedCalls.Add(call);
            }
            return Ok(session);
        }

        [HttpPut("{id}/tag")]
        public ActionResult<Session> PutTag(string id, [FromBody] CallTemplate call)
        {
            if (!SessionAvailableForModification(id, out var error, out Session session))
                return error;
            lock (session.TaggedCalls)
            {
                session.TaggedCalls.Add(call);
            }
            return Ok(session);
        }

        [HttpPut("{id}/decorate")]
        public ActionResult<Session> PutDecorate(string id, [FromBody] Header header)
        {
            if (!SessionAvailableForModification(id, out var error, out Session session))
                return error;
            lock (session.Decorations)
            {
                session.Decorations.Add(header);
            }
            return Ok(session);
        }

        private bool SessionAvailableForModification(string id, out ActionResult<Session> error, out Session session)
        {
            try
            {
                session = Store[id].Session;
            }
            catch (KeyNotFoundException)
            {
                session = null;
                error = new StatusCodeResult((int)System.Net.HttpStatusCode.NotFound);
                return false;
            }
            if (session.Active)
            {
                error = new StatusCodeResult((int)System.Net.HttpStatusCode.Conflict);
                return false;
            }
            error = null;
            return true;
        }
    }
}
