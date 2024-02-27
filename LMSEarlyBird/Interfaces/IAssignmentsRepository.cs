using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface IAssignmentsRepository
    {
        Task<List<Assignment>> GetCourseAssignments(int courseId);
        Task<List<StudentAssignment>> GetStudentAssignments(string studentId);
        Task<List<StudentAssignment>> GetStudentAssignmentsByCourse(string studentId, int courseId);

        Task<StudentAssignment> GetStudentAssignment(string studentId, int assignmentId);

        Task<Assignment> GetAssignment(int assignmentId);
        Task<bool> AddStudentAssignments(string userId, int courseId);
        Task<bool> RemoveStudentAssignments(string userId, int courseId);
        Task<Assignment> GetAssignmentById(int assignmentId);
		Task<bool> AddAssignment(Assignment assignment, int courseId);
        Task<bool> RemoveAssignment(Assignment assignment);
        bool UpdateAssignment(Assignment assignment);

        bool SetStudentAssignmentSubmitted(string studentId, int assignmentId, string txtSubmission);
        bool SetStudentAssignmentSubmitted(string fileName, string studentId, int assignmentId);
        bool Save();

    }
}
