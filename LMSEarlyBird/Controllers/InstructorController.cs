using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

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
        private readonly IBuildingRepository _buildingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAppUserRepository _appUserRepository;

        public InstructorController(ApplicationDbContext context, 
            IHttpContextAccessor contextAccessor, 
            ICourseRepository courseRepository, 
            IBuildingRepository buildingRepository, 
            IRoomRepository roomRepository,
            IDepartmentRepository departmentRepository,
            IAppUserRepository appUserRepository)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _courseRepository = courseRepository;
            _buildingRepository = buildingRepository;
            _roomRepository = roomRepository;
            _departmentRepository = departmentRepository;
            _appUserRepository = appUserRepository;
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
                return View("Dashboard", "User");
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
                return View("Dashboard", "User");
            }

            // get the building and departmen dropdowns
            List<Building> buildings = await _buildingRepository.GetBuildings();
            List<Department> departments = await _departmentRepository.GetAllDepartments();
            List<Room> rooms = await _roomRepository.GetRooms();
            ViewBag.Buildings = buildings;
            ViewBag.Departments = departments;
            ViewBag.Rooms = rooms;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCourse(AddCourseViewModel viewModel)
        {
            // check if the user is logged in to an account with the instructor role
            if (isNotInstructor())
            {
                return View("Dashboard", "User");
            }

            // check if the course is valid
            if (!ModelState.IsValid)
            {
                // get the building and departmen dropdowns
                List<Building> buildings = await _buildingRepository.GetBuildings();
                List<Department> departments = await _departmentRepository.GetAllDepartments();
                List<Room> rooms = await _roomRepository.GetRooms();
                ViewBag.Buildings = buildings;
                ViewBag.Departments = departments;
                ViewBag.Rooms = rooms;

                return View(viewModel);
            }


            // Find the days that have been selected
            string selectedDays = "";
            if(viewModel.DayOfWeekM == "true") selectedDays = selectedDays + "M";
            if (viewModel.DayOfWeekT == "true") selectedDays = selectedDays + "T";
            if (viewModel.DayOfWeekW == "true") selectedDays = selectedDays + "W";
            if (viewModel.DayOfWeekR == "true") selectedDays = selectedDays + "R";
            if (viewModel.DayOfWeekF == "true") selectedDays = selectedDays + "F";

            // get the room and department to add to the new course
            Room room = await _roomRepository.GetRoomById(viewModel.Room);
            Department department = await _departmentRepository.GetDepartmentById(viewModel.Department);
            Course course = new Course
            {
                CourseName = viewModel.CourseName,
                CourseNumber = viewModel.CourseNumber,
                CreditHours = viewModel.CreditHours,
                StartTime = viewModel.StartTime,
                EndTime = viewModel.EndTime,
                DaysOfWeek = selectedDays,
                Room = room,
                Department = department
            };

            AppUser instructor = await _appUserRepository.GetUser(_contextAccessor.HttpContext.User.GetUserId());
            
            _courseRepository.Add(course, instructor);

            return RedirectToAction("CourseList", "Instructor");
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> roomAvailability(int Room, TimeOnly? StartTime, TimeOnly? EndTime, string DayOfWeekM, string DayOfWeekT, string DayOfWeekW, string DayOfWeekR, string DayOfWeekF)
        {
            // inform user both start and end time must be selected to verify availability
            if (StartTime == null || EndTime == null)
            {
                return Json("Start and end times must be input to verify room availability.");
            }

            if(DayOfWeekM == "False" && DayOfWeekT == "False" && DayOfWeekW == "False" && DayOfWeekR == "False" && DayOfWeekF == "False")
			{
				return Json("At least one day must be selected to verify room availability.");
			}

			// Find the days that have been selected
			string selectedDays = "";
			if (DayOfWeekM == "true") selectedDays = selectedDays + "M";
			if (DayOfWeekT == "true") selectedDays = selectedDays + "T";
			if (DayOfWeekW == "true") selectedDays = selectedDays + "W";
			if (DayOfWeekR == "true") selectedDays = selectedDays + "R";
			if (DayOfWeekF == "true") selectedDays = selectedDays + "F";

			List<Course> potentialRoomConflicts = await _courseRepository.GetCourseByRoomId(Room);

            foreach (Course course in potentialRoomConflicts)
            {
                foreach(char potentialDayConflict in course.DaysOfWeek)
                {
                    foreach(char selectedDay in selectedDays)
                    {
						if (potentialDayConflict == selectedDay)
                        {
							bool startTimeConflict = (course.StartTime > StartTime) && (course.StartTime < EndTime);
							bool endTimeConflict = (course.EndTime > StartTime) && (course.EndTime < EndTime);
							if (startTimeConflict || endTimeConflict)
							{
								Room room = await _roomRepository.GetRoomById(Room);
								return Json($"Room {room.RoomNumber} is not available on {course.DaysOfWeek} between {course.StartTime} and {course.EndTime}");
							}
						}

					}
                }
					
            }

            return Json(true);
        }

        private bool isNotInstructor() {
            return !User.IsInRole(UserRoles.Teacher);
        }
    }
}
