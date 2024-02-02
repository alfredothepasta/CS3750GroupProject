using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LMSEarlyBird.Models
{
    public class Course
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public string CourseName { get; set; }
        public string CourseNumber { get; set; }
        public int CreditHours { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string DaysOfWeek { get; set; }
        
        public int RoomId { get; set; }
        public Room Room { get; set; }

        //relationships
        public List<StudentCourse> StudentCourses { get; set; }
        public List<InstructorCourse> InstructorCourses { get; set; }

    }
}
