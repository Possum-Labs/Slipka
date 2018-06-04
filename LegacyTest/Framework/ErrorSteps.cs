using PossumLabs.Specflow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace LegacyTest.Framework
{
    [Binding]
    public class ErrorSteps : StepBase
    {
        public ErrorSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        {

        }

        [Given(@"an error is expected")]
        public void GivenAnErrorIsExpected()
            => Executor.ExpectException = true;

        [Then(@"the Error has values")]
        public void ThenTheErrorHasValues(IEnumerable<Validation> validations)
        {
            if (Executor.Exception == null)
                throw new GherkinException("No excetion was caught.");
            Executor.Exception.Validate(validations);
        }

    }
}
