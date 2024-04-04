using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;

namespace TeamEarlyBirdUnitTest
{
    [TestClass]
    public class UnitTestUIProfilePayments
    {
        const string URL = "https://localhost:7243";

        [TestMethod]
        public void StudentCanAccessPayment_UI_TEST()
        {
            //Create a driver for Chrome
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl(URL + "/Account/Login");

            // elements for logging in
            IWebElement username = driver.FindElement(By.Id("EmailAddress"));
            IWebElement password = driver.FindElement(By.Id("Password"));
            IWebElement loginBtn = driver.FindElement(By.XPath("//button[@class='btn btn-primary' and text()='Login']"));

            // login
            username.SendKeys("billy@joe.com");
            password.SendKeys("Pass!@#123");
            loginBtn.Click();

            // element for making payment page
            IWebElement makePayment = driver.FindElement(By.XPath("//a[@class='nav-link text-dark' and text()='Make Payment']"));

            // make payment access steps
            makePayment.Click();

            // elements for payment
            IWebElement paymentAmount = driver.FindElement(By.Id("PaymentAmount"));
            IWebElement paymentButton = driver.FindElement(By.XPath("//input[@class='btn btn-outline-success float-right']"));

            // payment
            paymentAmount.Clear();
            paymentAmount.SendKeys("1");
            paymentButton.Click();

            // element for clicking checkout
            IWebElement checkout = driver.FindElement(By.XPath("//button[@class='button' and text()='Make Payment']"));
            checkout.Click();

            // elements for credit card
            IWebElement email = driver.FindElement(By.Id("email"));
            IWebElement cardNumber = driver.FindElement(By.Id("cardNumber"));
            IWebElement expiryDate = driver.FindElement(By.Id("cardExpiry"));
            IWebElement securityCode = driver.FindElement(By.Id("cardCvc"));
            IWebElement nameOnCard = driver.FindElement(By.Id("billingName"));
            IWebElement postcode = driver.FindElement(By.Id("billingPostalCode"));
            IWebElement checkbox = driver.FindElement(By.Id("enableStripePass"));
            //IWebElement submitBtn = driver.FindElement(By.XPath("//span[@class='SubmitButton-Text SubmitButton-Text--current Text Text-color--default Text-fontWeight--500 Text--truncate' and text()='Pay']"));
            // ^^ possibly being blocked by site security ^^ elements from stripe below
            // <span class="SubmitButton-Text SubmitButton-Text--current Text Text-color--default Text-fontWeight--500 Text--truncate" aria-hidden="false">Pay</span>


            // credit card
            email.SendKeys("billy@joe.com");
            cardNumber.SendKeys("4242424242424242");
            expiryDate.SendKeys("12/24");
            securityCode.SendKeys("555");
            nameOnCard.SendKeys("Billy Joe");
            postcode.SendKeys("55555");
            checkbox.Click();
            //submitBtn.Click();

            Assert.IsTrue(driver.Url.Contains("pay"));

            // Close the browser
            driver.Quit();
        }

        [TestMethod]
        public void StudentCanGoToEditProfile_UI_TEST()
        {
            //Create a driver for Chrome
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl(URL + "/Account/Login");

            // login elements
            IWebElement username = driver.FindElement(By.Id("EmailAddress"));
            IWebElement password = driver.FindElement(By.Id("Password"));
            IWebElement loginBtn = driver.FindElement(By.XPath("//button[@class='btn btn-primary' and text()='Login']"));

            // login
            username.SendKeys("billy@joe.com");
            password.SendKeys("Pass!@#123");
            loginBtn.Click();

            // profile display element
            IWebElement profileDisplay = driver.FindElement(By.XPath("//a[@class='nav-link text-dark' and text()='Profile']"));
            profileDisplay.Click();

            // edit profile element
            IWebElement editProfile = driver.FindElement(By.XPath("//a[@class='btn btn-outline-secondary' and text()='Edit']"));
            editProfile.Click();

            Assert.IsTrue(driver.Url.Contains("EditProfile"));

            // Close the browser
            driver.Quit();
        }
    }
}
