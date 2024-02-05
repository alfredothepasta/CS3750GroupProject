using LMSEarlyBird.Models;
using System.ComponentModel.DataAnnotations;

namespace LMSEarlyBird.ViewModels
{
    public class AddCourseViewModel
    {
        [Required]
        public string Department { get; set; }
        [Required]
        [Display(Name = "Course Number")]
		public string CourseNumber { get; set; }
        [Required]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }
        [Required]
        public int CreditHours { get; set; }
        [Required]
        [Display(Name = "Start Time")]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }
        [Required]
        public string DaysOfWeek { get; set; }
        [Required]
        public Building Building { get; set; }
        [Required]
        public Room Room { get; set; }
    }
}
