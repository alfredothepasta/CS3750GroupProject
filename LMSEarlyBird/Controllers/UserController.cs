using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.Repository;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMSEarlyBird.Controllers
{
    public class UserController : Controller
    {
        /// <summary>
        /// context variable for accessing the db
        /// </summary>
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// Context accessor for reading session data
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserIdentityService _userIdentityService;
        private readonly IAppUserRepository _appUserRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentCourseRepository _studentCourseRepository;
        private readonly IDepartmentRepository _departmentRepository;
        /// <summary>
        /// Context for accessing the address database
        /// </summary>
        private readonly IAddressRepository _addressRepository;
        /// <summary>
        /// Context for accessing the room database
        /// </summary>
        private readonly IRoomRepository _roomRepository;

        /// <summary>
        /// Constructor, initializes the instance variables
        /// </summary>
        /// <param name="context"></param>
        /// <param name="contextAccessor"></param>
        public UserController(ApplicationDbContext context, 
            IHttpContextAccessor contextAccessor, 
            ICourseRepository courseRepository, 
            IUserIdentityService userIdentityService,
            IAppUserRepository appUserRepository,
            IStudentCourseRepository studentCourseRepository, 
            IDepartmentRepository departmentRepository,
            IAddressRepository addressRepository,
            IRoomRepository roomRepository)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _courseRepository = courseRepository;
            _userIdentityService = userIdentityService;
            _appUserRepository = appUserRepository;
            _studentCourseRepository = studentCourseRepository;
            _departmentRepository = departmentRepository;
            _roomRepository = roomRepository;
        }

        private string FormatDaysOfWeek(string daysOfWeek)
        {
            string formatted = "";

            foreach (char day in daysOfWeek)
            {
                formatted += day + " ";
            }

            return formatted;
        }

        /// <summary>
        /// Returns the Dashboard page with login information if correct login, else returns to login with errors
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> /*Task<List<RegisterCourseViewModel>>*/ Dashboard(string? BuildingName = "")
        {
            // Checks to see if there is a current signed in user
            if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var currentUser = _contextAccessor.HttpContext?.User.GetUserId();
                if (currentUser != null)
                {
                    //AppUser userData = await _context.Users.FindAsync(currentUser);
                    //return View(userData);

                    //pull user based on logged in user
                    string userId = _userIdentityService.GetUserId();
                    AppUser profile = await _appUserRepository.GetUser(userId);

                    //pull the current courses
                    var courses = await _courseRepository.GetAllCoursesWithInstructor();

                    //Address userAddress = new Address();
                    //userAddress = await _addressRepository.getUserAddress(userId);



                    //Create list of department names
                    RegistrationViewModel result = new RegistrationViewModel();
                    List<string> departmentNames = _departmentRepository.GetAllDepartments().Result.Select(x => x.DeptName).ToList();
                    result.DepartmentNames = departmentNames;

                    // Create list of room numbers & Buildings
                    AddCourseViewModel roomInfo = new AddCourseViewModel();
                    List<Room> roomNumber = _roomRepository.GetRooms().Result.Select(x => new Room { RoomNumber = x.RoomNumber }).ToList();
                    List<Room> building = _roomRepository.GetRooms().Result.Select(x => new Room { BuildingID = x.BuildingID }).ToList();
                    string buildingName = _roomRepository.GetRoomById(building[0].BuildingID).Result.ToString();
                    BuildingName = buildingName;
                    roomInfo.RoomList = roomNumber;

                    //List<StudentCourse> SCourses = new List<StudentCourse>();

                    // build the model view
                    var userVM = new AppUser
                    {
                        FirstName = profile.FirstName,
                        LastName = profile.LastName,
                        StudentCourses = profile.StudentCourses,
                        InstructorCourses = profile.InstructorCourses,
                        
                    };
                    return View(userVM);
                    //List<RegisterCourseViewModel> result = new List<RegisterCourseViewModel>();
                    //foreach (var course in courses)
                    //{
                    //    var registrationViewModel = new RegisterCourseViewModel
                    //    {
                    //        Id = course.id,
                    //        CourseName = course.CourseName,
                    //        CourseNumber = course.CourseNumber,
                    //        CreditHours = course.CreditHours,
                    //        StartTime = course.StartTime,
                    //        EndTime = course.EndTime,
                    //        IsRegistered = profile.StudentCourses.Any(x => x.CourseId == course.id),
                    //        Department = course.Department.DeptCode,
                    //        InstructorName = course.InstructorCourses.FirstOrDefault().User.FirstName + " " + course.InstructorCourses.FirstOrDefault().User.LastName,
                    //        DaysOfWeek = FormatDaysOfWeek(course.DaysOfWeek),
                    //    };

                    //    bool temp = profile.StudentCourses.Any(x => x.CourseId == course.id);

                    //    result.Add(registrationViewModel);
                    //}
                    //return result;
                }
            }

            //return RedirectToActionResult("Account", "Login");
            return View("Account", "Login");
        }

        /// <summary>
        /// Returns the Calendar page
        /// </summary>
        /// <returns></returns>
        public IActionResult Calendar()
        {
            return View();
        }
    }
}
