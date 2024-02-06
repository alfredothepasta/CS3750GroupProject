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

        private readonly IStudentCourseRepository _studentCourseRepository;
        
        public StudentController(ICourseRepository courseRepository, IUserIdentityService userIdentityService, IAppUserRepository appUserRepository, IStudentCourseRepository studentCourseRepository)
        {
            _courseRepository = courseRepository;
            _userIdentityService = userIdentityService;
            _appUserRepository = appUserRepository;
            _studentCourseRepository = studentCourseRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DropClass(int id)
        {
            var userid = _userIdentityService.GetUserId();

            StudentCourse studentCourse = new StudentCourse
            {
                UserId = userid,
                CourseId = id
            };


            try
            {
                _studentCourseRepository.Delete(studentCourse);
            }
            catch(Exception ex)
            {
                return RedirectToAction(nameof(Registration));
            }
            

            // Stay on the registration page after the operation
            return RedirectToAction(nameof(Registration));
        }

        public IActionResult AddClass(int id)
        {
            var userid = _userIdentityService.GetUserId();

            StudentCourse studentCourse = new StudentCourse
            {
                UserId = userid,
                CourseId = id
            };

            try
            {
                _studentCourseRepository.Add(studentCourse);
            }
            catch(Exception ex)
            {
                return RedirectToAction(nameof(Registration));
            }

            return RedirectToAction(nameof(Registration));
        }

        private async Task<List<RegisterCourseViewModel>> GetRegisterCourseViewModels(string search = "", string department = "")
        {
            var id = _userIdentityService.GetUserId();
            var user = await _appUserRepository.GetUser(id);

            //var test = await _studentCourseRepository.GetAllStudentCourses();

            var courses = await _courseRepository.GetAllCourses();

            if (search != null)
            {
                courses = courses.Where(x => x.CourseName.Contains(search)).ToList();
            }

            List<RegisterCourseViewModel> result = new List<RegisterCourseViewModel>();
            foreach(var course in courses){
                var registrationViewModel = new RegisterCourseViewModel
                {
                    Id = course.id,
                    CourseName = course.CourseName,
                    CourseNumber = course.CourseNumber,
                    CreditHours = course.CreditHours,
                    StartTime = course.StartTime,
                    EndTime = course.EndTime,
                    IsRegistered = user.StudentCourses.Any(x => x.CourseId == course.id)
                };

                bool temp = user.StudentCourses.Any(x => x.CourseId == course.id);

                result.Add(registrationViewModel);
            }

            return result;
        }

        public async Task<IActionResult> Registration(){                  
            var result = await GetRegisterCourseViewModels();
            return View(result);
        }  
        
        public async Task<IActionResult> Search(string query, string category)
        {
            //Get filtered list of courses         
            var filteredList = await GetRegisterCourseViewModels(query, category);

            return View("Registration", filteredList);
        }     
    }
}