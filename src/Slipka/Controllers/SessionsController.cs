using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;

namespace Slipka.Controllers
{
    [Route("api/Sessions")]
    public class SessionsController : Controller
    {
        public SessionsController(ISessionRepository sessionRepository, IFileRepository fileRepository, IMessageRepository messageRepository)
        {
            SessionRepository = sessionRepository;
            FileRepository = fileRepository;
            MessageRepository = messageRepository;
        }

        private ISessionRepository SessionRepository { get; }
        private IFileRepository FileRepository { get; }
        private IMessageRepository MessageRepository { get; }

        private async Task<ActionResult> BuildResponseFromMessage(Message message)
        {
            if (message == null)
                return new StatusCodeResult((int)System.Net.HttpStatusCode.NotFound);

            
            var data = await FileRepository.Download(message.ContentId);
            
            var contentTypes = message.Headers.Where(x => x.Key == "Content-Type");

            var BadTypes = message.Headers.Where(x => x.Key == "Transfer-Encoding");
            
            foreach (var h in message.Headers.Except(contentTypes).Except(BadTypes))
                Response.Headers.Add(new KeyValuePair<string, StringValues>(h.Key, new StringValues(h.Values.ToArray())));
          
            return File(data,
                contentTypes.Any() ?
                contentTypes.First().Values.First() :
                "application/x-binary");
        }

        private async Task<Message> getMessageFromSession(Session session, int number, Func<Call, ObjectId> selector)
            => await MessageRepository.GetMessage(selector(session.Calls[number]));

        private async Task<ActionResult> SimulateMessage(string id, int number, Func<Call, ObjectId> selector)
        {
            var session = await SessionRepository.GetSession(id);
            if (session == null || session.Calls.Count <= number)
                return new StatusCodeResult((int)System.Net.HttpStatusCode.NotFound);
            
            var message = await getMessageFromSession(session, number, selector);
            
            var response = await BuildResponseFromMessage(message);
            
            return response;
        }

        [HttpGet("{id}/request/{number}")]
        public async Task<ActionResult> GetRequest(string id, int number)
            => await SimulateMessage(id, number, call => call.RequestId);

        [HttpGet("{id}/response/{number}")]
        public async Task<ActionResult> GetResponse(string id, int number)
             => await SimulateMessage(id, number, call => call.ResponseId);
    }
}