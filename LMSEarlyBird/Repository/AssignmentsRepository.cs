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
			throw new NotImplementedException();
		}

		public async Task<List<Assignment>> GetCourseAssignments(int courseId)
        {
			return await _context.Assignments.Where(c => c.CourseId == courseId).ToListAsync();
        }

        public async Task<List<Assignment>> GetStudentAssignments(string studentId)
        {
            throw new NotImplementedException();
        }
    

        public bool RemoveAssignment(Assignment assignment)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool UpdateAssignment(Assignment assignment)
        {
            throw new NotImplementedException();
        }
    }
}
