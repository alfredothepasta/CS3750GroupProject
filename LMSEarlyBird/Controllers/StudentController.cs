using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.Repository;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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

        private readonly IMemoryCache _cache;
        /// <summary>
        /// Context for accessing the balance database
        /// </summary>
        private readonly IBalanceRepository _balanceRepository;
        public StudentController(ICourseRepository courseRepository
            , IUserIdentityService? userIdentityService
            , IAppUserRepository appUserRepository
            , IStudentCourseRepository studentCourseRepository
            , IDepartmentRepository departmentRepository
            , IAssignmentsRepository assignmentsRepository
            , IBalanceRepository balanceRepository
            , IMemoryCache? cache)
        {
            _courseRepository = courseRepository;
            _userIdentityService = userIdentityService;
            _appUserRepository = appUserRepository;
            _studentCourseRepository = studentCourseRepository;
            _departmentRepository = departmentRepository;
            _assignmentsRepository = assignmentsRepository;
            _balanceRepository = balanceRepository;

            _cache = cache;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Registration(){

            if (isNotStudent())
            {
                return Redirect(Url.Action("Dashboard", "Dashboard"));
            }
            
            RegistrationViewModel result = new RegistrationViewModel();

            //Create list of department names
            List<string> departmentNames = _departmentRepository.GetAllDepartments().Result.Select(x => x.DeptName).ToList();
            result.DepartmentNames = departmentNames;

            result.Courses = await GetRegisterCourseViewModels();

            // pull user based on logged in user
            string userId = _userIdentityService.GetUserId();
            AppUser profile = await _appUserRepository.GetUser(userId);

            // provide a list of assignments for the user for the _Layout to display for the notifications
            List<StudentAssignment> assignments = await _assignmentsRepository.GetStudentAssignments(userId);

            result.StudentAssignment = assignments;

            return View(result);
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

            // pull user based on logged in user
            string userId = _userIdentityService.GetUserId();
            AppUser profile = await _appUserRepository.GetUser(userId);

            // provide a list of assignments for the user for the _Layout to display for the notifications
            List<StudentAssignment> assignments = await _assignmentsRepository.GetStudentAssignments(userId);

            result.StudentAssignment = assignments;

            return View("Registration", result);
        }    

        public async Task<bool> DBDropClass(StudentCourse studentCourse){
            // gather the course information as the credit hours will be required to remove the course cost
            studentCourse.Course = await _courseRepository.GetCourse(studentCourse.CourseId);

            try
            {
                bool deleted = _studentCourseRepository.Delete(studentCourse);
                if(deleted){
                    await _assignmentsRepository.RemoveStudentAssignments(studentCourse.UserId,studentCourse.CourseId);
                    await _balanceRepository.UpdateBalanceDropCourse(studentCourse.UserId, studentCourse.Course.CreditHours, studentCourse.Course.CourseName);
                    return true;
                }
            }
            catch(Exception ex)
            {

            }
            return false;
        }

        public async Task<IActionResult> DropClass(int id, string? search, string? deptSelected)
        {
            var userid = _userIdentityService.GetUserId();

            StudentCourse studentCourse = new StudentCourse
            {
                UserId = userid,
                CourseId = id
            };

            await DBDropClass(studentCourse);

            DeleteCachedClassCards(userid);

            if(search!=null || deptSelected!=null || deptSelected != "" || search != "")
            {
                return RedirectToAction(nameof(Search), new { query = search, category = deptSelected });
            }

            return RedirectToAction(nameof(Registration));
        }

        public async Task<bool> DBAddClass(StudentCourse studentCourse)
        {
            studentCourse.Course = _courseRepository.GetCourse(studentCourse.CourseId).Result;

            try
            {
                bool added = _studentCourseRepository.Add(studentCourse);

                if(added){
                    await _assignmentsRepository.AddStudentAssignments(studentCourse.UserId,studentCourse.CourseId);
                    await _balanceRepository.UpdateBalanceAddCourse(studentCourse.UserId, studentCourse.Course.CreditHours, studentCourse.Course.CourseName);
                }
        
                return added;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<IActionResult> AddClass(int id, string? search, string? deptSelected)
        {
            var userid = _userIdentityService.GetUserId();

            StudentCourse studentCourse = new StudentCourse
            {
                UserId = userid,
                CourseId = id   
            };

            // gather the course information as the credit hours will be required to add the course cost

            await DBAddClass(studentCourse);

            DeleteCachedClassCards(userid);

            if(search!=null || deptSelected!=null)
            {
                return RedirectToAction(nameof(Search), new { query = search, category = deptSelected });
            }

            return RedirectToAction(nameof(Registration));
        }

        void DeleteCachedClassCards(string userId)
        {
            var cacheKeyClassCards = $"user_{userId}_classcards";
            _cache.Remove(cacheKeyClassCards);
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

            if(courses == null)
                return new List<RegisterCourseViewModel>();

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

        public async Task<IActionResult> Course(int courseid){
            var userid = _userIdentityService.GetUserId();

            var user = await _appUserRepository.GetUser(userid);

            if(!user.StudentCourses.Any(x => x.CourseId == courseid))
            {
                return NotFound();
            }

            var courseAssignments = 
                await _assignmentsRepository.GetStudentAssignmentsByCourse(userid, courseid);

            courseAssignments = courseAssignments.OrderBy(x => x.Assignment.DueDate).ToList();

            // provide a list of assignments for the user for the _Layout to display for the notifications
            List<StudentAssignment> assignments = await _assignmentsRepository.GetStudentAssignments(userid);

            // CALCULATE GRADES *******************

            // get all students in the class
            var studentList = await _studentCourseRepository.GetStudentsByCourse(courseid);


            int numA = 0;
            int numAm = 0;
            int numBp = 0;
            int numB = 0;
            int numBm = 0;
            int numCp = 0;
            int numC = 0;
            int numCm = 0;
            int numDp = 0;
            int numD = 0;
            int numDm = 0;
            int numE = 0;
            int numF = 0;

            
            // create a list of all the students grades
            List<double> gradeList = new List<double>();
            foreach(var student in studentList)
            {
                var assignmentList = await _assignmentsRepository.GetStudentAssignmentsByCourse(student.Id, courseid);
                double gradePercentage = CaclulateCourseGrade(assignmentList);
                gradeList.Add(gradePercentage);

                // calculate letter grade
                if (gradePercentage >= 94.00)
                {
                    numA++;
                }
                else if (gradePercentage >= 90.00 && gradePercentage < 94.00)
                {
                    numAm++;
                }
                else if (gradePercentage >= 87.00 && gradePercentage < 90.00)
                {
                    numBp++;
                }
                else if (gradePercentage >= 84.00 && gradePercentage < 87.00)
                {
                    numB++;
                }
                else if (gradePercentage >= 80.00 && gradePercentage < 84.00)
                {
                    numBm++;
                }
                else if (gradePercentage >= 77.00 && gradePercentage < 80.00)
                {
                    numCp++;
                }
                else if (gradePercentage >= 74.00 && gradePercentage < 77.00)
                {
                    numC++;
                }
                else if (gradePercentage >= 70.00 && gradePercentage < 74.00)
                {
                    numCm++;
                }
                else if (gradePercentage >= 67.00 && gradePercentage < 70.00)
                {
                    numDp++;
                }
                else if (gradePercentage >= 64.00 && gradePercentage < 67.00)
                {
                    numD++;
                }
                else if (gradePercentage >= 60.00 && gradePercentage < 64.00)
                {
                    numDm++;
                }
                else if (gradePercentage < 60)
                {
                    numE++;
                }
                else
                {
                    numF++;
                }
            }


            // calculate the student's grade
            double grade = CaclulateCourseGrade(courseAssignments);
            string letterGrade = "F";

            // calculate letter grade
            if (grade >= 94.00)
            {
                letterGrade = "A";
            }
            else if (grade >= 90.00 && grade < 94.00)
            {
                letterGrade = "A-";
            }
            else if (grade >= 87.00 && grade < 90.00)
            {
                letterGrade = "B+";
            }
            else if (grade >= 84.00 && grade < 87.00)
            {
                letterGrade = "B";
            }
            else if (grade >= 80.00 && grade < 84.00)
            {
                letterGrade = "B-";
            }
            else if (grade >= 77.00 && grade < 80.00)
            {
                letterGrade = "C+";
            }
            else if (grade >= 74.00 && grade < 77.00)
            {
                letterGrade = "C";
            }
            else if (grade >= 70.00 && grade < 74.00)
            {
                letterGrade = "C-";
            }
            else if (grade >= 67.00 && grade < 70.00)
            {
                letterGrade = "D+";
            }
            else if (grade >= 64.00 && grade < 67.00)
            {
                letterGrade = "D";
            }
            else if (grade >= 60.00 && grade < 64.00)
            {
                letterGrade = "D-";
            }
            else if (grade < 60)
            {
                letterGrade = "E";
            }

            // get the highest, lowest and average grade
            double averageGrade = 0;
            double highestGrade = 0;
            double lowestGrade = 100;
            foreach(var x in gradeList)
            {
                if(x < lowestGrade)
                {
                    lowestGrade = x;
                }
                if(x > highestGrade)
                {
                    highestGrade = x;
                }
                averageGrade += x;
            }
            if(gradeList.Count > 0)
            {
                averageGrade = averageGrade / gradeList.Count;
                Math.Round(averageGrade, 2);
            }

            
            // create viewmodel
            CourseAssignmentsStudentViewModel viewModel = new CourseAssignmentsStudentViewModel
            {
                Course = await _courseRepository.GetCourse(courseid),
                Assignments = courseAssignments,
                StudentAssignment = assignments,
                Grade = grade,
                LetterGrade = letterGrade,
                numA = numA,
                numAm = numAm,
                numBp = numBp,
                numB = numB,
                numBm = numBm,
                numCp = numCp,
                numC = numC,
                numCm = numCm,
                numDp = numDp,
                numD = numD,
                numDm = numDm,
                numE = numE,
                numF = numF,
        };

            return View(viewModel);
        }

        /// <summary>
        /// Takes a list of all assigments in a students class to calculate their grade
        /// </summary>
        /// <param name="courseAssignments"></param>
        /// <returns></returns>
        private double CaclulateCourseGrade(List<StudentAssignment> courseAssignments)
        {
            decimal dGrade = 0.0m;
            int count = 0;
            foreach (var assignment in courseAssignments)
            {
                if (assignment.Graded)
                {
                    decimal temp = ((decimal)assignment.Score / assignment.Assignment.maxPoints) * 100;
                    dGrade += temp;
                    count++;
                }
            }

            if (count > 0)
            {
                dGrade = dGrade / count;
            }

            double grade = (double)dGrade;
            grade = Math.Round(grade, 2);

            return grade;
        }

        private string FormatDueDate(DateTime date){
            return date.ToString("MM/dd/yyyy hh:mm tt");
        }

        [HttpGet]
        public async Task<IActionResult> Submission(int assignmentId){
            var userid = _userIdentityService.GetUserId();

            //if student does not have the assignment do not show page
            var studentAssignments = await _assignmentsRepository.GetStudentAssignments(userid);

            if(!studentAssignments.Any(x => x.AssignmentId == assignmentId)){
                return NotFound();
            };

            var studentAssignment = studentAssignments.FirstOrDefault(x => x.AssignmentId == assignmentId);

            var assignment = await _assignmentsRepository.GetAssignment(assignmentId);

            // provide a list of assignments for the user for the _Layout to display for the notifications
            List<StudentAssignment> assignments = await _assignmentsRepository.GetStudentAssignments(userid);

            // get data for graph
            List<StudentAssignment> classAssignments = await _assignmentsRepository.GetSubmittedAssignmentsByAssignment(assignmentId);
            int classAverage = 0;
            int classMaxScore = 0;
            int classMinScore = assignment.maxPoints;
            int numGradedAssignments = 0;

            if (studentAssignment.Graded)
            {
                // loop through every assigment that was graded
                foreach (var classAssignment in classAssignments)
                {
                    if (classAssignment.Graded)
                    {
                        numGradedAssignments++;

                        // add up all the students points
                        classAverage += classAssignment.Score;

                        // find the highest score
                        if (classAssignment.Score > classMaxScore)
                        {
                            classMaxScore = classAssignment.Score;
                        }

                        // find the lowest score
                        if (classAssignment.Score < classMinScore)
                        {
                            classMinScore = classAssignment.Score;
                        }
                    }
                }
                if(numGradedAssignments > 0)
                {
                    // calculate the class average
                    classAverage = classAverage / numGradedAssignments;
                }
            }

            SubmissionViewModel submissionViewModel = new SubmissionViewModel
            {
                Title = assignment.Title,
                Description = assignment.Description,
                MaxPoints = assignment.maxPoints,
                Type = assignment.Type,
                DueDate = FormatDueDate(assignment.DueDate),
                AssignmentId = assignment.Id,
                Submitted = studentAssignment.Submitted,
                SubmissionTxt = studentAssignment.Submission,
                Graded = studentAssignment.Graded,
                Score = studentAssignment.Score,
                FileName = studentAssignment.FileName,
                CourseId = assignment.CourseId,
                StudentId = studentAssignment.StudentId,
                StudentAssignment = assignments
                classAverage = classAverage,
                classMaxScore = classMaxScore,
                classMinScore = classMinScore,
            };

            if(studentAssignment.Submitted){
                submissionViewModel.SubmissionDate = FormatDueDate((DateTime)studentAssignment.SubmissionTime);

                submissionViewModel.Late = studentAssignment.SubmissionTime > assignment.DueDate;

                submissionViewModel.SubmissionComment = studentAssignment.SubmissionComment;
            }

            return View(submissionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Submission(SubmissionViewModel submissionViewModel){
            var userid = _userIdentityService.GetUserId();
            var assignment = await _assignmentsRepository.GetAssignment(submissionViewModel.AssignmentId);

            var file = submissionViewModel.File;

            if(file != null){
                //Mark assignment as submitted
                _assignmentsRepository.SetStudentAssignmentSubmitted(file.FileName, userid, assignment.Id);

                // Ensure the wwwroot/assignments directory exists
                var webHostEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
                var assignmentsRoot = Path.Combine(webHostEnvironment.WebRootPath, "assignments");
                if (!Directory.Exists(assignmentsRoot))
                {
                    Directory.CreateDirectory(assignmentsRoot);
                }

                // Ensure the user's directory exists
                var userDirectory = Path.Combine(assignmentsRoot, userid.ToString());
                if (!Directory.Exists(userDirectory))
                {
                    Directory.CreateDirectory(userDirectory);
                }

                // Ensure the course's directory exists
                var courseDirectory = Path.Combine(userDirectory, assignment.CourseId.ToString());
                if (!Directory.Exists(courseDirectory))
                {
                    Directory.CreateDirectory(courseDirectory);
                }

                // Ensure Assignment directory exists
                var assignmentDirectory = Path.Combine(courseDirectory, assignment.Id.ToString());
                if (!Directory.Exists(assignmentDirectory))
                {
                    Directory.CreateDirectory(assignmentDirectory);
                }

                var filePath = Path.Combine(assignmentDirectory, file.FileName);

                // Copy the file
                if (file != null)
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            else{
                _assignmentsRepository.SetStudentAssignmentSubmitted(userid, assignment.Id, submissionViewModel.SubmissionTxt);
            }

            return RedirectToAction(nameof(Course), new { courseid = assignment.CourseId });
        }

        // used to mark new assignment as understood
        public async Task<IActionResult> MarkAsUnderstood(int assignmentId)
        {
            var userid = _userIdentityService.GetUserId();
            _assignmentsRepository.ChangeAssignmentNewStatusRead(userid, assignmentId);
            return RedirectToAction("Dashboard", "User");
        }

        // used to mark graded assignment as understood
        public async Task<IActionResult> MarkAsGradedUnderstood(int assignmentId)
        {
            var userid = _userIdentityService.GetUserId();
            _assignmentsRepository.ChangeAssignmentGradedStatusRead(userid, assignmentId);
            return RedirectToAction("Dashboard", "User");
        }

        //validation
        private bool isNotStudent()
        {
            return !User.IsInRole(UserRoles.Student);
        }
    }
}