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
        public string CourseName { get; set; }
        public string CourseNumber { get; set; }
        public int CreditHours { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string DaysOfWeek { get; set; }
        [ForeignKey("Room")]
        public int RoomId { get; set; }

    }
}
