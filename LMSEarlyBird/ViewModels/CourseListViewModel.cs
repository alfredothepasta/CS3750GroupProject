using LMSEarlyBird.Models;

namespace LMSEarlyBird.ViewModels
{
    public class CourseListViewModel
    {
        public List<Course> Courses { get; set; }
        public Dictionary<Course, double> AvgScorePerCourse { get; set; }
    }
}
