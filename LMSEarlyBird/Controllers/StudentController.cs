using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.Repository;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LMSEarlyBird.Controllers
{
    public class StudentController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUserIdentityService _userIdentityService;
        private readonly IAppUserRepository _appUserRepository;
        
        public StudentController(ICourseRepository courseRepository, IUserIdentityService userIdentityService, IAppUserRepository appUserRepository)
        {
            _courseRepository = courseRepository;
            _userIdentityService = userIdentityService;
            _appUserRepository = appUserRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Registration(){
            var courses = await _courseRepository.GetAllCourses();
            List<RegistrationViewModel> result = new List<RegistrationViewModel>();
            foreach(var course in courses){
                var registrationViewModel = new RegistrationViewModel
                {
                    Id = course.id,
                    CourseName = course.CourseName,
                    CourseNumber = course.CourseNumber,
                    CreditHours = course.CreditHours,
                    StartTime = course.StartTime,
                    EndTime = course.EndTime,
                };

                result.Add(registrationViewModel);
            }

            var newCourse = new Course
            {
                CourseName = "Software Engineering",
                CourseNumber = "1234",
                CreditHours = 12,
                StartTime = new TimeOnly(5,0,0),
                EndTime = new TimeOnly(),
                DaysOfWeek = "Monday, Wednesday, Friday",
                RoomId = 1234,
            };

            //_courseRepository.Add(newCourse);

            var testModel = new RegistrationViewModel
            {
                Id = 5,
                CourseName = "Software Engineering",
                CourseNumber = "1234",
                CreditHours = 12,
                StartTime = new TimeOnly(5,0,0),
                EndTime = new TimeOnly(),
                IsRegistered = false,
            };

            result.Add(testModel);
                
            var id = _userIdentityService.GetUserId();

            var user = await _appUserRepository.GetUser(id);

            return View(result);
        }
    }
}