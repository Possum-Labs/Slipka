using PossumLabs.Specflow.Core;
using PossumLabs.Specflow.Slipka;
using PossumLabs.Specflow.Slipka.ValueObjects;
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
    public class HeaderSteps : RepositoryStepBase<Header>
    {
        public HeaderSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        {
        }
    }
}
