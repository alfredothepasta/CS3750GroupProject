using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Migrations;
using LMSEarlyBird.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace LMSEarlyBird.Repository
{
    public class AssignmentsRepository : IAssignmentsRepository
    {
        private readonly ApplicationDbContext _context;

        public AssignmentsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAssignment(Assignment assignment, int courseId)
        {
			// get a list of users that are enrolled in the course
			List<AppUser> courseUsers = _context.
				AppUsers.
				Where(u => u.StudentCourses.
					Where(c => c.CourseId == courseId)
					.Any()
				).ToList();


			List<StudentAssignment> studentAssignments = new List<StudentAssignment>();
			foreach (AppUser user in courseUsers)
			{
				StudentAssignment newAssignment = new StudentAssignment()
				{
					Student = user,
					Assignment = assignment,
					Score = 0,
					Submitted = false,
					Graded = false,
                    CreatedNotification = true,
                    GradedNotification = false
				};
				studentAssignments.Add(newAssignment);
			}
			_context.AddRange(studentAssignments);
			_context.Add(assignment);
			return Save();
        }

		public async Task<bool> AddStudentAssignments(string userId, int courseId)
		{
            var user = await _context.AppUsers.FirstAsync(x => x.Id == userId);

            //Get all current assignmnets for course
            List<Assignment> assignments = await GetCourseAssignments(courseId);

            //Create new StudentAssignments for each assignment
            var studentAssignments = new List<StudentAssignment>();
            foreach(var assignment in assignments)
            {
                StudentAssignment newAssignment = new StudentAssignment()
                {
                    Student = user,
                    Assignment = assignment,
                    Score = 0,
                    Submitted = false,
                    Graded = false,
                    CreatedNotification = true,
                    GradedNotification = false
                };

                studentAssignments.Add(newAssignment);
            }

            _context.AddRange(studentAssignments);
            return Save();
		}

        public async Task<Assignment> GetAssignmentById(int assignmentId)
        {
                return await _context.Assignments.Where(a => a.Id == assignmentId).FirstAsync();
        }

        public async Task<Assignment> GetAssignment(int assignmentId)
        {
            return await _context.Assignments.Where(x => x.Id == assignmentId).FirstAsync();
        }

        public async Task<List<Assignment>> GetCourseAssignments(int courseId)

        {
			return await _context.Assignments.Where(c => c.CourseId == courseId).ToListAsync();
        }

        public async Task<List<StudentAssignment>> GetStudentAssignments(string studentId)
        {
            var user = await _context.AppUsers
            .Include(x => x.StudentAssignment)
            .ThenInclude(sa => sa.Assignment)
            .FirstAsync(x => x.Id == studentId);

            return user.StudentAssignment;
        }

        public async Task<List<StudentAssignment>> GetStudentAssignmentsByCourse(string studentId, int courseId)
        {
            var user = await _context.AppUsers
            .Include(x => x.StudentAssignment)
            .ThenInclude(sa => sa.Assignment)
            .FirstAsync(x => x.Id == studentId);

            return user.StudentAssignment
            .Where(x => x.Assignment.CourseId == courseId)
            .ToList();
        }

        public async Task<bool> RemoveAssignment(Assignment assignment)
        {
            List<StudentAssignment> studentAssignments = await _context.StudentAssignments.
                Where(x => x.AssignmentId == assignment.Id).
                ToListAsync();

            _context.RemoveRange(studentAssignments);
            _context.Remove(assignment);
            return Save();
        }


        public async Task<bool> RemoveStudentAssignments(string studentId, int courseId)
        {
            var user = await _context.AppUsers
            .Include(x => x.StudentAssignment)
            .ThenInclude(sa => sa.Assignment)
            .FirstAsync(x => x.Id == studentId);

            var courseStudentAssignments = user.StudentAssignment
            .Where(x => x.Assignment.CourseId == courseId);

            _context.RemoveRange(courseStudentAssignments);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool SetStudentAssignmentSubmitted(string fileName, string studentId, int assignmentId)
        {
            var assignment = GetStudentAssignments(studentId).Result.FirstOrDefault(x => x.AssignmentId == assignmentId);

            if (assignment == null)
            {
                return false;
            }

            assignment.Submitted = true;
            assignment.FileName = fileName;
            assignment.SubmissionTime = DateTime.Now;

            return Save();
        }

        public bool SetStudentAssignmentSubmitted(string studentId, int assignmentId, string txtSubmission)
        {
            var assignment = GetStudentAssignments(studentId).Result.FirstOrDefault(x => x.AssignmentId == assignmentId);

            if (assignment == null)
            {
                return false;
            }

            assignment.Submitted = true;
            assignment.Submission = txtSubmission;
            assignment.SubmissionTime = DateTime.Now;

            return Save();
        }

        public bool UpdateAssignment(Assignment assignment)
        {
            _context.Assignments.Update(assignment);
            return Save();
        }

        public async Task<StudentAssignment> GetStudentAssignment(string studentId, int assignmentId)
        {
            return await _context.StudentAssignments.Include(x => x.Assignment).Include(x => x.Student).FirstOrDefaultAsync(x => x.AssignmentId == assignmentId && x.StudentId == studentId);
        }

        public bool GradeAssignment(string studentId, int assignmentId, int grade, string comment)
        {
            var assignment = GetStudentAssignment(studentId, assignmentId).Result;
            assignment.Graded = true;
            assignment.Score = grade;
            assignment.SubmissionComment = comment;
            assignment.GradedNotification = true;

            _context.StudentAssignments.Update(assignment);
            return Save();
        }
    }
}
