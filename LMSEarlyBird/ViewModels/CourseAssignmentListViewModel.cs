using LMSEarlyBird.Models;

namespace LMSEarlyBird.ViewModels
{
    public class CourseAssignmentListViewModel
    {
        public Course Course { get; set; }
        public List<Assignment>? Assignments { get; set; }
    }
}
