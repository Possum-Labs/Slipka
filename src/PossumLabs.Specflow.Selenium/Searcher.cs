using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace PossumLabs.Specflow.Selenium
{
    internal class Searcher
    {
        public Searcher(Func<string> messages, Func<IWebDriver, IEnumerable<Element>> search)
        {
            Search = search;
            Messages = messages;
        }

        public Func<IWebDriver, IEnumerable<Element>> Search { get; }
        private Func<string> Messages { get; }

        internal IEnumerable<Element> SearchIn(IWebDriver driver)
            => Search(driver);

        public string LogFormat()
            => Messages();
    }
}
