using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSEarlyBird.Models
{
    public class InstructorCourse
    {
        [ForeignKey("User")]
        [Column(Order = 0)]
        public string UserId { get; set; }
        [ForeignKey("Course")]
        [Column(Order = 1)]
        public int CourseId { get; set; }
    }
}