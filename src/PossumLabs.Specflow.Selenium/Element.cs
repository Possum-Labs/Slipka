using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using PossumLabs.Specflow.Core.Validations;

namespace PossumLabs.Specflow.Selenium
{
    public class Element
    {
        private IWebElement WebElement { get; }
        private IWebDriver WebDriver { get; }

        public Element(IWebElement element, IWebDriver driver)
        {
            WebElement = element;
            WebDriver = driver;
        }

        public string Tag => WebElement.TagName;
        public IEnumerable<string> Classes => WebElement.GetAttribute("class").Split(' ').Select(x=>x.Trim());
        public string Id => WebElement.GetAttribute("id");
        public virtual List<string> Values => new List<string>() { WebElement.GetAttribute("value"), WebElement.Text };

        public void Select()
            => WebElement.Click();

        //https://www.grazitti.com/resources/articles/automating-different-input-fields-using-selenium-webdriver/

        public virtual void Enter(string text)
        {
            if(WebElement.TagName == "select")
            {
                var select = new SelectElement(WebElement);
                try
                {
                    select.SelectByText(text);
                    return;
                }
                catch
                {
                    select.DeselectByValue(text);
                    return;
                }
            }
            else if (WebElement.TagName == "input" && !String.IsNullOrEmpty(WebElement.GetAttribute("list")))
            {
                var select = new SelectElement(WebElement);
                try
                {
                    select.SelectByText(text);
                    return;
                }
                catch
                {
                    select.DeselectByValue(text);
                    return;
                }
            }
            //TODO: v2 Radio buttons
            //TODO: v2 Check Boxes
            WebElement.Clear();
            WebElement.SendKeys(text);
        }

        public void Click()
            => WebElement.Click();
    }
}
