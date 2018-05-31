using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Slipka.Controllers
{
    [Produces("application/json")]
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

        //TODO: make this async
        private Func<Task<Message>, ActionResult> SimulateMessage()
        {
            return messageTaskResults =>
            {
                var message = messageTaskResults.Result;
                var data = FileRepository.Download(message.Content).Result;
                var contentTypes = message.Headers.Where(x => x.Key == "Content-Type");

                foreach (var h in message.Headers.Except(contentTypes))
                    Response.Headers.Add(new KeyValuePair<string, StringValues>(h.Key, new StringValues(h.Values.ToArray())));

                return File(data,
                    contentTypes.Any() ?
                    contentTypes.First().Values.First() :
                    "application/x-binary") as ActionResult;
            };
        }

        [HttpGet("{id}/request/{number}")]
        public async Task<ActionResult> GetRequest(string id, int number)
        {
            return await SessionRepository.GetSession(id).ContinueWith(sessionTaskResults =>
            {
                var session = sessionTaskResults.Result;
                var call = session.Calls[number];
                return MessageRepository.GetMessage(call.RequestId).Result;
            }).ContinueWith(SimulateMessage());
        }

        [HttpGet("{id}/response/{number}")]
        public async Task<ActionResult> GetResponse(string id, int number)
        {
            return await SessionRepository.GetSession(id).ContinueWith(sessionTaskResults =>
            {
                var session = sessionTaskResults.Result;
                var call = session.Calls[number];
                return MessageRepository.GetMessage(call.ResponseId).Result;
            }).ContinueWith(SimulateMessage());
        }
    }
}