using PossumLabs.Specflow.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace LegacyTest.Selenium
{
    [Binding]
    public class FrameworkInitializationSteps : StepBase
    {
        public FrameworkInitializationSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        {
            WebDriverManager = new PossumLabs.Specflow.Selenium.WebDriverManager();
        }

        private PossumLabs.Specflow.Selenium.WebDriverManager WebDriverManager { get; }

        [BeforeScenario(Order = int.MinValue+1)]
        public void Setup()
        {
            base.Register(WebDriverManager);
            base.Register(new PossumLabs.Specflow.Selenium.WebValidationFactory(Interpeter));
            base.Register(new PossumLabs.Specflow.Selenium.SelectorFactory());
        }

        [BeforeScenario("SingleBrowser")]
        public void SetupBrowser()
        {
            WebDriverManager.Current = new WebDriver(WebDriverManager.Create(), ()=>WebDriverManager.BaseUrl);
        }
    }
}
