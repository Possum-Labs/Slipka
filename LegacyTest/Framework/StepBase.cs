using PossumLabs.Specflow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace LegacyTest
{
    public abstract class StepBase
    {
        public StepBase(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            ScenarioContext = scenarioContext;
            FeatureContext = featureContext;
        }

        protected readonly ScenarioContext ScenarioContext;
        protected readonly FeatureContext FeatureContext;

        protected Interpeter Interpeter => ScenarioContext.Get<Interpeter>((typeof(Interpeter).FullName));

        internal void Register<T>(T item)
        {
            ScenarioContext.Add(typeof(T).FullName, item);
        }
    }
}
