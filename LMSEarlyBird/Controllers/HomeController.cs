using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using LMSEarlyBird.Data;
using System.Numerics;
using LMSEarlyBird.ViewModels;


namespace LMSEarlyBird.Controllers
{
    /// <summary>
    /// Controller that handles the home pages
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// context variable for accessing the db
        /// </summary>
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// Context accessor for reading session data
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// Constructor, initializes the instance variables
        /// </summary>
        /// <param name="context"></param>
        /// <param name="contextAccessor"></param>
        public HomeController(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor=contextAccessor;
        }

        /// <summary>
/// Handles the logic for the Index page.
/// Redirects to Login page if the user is not authenticated.
/// </summary>
/// <returns></returns>
public async Task<IActionResult> Index()
{
    // Checks to see if there is a current signed in user
    if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
    {
        var currentUser = _contextAccessor.HttpContext?.User.GetUserId();
        if (currentUser != null)
        {
            AppUser userData = await _context.Users.FindAsync(currentUser);
            return View(userData);
        }
    }

    return RedirectToAction(nameof(AccountController.Login), "Account");
}

        /// <summary>
        /// Returns the Privacy page
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Returns the error page
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}