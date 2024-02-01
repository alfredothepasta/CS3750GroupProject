using LMSEarlyBird.Data;
using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LMSEarlyBird.Interfaces;

namespace LMSEarlyBird.Controllers
{
    public class TestingController : Controller
    {
        /// <summary>
        /// Context accessor for reading session data
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAddressRepository _addressRepository;

        public TestingController(IAddressRepository addressRepository, IHttpContextAccessor contextAccessor)
        {
            _addressRepository = addressRepository;
            _contextAccessor = contextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var userID = _contextAccessor.HttpContext.User.GetUserId();
            Address addresss = await _addressRepository.getUserAddress(userID);

            return View(addresss);
        }
    }
}
