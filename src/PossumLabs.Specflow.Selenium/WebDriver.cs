using OpenQA.Selenium;
using PossumLabs.Specflow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PossumLabs.Specflow.Selenium
{
    public class WebDriver
    {
        public WebDriver(IWebDriver driver, Func<Uri> rootUrl)
        {
            Driver = driver;
            SuccessfulSearchers = new List<Searcher>();
            RootUrl = rootUrl;
        }

        private Func<Uri> RootUrl {get;set;}
        private IWebDriver Driver { get; }
        private List<Searcher> SuccessfulSearchers { get; }

        //TODO: check this form
        public void NavigateTo(string url)
            => Driver.Navigate().GoToUrl(RootUrl().AbsoluteUri+"/"+url);

        public Element Select(Selector selector)
        {
            var loggingWebdriver = new LoggingWebDriver(Driver);
            foreach (var searcher in selector.PrioritizedSearchers)
            {
                var results = searcher.SearchIn(loggingWebdriver);

                if (results.One())
                {
                    SuccessfulSearchers.Add(searcher);
                    return results.First();
                }
                else if (results.Many())
                    throw new Exception($"Multiple results were found using {searcher.LogFormat()}");
            }
            throw new Exception($"element was not found; tried:\n{loggingWebdriver.GetLogs()}");
        }


        public IEnumerable<TableElement> Tables
            => Driver.FindElements(By.TagName("table")).Select(t => new TableElement(t, Driver));
    }
}
