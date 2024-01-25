using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using LMSEarlyBird.Data;
using System.Numerics;

namespace LMSEarlyBird.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public HomeController(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor=contextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext?.User.GetUserId();
                if(currentUser != null)
                {
                    AppUser userData = await _context.Users.FindAsync(currentUser);
                    return View(userData);
                }
            } catch
            {
            
            }

            return View();


        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
