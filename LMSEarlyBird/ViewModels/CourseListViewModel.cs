using LMSEarlyBird.Models;

namespace LMSEarlyBird.ViewModels
{
    public class CourseListViewModel
    {
        public List<Course> Courses { get; set; }
        public Dictionary<string, double> AvgScorePerCourse { get; set; }
    }
}
