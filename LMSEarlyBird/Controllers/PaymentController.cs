using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.Repository;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace LMSEarlyBird.Controllers
{
    public class PaymentController : Controller
    {
        /// <summary>
        /// Context for accessing the user identity database
        /// </summary>
        private readonly IUserIdentityService _userIdentityService;
        /// <summary>
        /// Context for accessing the balance database
        /// </summary>
        private readonly IBalanceRepository _balanceRepository;

        public PaymentController(IUserIdentityService userIdentityService, IBalanceRepository balanceRepository)
        {
            _userIdentityService = userIdentityService;
            _balanceRepository = balanceRepository;
        }

        //private static decimal paymentAmount = 0;

        [HttpGet]
        public async Task<IActionResult> PaymentPage()
        {
            string userId = _userIdentityService.GetUserId();

            // gather the current balance for the user
            decimal currentBalance = await _balanceRepository.GetCurrentBalance(userId);

            // insert the current balance into the view
            PaymentViewModel paymentVM = new PaymentViewModel
            {
                CurrentBalance = currentBalance
            };
            // if the balance is not 0, gather a list of the balances for the user
            // find a better way to do this, as if they have a paid off balance, it should still be displayed
            //if (paymentVM.CurrentBalance != 0)
            //{
                
            //}

            List<BalanceHistory> balances = await _balanceRepository.GetBalanceHistory(userId);
            paymentVM.Payments = balances;

            return View(paymentVM);
        }
        [HttpGet]
        public async Task<IActionResult> Success(PaymentViewModel PaymentVM)
        {
            // pass on the payment view model for the payment amount
            return View(PaymentVM);
        }
        [HttpGet]
        public async Task<IActionResult> Checkout(PaymentViewModel paymentVM)
        {
            // pass on the payment view model for the payment amount
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
            string userId = _userIdentityService.GetUserId();

            // gather the current balance for the user
            decimal currentBalance = await _balanceRepository.GetCurrentBalance(userId);

            PaymentViewModel paymentVM = new PaymentViewModel
            {
                PaymentAmount = paymentAmount,
                CurrentBalance = currentBalance
            };

            // check to verify the user has entered an amount to pay
            if (paymentAmount == 0)
            {
                // gather a list of the balances
                List<BalanceHistory> balances = await _balanceRepository.GetBalanceHistory(userId);
                paymentVM.Payments = balances;

                return View(paymentVM);
            }

            // send all information to the payment view
            return RedirectToAction("Checkout", "Payment", paymentVM);
        }


        // run the payment process
        [HttpPost]
        //public ActionResult Create()
        public async Task<IActionResult> CreateCheckout(PaymentViewModel paymentVM)
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
        {
          new SessionLineItemOptions
          {
            PriceData = new SessionLineItemPriceDataOptions
            {
              UnitAmount = Convert.ToInt64(paymentVM.PaymentAmount) * 100,
              Currency = "usd",
              ProductData = new SessionLineItemPriceDataProductDataOptions
              {
                Name = "Payment on Tuition",
              },
            },
            Quantity = 1,
          },
        },
                Mode = "payment",
                SuccessUrl = "https://localhost:7243/Payment/Success",
                CancelUrl = "https://localhost:7243/Payment/Cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession()
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
        {
          new SessionLineItemOptions
          {
            PriceData = new SessionLineItemPriceDataOptions
            {
              UnitAmount = 2000,
              Currency = "usd",
              ProductData = new SessionLineItemPriceDataProductDataOptions
              {
                Name = "T-shirt",
              },
            },
            Quantity = 1,
          },
        },
                Mode = "payment",
                SuccessUrl = "https://localhost:7243/Payment/success",
                CancelUrl = "https://localhost:7243/Payment/cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}
