using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using PossumLabs.Specflow.Core;
using System.Collections.ObjectModel;

namespace PossumLabs.Specflow.Selenium
{
    public class LoggingWebDriver :IWebDriver
    {
        public LoggingWebDriver(IWebDriver driver)
        {
            Driver = driver;
            Messages = new List<string>();
        }

        private List<string> Messages { get; }
        public string Url { get => Driver.Url; set => Driver.Url=value; }

        public string Title => Driver.Title;

        public string PageSource => Driver.PageSource;

        public string CurrentWindowHandle => Driver.CurrentWindowHandle;

        public ReadOnlyCollection<string> WindowHandles => Driver.WindowHandles;

        private IWebDriver Driver;
        public string GetLogs()=> Messages.LogFormat();

        public void Close() => Driver.Close();

        public void Quit() => Driver.Quit();

        public IOptions Manage() => Driver.Manage();

        public INavigation Navigate() => Driver.Navigate();

        public ITargetLocator SwitchTo() => Driver.SwitchTo();

        public IWebElement FindElement(By by)
        {
            Messages.Add(by.ToString());
            return Driver.FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            Messages.Add(by.ToString());
            return Driver.FindElements(by);
        }

        public void Dispose() => Driver.Dispose();
    }
}
