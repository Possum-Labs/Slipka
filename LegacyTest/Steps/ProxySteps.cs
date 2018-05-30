using LegacyTest.Steps;
using PossumLabs.Specflow.Slipka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace LegacyTest
{
    [Binding]
    public class ProxySteps : RepositoryStepBase<ProxyWrapper>
    {
        public ProxySteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        {
        }

        protected override void Create(ProxyWrapper item)
        {
            //Constructor tackles this
        }

        [Given(@"the Slipka Prox(?:y|ies)")]
        public void GivenTheSlipkaProxyFor(Dictionary<string, ProxyWrapper> proxies)
            => proxies.Keys.ToList().ForEach(k=> Add(k, proxies[k]));

        [Given(@"the Proxy '(.*)' intercepts the calls")]
        public void GivenTheProxyInterceptsTheCalls(ProxyWrapper proxy, List<CallTemplate> calls)
            => calls.ForEach(c=> proxy.RegisterIntercept(c));

        [Given(@"the Proxy '(.*)' records the calls")]
        public void GivenTheProxyRecordsTheCalls(ProxyWrapper proxy, List<CallTemplate> calls)
            => calls.ForEach(c => proxy.RegisterRecording(c));

        [When(@"retrieving the recorded calls from Proxy '(.*)' as '(.*)'")]
        public void WhenRetrievingTheRecordedCallsFromProxyAs(ProxyWrapper proxy, string name)
            => Interpeter.Add(name, proxy.GetRecordedCalls());


    }
}
