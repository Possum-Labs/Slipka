using PossumLabs.Specflow.Core;
using PossumLabs.Specflow.Slipka;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace LegacyTest.Steps
{
    [Binding]
    public class CallTemplateSteps : RepositoryStepBase<CallTemplate>
    {
        public CallTemplateSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        {
        }
        

        protected override void Create(CallTemplate item)
        {
            throw new NotImplementedException();
        }
    }
}
