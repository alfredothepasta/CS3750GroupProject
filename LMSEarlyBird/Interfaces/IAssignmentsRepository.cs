using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface IAssignmentsRepository
    {
        Task<List<Assignment>> GetCourseAssignments(int courseId);
        Task<List<StudentAssignment>> GetStudentAssignments(string studentId);
        Task<List<StudentAssignment>> GetStudentAssignmentsByCourse(string studentId, int courseId);

        Task<Assignment> GetAssignment(int assignmentId);
        Task<bool> AddStudentAssignments(string userId, int courseId);
        Task<bool> RemoveStudentAssignments(string userId, int courseId);

		Task<bool> AddAssignment(Assignment assignment, int courseId);
        bool RemoveAssignment(Assignment assignment);
        bool UpdateAssignment(Assignment assignment);

        bool SetStudentAssignmentSubmitted(string studentId, int assignmentId, string txtSubmission);
        bool SetStudentAssignmentSubmitted(string studentId, int assignmentId);
        bool Save();

    }
}
