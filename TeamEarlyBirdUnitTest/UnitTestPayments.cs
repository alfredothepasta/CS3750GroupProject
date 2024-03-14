using LMSEarlyBird.Controllers;
using LMSEarlyBird.Data;
using LMSEarlyBird.Models;
using LMSEarlyBird.Repository;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LMSEarlyBird.Interfaces;

namespace TeamEarlyBirdUnitTest
{
    [TestClass]
    public class UnitTestPayments
    {
        private ApplicationDbContext _dbContext;
        private PaymentController _paymentController;

        [TestMethod]
        public async Task StudentCanMakeAPayment()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_S24_EarlyBird;User ID=3750_S24_EarlyBird;Password=earlyBird@2;TrustServerCertificate=True")
                .Options;

            // given a user id
            string userId = "1ff03b0c-ce4b-42de-ac48-0a45bd4dcd4d";

            // set up the context
            _dbContext = new ApplicationDbContext(options);

            _paymentController = new PaymentController(
                new HttpContextAccessor(),
                new BalanceRepository(_dbContext)
            );

            // given a receipt
            string Test = "Test";

            //count the number of balances in the database
            int numBalances = _dbContext.PaymentHistory.Count();

            // create payment
            PaymentViewModel TestViewModel = new PaymentViewModel();
            TestViewModel.PaymentAmount = 1;

            // call the method
            await _paymentController.pushPaymentToDb(TestViewModel, userId, Test);

            // make sure it worked
            int numBalancesAfterTest = _dbContext.PaymentHistory.Count();

            Assert.IsTrue(numBalancesAfterTest == (numBalances + 1));
        }

        [TestMethod]
        public async Task StudentAddAndDropAClassAdjustment()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_S24_EarlyBird;User ID=3750_S24_EarlyBird;Password=earlyBird@2;TrustServerCertificate=True")
                .Options;

            // given a user id
            string userId = "1ff03b0c-ce4b-42de-ac48-0a45bd4dcd4d";

            // set up the context
            _dbContext = new ApplicationDbContext(options);

            _paymentController = new PaymentController(
                new HttpContextAccessor(),
                new BalanceRepository(_dbContext)
            );

            // given a course hours
            int hours = 3;

            // given a course Name
            string courseName = "Unit Test Course";

            //  count the number of balances in the database
            int numBalances = _dbContext.PaymentHistory.Count();

            // call the method to add the class
            await _paymentController.pushClassAddedToDb(userId, hours, courseName);

            // call the method to drop the class
            await _paymentController.pushClassDroppedToDb(userId, hours, courseName);

            // make sure it passed the payments
            int numBalancesAfterTest = _dbContext.PaymentHistory.Count();

            Assert.IsTrue(numBalancesAfterTest == (numBalances + 2));
        }
    }
}