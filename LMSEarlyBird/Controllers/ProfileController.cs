using LMSEarlyBird.Data;
using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using LMSEarlyBird.Repository;

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
        private readonly IUserIdentityService _userIdentityService;
        private readonly IAppUserRepository _appUserRepository;

        public ProfileController(IAddressRepository addressRepository, IHttpContextAccessor contextAccessor, IUserIdentityService userIdentityService,
            IAppUserRepository appUserRepository)
        {
            _addressRepository = addressRepository;
            _contextAccessor = contextAccessor;
            _userIdentityService = userIdentityService;
            _appUserRepository = appUserRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Profiles(string id)
        {
            // pull user based on logged in user
            string userId = _userIdentityService.GetUserId();
            AppUser profile = await _appUserRepository.GetUser(userId);

            Address userAddress = new Address();

            // check if user has address implemented tied to the user ID
            if (_addressRepository.hasUserAddress(userId))
            {
                userAddress = await _addressRepository.getUserAddress(userId);
            }
            else
            {
                // One way of doing it
                //userAddress.UserID = userId;
                //_addressRepository.addUserAddress(userAddress);

                // other way, using preset AppUser = profile
                userAddress.User = profile;
                _addressRepository.addUserAddress(userAddress);
            }

            // build the model view
            var profileVM = new EditProfileViewModel
            {
                ProfileId = userId,
                Email = profile.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                AddressId = userAddress.Id,
                Address = userAddress
            };
            return View(profileVM);
        }

        //public async Task<IActionResult> Tester()
        //{
        //    var userID = _contextAccessor.HttpContext.User.GetUserId();
        //    Address address = await _addressRepository.getUserAddress(userID);

        //    return View(address);
        //}


        /// <summary>
        /// When accessed via a POST operation, attempts to update the user account
        /// </summary>
        /// <param name="EditProfileViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        //public async Task<IActionResult> Profiles(ProfileViewModel profileViewModel, RegisterViewModel registerViewModel)
        public async Task<IActionResult> Profiles(EditProfileViewModel profileVM)
        {
            // validate the ModelState passes (User has entred information)
            if (!(ModelState.IsValid)) return View("Profiles", profileVM);
            
            // gather the userId based on the logged in profile
            var userId = _userIdentityService.GetUserId();

            // Pulls the User Info from the forms on the Profile View
            AppUser currentUser = await _appUserRepository.GetUser(userId);
            currentUser.FirstName = profileVM.FirstName;
            currentUser.LastName = profileVM.LastName;
            // update the currentUser
            _appUserRepository.UpdateUser(currentUser);
            // pull the address info from the Profile View
            Address userAddress = profileVM.Address;
            userAddress.Id = profileVM.AddressId;
            // update the address
            _addressRepository.updateUserAddress(userAddress);

            return RedirectToAction("Profiles", "Profile");
        }
    }
}
