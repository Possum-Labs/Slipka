using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace PossumLabs.Specflow.Selenium
{
    public class RadioElement : Element
    {
        private Dictionary<string, IWebElement> Options { get; }

        public RadioElement(ReadOnlyCollection<IWebElement> elements, IWebDriver driver): base(null,driver)
        {
            Options = new Dictionary<string, IWebElement>();
            foreach(var e in elements)
            {
                if (!string.IsNullOrWhiteSpace(e.GetAttribute("aria-labelledby")))
                {
                    var lables = e.GetAttribute("aria-labelledby").Split(' ').Select(id => driver.FindElement(By.Id(id)));
                    var text = lables.Select(l => l.Text).OrderBy(s => s).Aggregate((x, y) => x + " " + y);
                    Options.Add(text, e);
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(e.GetAttribute("aria-label")))
                {
                    Options.Add(e.GetAttribute("aria-label"), e);
                    continue;
                }
                var forLabels = driver.FindElements(By.XPath($"//label[@for='{e.GetAttribute("id")}']"));
                if(forLabels.Any())
                {
                    var lables = forLabels;
                    var text = lables.Select(l => l.Text).OrderBy(s => s).Aggregate((x, y) => x + " " + y);
                    Options.Add(text, e);
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(e.Text))
                {
                    Options.Add(e.Text, e);
                    continue;
                }
                throw new NotImplementedException("unknown labeling of radio buttons.");
            }
        }

        public override void Enter(string text)
        {
            if (Options.ContainsKey(text))
                Options[text].Click();
            //TODO: wonky order stuff
        }

        public override List<string> Values => new List<string>(Options.Keys.Where(k=> Options[k].GetAttribute("checked")=="true"));
        }
}
