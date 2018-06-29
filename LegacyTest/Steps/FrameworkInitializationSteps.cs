using PossumLabs.Specflow.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace LegacyTest.Steps
{
    [Binding]
    public class FrameworkInitializationSteps:StepBase
    {
        public FrameworkInitializationSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        {

        }
        [BeforeScenario(Order = int.MinValue)]
        public void Setup()
        {
            base.Register(new PossumLabs.Specflow.Core.Variables.Interpeter());
            base.Register(new PossumLabs.Specflow.Core.Exceptions.ActionExecutor());
            base.Register((PossumLabs.Specflow.Core.Logging.ILog)new DefaultLogger(new DirectoryInfo(Environment.CurrentDirectory)));
        }
    }
}
