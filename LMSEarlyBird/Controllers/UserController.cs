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
		private readonly ICourseRepository _courseRepository;
        private readonly IStudentCourseRepository _studentCourseRepository;

		/// <summary>
		/// Constructor, initializes the instance variables
		/// </summary>
		/// <param name="context"></param>
		/// <param name="contextAccessor"></param>
		public UserController(ApplicationDbContext context, IHttpContextAccessor contextAccessor
            , ICourseRepository courseRepository, IStudentCourseRepository studentCourseRepository)
        {
			_courseRepository = courseRepository;
			_context = context;
            _contextAccessor = contextAccessor;
            _studentCourseRepository = studentCourseRepository;
		}

        /// <summary>
        /// Returns the Dashboard page with login information if correct login, else returns to login with errors
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Dashboard()
        {
            // Checks to see if there is a current signed in user
            if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var currentUser = _contextAccessor.HttpContext?.User.GetUserId();
                if (currentUser != null)
                {
                    AppUser userData = await _context.Users.FindAsync(currentUser);
                    return View(userData);
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
            List<Course> courses;
			// get the courses associated with the student
			// var studentId = _contextAccessor.HttpContext.User.GetUserId();
			// courses = await _studentCourseRepository.GetCoursesByStudent(studentId);

			// if the user is a teacher, get the courses associated with the teacher
			if (User.IsInRole(UserRoles.Teacher))
            {
				var instructorId = _contextAccessor.HttpContext.User.GetUserId();
				courses = await _courseRepository.GetCoursesByTeacher(instructorId);
			}
            else
            {
                return View("Calendar");
            }
			
			
            List<CalendarEvent> events = new List<CalendarEvent>();

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
								backgroundColor = eventColor
							};

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
    }
}
