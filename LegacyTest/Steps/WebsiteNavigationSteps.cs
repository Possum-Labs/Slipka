using LegacyTest.Selenium;
using PossumLabs.Specflow.Slipka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace LegacyTest.Steps
{
    [Binding]
    class WebsiteNavigationSteps: WebDriverStepBase
    {
        public WebsiteNavigationSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        { }

        [Given(@"Navigated to the (Homepage|Create Proxy)")]
        public void GivenNavigatedTo(string page)
        {
            switch(page)
            {
                case "Homepage":
                    base.WebDriver.NavigateTo("/");
                    break;
                case "Create Proxy":
                    base.WebDriver.NavigateTo("/Create");
                    break;
                default:
                    throw new NotImplementedException($"the page {page} is not yet supported in navigation");
            } 
        }

        [Given(@"Navigated to the (Session) for Proxy (.*)")]
        public void GivenNavigatedTo(string page, ProxyWrapper proxy)
        {
            switch (page)
            {
                case "Session":
                    base.WebDriver.NavigateTo($"Session/{proxy.Id}");
                    break;
                default:
                    throw new NotImplementedException($"the page {page} is not yet supported in navigation");
            }
        }

    }
}
