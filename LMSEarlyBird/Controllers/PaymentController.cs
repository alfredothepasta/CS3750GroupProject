using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.Repository;
using Microsoft.AspNetCore.Mvc;

namespace LMSEarlyBird.Controllers
{
    public class PaymentController : Controller
    {
        /// <summary>
        /// Context for accessing the user identity database
        /// </summary>
        private readonly IUserIdentityService _userIdentityService;
        /// <summary>
        /// Context for accessing the user database
        /// </summary>
        private readonly IAppUserRepository _appUserRepository;

        public PaymentController(IUserIdentityService userIdentityService, IAppUserRepository appUserRepository)
        {
            _userIdentityService = userIdentityService;
            _appUserRepository = appUserRepository;
        }

        [HttpGet]
        public async Task<IActionResult> PaymentPage()
        {
            //pull user based on logged in user
            string userId = _userIdentityService.GetUserId();
            AppUser profile = await _appUserRepository.GetUser(userId);

            var userVM = new AppUser
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                StudentCourses = profile.StudentCourses,
                InstructorCourses = profile.InstructorCourses,

            };
            return View(userVM);
        }

        public async Task<IActionResult> Success()
        {
            Startup startup = new Startup();
            return View(startup);
        }

        public async Task<IActionResult> Checkout()
        {
            Program program = new Program { };
            Startup startup = new Startup();
            return View(startup);
        }

        public async Task<IActionResult> Cancel()
        {
            return View();
        }
    }
}
