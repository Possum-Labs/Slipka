﻿using PossumLabs.Specflow.Core;
using PossumLabs.Specflow.Slipka;
using PossumLabs.Specflow.Slipka.ValueObjects;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace LegacyTest.Steps
{
    [Binding]
    public class CallCollectionSteps : RepositoryStepBase<CallCollection>
    {
        public CallCollectionSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        {
        }

        protected override void Create(CallCollection l)
        {
            throw new NotImplementedException();
        }

        //TODO: cleanup
        new public void Add(string key, CallCollection value) 
            => base.Add(key, value);
    }
}