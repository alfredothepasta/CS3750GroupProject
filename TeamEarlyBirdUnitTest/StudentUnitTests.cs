using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMSEarlyBird.Controllers;
using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TeamEarlyBirdUnitTest
{
    [TestClass]
    public class StudentUnitTests
    {

        private ApplicationDbContext _dbContext;
        private StudentController _testController;

        public StudentUnitTests(){
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_S24_EarlyBird;User ID=3750_S24_EarlyBird;Password=earlyBird@2;TrustServerCertificate=True")
                .Options;


            _dbContext = new ApplicationDbContext(options);
            _testController = new StudentController(
                new CourseRepository(_dbContext),
                null,
                new AppUserRepository(_dbContext),
                new StudentCourseRepository(_dbContext),
                new DepartmentRepository(_dbContext),
                new AssignmentsRepository(_dbContext),
                new BalanceRepository(_dbContext),
                null
            );
        }

        [TestMethod]
        public async Task StudentRegisterCourseTest(){

            string userID = "e29ce7d5-9e6e-4ce5-bceb-51692401ac06";
            StudentCourse studentCourse = new StudentCourse
            {
                UserId = userID,
                CourseId = 5
            };
            
            await _testController.DBDropClass(studentCourse);

            int numCourses = _dbContext.StudentCourses
                .Where(c => c.UserId == userID)
                .Count();

            bool added = await _testController.DBAddClass(studentCourse);
            Assert.IsTrue(added);

            int newNumCourses = _dbContext.StudentCourses.Where(c => c.UserId == userID)
                .Count();
            Assert.IsTrue(newNumCourses > numCourses);
        }

        [TestMethod]
        public async Task StudentDropCourseTest(){
            string userID = "e29ce7d5-9e6e-4ce5-bceb-51692401ac06";

            StudentCourse studentCourse = new StudentCourse
            {
                UserId = userID,
                CourseId = 7
            };

            await _testController.DBAddClass(studentCourse);

            int numCourses = _dbContext.StudentCourses
                .Where(c => c.UserId == userID)
                .Count();
                
            bool dropped = await _testController.DBDropClass(studentCourse);
            Assert.IsTrue(dropped);

            int newNumCourses = _dbContext.StudentCourses.Where(c => c.UserId == userID)
                .Count();
            Assert.IsTrue(newNumCourses < numCourses);
        }
    }
}