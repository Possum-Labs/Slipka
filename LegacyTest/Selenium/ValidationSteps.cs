using PossumLabs.Specflow.Core.Validations;
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
    public class ValidationSteps : WebDriverStepBase
    {
        public ValidationSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        { }

        [StepArgumentTransformation]
        public WebValidation TransformWebValidation(string Constructor) 
            => WebValidationFactory.Create(Constructor);

        [StepArgumentTransformation]
        public TableValidation TransformForHas(Table table) 
            => WebValidationFactory.Create(table.Rows.Select(r=>table.Header.ToDictionary(h=>h, h=> WebValidationFactory.Create(r[h]))).ToList());

        [Then(@"the element '(.*)' has the value '(.*)'")]
        public void ThenTheElementHasTheValue(Selector selector, WebValidation validation)
            => WebDriver.Select(selector).Validate(validation);

        [Then(@"the table contains")]
        public void ThenTheTableContains(TableValidation table)
            => WebDriver.Tables.Validate(table);

        [Then(@"the element '(.*)' is '(.*)'")]
        public void ThenTheElementIs(Selector selector, WebValidation validation)
            => WebDriver.Select(selector).Validate(validation);
    }
}
