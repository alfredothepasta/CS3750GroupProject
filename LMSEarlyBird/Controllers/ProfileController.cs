﻿using LMSEarlyBird.Data;
using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using LMSEarlyBird.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public ProfileController(IAddressRepository addressRepository, IUserIdentityService userIdentityService,
            IAppUserRepository appUserRepository, ILinksRepository linksRepository)
        {
            _addressRepository = addressRepository;
            _userIdentityService = userIdentityService;
            _appUserRepository = appUserRepository;
            _linksRepository = linksRepository;
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

            // build the model view
            var profileVM = new EditProfileViewModel
            {
                ProfileId = userId,
                Email = profile.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                AddressId = userAddress.Id,
                Address = userAddress,
                Links = userLinks
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

                // other way, using preset AppUser = profile
                userLinks.AppUser = profile;
                _linksRepository.addUserLinks(userLinks);
            }

            // build the model view
            var profileVM = new EditProfileViewModel
            {
                ProfileId = userId,
                Email = profile.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                AddressId = userAddress.Id,
                Address = userAddress,
                Links = userLinks
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
            UserLinks userLinks = profileVM.Links;
            userLinks.Id = profileVM.UserLinkId;
            // update the links
            _linksRepository.UpdateLinks(userLinks);


            return RedirectToAction("DisplayProfile", "Profile");
        }
    }
}
