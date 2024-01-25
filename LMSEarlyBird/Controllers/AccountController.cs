using Microsoft.AspNetCore.Mvc;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Identity;
using LMSEarlyBird.Data;
using LMSEarlyBird.Models;

namespace LMSEarlyBird.Controllers
{
    /// <summary>
    /// Handles the logic for Accounts
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// API for managing users in the user store
        /// </summary>
        private readonly UserManager<AppUser> _userManager;
        /// <summary>
        /// API for the user sign in
        /// </summary>
        private readonly SignInManager<AppUser> _signInManager;
        /// <summary>
        /// Context for accessing the database
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for the Controller
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="context"></param>
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _userManager=userManager;
            _signInManager=signInManager;
            _context=context;
        }

        /// <summary>
        /// When the web page is accessed by the GET statement - Pull up the default login page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        /// <summary>
        /// When the Login page recieves a POST request, process the user input and attempt to log in
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            // If the model state is invalid, load the get page
            if (!(ModelState.IsValid)) return View(loginViewModel);

            // gets the user email from the database
            var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);
            
            // if user is found, perform password validataion
            if(user != null)
            {
                // gets the result of the sign in attempt   
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    // password Correct, sign in
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            
            // user not found
            TempData["Error"] = "Wrong credentials. Please try again.";
            return View(loginViewModel);
        }

        /// <summary>
        /// When the web page is accessed by the GET statement - Pull up the default Register page 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        /// <summary>
        /// When accessed via a POST operation, attempts to create a new user account
        /// </summary>
        /// <param name="registerViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!(ModelState.IsValid)) return View(registerViewModel);

            // checks the database for a user with the email address
            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            // If user already existst
            if (user != null)
            {
                // TempData is a variable that is passed into the 
                TempData["Error"] = "This email address is already in use";
                return View(registerViewModel);
            }

            // Pulls the User Info from the forms on the 
            var newUser = new AppUser()
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName
            };

            // 
            string password = registerViewModel.Password;
            var newUserResponse = await _userManager.CreateAsync(newUser, password);

            if(newUserResponse.Succeeded)
            {
#if DEBUG
#else
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
#endif
            }


            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
