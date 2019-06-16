using BoDi;
using PossumLabs.Specflow.Core.Variables;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace Slipka.Test.Steps
{
    public class Fake :IValueObject { }

    [Binding]
    public class NullSteps : RepositoryStepBase<Fake>
    {
        public NullSteps(IObjectContainer objectContainer) : base(objectContainer)
        {
        }

        [BeforeScenario]
        public void Seed()
        {
            this.Add("null", null);
        }
    }
}
