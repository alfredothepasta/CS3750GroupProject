using LMSEarlyBird.Controllers;
using LMSEarlyBird.Data;
using LMSEarlyBird.Models;
using LMSEarlyBird.Repository;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using Stripe;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TeamEarlyBirdUnitTest
{
    
    [TestClass]
    public class UnitTest1
    {
        private ApplicationDbContext _dbContext;
        private InstructorController _testController;
        private PaymentController _paymentController;
        private HttpContextAccessor _httpContextAccessor;

        [TestMethod]
        public async Task InstructorCanCreateACourseTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_S24_EarlyBird;User ID=3750_S24_EarlyBird;Password=earlyBird@2;TrustServerCertificate=True")
                .Options;


            _dbContext = new ApplicationDbContext(options);
            _testController = new InstructorController(
                _dbContext,
                new HttpContextAccessor(),
                new CourseRepository(_dbContext),
                new BuildingRepository(_dbContext),
                new RoomRepository(_dbContext),
                new DepartmentRepository(_dbContext),
                new AppUserRepository(_dbContext),
                new AssignmentsRepository(_dbContext),
                new StudentCourseRepository(_dbContext),
                new BalanceRepository(_dbContext)
            );

            // given an instructor ID
            string instructorId = "85dc9d8f-efcf-480f-99b8-e10a3a29127c";

            // get the number of courses
            int numCourses = _dbContext.Courses
                .Where(c => c.InstructorCourses
                    .Where(i => i.UserId == instructorId)
                    .Any())
                .Count();

            // exorsice the demons in the code
            AddCourseViewModel testViewModel = new AddCourseViewModel();
            testViewModel.Department = 1;
            testViewModel.CourseNumber = "1234";
            testViewModel.CourseName = "Test";
            testViewModel.CreditHours = 4;
            testViewModel.StartTime = new TimeOnly(11, 30);
            testViewModel.EndTime = new TimeOnly(13, 0);
            testViewModel.Building = 1;
            testViewModel.Room = 1;

            await _testController.pushCourseToDb(testViewModel, instructorId);

            int numCoursesAfterTest = _dbContext.Courses
                    .Where(c => c.InstructorCourses
                        .Where(i => i.UserId == instructorId)
                        .Any())
                    .Count();

            Assert.IsTrue(numCoursesAfterTest == (numCourses + 1));

        }

        [TestMethod]
        public async Task InstructorCanCreateAnAssignmentTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_S24_EarlyBird;User ID=3750_S24_EarlyBird;Password=earlyBird@2;TrustServerCertificate=True")
                .Options;


            _dbContext = new ApplicationDbContext(options);
            _testController = new InstructorController(
                _dbContext,
                new HttpContextAccessor(),
                new CourseRepository(_dbContext),
                new BuildingRepository(_dbContext),
                new RoomRepository(_dbContext),
                new DepartmentRepository(_dbContext),
                new AppUserRepository(_dbContext),
                new AssignmentsRepository(_dbContext),
                new StudentCourseRepository(_dbContext),
                new BalanceRepository(_dbContext)
            );

            // given a course id
            int courseId = 17;

            int numAssignments = _dbContext.Assignments.Count();

            // create assignment
            CreateAssignmentViewModel viewModel = new CreateAssignmentViewModel();
            viewModel.Title = "testAssignment";
            viewModel.DueDate = new DateTime(2024, 5, 1, 3, 0, 0);
            viewModel.Description = "Description";
            viewModel.Type = "text";
            viewModel.maxPoints = 100;

            // call the method
            await _testController.pushAssignmentToDb(viewModel, courseId);


            // make sure it worked
            int numAssignmentsAfterTest = _dbContext.Assignments.Count();

            Assert.IsTrue(numAssignmentsAfterTest == (numAssignments + 1));
        }

        [TestMethod]
        public async Task StudentCanMakeAPayment()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_S24_EarlyBird;User ID=3750_S24_EarlyBird;Password=earlyBird@2;TrustServerCertificate=True")
                .Options;

            var userManager = new UserManager<AppUser>(new UserStore<AppUser>(_dbContext), null, null, null, null, null, null, null, null);

            _paymentController = new PaymentController(
                _dbContext,
                new UserIdentityService(userManager, _httpContextAccessor),
                new BalanceRepository(_dbContext)
            );

            // given a user id
            string userId = "1ff03b0c-ce4b-42de-ac48-0a45bd4dcd4d";

            // given a receipt
            string Test = "Test";

            //int numAssignments = _dbContext.Assignments.Count();
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
    }
}