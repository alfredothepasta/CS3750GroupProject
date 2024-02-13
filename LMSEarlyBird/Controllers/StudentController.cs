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
        private readonly IAssignmentsRepository _assignmentsRepository;
        private readonly IDepartmentRepository _departmentRepository;
        public StudentController(ICourseRepository courseRepository, IUserIdentityService userIdentityService, IAppUserRepository appUserRepository, IStudentCourseRepository studentCourseRepository, IDepartmentRepository departmentRepository, IAssignmentsRepository assignmentsRepository)
        {
            _courseRepository = courseRepository;
            _userIdentityService = userIdentityService;
            _appUserRepository = appUserRepository;
            _studentCourseRepository = studentCourseRepository;
            _departmentRepository = departmentRepository;
            _assignmentsRepository = assignmentsRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Registration(){      
            RegistrationViewModel result = new RegistrationViewModel();

            //Create list of department names
            List<string> departmentNames = _departmentRepository.GetAllDepartments().Result.Select(x => x.DeptName).ToList();
            result.DepartmentNames = departmentNames;

            result.Courses = await GetRegisterCourseViewModels();
            return View(result);
        }  

        public async Task<IActionResult> Course(int courseid){
            var userid = _userIdentityService.GetUserId();

            var user = await _appUserRepository.GetUser(userid);

            if(!user.StudentCourses.Any(x => x.CourseId == courseid))
            {
                return NotFound();
            }

            var courseAssignments = 
                await _assignmentsRepository.GetStudentAssignmentsByCourse(userid, courseid);         

            CourseAssignmentListViewModel viewModel = new CourseAssignmentListViewModel
            {
                Course = await _courseRepository.GetCourse(courseid),
                Assignments = courseAssignments
            };

            return View(viewModel);
        }
        
        public async Task<IActionResult> Search(string? query, string? category)
        {

            RegistrationViewModel result = new RegistrationViewModel();

            //Create list of department names 
            List<string> departmentNames = _departmentRepository.GetAllDepartments().Result.Select(x => x.DeptName).ToList();
            result.DepartmentNames = departmentNames;
            
            if(!result.DepartmentNames.Any(x => x == category))
            {
                category = null;
            }
            else if(category != null)
            {
                result.SelectedDept = category;
            }

            result.Search = query;

            //Get filtered list of courses         
            result.Courses = await GetRegisterCourseViewModels(query, category);
            return View("Registration", result);
        }     

        public async Task<IActionResult> DropClass(int id, string? search, string? deptSelected)
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
                await _assignmentsRepository.RemoveStudentAssignments(userid,id);
            }
            catch(Exception ex)
            {
                
            }

            var testt = await _appUserRepository.GetUser(userid);

            if(search!=null || deptSelected!=null || deptSelected != "" || search != "")
            {
                return RedirectToAction(nameof(Search), new { query = search, category = deptSelected });
            }

            return RedirectToAction(nameof(Registration));
        }

        public async Task<IActionResult> AddClass(int id, string? search, string? deptSelected)
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
                await _assignmentsRepository.AddStudentAssignments(userid,id);
            }
            catch(Exception ex)
            {
                
            }

            if(search!=null || deptSelected!=null)
            {
                return RedirectToAction(nameof(Search), new { query = search, category = deptSelected });
            }

            return RedirectToAction(nameof(Registration));
        }

        private string FormatDaysOfWeek(string daysOfWeek){
            string formatted = "";
            
            foreach(char day in daysOfWeek){
                formatted += day + " ";
            }

            return formatted;
        }

        private async Task<List<RegisterCourseViewModel>> GetRegisterCourseViewModels(string? search = "", string? department = "")
        {
            var id = _userIdentityService.GetUserId();
            var user = await _appUserRepository.GetUser(id);

            //var test = await _studentCourseRepository.GetAllStudentCourses();

            var courses = await _courseRepository.GetAllCoursesWithInstructor();

            if (search != null)
            {
                courses = courses.Where(x => x.CourseName.ToUpper().Contains(search.ToUpper())).ToList();
            }

            if(department != null && department != "")
            {
                courses = courses.Where(x => x.Department.DeptName == department).ToList();
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
                    IsRegistered = user.StudentCourses.Any(x => x.CourseId == course.id),
                    Department = course.Department.DeptCode,
                    InstructorName = course.InstructorCourses.FirstOrDefault().User.FirstName + " " + course.InstructorCourses.FirstOrDefault().User.LastName,
                    DaysOfWeek = FormatDaysOfWeek(course.DaysOfWeek),
                };

                bool temp = user.StudentCourses.Any(x => x.CourseId == course.id);

                result.Add(registrationViewModel);
            }

            return result;
        }
    }
}