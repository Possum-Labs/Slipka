using BoDi;
using PossumLabs.DSL.Core;
using PossumLabs.DSL.Slipka;
using PossumLabs.DSL.Slipka.ValueObjects;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Slipka.Test.English
{
    [Binding]
    public class SessionSteps : RepositoryStepBase<Session>
    {
        public SessionSteps(IObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }
}
