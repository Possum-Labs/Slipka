using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace PossumLabs.Specflow.Selenium
{
    public class Selector
    {
        public Selector(string constructor, By by)
        {
            Constructor = constructor;
            By = by;
        }

        public Selector(string label, List<Func<string, IWebDriver, IEnumerable<Element>>> sequencedByOrder)
        {
            Label = label;
            SequencedByOrder = sequencedByOrder;
        }

        private string Constructor { get; }
        private By By { get; }
        private string Label { get; }
        private List<Func<string, IWebDriver, IEnumerable<Element>>> SequencedByOrder { get; }

        internal IEnumerable<Searcher> PrioritizedSearchers
        {
            get
            {
                if (By != null)
                    return new[] {
                        new Searcher(
                            () => Constructor, 
                            (driver) => driver.FindElements(By).Select(element => new Element(element, driver)))
                    };
                else
                {
                    return SequencedByOrder
                        .Select(By => new Searcher(
                            () => Label,
                            (driver) => By(Label,driver)));
                }

            }
        }

   
    }
}

