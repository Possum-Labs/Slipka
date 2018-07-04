using OpenQA.Selenium;
using PossumLabs.Specflow.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace LegacyTest.Selenium
{
    public abstract class WebDriverStepBase:StepBase
    {
        public WebDriverStepBase(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        { }

        protected WebDriver WebDriver => ScenarioContext.Get<WebDriverManager>((typeof(WebDriverManager).FullName)).Current;

        protected WebDriverManager WebDriverManager => ScenarioContext.Get<WebDriverManager>((typeof(WebDriverManager).FullName));
        protected WebValidationFactory WebValidationFactory => ScenarioContext.Get<WebValidationFactory>((typeof(WebValidationFactory).FullName));
        protected SelectorFactory SelectorFactory => ScenarioContext.Get<SelectorFactory>((typeof(SelectorFactory).FullName));
    }
}
