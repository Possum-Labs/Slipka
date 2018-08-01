using OpenQA.Selenium;
using PossumLabs.Specflow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PossumLabs.Specflow.Selenium
{
    class SelectElement : Element
    {
        private Dictionary<string, IWebElement> Options { get; }

        public SelectElement(IWebElement element, IWebDriver driver) : base(element, driver)
        {
            if (element.TagName == "select")
            {
                OldStyleSelect = new OpenQA.Selenium.Support.UI.SelectElement(element);
                AvailableOptions = OldStyleSelect.Options;
                SelectedOptions = OldStyleSelect.AllSelectedOptions;
            }
            else
            {
                var listId = element.GetAttribute("list");
                AvailableOptions = driver.FindElements(By.XPath($"//datalist[@id='{listId}']/option"));
                SelectedOptions = new List<IWebElement>();
                var value = element.GetAttribute("value");
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SelectedOptions.Add(AvailableOptions.First(o => o.GetAttribute("value") == value));
                }
            }
        }

        private OpenQA.Selenium.Support.UI.SelectElement OldStyleSelect { get; }
        private IList<IWebElement> AvailableOptions { get; }
        private IList<IWebElement> SelectedOptions { get; }

        public override void Enter(string text)
        {
            if (OldStyleSelect != null)
            {
                var options = AvailableOptions.Where(o => 
                    string.Equals(o.Text, text, ComparisonDefaults.StringComparison) || 
                    string.Equals(o.GetAttribute("value"), text, ComparisonDefaults.StringComparison));
                if (options.One())
                    OldStyleSelect.SelectByText(options.First().Text);
                else if (options.Many())
                        throw new GherkinException("too many matches"); //TODO: cleanup
                else
                    throw new GherkinException("no matches"); //TODO: cleanup
            }
            else
            {
                var options = AvailableOptions.Where(o => string.Equals(o.GetAttribute("value"), text, ComparisonDefaults.StringComparison));
                if (options.One())
                    WebElement.SendKeys(options.First().GetAttribute("value"));
                else if (options.Many())
                    throw new GherkinException("too many matches"); //TODO: cleanup
                else
                    throw new GherkinException("no matches"); //TODO: cleanup
            }
        }

        public override List<string> Values => SelectedOptions
            .SelectMany(x=>new List<string>() { x.Text, x.GetAttribute("value") })
            .Where(s=>!string.IsNullOrWhiteSpace(s))
            .ToList();
    }
}
