using LMSEarlyBird.Data;
using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace LMSEarlyBird.Controllers
{
    public class ProfileController : Controller
    {
        /// <summary>
        /// API for managing users in the user store
        /// </summary>
        private readonly UserManager<AppUser> _userManager;
        /// <summary>
        /// API for managing users in the user store
        /// </summary>
        private readonly UserManager<Address> _profileManager;
        /// <summary>
        /// Context accessor for reading session data
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;
        /// <summary>
        /// Context for accessing the address database
        /// </summary>
        private readonly IAddressRepository _addressRepository;

        public ProfileController(IAddressRepository addressRepository, IHttpContextAccessor contextAccessor)
        {
            _addressRepository = addressRepository;
            _contextAccessor = contextAccessor;
        }

        public async Task<IActionResult> Profiles()
        {
            var userID = _contextAccessor.HttpContext.User.GetUserId();
            Address addresss = await _addressRepository.getUserAddress(userID);

            return View(addresss);
        }

        public async Task<IActionResult> Index()
        {
            var userID = _contextAccessor.HttpContext.User.GetUserId();
            Address addresss = await _addressRepository.getUserAddress(userID);

            return View(addresss);
        }


        /// <summary>
        /// When accessed via a POST operation, attempts to create a new user account
        /// </summary>
        /// <param name="profileViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Profiles(ProfileViewModel profileViewModel, RegisterViewModel registerViewModel)
        {
            if (!(ModelState.IsValid)) return View(profileViewModel);
            if (!(ModelState.IsValid)) return View(registerViewModel);

            // checks the database for a user with the email address
            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);

            // Pulls the User Info from the forms on the 
            var updateProfle = new Address()
            {
                LineOne = profileViewModel.AddressLine1,
                LineTwo = profileViewModel.AddressLine2,
                City = profileViewModel.City,
                State = profileViewModel.State,
                ZipCode = profileViewModel.Zip,
                UserID = registerViewModel.EmailAddress
            };

            await _profileManager.UpdateAsync(updateProfle);

            return RedirectToAction("Profile", "Profiles");
        }
    }
}
