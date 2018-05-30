using PossumLabs.Specflow.Core;
using PossumLabs.Specflow.Slipka;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace LegacyTest.Steps
{
    [Binding]
    public class CallSteps : RepositoryStepBase<Call>
    {
        public CallSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        {
        }
        

        protected override void Create(Call item)
        {
            throw new NotImplementedException();
        }

        [Given(@"the Calls?")]
        public void GivenTheCalls(Dictionary<string, Call> calls)
            => calls.Keys.ToList().ForEach(k => Add(k, calls[k]));

        [When(@"the Call '(.*)' is executed")]
        public void WhenTheCallIsExecuted(Call c)
        {
            Stopwatch stopWatch = new Stopwatch();
            var Client = new RestClient(c.Uri);
            var request = new RestRequest(c.Uri, c.Method.ToEnum<Method>());
            if (c.Request != null)
            {
                if (!string.IsNullOrWhiteSpace(c.Request.Content))
                    request.AddBody(c.Request.Content);
                foreach (var h in c.Request.Headers)
                    foreach (var v in h.Value)
                        request.AddHeader(h.Key, v);
            }
            stopWatch.Start();
            var response = Client.Execute(request);
            stopWatch.Stop();

            c.StatusCode = ((int)response.StatusCode).ToString();
            c.Response = new Message()
            {
                Content = response.Content,
                Headers = response.Headers.Select(h => new KeyValuePair<string, List<string>>(h.Name, new List<string>() { h.Value.ToString() })).ToList()
            };
            c.Duration = stopWatch.ElapsedMilliseconds;
        }
    }
}
