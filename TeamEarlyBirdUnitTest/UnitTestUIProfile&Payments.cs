using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TeamEarlyBirdUnitTest
{
    [TestClass]
    internal class UnitTestUIDashboard_Payments
    {
        [TestMethod]
        public void StudentCanAccessPayment_UI_TEST()
        {
            //Create a driver for Chrome
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl("https://lmsearlybird20240227112424.azurewebsites.net/Account/Login");

            // elements for logging in
            IWebElement username = driver.FindElement(By.Id("EmailAddress"));
            IWebElement password = driver.FindElement(By.Id("Password"));
            IWebElement loginBtn = driver.FindElement(By.XPath("//button[@class='btn btn-primary' and text()='Login']"));

            // login
            username.SendKeys("billy@joe.com");
            password.SendKeys("Pass!@#123");
            loginBtn.Click();

            // element for making payment page
            IWebElement makePayment = driver.FindElement(By.XPath("//button[@class='btn btn-primary' and text()='MakePayment']"));
            
            // make payment access steps
            makePayment.Click();

            // elements for payment
            IWebElement paymentAmount = driver.FindElement(By.Id("PaymentAmount"));
            IWebElement paymentButton = driver.FindElement(By.XPath("//input[@class='btn btn-outline-success float-right']"));

            // payment
            paymentAmount.SendKeys("1");
            paymentButton.Click();

            // element for clicking checkout
            IWebElement checkout = driver.FindElement(By.XPath("//button[@class='btn btn-primary' and text()='Make Payment']"));
            checkout.Click();

            // elements for credit card
            IWebElement cardNumber = driver.FindElement(By.Id("CardNumber"));
            IWebElement expiryDate = driver.FindElement(By.Id("ExpiryDate"));
            IWebElement securityCode = driver.FindElement(By.Id("SecurityCode"));
            IWebElement nameOnCard = driver.FindElement(By.Id("NameOnCard"));
            IWebElement submitBtn = driver.FindElement(By.XPath("//input[@class='btn btn-outline-success float-right']"));

            // credit card
            cardNumber.SendKeys("4242424242424242");
            expiryDate.SendKeys("12/24");
            securityCode.SendKeys("123");
            nameOnCard.SendKeys("Billy Joe");
            submitBtn.Click();

            Assert.IsTrue(driver.Url.Contains("Success"));

            // Close the browser
            driver.Quit();
        }

        [TestMethod]
        public void StudentCanGoToEditProfile_UI_TEST()
        {
            //Create a driver for Chrome
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl("https://lmsearlybird20240227112424.azurewebsites.net/Account/Login");

            // login elements
            IWebElement username = driver.FindElement(By.Id("EmailAddress"));
            IWebElement password = driver.FindElement(By.Id("Password"));
            IWebElement loginBtn = driver.FindElement(By.XPath("//button[@class='btn btn-primary' and text()='Login']"));

            // login
            username.SendKeys("billy@joe.com");
            password.SendKeys("Pass!@#123");
            loginBtn.Click();

            // profile display element
            IWebElement profileDisplay = driver.FindElement(By.XPath("//a[@class='btn btn-primary' and text()='Profile']"));
            profileDisplay.Click();

            // edit profile element
            IWebElement editProfile = driver.FindElement(By.XPath("//a[@class='btn btn-primary' and text()='Edit']"));
            editProfile.Click();

            Assert.IsTrue(driver.Url.Contains("EditProfile"));

            // Close the browser
            driver.Quit();
        }
    }
}
