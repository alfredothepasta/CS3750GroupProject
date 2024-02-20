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
					Graded = false
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
                    Graded = false
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

        public async Task<List<Assignment>> GetStudentAssignmentsByCourse(string studentId, int courseId)
        {
            var user = await _context.AppUsers
            .Include(x => x.StudentAssignment)
            .ThenInclude(sa => sa.Assignment)
            .FirstAsync(x => x.Id == studentId);

            return user.StudentAssignment
            .Where(x => x.Assignment.CourseId == courseId)
            .Select(x => x.Assignment)
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

        public bool UpdateAssignment(Assignment assignment)
        {
            _context.Assignments.Update(assignment);
            return Save();
        }
    }
}
