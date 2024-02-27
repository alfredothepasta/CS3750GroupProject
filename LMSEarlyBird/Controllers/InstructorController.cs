using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.Repository;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Web;

namespace LMSEarlyBird.Controllers
{
    public class InstructorController : Controller
    {
        #region params
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
        private readonly IAssignmentsRepository _assignmentRepository;
        //private readonly FileExtensionContentTypeProvider _contentTypeProvider;
        private readonly IStudentCourseRepository _studentCourseRepository;
        #endregion

        #region constructor
        public InstructorController(ApplicationDbContext context, 
            IHttpContextAccessor contextAccessor, 
            ICourseRepository courseRepository, 
            IBuildingRepository buildingRepository, 
            IRoomRepository roomRepository,
            IDepartmentRepository departmentRepository,
            IAppUserRepository appUserRepository,
            IAssignmentsRepository assignmentRepository,
            IStudentCourseRepository studentCourseRepository)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _courseRepository = courseRepository;
            _buildingRepository = buildingRepository;
            _roomRepository = roomRepository;
            _departmentRepository = departmentRepository;
            _appUserRepository = appUserRepository;
            _assignmentRepository = assignmentRepository;
            //_contentTypeProvider = contentTypeProvider;
            _studentCourseRepository = studentCourseRepository;
        }

        #endregion  

        #region CourseListAndCreation
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

        #region Add Course
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

            AddCourseViewModel viewModel = new AddCourseViewModel
            {
                BuildingList = buildings,
                DepartmentList = departments,
                RoomList = rooms
            };

            return View(viewModel);
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
                viewModel.BuildingList = buildings;
                viewModel.DepartmentList = departments;
                viewModel.RoomList = rooms;

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

        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            
            bool canDelete = await isInstructorAssignedToCourse(courseId);
            if (!canDelete)
            {
                return RedirectToAction("CourseList", "Instructor");
            }

