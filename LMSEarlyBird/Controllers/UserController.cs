using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.Repository;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        /// Context for accessing the assignments database
        /// </summary>
        private readonly IAssignmentsRepository _assignmentRepository;

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
            IBuildingRepository buildingRepository,
            IAssignmentsRepository assignmentsRepository)
        {
            _contextAccessor = contextAccessor;
            _courseRepository = courseRepository;
            _userIdentityService = userIdentityService;
            _appUserRepository = appUserRepository;
            _departmentRepository = departmentRepository;
            _roomRepository = roomRepository;
            _buildingRepository = buildingRepository;
            _assignmentRepository = assignmentsRepository;
        }

        /// <summary>
        /// Returns the Dashboard page with login information if correct login, else returns to login with errors
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Dashboard()
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

                    ////pull the current courses, required for obtaining department names
                    var courses = await _courseRepository.GetAllCoursesWithInstructor();

                    // call the dashboard VM for gathering the course information
                    DashboardViewModel dashboardVM = new DashboardViewModel();

                    // gather department names
                    List<string> departmentNames = _departmentRepository.GetAllDepartments().Result.Select(x => x.DeptName).ToList();
                    dashboardVM.DepartmentNames = departmentNames;

                    // gather the room numbers
                    List<Room> roomNumbers = _roomRepository.GetRooms().Result.Select(x => new Room { RoomNumber = x.RoomNumber }).ToList();
                    dashboardVM.RoomList = roomNumbers;

                    // gather the building names
                    List<Building> buildings = _buildingRepository.GetBuildings().Result.ToList();
                    dashboardVM.BuildingList = buildings;

                    // gather the list of assignments
                    List<StudentAssignment> assignments = await _assignmentRepository.GetStudentAssignments(userId);

                    // build the model view for the user to display First and Last name as well as the courses and assignments
                    var userVM = new AppUser
                    {
                        FirstName = profile.FirstName,
                        LastName = profile.LastName,
                        StudentCourses = profile.StudentCourses,
                        InstructorCourses = profile.InstructorCourses,
                        StudentAssignment = profile.StudentAssignment,
                    };
                    // pass everything gathered into the view
                    return View(userVM);
                }
            }
            return View("Account", "Login");
        }

        /// <summary>
        /// Returns the Calendar page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Calendar()
        {
            // get the courses associated with the student
            List<Course> courses;
            List<StudentAssignment> assignments;
            // gather the events for the callendar
            List<CalendarEvent> events = new List<CalendarEvent>();

            // if the user is a teacher, get the courses associated with the teacher
            if (User.IsInRole(UserRoles.Teacher))
            {
                var instructorId = _contextAccessor.HttpContext.User.GetUserId();
                courses = await _courseRepository.GetCoursesByTeacher(instructorId);

            }
            else
            {
                var studentId = _contextAccessor.HttpContext.User.GetUserId();
                courses = await _courseRepository.GetCoursesByStudent(studentId);

                // get the assignments associated with the student
                assignments = await _assignmentRepository.GetStudentAssignments(studentId);
                // create a calendar event for each assignment
                foreach (var assignment in assignments)
                {
                    // Create a CalendarEvent for the current assignment and date
                    CalendarEvent calendarEvent = new CalendarEvent
                    {
                        title = assignment.Assignment.Title,
                        start = assignment.Assignment.DueDate - TimeSpan.FromHours(1),
                        end = assignment.Assignment.DueDate, 
                        backgroundColor = "#FFF",
                        borderColor = "#000",
                        textColor = "#000",
                        url = Url.Action("Submission", "Student", new { assignmentId = assignment.AssignmentId })
					};
                    events.Add(calendarEvent);
                }
            }


			// Define a mapping of day abbreviations to numbers
			Dictionary<string, int> dayAbbreviationToNumber = new Dictionary<string, int>
            {
	            { "S", 0 }, // Sunday
                { "M", 1 }, // Monday
                { "T", 2 }, // Tuesday
                { "W", 3 }, // Wednesday
                { "R", 4 }, // Thursday
                { "F", 5 }, // Friday
                { "A", 6 }  // Saturday
            };

			// Define the start and end dates of the semester
			DateTime semesterStartDate = new DateTime(2024, 01, 08);
			DateTime semesterEndDate = new DateTime(2024, 04, 20);

			// Iterate over the courses
			foreach (Course course in courses)
            {
				// Iterate over the days each course is on
				foreach (char day in course.DaysOfWeek)
                {
					// Check if the day abbreviation is valid
					if (dayAbbreviationToNumber.ContainsKey(day.ToString()))
					{
						// Get the corresponding day number
						int dayNumber = dayAbbreviationToNumber[day.ToString()];

						// Calculate the first occurrence of the day of the week within the semester date range
						DateTime startDate = semesterStartDate;
						while (startDate.DayOfWeek != (DayOfWeek)dayNumber)
						{
							startDate = startDate.AddDays(1);
						}

						// Iterate over the dates starting from the calculated start date up to the end of the semester
						for (DateTime date = startDate; date <= semesterEndDate; date = date.AddDays(7))
						{
							// set event color by course number
							string eventColor = "#4287f5";
							int cn = int.Parse(course.CourseNumber);
							if (cn < 1000)
							{
								eventColor = "#4287f5";
							} 
							else if(cn < 2000)
							{
								eventColor = "#26d426";
							}
							else if (cn < 3000)
							{
								eventColor = "#de8b1f";
							}
							else if (cn < 4000)
							{
								eventColor = "#f01111";
							}
							else if (cn >= 4000)
							{
								eventColor = "#da0dde";
							}

							// Create a CalendarEvent for the current course and date
							CalendarEvent calendarEvent = new CalendarEvent
							{
								title = course.Department + course.CourseNumber + " " + course.CourseName,
								start = date + course.StartTime.ToTimeSpan(),
								end = date + course.EndTime.ToTimeSpan(),
								backgroundColor = eventColor,
                                borderColor = eventColor,
							};

                            if (User.IsInRole(UserRoles.Teacher))
                            {
                                // make it so a student can access as well
                                calendarEvent.url = Url.Action("CourseAssignmentList", "Instructor", new { courseId = course.id });
                            }
                            else if(User.IsInRole(UserRoles.Student))
                            {
                                // make it so a teacher can access as well
                                calendarEvent.url = Url.Action("Course", "Student", new { courseId = course.id });
							}

                            // Add the CalendarEvent to the events list
                            events.Add(calendarEvent);
						}

					}
				}
			}
			// Convert the events list to JSON
			string jsonEvents = Newtonsoft.Json.JsonConvert.SerializeObject(events);

			ViewData["Events"] = jsonEvents;

			return View("Calendar");
        }

		public async Task<IActionResult> Chart()
        {
            return View("Chart");
        }

	}
}
