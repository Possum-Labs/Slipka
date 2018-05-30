using PossumLabs.Specflow.Core;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace LegacyTest.Framework
{
    [Binding]
    public class ValidationSteps: StepBase
    {
        public ValidationSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : 
            base(scenarioContext, featureContext)
        {
            ValidationFactory = new ValidationFactory(base.Interpeter);
        }

        private ValidationFactory ValidationFactory;

        [StepArgumentTransformation]
        public object Transform(string id) => 
            Interpeter.Resolve(id);

        [StepArgumentTransformation]
        public Validation[] Transform(Table table) => 
            table.Rows.SelectMany(r=>
                table.Header
                    .Where(h=>!String.IsNullOrWhiteSpace(r[h]))
                    .Select(h=>ValidationFactory.Create(r[h],h)))
            .ToArray();

        [StepArgumentTransformation]
        public Validation TransformValidation(string Constructor) => 
            ValidationFactory.Create(Constructor);

        [Then(@"'(.*)' has the values")]
        public void ThenTheCallHasTheValues(object o, Validation[] validations)
            => Validate(o, validations);

        [Then(@"'(.*)' has the value '(.*)'")]
        public void ThenTheCallHasTheValue(object o, Validation validations)
            => Validate(o, validations );

        private void Validate(object o, params Validation[] validations)
        {
            var failedVaidations = validations
                .Select(x => x.Validate(o))
                .Where(x => x != null)
                .ToArray();

            if (failedVaidations.Any())
                throw new AggregateException(failedVaidations.OrderBy(e=>e.Message.Length));
        }
    }
}
