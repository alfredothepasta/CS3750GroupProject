using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TeamEarlyBirdUnitTest
{
    [TestClass]
    public class UnitTest_UI
    {
        [TestMethod]
        public void InstructorCanLogin_UI_TEST()
        {
            //Create a driver for Chrome
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl("https://lmsearlybird20240227112424.azurewebsites.net/Account/Login");

            IWebElement username = driver.FindElement(By.Id("EmailAddress"));
            IWebElement password = driver.FindElement(By.Id("Password"));
            IWebElement loginBtn = driver.FindElement(By.XPath("//button[@class='btn btn-primary' and text()='Login']"));

            username.SendKeys("prof@mail.com");
            password.SendKeys("Test1234*");

            loginBtn.Click();

            Assert.IsTrue(driver.Url.Contains("Dashboard"));

            // Close the browser
            driver.Quit();
        }
    }
}