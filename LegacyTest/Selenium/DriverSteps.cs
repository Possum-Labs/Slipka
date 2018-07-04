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
    public class DriverSteps: WebDriverStepBase
    {
        public DriverSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        { }

        [StepArgumentTransformation]
        public Selector TransformSelector(string Constructor)
            => SelectorFactory.Create(Constructor);

        [When(@"clicking the element '(.*)'")]
        public void WhenClickingTheElement(Selector selector)
            => WebDriver.Select(selector).Click();

        [When(@"selecting the element '(.*)'")]
        public void WhenSelectingTheElement(Selector selector)
            => WebDriver.Select(selector).Select();

        [When(@"entering '(.*)' for the element '(.*)'")]
        public void WhenEnteringForTheElement(Selector selector, string text)
            => WebDriver.Select(selector).Enter(text);
    }
}
