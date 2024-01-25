using System.ComponentModel.DataAnnotations;

namespace LMSEarlyBird.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required")]
        public string EmailAddress { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name address is required")]
        public string FirstName {  get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name address is required")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not match")]
        public string ConfirmPassword { get; set; }

    }
}