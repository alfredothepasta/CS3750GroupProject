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
        
        [ForeignKey("Address")]
        public int? AddressId { get; set; }

        // Has a get set for email, phone number, etc. inherited from IdentityUser
    }
}
