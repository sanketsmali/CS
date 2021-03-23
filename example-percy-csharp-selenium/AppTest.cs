using System;
using System.Collections.Generic;

using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using percy_csharp_selenium;

namespace example_percy_csharp_selenium
{

    /**
    * Unit test for example App.
    */
    public class AppTest
    {
        private static readonly String TEST_URL = "http://localhost:8000";
        private static IWebDriver driver;
        private static Percy percy;
        private static TestServer server;

        public void StartAppAndOpenBrowser()
        {
            server = new TestServer();
            server.StartTestServer();
            // Create a headless Chrome browser.
            // ChromeOptions options = new ChromeOptions();
            // options.AddArguments("--headless");
            // driver = new ChromeDriver(options);
            OpenQA.Selenium.Chrome.ChromeOptions capability = new OpenQA.Selenium.Chrome.ChromeOptions();
            capability.AddAdditionalCapability("os_version", "10", true);
            capability.AddAdditionalCapability("resolution", "1920x1080", true);
            capability.AddAdditionalCapability("browser", "Chrome", true);
            capability.AddAdditionalCapability("browser_version", "87.0", true);
            capability.AddAdditionalCapability("os", "Windows", true);
            capability.AddAdditionalCapability("name", "BStack-[C_sharp] Sample Test", true); // test name
            capability.AddAdditionalCapability("build", "BStack Build Number 1", true); // CI/CD job or build name
            capability.AddAdditionalCapability("browserstack.user", "sanketmali4", true);
            capability.AddAdditionalCapability("browserstack.key", "XaqpcHttuyFzXSzC3uNM", true);
            driver = new RemoteWebDriver(
               new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capability);
            //driver = new ChromeDriver(options);
            percy = new Percy(driver);
        }

        public void closeBrowser()
        {
            // Close our test browser.
            driver.Quit();
            server.StopTestServer();
        }

        public void loadsHomePage()
        {
            driver.Navigate().GoToUrl(TEST_URL);
            IWebElement element = driver.FindElement(By.ClassName("todoapp"));
            Assert.IsNotNull(element);
            // Take a Percy snapshot.
            percy.Snapshot("Home Page");
        }

        public void acceptsANewTodo()
        {
            driver.Navigate().GoToUrl(TEST_URL);

            // We start with zero todos.
            var todoEls = driver.FindElements(By.CssSelector(".todo-list li"));
            Assert.AreEqual(0, todoEls.Count);
            // Add a todo in the browser.
            IWebElement newTodoEl = driver.FindElement(By.ClassName("new-todo"));
            newTodoEl.SendKeys("A new fancy todo!");
            newTodoEl.SendKeys(Keys.Return);

            // Now we should have 1 todo.
            todoEls = driver.FindElements(By.CssSelector(".todo-list li"));
            Assert.AreEqual(1, todoEls.Count);
            // Take a Percy snapshot specifying browser widths.
            percy.Snapshot("One todo", new List<int> { 768, 992, 1200 });
            driver.FindElement(By.ClassName("toggle")).Click();
            driver.FindElement(By.ClassName("clear-completed")).Click();
        }

        public void letsYouCheckOffATodo()
        {
            driver.Navigate().GoToUrl(TEST_URL);

            IWebElement newTodoEl = driver.FindElement(By.ClassName("new-todo"));
            newTodoEl.SendKeys("A new todo to check off");
            newTodoEl.SendKeys(Keys.Return);

            IWebElement todoCountEl = driver.FindElement(By.ClassName("todo-count"));
            Assert.AreEqual("1 item left", todoCountEl.Text);

            driver.FindElement(By.ClassName("toggle")).Click();

            todoCountEl = driver.FindElement(By.ClassName("todo-count"));
            Assert.AreEqual("0 items left", todoCountEl.Text);

            // Take a Percy snapshot specifying a minimum height.
            percy.Snapshot("Checked off todo", null, 1200, false, ".clear-completed { visibility: hidden; }");

        }

    }

}