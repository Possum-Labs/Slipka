using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Text;

namespace PossumLabs.Specflow.Selenium
{
    public class WebDriverManager
    {
        public WebDriver Current { get; set; }
        public Uri BaseUrl { get; set; }

        public Func<IWebDriver> Create => () =>
        {
            var options = new ChromeOptions();
            // https://stackoverflow.com/questions/22322596/selenium-error-the-http-request-to-the-remote-webdriver-timed-out-after-60-sec?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
            options.AddArgument("no-sandbox"); //might be a fix :/
            options.AddArgument("disable-popup-blocking");
            //TODO: Config value
            var driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options.ToCapabilities(), TimeSpan.FromSeconds(180));
            //do not change this, the site is a bloody nightmare with overlaying buttons etc.
            driver.Manage().Window.Size = new System.Drawing.Size(1440, 900);
            var allowsDetection = driver as IAllowsFileDetection;
            if (allowsDetection != null)
            {
                allowsDetection.FileDetector = new LocalFileDetector();
            }
            return driver;
        };
    }
}
