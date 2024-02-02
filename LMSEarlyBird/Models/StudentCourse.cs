using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSEarlyBird.Models
{
    
    public class StudentCourse
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}