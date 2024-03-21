using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.Repository;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
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
        /// <summary>
        /// Context for accessing the balance database
        /// </summary>
        private readonly IBalanceRepository _balanceRepository;
        private readonly IMemoryCache _cache;
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
            IStudentCourseRepository studentCourseRepository,
            IBalanceRepository balanceRepository,
            IMemoryCache? cache = null)
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
            _balanceRepository = balanceRepository;
            _cache = cache;
        }

        #endregion

        #region CourseListAndCreation

        #region Course List
        [HttpGet]
        public async Task<IActionResult> CourseList()
        {
            // check if the user is logged in to an account with the instructor role
            if (isNotInstructor())
            {
                return RedirectToAction("Dashboard", "User");
            }

            // get the current users's roles for accessing the courses associated with them in the db
            var instructorId = _contextAccessor.HttpContext.User.GetUserId();
            // get the courses associated with the user
            List<Course> courses = await _courseRepository.GetCoursesByTeacher(instructorId);
            Dictionary<string, double> avgCourseScore = new Dictionary<string, double>();
            
            foreach(Course course in courses)
            {
                List<StudentAssignment> studentAssingments = await _assignmentRepository.GetStudentAssignmentsByCourse(course.id);

                double gradedSum = 0;
                double maxGradedSum = 0;
                double averageGrade = 0;

                foreach (StudentAssignment individualAssignment in studentAssingments)
                {
                    if (individualAssignment.Graded)
                    {
                        gradedSum += individualAssignment.Score;
                        maxGradedSum += individualAssignment.Assignment.maxPoints;
                    }
                }
                
                if(maxGradedSum > 0)
                {
                    averageGrade = gradedSum / maxGradedSum * 100;
                }

                string key = $"{course.Department.DeptCode} {course.CourseNumber} {course.CourseName}";
                avgCourseScore[key] = averageGrade;
            }

            CourseListViewModel viewModel = new CourseListViewModel()
            {
                Courses = courses,
                AvgScorePerCourse = avgCourseScore
            };

            return View(viewModel);

        }
        #endregion

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
                return RedirectToAction("Dashboard", "User");
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
            string instructorId = _contextAccessor.HttpContext.User.GetUserId();
            await pushCourseToDb(viewModel, instructorId);

            DeleteCachedClassCards(instructorId);

            return RedirectToAction("CourseList", "Instructor");
        }

        void DeleteCachedClassCards(string userId)
        {
            var cacheKeyClassCards = $"user_{userId}_classcards";
            _cache.Remove(cacheKeyClassCards);
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
            var userId = _contextAccessor.HttpContext.User.GetUserId();
            bool canDelete = await isInstructorAssignedToCourse(courseId, userId);
            if (!canDelete)
            {
                return RedirectToAction("CourseList", "Instructor");
            }

            await DeleteCourseAndRemoveStudents(courseId, userId);
            //Clear cached cards
            DeleteCachedClassCards(userId);

            return RedirectToAction("CourseList", "Instructor");
        }

        #region Course Helpers
        public async Task pushCourseToDb(AddCourseViewModel viewModel, string userId)
        {

            // Find the days that have been selected
            string selectedDays = "";
            if (viewModel.DayOfWeekM == "true") selectedDays = selectedDays + "M";
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

            AppUser instructor = await _appUserRepository.GetUser(userId);

            _courseRepository.Add(course, instructor);
        }

        public async Task DeleteCourseAndRemoveStudents(int courseId, string instructorId)
        {
            // create a list of the students that are assigned to the course
            List<AppUser> students = await _studentCourseRepository.GetStudentsByCourse(courseId);

            // delete the course
            Course courseToDelete = await _courseRepository.GetCourse(courseId);
            await _courseRepository.Delete(courseToDelete);

            // delete each student's balance in the list for the course that was just deleted
            foreach (AppUser student in students)
            {
                await _balanceRepository.UpdateBalanceDropCourse(student.Id, courseToDelete.CreditHours, courseToDelete.CourseName);
            }
        }

        #endregion

        #endregion

        #region Edit Course

        [HttpGet]
        public async Task<IActionResult> EditCourse(int courseId)
        {
            var userId = _contextAccessor.HttpContext.User.GetUserId();
            if (isNotInstructor() || ! await isInstructorAssignedToCourse(courseId, userId))
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
            var userId = _contextAccessor.HttpContext.User.GetUserId();
            // todo: validate that the user can access this course
            bool courseValidation = await isInstructorAssignedToCourse(courseId, userId);

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

            var userId = _contextAccessor.HttpContext.User.GetUserId();

            bool courseValidation = await isInstructorAssignedToCourse(courseId, userId);

            if (!courseValidation)
            {
                return RedirectToAction("Dashboard", "User");
            }

            await pushAssignmentToDb(viewModel, courseId);

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            return RedirectToAction("CourseAssignmentList", new { courseId = viewModel.Course.id });

		}

        public async Task pushAssignmentToDb(CreateAssignmentViewModel viewModel, int courseId)
        {
            Course course = await _courseRepository.GetCourse(courseId);
			viewModel.Course = course;

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
            var userId = _contextAccessor.HttpContext.User.GetUserId();
            // check if the user is logged in to an account with the instructor role
            if (isNotInstructor() || !await isInstructorAssignedToCourse(courseId, userId))
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
            var userId = _contextAccessor.HttpContext.User.GetUserId();
            // check if the user is logged in to an account with the instructor role
            if (isNotInstructor() || !await isInstructorAssignedToCourse(courseId, userId))
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
            Assignment assignment = await _assignmentRepository.GetAssignmentById(assignmentId);
            
            double maxGrade = 0;
            double minGrade = assignment.maxPoints;
            double avgGrade = 0;
            double gradeSum = 0;
            int numGraded = 0;


            foreach (var student in registeredStudents)
            {
                StudentAssignment studentAssignment = await _assignmentRepository.GetStudentAssignment(student.Id,assignmentId);

                if (studentAssignment.Graded)
                {
                    if(studentAssignment.Score < minGrade) minGrade = studentAssignment.Score;
                    if(studentAssignment.Score > maxGrade) maxGrade = studentAssignment.Score;
                    gradeSum += studentAssignment.Score;
                    numGraded++;
                }

                if (studentAssignment.Submitted)
                {
                    studentAssignments.Add(studentAssignment);
                }
            }

            if(numGraded == 0) minGrade = 0;
            if(studentAssignments.Count > 0) avgGrade = gradeSum / studentAssignments.Count;

            AssignmentSubmissionsListViewModel viewModel = new AssignmentSubmissionsListViewModel
            {
                Course = await _courseRepository.GetCourse(courseId),
                Assignment = await _assignmentRepository.GetAssignment(assignmentId),
                Assignments = studentAssignments,
                MinimumGrade = minGrade,
                MaximumGrade = maxGrade,
                AverageGrade = avgGrade,
            };
            return View(viewModel);
        }

        #endregion

        #region AssignmentGradePage

        [HttpGet]
        public async Task<IActionResult> AssignmentGrade(int assignmentId, string studentId){
            if (isNotInstructor())
            {
                return NotFound();
            }

            //Get student assignment 
            var assignment = await _assignmentRepository.GetStudentAssignment(studentId, assignmentId);

            if(assignment == null){
                return NotFound();
            }

            var viewModel = new AssigmentGradeViewModel
            {
                AssignmentName = assignment.Assignment.Title,
                StudentName = assignment.Student.FirstName + " " + assignment.Student.LastName,
                DueDate = FormatDueDate(assignment.Assignment.DueDate),
                //ADD SUBMISSION DATE HERE WHEN ADDED TO DATABASE
                SubmissionDate = FormatDueDate((DateTime)assignment.SubmissionTime),
                Graded = assignment.Graded,
                GradedPoints = assignment.Score,
                MaxPoints = assignment.Assignment.maxPoints,
                Submitted = assignment.Submitted,
                LateSubmission = assignment.SubmissionTime > assignment.Assignment.DueDate,
                FileName = assignment.FileName,

                AssignmentId = assignment.AssignmentId,
                StudentId = assignment.StudentId,
                CourseId = assignment.Assignment.CourseId
            };

            if(assignment.FileName != null && assignment.FileName.Length > 0){
                viewModel.FileSubmission = true;
            }
            else{
                viewModel.TextSubmission = assignment.Submission;
            }
            if(assignment.SubmissionComment != null){
                viewModel.SubmissionComment = assignment.SubmissionComment;
            }


            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignmentGrade(AssigmentGradeViewModel gradeInfo){
            if (isNotInstructor()){
                return NotFound();
            }

            _assignmentRepository.GradeAssignment(gradeInfo.StudentId, gradeInfo.AssignmentId, gradeInfo.GradedPoints, gradeInfo.SubmissionComment);
            return RedirectToAction("AssignmentSubmissionsList", "Instructor", new { assignmentId = gradeInfo.AssignmentId, courseId = gradeInfo.CourseId });
        }

        public ActionResult DownloadAssignment(string studentId,int courseId, int assignmentId, string fileName)
        {
            if (isNotInstructor())
            {
                if(studentId != _contextAccessor.HttpContext.User.GetUserId()){
                    return NotFound();
                }
            }

            var webHostEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var assignmentsRoot = Path.Combine(webHostEnvironment.WebRootPath, "assignments");

            string contentType = "application/octet-stream";
            var fileProvider = new PhysicalFileProvider(assignmentsRoot);
            IFileInfo fileInfo = fileProvider.GetFileInfo(Path.Combine(studentId, courseId.ToString(), assignmentId.ToString(), fileName));

            if (!fileInfo.Exists)
            {
                return NotFound();
            }

            var readStream = fileInfo.CreateReadStream();
            return File(readStream, contentType, fileName);

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
        
        #endregion

        #region Validation Methods
        private bool isNotInstructor() {
            return !User.IsInRole(UserRoles.Teacher);
        }

		private async Task<bool> isInstructorAssignedToCourse(int courseId, string userId)
		{
            
			List<Course> courses = await _courseRepository.GetCoursesByTeacher(userId);
			Course currentCourse = await _courseRepository.GetCourse(courseId);
			return courses.Contains(currentCourse);
		}
		#endregion  
	}
}