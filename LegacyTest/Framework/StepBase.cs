using PossumLabs.Specflow.Core;
using PossumLabs.Specflow.Core.Exceptions;
using PossumLabs.Specflow.Core.Logging;
using PossumLabs.Specflow.Core.Variables;
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
        protected ActionExecutor Executor => ScenarioContext.Get<ActionExecutor>((typeof(ActionExecutor).FullName));
        protected ILog Log => ScenarioContext.Get<ILog>((typeof(ILog).FullName));

        internal void Register<T>(T item)
        {
            ScenarioContext.Add(typeof(T).FullName, item);
        }
    }
}
