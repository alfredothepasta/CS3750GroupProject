using LMSEarlyBird.Models;

namespace LMSEarlyBird.ViewModels
{
    public class AssignmentSubmissionsListViewModel
    {
        public Course Course { get; set; }
        public Assignment Assignment { get; set; }
        public List<StudentAssignment>? Assignments { get; set; }
    }
}
