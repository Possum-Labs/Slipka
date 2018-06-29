using PossumLabs.Specflow.Slipka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TechTalk.SpecFlow;

namespace LegacyTest.Steps
{
    [Binding]
    public sealed class SlipkaSteps:StepBase
    {
        public SlipkaSteps(ScenarioContext scenarioContext, FeatureContext featureContext):base(scenarioContext, featureContext)
        {
            ProxySteps = new ProxySteps(scenarioContext, featureContext);
            ProxyAdministration = new Uri("http://localhost:4445");
            Wrapper = new ProxyWrapper(ProxyAdministration);
        }

        private Uri ProxyAdministration { get; }

        private ProxySteps ProxySteps { get; }

        private ProxyWrapper Wrapper { get; }

        [BeforeScenario("Slipka")]
        public void BeforeScenario()
        {
            Wrapper.Open(ProxyAdministration);
            Wrapper.RegisterRecording(new CallTemplate { Method = "POST" });
            Wrapper.RegisterRecording(new CallTemplate { Method = "PUT" });
            ProxySteps.AddDefault("Host", Wrapper.ProxyUri.ToString());
        }

        [AfterScenario("Slipka")]
        public void AfterScenario()
        {
            Wrapper.CloseAsync();

            var content = $"### Full session is availble at {ProxyAdministration}/Session/{Wrapper.Id}\n";
            //TODO: Better solution
            Thread.Sleep(1000);
            foreach (var call in Wrapper.GetCalls(recorded: true))
                content += call.ToHttpFormat();

            Log.Section("Slipka Logs", content);
        }
    }
}
