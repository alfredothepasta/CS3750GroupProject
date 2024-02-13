using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface IAssignmentsRepository
    {
        Task<List<Assignment>> GetCourseAssignments(int courseId);
        Task<List<Assignment>> GetStudentAssignments(string studentId);

        bool AddAssignment(Assignment assignment);
        bool RemoveAssignment(Assignment assignment);
        bool UpdateAssignment(Assignment assignment);
        bool Save();

    }
}
