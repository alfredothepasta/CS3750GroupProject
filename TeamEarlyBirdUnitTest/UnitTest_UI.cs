using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Stripe;

namespace TeamEarlyBirdUnitTest
{
    [TestClass]
    public class UnitTest_UI
    {
        const string URL = "https://localhost:7243";
        //https://lmsearlybird20240227112424.azurewebsites.net

        [TestMethod]
        public void InstructorCanLogin_UI_TEST()
        {
            //Create a driver for Chrome
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl(URL + "/Account/Login");

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

        [TestMethod]
        public void StudentCanLogin_UI_TEST()
        {
            //Create a driver for Chrome
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl(URL + "/Account/Login");

            IWebElement username = driver.FindElement(By.Id("EmailAddress"));
            IWebElement password = driver.FindElement(By.Id("Password"));
            IWebElement loginBtn = driver.FindElement(By.XPath("//button[@class='btn btn-primary' and text()='Login']"));

            username.SendKeys("jordanice52@gmail.com");
            password.SendKeys("Spider48*");

            loginBtn.Click();

            Assert.IsTrue(driver.Url.Contains("Dashboard"));

            // Close the browser
            driver.Quit();
        }

        [TestMethod]
        public void StudentCanSignUp_UI_TEST()
        {
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl(URL +  "/Account/Login");

            IWebElement signUpBtn = driver.FindElement(By.XPath("//a[@class='btn btn-primary' and text()='Sign Up']"));

            signUpBtn.Click();

            IWebElement email = driver.FindElement(By.Id("EmailAddress"));
            IWebElement firstName = driver.FindElement(By.Id("FirstName"));
            IWebElement lastName = driver.FindElement(By.Id("LastName"));
            IWebElement birthDate = driver.FindElement(By.Id("BirthDate"));
            IWebElement password = driver.FindElement(By.Id("Password"));
            IWebElement confirmPassword = driver.FindElement(By.Id("ConfirmPassword"));
            IWebElement selectRole = driver.FindElement(By.Id("UserRole"));
            IWebElement submit = driver.FindElement(By.XPath("//input[@class='btn btn-outline-success float-right']"));

            Random random = new Random();
            int rand = random.Next(1, 10000);

            string randEmail = "random" + rand.ToString() + "@gmail.com";

            email.SendKeys(randEmail);
            firstName.SendKeys("Computer");
            lastName.SendKeys("Jones");
            birthDate.SendKeys("01/01/2000");
            password.SendKeys("P@ssword99");
            confirmPassword.SendKeys("P@ssword99");
            selectRole.SendKeys("Student");
            submit.Click();

            IWebElement username = driver.FindElement(By.Id("EmailAddress"));
            IWebElement loginPasword = driver.FindElement(By.Id("Password"));
            IWebElement loginBtn = driver.FindElement(By.XPath("//button[@class='btn btn-primary' and text()='Login']"));

            username.SendKeys(randEmail);
            loginPasword.SendKeys("P@ssword99");

            loginBtn.Click();

            Assert.IsTrue(driver.Url.Contains("Dashboard"));

            // Close the browser
            driver.Quit();
        }

        [TestMethod]
        public void InstructorCanCreateCourse_UI_TEST()
        {
            // do the stuff to log in as a proffessor
            //Create a driver for Chrome
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl(URL + "/Account/Login");

            IWebElement username = driver.FindElement(By.Id("EmailAddress"));
            IWebElement password = driver.FindElement(By.Id("Password"));
            IWebElement loginBtn = driver.FindElement(By.XPath("//button[@class='btn btn-primary' and text()='Login']"));

            username.SendKeys("prof@mail.com");
            password.SendKeys("Test1234*");

            loginBtn.Click();

            IWebElement Courses = driver.FindElement(By.Id("nav_courses"));

            Courses.Click();

            IWebElement NewCourse = driver.FindElement(By.Id("btn_addCourse"));

            NewCourse.Click();

            Assert.IsTrue(driver.Url.Contains("AddCourse"));
            driver.Quit();
        }

        [TestMethod]
        public void InstructorCanAccessCalendar_UI_TEST()
        {
            // do the stuff to log in as a proffessor
            //Create a driver for Chrome
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl(URL + "/Account/Login");

            IWebElement username = driver.FindElement(By.Id("EmailAddress"));
            IWebElement password = driver.FindElement(By.Id("Password"));
            IWebElement loginBtn = driver.FindElement(By.XPath("//button[@class='btn btn-primary' and text()='Login']"));

            username.SendKeys("prof@mail.com");
            password.SendKeys("Test1234*");

            loginBtn.Click();

            IWebElement calendarNavItem = driver.FindElement(By.Id("nav_calendar"));

            calendarNavItem.Click();

            Assert.IsTrue(driver.Url.Contains("Calendar"));

            driver.Quit();
        }
    }
}