            Course courseToDelete = await _courseRepository.GetCourse(courseId);
            await _courseRepository.Delete(courseToDelete);
            return RedirectToAction("CourseList", "Instructor");
        }
        #endregion


        #region Edit Course

        [HttpGet]
        public async Task<IActionResult> EditCourse(int courseId)
        {
            if (isNotInstructor() || ! await isInstructorAssignedToCourse(courseId))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            } 

            // get the course
            Course currentCourse = await _courseRepository.GetCourse(courseId);
            if (currentCourse == null)
            {
                // do something
                return NoContent();
            }
            var buildings = await _buildingRepository.GetBuildings();
            var rooms = await _roomRepository.GetRooms();
            var departments = await _departmentRepository.GetAllDepartments();

			AddCourseViewModel viewModel = new AddCourseViewModel()
            {
                CourseID = currentCourse.id,
				Department = currentCourse.DepartmentId,
                CourseNumber = currentCourse.CourseNumber,
                CourseName = currentCourse.CourseName,
                CreditHours = currentCourse.CreditHours,
                StartTime = currentCourse.StartTime,
                EndTime = currentCourse.EndTime,
                Building = currentCourse.Room.BuildingID,
                Room = currentCourse.RoomId,
                BuildingList = buildings,
                RoomList = rooms,
                DepartmentList = departments,
            };
            
            foreach(char c in currentCourse.DaysOfWeek)
            {
                switch (c)
                {
                    case 'M':
                        viewModel.DayOfWeekM = "true";
                        break;
                    case 'T':
                        viewModel.DayOfWeekT = "true";
                        break;
                    case 'W':
                        viewModel.DayOfWeekW = "true"; 
                        break;
                    case 'R':
                        viewModel.DayOfWeekR = "true";
                        break;
                    case 'F':
                        viewModel.DayOfWeekF = "true";
                        break;
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditCourse(AddCourseViewModel viewModel, int courseId)
        {
			if (!ModelState.IsValid)
			{
				// get the building and departmen dropdowns
				List<Building> buildings = await _buildingRepository.GetBuildings();
				List<Department> departments = await _departmentRepository.GetAllDepartments();
				List<Room> rooms = await _roomRepository.GetRooms();
				viewModel.BuildingList = buildings;
				viewModel.DepartmentList = departments;
				viewModel.RoomList = rooms;

				return View(viewModel);
			}

            Course updatedCourse = await _courseRepository.GetCourse(courseId);
			
            // update all the course values
            updatedCourse.CourseNumber = viewModel.CourseNumber;
			updatedCourse.CourseName = viewModel.CourseName;
			updatedCourse.CreditHours = viewModel.CreditHours;
			updatedCourse.StartTime = viewModel.StartTime;
			updatedCourse.EndTime = viewModel.EndTime;
            updatedCourse.DepartmentId = viewModel.Department;
			updatedCourse.RoomId = viewModel.Room;
			
            // build the days of week
            string selectedDays = "";
			if (viewModel.DayOfWeekM == "true") selectedDays = selectedDays + "M";
			if (viewModel.DayOfWeekT == "true") selectedDays = selectedDays + "T";
			if (viewModel.DayOfWeekW == "true") selectedDays = selectedDays + "W";
			if (viewModel.DayOfWeekR == "true") selectedDays = selectedDays + "R";
			if (viewModel.DayOfWeekF == "true") selectedDays = selectedDays + "F";

            updatedCourse.DaysOfWeek = selectedDays;

            _courseRepository.Update(updatedCourse);

			return RedirectToAction("CourseList", "Instructor");
        }

		#endregion

		#endregion

        #region AssignmentGradePage
        public async Task<IActionResult> AssignmentGrade(int assignmentId, string studentId){
            if (isNotInstructor())
            {
                return NotFound();
            }

            var viewModel = new AssigmentGradeViewModel();

            //Get student assignment 
            var assignment = await _assignmentRepository.GetStudentAssignment(studentId, assignmentId);

            if(assignment == null){
                return NotFound();
            }

            viewModel.StudentName = assignment.Student.FirstName + " " + assignment.Student.LastName;
            viewModel.DueDate = FormatDueDate(assignment.Assignment.DueDate);
            //ADD SUBMISSION DATE HERE WHEN ADDED TO DATABASE
            viewModel.SubmissionDate = "02/26/2024 10:30 AM";
            viewModel.Graded = assignment.Graded;
            viewModel.GradedPoints = 90;
            viewModel.MaxPoints = assignment.Assignment.maxPoints;
            viewModel.Submitted = assignment.Submitted;
            viewModel.LateSubmission = true;

            viewModel.AssignmentId = assignment.AssignmentId;
            viewModel.StudentId = assignment.StudentId;
            viewModel.CourseId = assignment.Assignment.CourseId;
            
            return View(viewModel);
        }

        public ActionResult DownloadAssignment(string studentId,int courseId, int assignmentId)
        {
                var webHostEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
                var assignmentsRoot = Path.Combine(webHostEnvironment.WebRootPath, "assignments");

            string fileName = "";
            string dir = Path.Combine(assignmentsRoot, studentId + "/" + courseId.ToString() + "/" + assignmentId.ToString());
            string filePath = Path.Combine(dir, fileName);
            string contentType;

            return File(filePath, "application/octet-stream", fileName);

            // // Try to get the content type based on the file extension
            // if (_contentTypeProvider.TryGetContentType(fileName, out contentType))
            // {
            //     return File(filePath, contentType, fileName);
            // }
            // else
            // {
                
            // }

        }

        private string FormatDueDate(DateTime date){
            return date.ToString("MM/dd/yyyy hh:mm tt");
        }

        #endregion

		#region Assignment Methods

		#region Create Assignment
		public async Task<IActionResult> CourseAssignmentList(int courseId)
        {
            // check if the user is logged in to an account with the instructor role
            if (isNotInstructor())
            {
                return View("Dashboard", "User");
            }

            Course course = await _courseRepository.GetCourse(courseId);
            List<Assignment> assignments = await _assignmentRepository.GetCourseAssignments(courseId);


            CourseAssignmentListViewModel viewModel = new CourseAssignmentListViewModel
            {
                Course = course,
                Assignments = assignments
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAssignment(int courseId)
        {
            // check if the user is logged in to an account with the instructor role
            if (isNotInstructor())
            {
                return RedirectToAction("Dashboard", "User");
            }

            // todo: validate that the user can access this course
            bool courseValidation = await isInstructorAssignedToCourse(courseId);

			Course course = await _courseRepository.GetCourse(courseId);
                
            if (!courseValidation)
            {
                return RedirectToAction("Dashboard", "User");
            }

            CreateAssignmentViewModel viewModel = new CreateAssignmentViewModel
            {
                Course = course
            };


            return View(viewModel);
        }

		

		[HttpPost]
        public async Task<IActionResult> CreateAssignment(int courseId, CreateAssignmentViewModel viewModel)
        {
            // check if the user is logged in to an account with the instructor role
            if (isNotInstructor())
            {
                return RedirectToAction("Dashboard", "User");
            }

            bool courseValidation = await isInstructorAssignedToCourse(courseId);

            if (!courseValidation)
            {
                return RedirectToAction("Dashboard", "User");
            }

            Course course = await _courseRepository.GetCourse(courseId);
			viewModel.Course = course;
            

			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			Assignment assignment = new Assignment
            {
				Title = viewModel.Title,
                Description = viewModel.Description,
				Type = viewModel.Type,
				DueDate = viewModel.DueDate,
				Course = viewModel.Course,
				maxPoints = viewModel.maxPoints
			};

            await _assignmentRepository.AddAssignment(assignment, courseId);
			return RedirectToAction("CourseAssignmentList", new { courseId = viewModel.Course.id });

		}

        [HttpGet]
        public async Task<IActionResult> DeleteAssignment(int assignmentId, int courseId)
        {
            Assignment assignmentToDelete = await _assignmentRepository.GetAssignmentById(assignmentId);
            if (await _assignmentRepository.RemoveAssignment(assignmentToDelete))
            {
                return RedirectToAction("CourseAssignmentList", new { courseId = courseId });
            } else
            {
                return StatusCode(StatusCodes.Status408RequestTimeout);
            }
        }
        #endregion

        #region Edit Assignment

        [HttpGet]
        public async Task<IActionResult> EditAssignment(int assignmentId, int courseId)
        {
			// check if the user is logged in to an account with the instructor role
			if (isNotInstructor() || !await isInstructorAssignedToCourse(courseId))
			{
				return StatusCode(StatusCodes.Status403Forbidden);
			}

            Assignment assignmentToEdit = await _assignmentRepository.GetAssignmentById(assignmentId);


			CreateAssignmentViewModel viewModel = new CreateAssignmentViewModel
			{
				AssignmentId = assignmentToEdit.Id,
				Course = assignmentToEdit.Course,
				Title = assignmentToEdit.Title,
				Description = assignmentToEdit.Description,
				Type = assignmentToEdit.Type,
				DueDate = assignmentToEdit.DueDate,
				maxPoints = assignmentToEdit.maxPoints
			};

			return View(viewModel);
		}

        [HttpPost]
		public async Task<IActionResult> EditAssignment(int courseId, int assignmentId, CreateAssignmentViewModel viewModel)
        {
			// check if the user is logged in to an account with the instructor role
			if (isNotInstructor() || !await isInstructorAssignedToCourse(courseId))
			{
				return StatusCode(StatusCodes.Status403Forbidden);
			}

			if (!ModelState.IsValid)
			{
                viewModel.Course = await _courseRepository.GetCourse(courseId);
				return View(viewModel);
			}

			Assignment assignmentToEdit = await _assignmentRepository.GetAssignmentById(assignmentId);
			assignmentToEdit.Title = viewModel.Title;
			assignmentToEdit.Description = viewModel.Description;
			assignmentToEdit.Type = viewModel.Type;
			assignmentToEdit.DueDate = viewModel.DueDate;
			assignmentToEdit.maxPoints = viewModel.maxPoints;

            if (_assignmentRepository.UpdateAssignment(assignmentToEdit))
            {
				return RedirectToAction("CourseAssignmentList", new { courseId = courseId });
			} else
            {
				return StatusCode(StatusCodes.Status408RequestTimeout);
			}
		}
        #endregion

        #region AssignmentList

        public async Task<IActionResult> AssignmentSubmissionsList(int assignmentId, int courseId)
        {
            // check if the user is logged in to an account with the instructor role
            if (isNotInstructor())
            {
                return View("Dashboard", "User");
            }

            List<AppUser> registeredStudents = await _studentCourseRepository.GetStudentsByCourse(courseId);
            List<StudentAssignment> studentAssignments = new List<StudentAssignment>();

            foreach (var student in registeredStudents)
            {
                StudentAssignment studentAssignment = await _assignmentRepository.GetStudentAssignment(student.Id,assignmentId);

                if (studentAssignment.Submitted)
                {
                    studentAssignments.Add(studentAssignment);
                }
            }

            AssignmentSubmissionsListViewModel viewModel = new AssignmentSubmissionsListViewModel
            {
                Course = await _courseRepository.GetCourse(courseId),
                Assignment = await _assignmentRepository.GetAssignment(assignmentId),
                Assignments = studentAssignments
            };
            return View(viewModel);
        }

        #endregion

        #endregion

        #region Validation Methods
        private bool isNotInstructor() {
            return !User.IsInRole(UserRoles.Teacher);
        }

		private async Task<bool> isInstructorAssignedToCourse(int courseId)
		{
            var userId = _contextAccessor.HttpContext.User.GetUserId();
			List<Course> courses = await _courseRepository.GetCoursesByTeacher(userId);
			Course currentCourse = await _courseRepository.GetCourse(courseId);
			return courses.Contains(currentCourse);
		}
		#endregion  
	}
}