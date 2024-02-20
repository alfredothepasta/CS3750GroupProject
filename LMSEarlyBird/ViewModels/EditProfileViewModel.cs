using LMSEarlyBird.Models;
using System.ComponentModel.DataAnnotations;

namespace LMSEarlyBird.ViewModels
{
    public class EditProfileViewModel
    {
        public string ProfileId { get; set; }
        public string Email { get; set; }
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is Required")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is Required")]
        public string LastName { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }

        public int UserLinkId { get; set; }
        public UserLinks Links { get; set; }
    }
}
