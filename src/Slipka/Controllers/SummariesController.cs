using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Slipka.Controllers
{
    [Produces("application/json")]
    [Route("api/Summaries")]
    public class SummariesController : Controller
    {
        // GET: api/Summary
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Summary/Tag/5
        [HttpGet("Tag/{tag}", Name = "Get")]
        public string Get(string tag)
        {
            return "value";
        }
    }
}
