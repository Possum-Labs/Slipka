using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace PossumLabs.Specflow.Selenium
{
    public class TableElement
    {
        public TableElement(IWebElement table, IWebDriver driver)
        {
            RootElement = table;
            Driver = driver;

            Columns = new List<string>();
            Rows = new List<Dictionary<string, Element>>();
            Header = new Dictionary<string, Element>();

            Setup();
        }

        private IWebElement RootElement { get; }
        private IWebDriver Driver { get; }
        public List<Dictionary<string, Element>> Rows { get; }
        public Dictionary<string, Element> Header { get; }

        private List<string> Columns { get; }

        private void Setup()
        {
            var rows = RootElement.FindElements(By.TagName("tr"));
            var firstRow = rows.Take(1).First();
            firstRow.FindElements(By.XPath("//*[self::td or self::th]")).ToList().ForEach(e => Header.Add(e.Text, CreateElement(e)));

            var rowElements = rows.AsParallel().Select(r => new { row = r, elements = r.FindElements(By.TagName("td")) });

            foreach(var r in rowElements)
            {
                var row = Header.Keys.AsParallel().ToDictionary(
                    column => column, 
                    column => CreateElement(r.elements[Header.Keys.ToList().IndexOf(column)]));
                Rows.Add(row);
            }            
        }

        private Element CreateElement(IWebElement root)
        {
            var e = root;
            var kids = e.FindElements(By.XPath("//*[self::a or self::button or @role='button' or @role='link' or @role='menuitem' or self::input or self::textarea or self::select or self::label]"));
            if (kids.Any())
                e = kids.First();
            return new Element(e, Driver);
        }
    }
}
