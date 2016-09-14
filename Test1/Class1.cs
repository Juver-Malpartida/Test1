using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Test1
{
    public class Class1
    {
        NgWebDriver driver;
        IWebDriver _driver;
        ITakesScreenshot takesScreenshot;
        WebDriverWait wait;
        string urlBase, userName, passWord;

        [SetUp]
        public void Initial()
        {
            urlBase = "http://portal-dr.epiq11.com/#/search/searchcases";
            userName = "amer\\vsanchez";
            passWord = "Welcome123Epiq!";
            _driver = new ChromeDriver();
            driver = new NgWebDriver(_driver);
            takesScreenshot = _driver as ITakesScreenshot;
            wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(60));
            driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(60));
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(urlBase);
            driver.FindElement(By.Id("sign-in")).Click();
            driver.FindElement(By.Id("user-name-field")).SendKeys(userName);
            driver.FindElement(By.Id("pass-field")).SendKeys(passWord);
            driver.FindElement(By.CssSelector("input[value='Sign In']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("search-options-section")));
            driver.WaitForAngular();
        }

        [Test]
        public void Test1()
        {
            By by = By.CssSelector("a[ng-bind-html='caseName']");
            var casesCount = driver.FindElements(by).Count;
            for (int i = 0; i < casesCount; i++)
            {
                var caseElement = driver.FindElements(by)[i];
                var caseName = caseElement.Text;

                caseElement.Click();
                waitForPage();
                takeScreenShot(caseName);

                Console.WriteLine("********** Case Name " + caseName);
                if (driver.FindElements(By.Id("iframeContent")).Count > 0)
                {
                    driver.SwitchTo().Frame("iframeContent");
                    var links = _driver.FindElements(By.CssSelector("#page a"));
                    foreach (var link in links)
                    {
                        Console.WriteLine("Link Text[" + link.Text + "] HREF[" + link.GetAttribute("href") + "]");
                    }
                    driver.SwitchTo().DefaultContent();
                }
                driver.Navigate().GoToUrl(urlBase);
            }
        }

        [TearDown]
        public void Final()
        {
            driver.Quit();
        }

        private string cleanString(string name)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            string str = rgx.Replace(name, "");
            return str;
        }

        private void takeScreenShot(string caseName)
        {
            var caseNameClean = cleanString(caseName);
            string fileName = Path.Combine("C:\\evidence\\screenshots", caseNameClean + ".jpg");
            takesScreenshot.GetScreenshot().SaveAsFile(fileName, ImageFormat.Jpeg);
        }

        private void waitForPage()
        {
            driver.WaitForAngular();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".in-case-search-bar-container")));
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".document-wrapper")));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(".card-loading-container")));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("spinner")));
            driver.WaitForAngular();
            Thread.Sleep(1000);
        }
    }
}
