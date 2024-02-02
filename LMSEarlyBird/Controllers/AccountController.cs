using Microsoft.AspNetCore.Mvc;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Identity;
using LMSEarlyBird.Data;
using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using LMSEarlyBird.Interfaces;

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
        /// Context for accessing the AppUser database
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
                        return RedirectToAction("Dashboard", "User");
                    }
                }
            }
            
            // user not found
            TempData["Error"] = "Invalid credentials. Please try again.";
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
            DateTime cuurentDate = DateTime.Now;
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
                LastName = registerViewModel.LastName,
                Birthday = registerViewModel.BirthDate
            };

            // verify Birthday and age > 16
            // Check if it's 16 years later, then check if it is or is past their birthday if it is exacly 16 years later
            var yearDifference = cuurentDate.Year - newUser.Birthday.Year;
            var dayDifference = cuurentDate.DayOfYear - newUser.Birthday.DayOfYear;
            if(yearDifference < 16)
            {
                ModelState.AddModelError("AgeError", "You must be at least 16 years old to create an account.");
                return View();
            } else if (yearDifference == 16 && dayDifference < 0)
            {
                ModelState.AddModelError("AgeError", "You must be at least 16 years old to create an account.");
                return View();
            }
           

            // get the selected role and validate it
            var role = registerViewModel.UserRole;
            if(role != "teacher" && role != "student")
            {
                ModelState.AddModelError("invalidRole", "Must select either teacher or student");
                return View();
            }
            // attempts to do password
            string password = registerViewModel.Password;
            var newUserResponse = await _userManager.CreateAsync(newUser, password);

            

            if(newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, role);
            } else
            {
                TagBuilder tag = new TagBuilder("ul");
                // Get the error message from the newUserResponse
                string error = newUserResponse.Errors.First<IdentityError>().Description;
                ModelState.AddModelError("PasswordValidationError", "Password Requirements not met:");
                foreach(IdentityError errorMessage in newUserResponse.Errors)
                {
                    ModelState.AddModelError("PasswordValidationError", errorMessage.Description);
                }
                return View();
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
