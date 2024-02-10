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
        /// Context accessor for reading Session data
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;
        /// <summary>
        /// Context accessor for reading User Identification data
        /// </summary>
        private readonly IUserIdentityService _userIdentityService;
        /// <summary>
        /// Context accessor for reading User data
        /// </summary>
        private readonly IAppUserRepository _appUserRepository;
        /// <summary>
        /// Context accessor for reading Courses database
        /// </summary>
        private readonly ICourseRepository _courseRepository;
        /// <summary>
        /// Context accessor for reading Departments database
        /// </summary>
        private readonly IDepartmentRepository _departmentRepository;
        /// <summary>
        /// Context for accessing the room database
        /// </summary>
        private readonly IRoomRepository _roomRepository;
        /// <summary>
        /// Context for accessing the building database
        /// </summary>
        private readonly IBuildingRepository _buildingRepository;

        /// <summary>
        /// Constructor, initializes the instance variables
        /// </summary>
        /// <param name="context"></param>
        /// <param name="contextAccessor"></param>
        public UserController(
            IHttpContextAccessor contextAccessor, 
            ICourseRepository courseRepository, 
            IUserIdentityService userIdentityService,
            IAppUserRepository appUserRepository,
            IDepartmentRepository departmentRepository,
            IRoomRepository roomRepository,
            IBuildingRepository buildingRepository)
        {
            _contextAccessor = contextAccessor;
            _courseRepository = courseRepository;
            _userIdentityService = userIdentityService;
            _appUserRepository = appUserRepository;
            _departmentRepository = departmentRepository;
            _roomRepository = roomRepository;
            _buildingRepository = buildingRepository;
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
        public async Task<IActionResult> Dashboard(string? BuildingName = "")
        {
            // Checks to see if there is a current signed in user
            if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var currentUser = _contextAccessor.HttpContext?.User.GetUserId();
                if (currentUser != null)
                {
                    //pull user based on logged in user
                    string userId = _userIdentityService.GetUserId();
                    AppUser profile = await _appUserRepository.GetUser(userId);

                    //pull the current courses, required for obtaining department names
                    var courses = await _courseRepository.GetAllCoursesWithInstructor();
                    //Create list of department names
                    RegistrationViewModel result = new RegistrationViewModel();
                    List<string> departmentNames = _departmentRepository.GetAllDepartments().Result.Select(x => x.DeptName).ToList();
                    result.DepartmentNames = departmentNames;

                    // Create list of room numbers
                    AddCourseViewModel roomInfo = new AddCourseViewModel();
                    List<Room> roomNumber = _roomRepository.GetRooms().Result.Select(x => new Room { RoomNumber = x.RoomNumber }).ToList();
                    roomInfo.RoomList = roomNumber;

                    // Create list of Buildings
                    List<Building> buildings = _buildingRepository.GetBuildings().Result.ToList();
                    roomInfo.BuildingList = buildings;

                    // build the model view for the user to display First and Last name as well as the courses//
                    var userVM = new AppUser
                    {
                        FirstName = profile.FirstName,
                        LastName = profile.LastName,
                        StudentCourses = profile.StudentCourses,
                        InstructorCourses = profile.InstructorCourses,
                        
                    };
                    return View(userVM);
                }
            }
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
