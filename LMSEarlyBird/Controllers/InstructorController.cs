using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Mvc;

namespace LMSEarlyBird.Controllers
{
    public class InstructorController : Controller
    {

        /// <summary>
        /// context variable for accessing the db
        /// </summary>
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// Context accessor for reading session data
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICourseRepository _courseRepository;

        public InstructorController(ApplicationDbContext context, IHttpContextAccessor contextAccessor, ICourseRepository courseRepository)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _courseRepository = courseRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CourseList()
        {
            // check if the user is logged in to an account with the instructor role
            if (isNotInstructor())
            {
                return View("User", "Dashboard");
            }
            
            // get the current users's roles for accessing the courses associated with them in the db
            var instructorId = _contextAccessor.HttpContext.User.GetUserId();
            // get the courses associated with the user
            List<Course> courses = await _courseRepository.GetCoursesByTeacher(instructorId);
            // now how do I pass those courses in.... 


            return View(courses);

        }

        [HttpGet]
        public async Task<IActionResult> AddCourse()
        {
            // check if the user is logged in to an account with the instructor role
            if (isNotInstructor())
            {
                return View("User", "Dashboard");
            }

            

            return View();
        }

        private bool isNotInstructor() {
            return !User.IsInRole(UserRoles.Teacher);
        }
    }
}
