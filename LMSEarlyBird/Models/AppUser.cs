using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSEarlyBird.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public DateTime Birthday { get; set; }
        public int? AddressId { get; set; }
        public Address Address { get; set; }

        // Relationships
        public List<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public List<InstructorCourse> InstructorCourses { get; set; }

		public List<StudentAssignment> StudentAssignment { get; set; }
	  } 
}
