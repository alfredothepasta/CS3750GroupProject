using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.Repository;
using LMSEarlyBird.ViewModels;
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

        private static decimal paymentAmount = 0;

        [HttpGet]
        public async Task<IActionResult> PaymentPage()
        {
            PaymentViewModel paymentVM = new PaymentViewModel
            {
                PaymentAmount = 0
            };
            return View(paymentVM);
        }
        [HttpGet]
        public async Task<IActionResult> Success()
        {
            //Startup startup = new Startup();
            //return View(startup);
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            PaymentViewModel paymentVM = new PaymentViewModel
            {
                PaymentAmount = paymentAmount
            };
            return View(paymentVM);
        }
        [HttpGet]
        public async Task<IActionResult> Cancel()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PaymentPage(PaymentViewModel paymentVM)
        {
            paymentAmount = paymentVM.PaymentAmount;

            if (paymentAmount == 0)
            {
                return View();
            }

            return RedirectToAction("Checkout", "Payment");
        }
    }
}
