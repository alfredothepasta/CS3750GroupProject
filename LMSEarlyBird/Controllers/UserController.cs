using LMSEarlyBird.Data;
using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMSEarlyBird.Controllers
{
    public class UserController : Controller
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
        public UserController(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<IActionResult> Dashboard()
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

            return View("Account", "Login");
        }
    }
}
