using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface IAssignmentsRepository
    {
        Task<List<Assignment>> GetCourseAssignments(int courseId);
        Task<List<StudentAssignment>> GetStudentAssignments(string studentId);
        Task<List<Assignment>> GetStudentAssignmentsByCourse(string studentId, int courseId);
        Task<bool> AddStudentAssignments(string userId, int courseId);
        Task<bool> RemoveStudentAssignments(string userId, int courseId);

		Task<bool> AddAssignment(Assignment assignment, int courseId);
        bool RemoveAssignment(Assignment assignment);
        bool UpdateAssignment(Assignment assignment);
        bool Save();

    }
}
