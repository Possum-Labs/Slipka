using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoDi;
using PossumLabs.Specflow.Core;
using TechTalk.SpecFlow;

namespace Slipka.Test.Steps
{
    [Binding]
    public class ResolvedStringSteps : StepBase
    {
        public ResolvedStringSteps(IObjectContainer objectContainer) : base(objectContainer)
        {
        }

        [StepArgumentTransformation]
        public ResolvedString Transform(string id) => Interpeter.Get<string>(id);


    }
}
