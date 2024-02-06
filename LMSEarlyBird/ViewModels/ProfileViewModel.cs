using LMSEarlyBird.Models;
using System.ComponentModel.DataAnnotations;

namespace LMSEarlyBird.ViewModels
{
    public class ProfileViewModel
    {
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }
        [Display(Name = "Zipcode")]
        public int Zip { get; set; }

        public string UserID { get; set; }
    }
}
