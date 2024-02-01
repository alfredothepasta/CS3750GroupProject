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

        public IActionResult Index()
        {
            var userID = _contextAccessor.HttpContext.User.GetUserId();
            Address address = new Address();
            address.UserID = userID;
            address.LineOne = "1234 Place Street";
            address.ZipCode = 123456;
            address.City = "Townsville";
            address.State = "State";
            _addressRepository.addUserAddress(address);

            return View();
        }
    }
}
