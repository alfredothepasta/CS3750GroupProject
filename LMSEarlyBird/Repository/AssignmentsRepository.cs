using LMSEarlyBird.Data;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using Microsoft.EntityFrameworkCore;

namespace LMSEarlyBird.Repository
{
    public class AssignmentsRepository : IAssignmentsRepository
    {
        private readonly ApplicationDbContext _context;

        public AssignmentsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddAssignment(Assignment assignment)
        {
            _context.Add(assignment);
            return Save();
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
