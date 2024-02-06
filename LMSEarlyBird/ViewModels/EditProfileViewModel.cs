using LMSEarlyBird.Models;

namespace LMSEarlyBird.ViewModels
{
    public class EditProfileViewModel
    {
        public string ProfileId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
    }
}
