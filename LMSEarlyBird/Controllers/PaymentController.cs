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
        /// <summary>
        /// Context for accessing the balance database
        /// </summary>
        private readonly IBalanceRepository _balanceRepository;

        public PaymentController(IUserIdentityService userIdentityService, IAppUserRepository appUserRepository, IBalanceRepository balanceRepository)
        {
            _userIdentityService = userIdentityService;
            _appUserRepository = appUserRepository;
            _balanceRepository = balanceRepository;
        }

        //private static decimal paymentAmount = 0;

        [HttpGet]
        public async Task<IActionResult> PaymentPage()
        {
            string userId = _userIdentityService.GetUserId();
            //AppUser profile = await _appUserRepository.GetUser(userId);

            // gather the current balance for the user
            decimal currentBalance = await _balanceRepository.GetCurrentBalance(userId);

            // insert the current balance into the view
            PaymentViewModel paymentVM = new PaymentViewModel
            {
                CurrentBalance = currentBalance
            };
            // if the balance is not 0, gather a list of the balances for the user
            // find a better way to do this, as if they have a paid off balance, it should still be displayed
            if (paymentVM.CurrentBalance != 0)
            {
                
            }

            List<BalanceHistory> balances = await _balanceRepository.GetBalanceHistory(userId);
            paymentVM.Payments = balances;

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
                //PaymentAmount = paymentAmount
            };
            return View(paymentVM);
        }
        [HttpGet]
        public async Task<IActionResult> Cancel()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PaymentPage(decimal paymentAmount)
        {
            //paymentAmount = paymentVM.PaymentAmount;

            //PaymentViewModel paymentVM = new PaymentViewModel
            //{
            //    CurrentBalance = paymentAmount
            //};

            if (paymentAmount == 0)
            {
                return View();
            }

            return RedirectToAction("Checkout", "Payment");
        }
    }
}
