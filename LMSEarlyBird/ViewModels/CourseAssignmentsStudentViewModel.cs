using LMSEarlyBird.Models;

namespace LMSEarlyBird.ViewModels
{
    public class CourseAssignmentsStudentViewModel
    {
        public Course Course { get; set; }
        public List<StudentAssignment>? Assignments { get; set; }

        public List<StudentAssignment> StudentAssignment { get; set; } = new List<StudentAssignment>();
    }
}