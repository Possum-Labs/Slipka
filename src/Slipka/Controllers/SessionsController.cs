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
        private readonly ISessionRepository _sessionRepository;
        private readonly IFileRepository _fileRepository;

        public SessionsController(ISessionRepository sessionRepository, IFileRepository fileRepository)
        {
            _sessionRepository = sessionRepository;
            _fileRepository = fileRepository;
        }
        
        [HttpGet]
        public async Task<IEnumerable<Session>> Get()
        {
            return await _sessionRepository.GetAllSessions();
        }
        
        [HttpGet("{id}")]
        public async Task<Session> GetById(string id)
        {
            return await _sessionRepository.GetSession(id);
        }

        [HttpGet("{id}/response/{number}")]
        public async Task<ActionResult> GetResponse(string id, int number)
        {
            return await _sessionRepository.GetSession(id).ContinueWith(session =>
            {
                var call = session.Result.Calls[number].Response;
                var data = _fileRepository.Download(call.Content);

                var contentTypes = call.Headers.Where(x => x.Key == "Content-Type");

                foreach (var h in call.Headers.Except(contentTypes))
                    Response.Headers.Add(new KeyValuePair<string, StringValues>(h.Key, new StringValues(h.Value.ToArray())));

                return File(data,
                    contentTypes.Any() ?
                    contentTypes.First().Value.First() :
                    "application/x-binary") as ActionResult;
            });
        }

        [HttpGet("{id}/request/{number}")]
        public async Task<ActionResult> GetRequest(string id, int number)
        {
            return await _sessionRepository.GetSession(id).ContinueWith(session =>
            {
                var call = session.Result.Calls[number].Request;
                var data = _fileRepository.Download(call.Content);

                var contentTypes = call.Headers.Where(x => x.Key == "Content-Type");

                foreach (var h in call.Headers.Except(contentTypes))
                    Response.Headers.Add(new KeyValuePair<string, StringValues>(h.Key, new StringValues(h.Value.ToArray())));

                return File(data,
                    contentTypes.Any() ?
                    contentTypes.First().Value.First() :
                    "application/x-binary") as ActionResult;
            });
        }
    }
}