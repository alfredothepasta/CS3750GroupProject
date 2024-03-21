using LMSEarlyBird.Data;
using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using LMSEarlyBird.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Buffers.Text;

namespace LMSEarlyBird.Controllers
{
    public class ProfileController : Controller
    {
        
        /// <summary>
        /// Context for accessing the address database
        /// </summary>
        private readonly IAddressRepository _addressRepository;
        /// <summary>
        /// Context for accessing the user identity database
        /// </summary>
        private readonly IUserIdentityService _userIdentityService;
        /// <summary>
        /// Context for accessing the user database
        /// </summary>
        private readonly IAppUserRepository _appUserRepository;
        /// <summary>
        /// Context for accessing the user links
        /// </summary>
        private readonly ILinksRepository _linksRepository;
        /// <summary>
        /// Context for accessing the assignments database
        /// </summary>
        private readonly IAssignmentsRepository _assignmentRepository;

        public ProfileController(IAddressRepository addressRepository, IUserIdentityService userIdentityService,
            IAppUserRepository appUserRepository, ILinksRepository linksRepository, IAssignmentsRepository assignmentRepository)
        {
            _addressRepository = addressRepository;
            _userIdentityService = userIdentityService;
            _appUserRepository = appUserRepository;
            _linksRepository = linksRepository;
            _assignmentRepository = assignmentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> DisplayProfile()
        {
            // pull user based on logged in user
            string userId = _userIdentityService.GetUserId();
            AppUser profile = await _appUserRepository.GetUser(userId);

            Address userAddress = new Address();
            UserLinks userLinks = new UserLinks();

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

            if (_linksRepository.hasUserLinks(userId))
            {
                userLinks = await _linksRepository.getUserLinks(userId);
            }
            else
            {
                userLinks.AppUser = profile;
                _linksRepository.addUserLinks(userLinks);
            }

            // check if user has links implemented tied to the user ID
            if (_linksRepository.hasUserLinks(userId))
            {
                userLinks = await _linksRepository.getUserLinks(userId);
            }
            else
            {
                userLinks.AppUser = profile;
                _linksRepository.addUserLinks(userLinks);
            }

            // provide a list of assignments for the user for the _Layout to display for the notifications
            List<StudentAssignment> assignments = await _assignmentRepository.GetStudentAssignments(userId);

            // build the model view
            var profileVM = new EditProfileViewModel
            {
                ProfileId = userId,
                Email = profile.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                AddressId = userAddress.Id,
                Address = userAddress,
                Links = userLinks,
                StudentAssignment = assignments
            };
            return View(profileVM);
        }



        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            // pull user based on logged in user
            string userId = _userIdentityService.GetUserId();
            AppUser profile = await _appUserRepository.GetUser(userId);

            Address userAddress = new Address();
            UserLinks userLinks = new UserLinks();

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

            // check if user has links implemented tied to the user ID
            if (_linksRepository.hasUserLinks(userId))
            {
                userLinks = await _linksRepository.getUserLinks(userId);
            }
            else
            {
                userLinks.AppUser = profile;
                _linksRepository.addUserLinks(userLinks);
            }

            // provide a list of assignments for the user for the _Layout to display for the notifications
            List<StudentAssignment> assignments = await _assignmentRepository.GetStudentAssignments(userId);

            // build the model view
            var profileVM = new EditProfileViewModel
            {
                ProfileId = userId,
                Email = profile.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                AddressId = userAddress.Id,
                Address = userAddress,
                Links = userLinks,
                StudentAssignment = assignments
            };
            return View(profileVM);
        }

        /// <summary>
        /// When accessed via a POST operation, attempts to update the user account
        /// </summary>
        /// <param name="EditProfileViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        //public async Task<IActionResult> Profiles(ProfileViewModel profileViewModel, RegisterViewModel registerViewModel)
        public async Task<IActionResult> EditProfile(EditProfileViewModel profileVM)
        {
            // validate the ModelState passes (User has entred information)
            if (!(ModelState.IsValid)) return View("EditProfile", profileVM);

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

            // pull the links info from the Profile View
            UserLinks userLinks = await _linksRepository.getUserLinks(userId);
            userLinks.Link1 = profileVM.Links.Link1;
            userLinks.Link2 = profileVM.Links.Link2;
            userLinks.Link3 = profileVM.Links.Link3;
            // update the links
            _linksRepository.UpdateLinks(userLinks);


            return RedirectToAction("DisplayProfile", "Profile");
        }
    }
}
