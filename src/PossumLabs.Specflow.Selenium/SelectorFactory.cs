using OpenQA.Selenium;
using PossumLabs.Specflow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PossumLabs.Specflow.Selenium
{
    public class SelectorFactory
    {
        public Selector Create(string constructor)
        {
            if (Parser.IsId.IsMatch(constructor))
                return new Selector(constructor, By.Id(Parser.IsId.Match(constructor).Groups[1].Value));
            if (Parser.IsElement.IsMatch(constructor))
                return new Selector(constructor, By.TagName(Parser.IsElement.Match(constructor).Groups[1].Value));
            if (Parser.IsClass.IsMatch(constructor))
                return new Selector(constructor, By.ClassName(Parser.IsClass.Match(constructor).Groups[1].Value));
            return new Selector(constructor, SequencedByOrder);
        }

        virtual protected List<Func<string, IWebDriver, IEnumerable<Element>>> SequencedByOrder
           => new List<Func<string, IWebDriver, IEnumerable<Element>>>
           {
                    ByForAttribute,
                    ByNestedInLabel,
                    ByNested,
                    ByText,
                    ByTitle,
                    ByLabelledBy,
                    RadioByName,
                    SpecialButtons
           };

        //https://w3c.github.io/using-aria/

        //<label for="female">target</label>
        //label[@for and text()='{target}']
        virtual protected Func<string, IWebDriver, IEnumerable<Element>> ByForAttribute =>
            (target, driver) =>
            {
                var elements = driver.FindElements(By.XPath($"//label[@for and text()={target.XpathEncode()}]"));
                if (elements.Any())
                    return elements.SelectMany(e => driver.FindElements(By.Id(e.GetAttribute("for")))).Select(e => new Element(e, driver));
                return new Element[] { };
            };
        
        //label[text()[normalize-space(.)='Bob']]/*[self::input]
        //<label>target<input type = "text" ></ label >
        //label[text()='{target}']/*[self::input or self::textarea or self::select]
        virtual protected Func<string, IWebDriver, IEnumerable<Element>> ByNestedInLabel =>
            (target, driver) => driver
            .FindElements(By.XPath($"//label[text()[normalize-space(.)={target.XpathEncode()}]]/*[self::input or self::textarea or self::select or self::button]"))
            .Select(e => new Element(e, driver));

        virtual protected Func<string, IWebDriver, IEnumerable<Element>> SpecialButtons =>
            (target, driver) => driver
            .FindElements(By.XPath($"//*[@type={target.XpathEncode()} and (@type='submit' or @type='reset') and (self::input or self::button)]"))
            .Select(e => new Element(e, driver));

        //<input aria-label="target">
        //*[(self::a or self::button or @role='button' or @role='link' or @role='menuitem' or self::input or self::textarea or self::select) and @aria-label='{target}']
        virtual protected Func<string, IWebDriver, IEnumerable<Element>> ByNested =>
            (target, driver) => driver
            .FindElements(By.XPath($"//*[(self::a or self::button or self::input or self::textarea or self::select or @role='button' or @role='link' or @role='menuitem' ) and (text()={target.XpathEncode()} or label[text()={target.XpathEncode()}] or @aria-label={target.XpathEncode()} or (@type='radio' and @value={target.XpathEncode()}))]"))
            .Select(e => new Element(e, driver));

        //<a href = "https://www.w3schools.com/html/" >target</a>
        //*[(self::a or self::button or @role='button' or @role='link' or @role='menuitem') and text()='{target}']
        virtual protected Func<string, IWebDriver, IEnumerable<Element>> ByText =>
            (target, driver) => driver
            .FindElements(By.XPath($"//*[(self::a or self::button or @role='button' or @role='link' or @role='menuitem') and text()={target.XpathEncode()}]"))
            .Select(e => new Element(e, driver));

        //<a href = "https://www.w3schools.com/html/" title="target">Visit our HTML Tutorial</a>
        //a[@title='{target}']
        virtual protected Func<string, IWebDriver, IEnumerable<Element>> ByTitle =>
            (target, driver) => driver.FindElements(By.XPath($"//a[@title={target.XpathEncode()}]")).Select(e => new Element(e, driver));

        //<input type="radio" id="i1" name="target"
        virtual protected Func<string, IWebDriver, IEnumerable<Element>> RadioByName =>
            (target, driver) =>
            {
                var elements = driver.FindElements(By.XPath($"//input[@type='radio' and @name='{target}']"));
                if (elements.Any())
                    return new Element[] { new RadioElement(elements, driver) };
                return new Element[] { };
            };

        //<input aria-labelledby= "l1 l2 l3"/>
        //*[(self::a or self::button or @role='button' or @role='link' or @role='menuitem' or self::input or self::textarea or self::select) and  @aria-labelledby]
        virtual protected Func<string, IWebDriver, IEnumerable<Element>> ByLabelledBy =>
            (target, driver) =>
            {
                var elements = driver.FindElements(By.XPath($"//*[(self::a or self::button or @role='button' or @role='link' or @role='menuitem' or self::input or self::textarea or self::select) and  @aria-labelledby]"));
                if (elements.Any())
                {
                    return elements.Where(e =>
                    {
                        var ids = e.GetAttribute("aria-labelledby").Split(' ').Select(s => s.Trim()).Where(s => !String.IsNullOrEmpty(s));
                        var labels = ids.SelectMany(id => driver.FindElements(By.Id(id))).Select(l => l.Text);
                        var t = target;
                        foreach (var l in labels)
                            t = t.Replace(l, string.Empty);
                        return string.IsNullOrWhiteSpace(t);
                    }).Select(e => new Element(e, driver));
                }
                return new Element[] { };
            };
    }
}
