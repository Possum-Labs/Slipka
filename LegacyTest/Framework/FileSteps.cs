using PossumLabs.Specflow.Core.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace LegacyTest.Steps
{
    [Binding]
    public sealed class FileSteps : RepositoryStepBase<IFile>
    {
        public FileSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        {
        }

        [Then(@"the File '(.*)' has the content '(.*)'")]
        public void ThenTheFileHasTheContent(IFile file, string content)
        {
            //TODO: clean this up
            using (var s = file.Stream)
            {
                var r = new StreamReader(s);
                var fileContent = r.ReadToEnd();
                if (content != fileContent)
                    throw new Exception(
                        $"The File has content is different than the content provided; \n"+
                        $"The File has size: {fileContent.Length} vs provided : {content.Length} \n" +
                        $">>>File content: {fileContent} \n" +
                        $">>>Provided content: {content}");
            }
        }

        protected override void Create(IFile item)
        {
            throw new NotImplementedException();
        }
    }
}
